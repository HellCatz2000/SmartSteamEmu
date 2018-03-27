using AppInfo;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;
using vbAccelerator.Components.Shell;

namespace SSELauncher
{
    public partial class FrmMain : Form
    {
        CAppList m_AppList;
        CConfig Conf;
        string AppPath;
        List<string> AvailableCategories = new List<string>();
        AppInfoVDF appinfoVDF = null;
        string SteamInstallPath;
        bool FormFullyLoaded = false;
        FormWindowState LastWindowState = FormWindowState.Normal;
        ContextMenuStrip mnuTrayMenu = new ContextMenuStrip();
        bool MenuFirstInit = true;

        public FrmMain()
        {
            AppPath = AppDomain.CurrentDomain.BaseDirectory;

            m_AppList = Program.AppList;
            Conf = m_AppList.GetConfig();

            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            notifyIcon1.Icon = this.Icon; //set tray icon and title to the same as the main form
            notifyIcon1.Text = this.Text;


            m_AppList.EventAppClear += OnAppClear;
            m_AppList.EventAppAdded += OnAppAdded;
            m_AppList.EventAppDeleted += OnAppDeleted;

            BackgroundWorker bwWorker = new BackgroundWorker();
            bwWorker.DoWork += bwWorker_DoWorkInitVDF;
            bwWorker.RunWorkerAsync();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SHOWME)
            {
                Visible = true;
                WindowState = LastWindowState;
                Activate();
            }

            base.WndProc(ref m);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            editGameToolStripMenuItem.Enabled = false;
            deleteGameToolStripMenuItem.Enabled = false;

            this.Size = new Size((Conf.WindowSizeX == 0 ? this.Size.Width : Conf.WindowSizeX), (Conf.WindowSizeY == 0 ? this.Size.Height : Conf.WindowSizeY));
            this.Location = new Point((Conf.WindowPosX < 0 ? this.Location.X : Conf.WindowPosX), (Conf.WindowPosY < 0 ? this.Location.Y : Conf.WindowPosY));

            pbDrop.Visible = true;
            this.DragEnter += new DragEventHandler(lstApps_DragEnter);
            this.DragDrop += new DragEventHandler(lstApps_DragDrop);
            m_AppList.Refresh();
            OnSelectedAppChanged();
            MenuFirstInit = false;
            SortTrayMenu();

            FormFullyLoaded = true;
            AdjustSize();
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (!FormFullyLoaded) return;

            if (WindowState != FormWindowState.Minimized) LastWindowState = WindowState;
            if (Conf.HideToTray)
            {
                Visible = WindowState != FormWindowState.Minimized;
            }

            AdjustSize();
        }

        private void AdjustSize()
        {
            pbDrop.SetBounds(-10, -25, this.Width + 10, this.Height + 25);
        }

        private void lstApps_DoubleClick(object sender, EventArgs e)
        {
            LaunchApp();
        }

        private void lstApps_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnSelectedAppChanged();
            if (e.KeyChar == (char)Keys.Return)
            {
                LaunchApp();
            }
        }

        void OnSelectedAppChanged()
        {
            if (lstApps.SelectedItems.Count == 0)
            {
                largeIconToolStripMenuItem.Checked = lstApps.View == View.LargeIcon;
                smallIconToolStripMenuItem.Checked = lstApps.View == View.SmallIcon;
                listToolStripMenuItem.Checked = lstApps.View == View.List;
                tileToolStripMenuItem.Checked = lstApps.View == View.Tile;
                nameToolStripMenuItem.Checked = Conf.SortBy == CConfig.ESortBy.SortByName;
                dateAddedToolStripMenuItem.Checked = Conf.SortBy == CConfig.ESortBy.SortByDateAdded;
                noneToolStripMenuItem.Checked = Conf.GroupBy == CConfig.EGroupBy.GroupByNone;
                typeToolStripMenuItem.Checked = Conf.GroupBy == CConfig.EGroupBy.GroupByType;
                categoryToolStripMenuItem.Checked = Conf.GroupBy == CConfig.EGroupBy.GroupByCategory;
                hideMissingShortcutToolStripMenuItem.Checked = Conf.HideMissingShortcut;
                lstApps.ContextMenuStrip = ctxMenuViewStrip;

                editGameToolStripMenuItem.Enabled = false;
                deleteGameToolStripMenuItem.Enabled = false;
            }
            else
            {
                lstApps.ContextMenuStrip = ctxMenuStrip;
                editGameToolStripMenuItem.Enabled = true;
                deleteGameToolStripMenuItem.Enabled = true;

                ListViewItem lvi = lstApps.SelectedItems[0];
                CApp app = m_AppList.GetApp(lvi);

                if (app != null)
                {
                    launchNormallywithoutEmuToolStripMenuItem.Visible = app.AppId != -1;
                }
            }
        }

        void lstApps_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void lstApps_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
            {
                CApp app = new CApp();

                app.Path = CApp.MakeRelativePath(file);
                app.GameName = Path.GetFileNameWithoutExtension(app.Path);
                app.StartIn = CApp.MakeRelativePath(Path.GetDirectoryName(app.Path));

                if (file.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using (ShellLink link = new ShellLink(file))
                        {
                            app.Path = CApp.MakeRelativePath(link.Target);
                            app.GameName = Path.GetFileNameWithoutExtension(app.Path);
                            app.StartIn = CApp.MakeRelativePath(link.WorkingDirectory);
                            app.CommandLine = link.Arguments;
                        }
                    }
                    catch
                    { }
                }

                if (File.Exists(Path.Combine(CApp.GetAbsolutePath(app.StartIn), "bin\\launcher.dll")) ||
                    File.Exists(Path.Combine(CApp.GetAbsolutePath(app.StartIn), "hl.exe")) ||
                    File.Exists(Path.Combine(CApp.GetAbsolutePath(app.StartIn), "hlds.exe")) ||
                    File.Exists(Path.Combine(CApp.GetAbsolutePath(app.StartIn), "hltv.exe")))
                {
                    app.CommandLine = "-steam";
                }

                AutoAppConfig(file, app);
            }
        }

        private void lstApps_OnItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            OnSelectedAppChanged();
        }

        private void lstApps_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedAppChanged();
        }

        private void LaunchApp()
        {
            if (lstApps.SelectedItems.Count == 0) return;

            ListViewItem lvi = lstApps.SelectedItems[0];
            CApp app = m_AppList.GetApp(lvi);

            if (app.AppId == 0)
            {
                if (MessageBox.Show("You need to set up game app id first. You can find your game app id on steam store url: http://store.steampowered.com/app/<AppId> \n\nSetup now?", "Invalid app id", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DoEditGame();
                }

                return;
            }
            if (app.AppId == -1)
            {
                LaunchWithoutEmu(app);
            }
            else
            {
                WriteIniAndLaunch(app, Conf);
            }
        }

        private void OnAppClear(object sender, AppModifiedEventArgs e)
        {
            mnuTrayMenu.Items.Clear();
            PopulateTrayLaunchMenu();
            lstApps.Groups.Clear();
            lstApps.Clear();

            switch (Conf.SortBy)
            {
                case CConfig.ESortBy.SortByName:
                    lstApps.Sorting = SortOrder.Ascending;
                    break;
                case CConfig.ESortBy.SortByDateAdded:
                    lstApps.Sorting = SortOrder.None;
                    break;
            }
        }

        private void OnAppAdded(object sender, AppModifiedEventArgs e)
        {
            if (Conf.HideMissingShortcut)
            {
                FileInfo fi = new FileInfo(CApp.GetAbsolutePath(e.app.Path));
                if (!fi.Exists)
                {
                    return;
                }
            }

            ListViewItem lvi = new ListViewItem(e.app.GameName);
            SetListViewItemGroup(e.app, lvi);
            DoRefreshCategories(e.app);

            try
            {
                lvi.ImageKey = e.app.GetIconHash();
                BackgroundWorker bwWorker = new BackgroundWorker();
                bwWorker.DoWork += bwWorker_DoWork;
                bwWorker.RunWorkerCompleted += bwWorker_RunWorkerCompleted;
                bwWorker.RunWorkerAsync(new BgParam(lvi.GetHashCode().ToString(), e.app));
            }
            catch
            {

            }

            e.tag = lstApps.Items.Add(lvi);
            e.app.Tag = e.tag;
            AddTrayLaunchMenu(e.app);
            pbDrop.Visible = false;
            lstApps.Sort();
        }

        void bwWorker_DoWorkInitVDF(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (RegistryKey regBase = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    if (regBase != null)
                    {
                        RegistryKey regSteam = regBase.OpenSubKey("SOFTWARE\\Valve\\Steam");
                        if (regSteam != null)
                        {
                            SteamInstallPath = regSteam.GetValue("InstallPath").ToString();

                            // Probably initialize this on background worker for faster startup
                            appinfoVDF = new AppInfoVDF(Path.Combine(SteamInstallPath, "appcache\\appinfo.vdf"));
                        }
                    }
                }
            }
            catch
            { }
        }

        void bwWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BgParam param = (BgParam)e.Result;

            if (param.AppIcon != null)
            {
                imgList.Images.Add(param.App.GetIconHash(), param.AppIcon);
            }

            EditTrayLaunchMenu(param.App, true);
        }

        void bwWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BgParam param = (BgParam)e.Argument;
            param.AppIcon = CAppList.GetIcon(param.App);
            e.Result = param;
        }

        private void OnAppDeleted(object sender, AppModifiedEventArgs e)
        {
            DeleteTrayLaunchMenu(e.tag);
            lstApps.Items.Remove((ListViewItem)e.tag);

            if (lstApps.Items.Count > 0)
            {
                pbDrop.Visible = false;
            }
            else
            {
                pbDrop.Visible = true;
            }
        }

        public static void WriteIniAndLaunch(CApp app, CConfig gconf, string extra_commandline = null)
        {
            var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SmartSteamEmu");
            var launcherSettings = Path.Combine(baseDirectory, "launcher.ini");
            var launcherExecutable = Path.Combine(baseDirectory, "SmartSteamLoader" + (app.Use64Launcher ? "_x64" : "") + ".exe");

            bool lowViolence = (app.LowViolence == -1) ? gconf.LowViolence : Convert.ToBoolean(app.LowViolence);
            bool storageOnAppdata = (app.StorageOnAppdata == -1) ? gconf.StorageOnAppdata : Convert.ToBoolean(app.StorageOnAppdata);
            bool separateStorageByName = (app.SeparateStorageByName == -1) ? gconf.SeparateStorageByName : Convert.ToBoolean(app.SeparateStorageByName);
            bool automaticallyJoinInvite = (app.AutomaticallyJoinInvite == -1) ? gconf.AutomaticallyJoinInvite : Convert.ToBoolean(app.AutomaticallyJoinInvite);
            bool enableHTTP = (app.EnableHTTP == -1) ? gconf.EnableHTTP : Convert.ToBoolean(app.EnableHTTP);
            bool enableInGameVoice = (app.EnableInGameVoice == -1) ? gconf.EnableInGameVoice : Convert.ToBoolean(app.EnableInGameVoice);
            bool enableLobbyFilter = (app.EnableLobbyFilter == -1) ? gconf.EnableLobbyFilter : Convert.ToBoolean(app.EnableLobbyFilter);
            bool disableFriendList = (app.DisableFriendList == -1) ? gconf.DisableFriendList : Convert.ToBoolean(app.DisableFriendList);
            bool disableLeaderboard = (app.DisableLeaderboard == -1) ? gconf.DisableLeaderboard : Convert.ToBoolean(app.DisableLeaderboard);
            bool securedServer = (app.SecuredServer == -1) ? gconf.SecuredServer : Convert.ToBoolean(app.SecuredServer);
            bool enableVR = (app.VR == -1) ? gconf.EnableVR : Convert.ToBoolean(app.VR);
            bool offline = (app.Offline == -1) ? gconf.Offline : Convert.ToBoolean(app.Offline);
            bool enableOverlay = (app.EnableOverlay == -1) ? gconf.EnableOverlay : Convert.ToBoolean(app.EnableOverlay);
            bool enableOnlinePlay = (app.EnableOnlinePlay == -1) ? gconf.EnableOnlinePlay : Convert.ToBoolean(app.EnableOnlinePlay);
            bool enableLog = ((app.EnableDebugLogging == -1) ? gconf.EnableLog : Convert.ToBoolean(app.EnableDebugLogging));

            long manualSteamID = (app.ManualSteamId == -1L) ? gconf.ManualSteamId : app.ManualSteamId;

            string avatarPath = string.IsNullOrEmpty(app.AvatarPath) ? ((gconf.AvatarPath == "avatar.png") ? gconf.AvatarPath : CApp.GetAbsolutePath(gconf.AvatarPath)) : CApp.GetAbsolutePath(app.AvatarPath);
            string personaName = string.IsNullOrEmpty(app.PersonaName) ? gconf.PersonaName : app.PersonaName;
            string steamIdGeneration = string.IsNullOrEmpty(app.SteamIdGeneration) ? gconf.SteamIdGeneration : app.SteamIdGeneration;
            string quickJoinHotkey = string.IsNullOrEmpty(app.QuickJoinHotkey) ? gconf.QuickJoinHotkey : app.QuickJoinHotkey;
            string language = string.IsNullOrEmpty(app.Language) ? gconf.Language : app.Language;
            string overlayLanguage = gconf.OverlayLanguage;

            string masterServerAddress = "";
            foreach (var server in gconf.MasterServerAddress)
            {
                masterServerAddress += server + " ";
            }


            if (!string.IsNullOrWhiteSpace(overlayLanguage))
            {
                if (language.Equals("Simplified Chinese", StringComparison.OrdinalIgnoreCase))
                {
                    language = "schinese";
                }
                else if (language.Equals("Traditional Chinese", StringComparison.OrdinalIgnoreCase))
                {
                    language = "tchinese";
                }
            }

            if (!string.IsNullOrWhiteSpace(overlayLanguage))
            {
                if (overlayLanguage.Equals("Simplified Chinese", StringComparison.OrdinalIgnoreCase))
                {
                    overlayLanguage = "schinese";
                }
                else if (overlayLanguage.Equals("Traditional Chinese", StringComparison.OrdinalIgnoreCase))
                {
                    overlayLanguage = "tchinese";
                }
            }


            try
            {
                // Empty log file
                if (enableLog && gconf.CleanLog)
                {
                    var logFile = Path.Combine(baseDirectory, "SmartSteamEmu.log");

                    if (File.Exists(logFile))
                    {
                        using (new FileStream(logFile, FileMode.Truncate)) { }
                    }
                }

                using (var sw = new StreamWriter(new FileStream(launcherSettings, FileMode.Create, FileAccess.ReadWrite), Encoding.Unicode))
                {
                    sw.WriteLine("# This file is generated by and will be overwritten by SSELauncher");
                    sw.WriteLine("#");
                    sw.WriteLine("");
                    sw.WriteLine("");
                    sw.WriteLine("[Launcher]");
                    sw.WriteLine("Target = " + CApp.GetAbsolutePath(app.Path));
                    sw.WriteLine("StartIn = " + CApp.GetAbsolutePath(app.StartIn));
                    sw.WriteLine("CommandLine = " + app.CommandLine + (string.IsNullOrEmpty(extra_commandline) ? "" : extra_commandline));
                    sw.WriteLine("SteamClientPath = " + Path.Combine(baseDirectory, "SmartSteamEmu.dll"));
                    sw.WriteLine("SteamClientPath64 = " + Path.Combine(baseDirectory, "SmartSteamEmu64.dll"));
                    sw.WriteLine("Persist = " + (app.Persist ? 1 : 0));
                    sw.WriteLine("ParanoidMode = " + (gconf.ParanoidMode ? 1 : 0));
                    sw.WriteLine("InjectDll = " + (app.InjectDll ? 1 : 0));
                    sw.WriteLine("");

                    sw.WriteLine("[SmartSteamEmu]");
                    sw.WriteLine("AvatarFilename = " + avatarPath);
                    sw.WriteLine("PersonaName = " + personaName);
                    sw.WriteLine("AppId = " + app.AppId);
                    sw.WriteLine("SteamIdGeneration = " + steamIdGeneration);
                    sw.WriteLine("ManualSteamId = " + manualSteamID);
                    sw.WriteLine("Language = " + language);
                    sw.WriteLine("LowViolence = " + lowViolence.ToString());
                    sw.WriteLine("StorageOnAppdata = " + storageOnAppdata.ToString());
                    sw.WriteLine("SeparateStorageByName = " + separateStorageByName.ToString());
                    if (!string.IsNullOrEmpty(app.RemoteStoragePath) && !string.IsNullOrWhiteSpace(app.RemoteStoragePath))
                    {
                        sw.WriteLine("RemoteStoragePath = " + CApp.GetAbsolutePath(app.RemoteStoragePath));
                    }
                    sw.WriteLine("AutomaticallyJoinInvite = " + automaticallyJoinInvite.ToString());
                    sw.WriteLine("EnableHTTP = " + enableHTTP.ToString());
                    sw.WriteLine("EnableInGameVoice = " + enableInGameVoice.ToString());
                    sw.WriteLine("EnableLobbyFilter = " + enableLobbyFilter.ToString());
                    sw.WriteLine("DisableFriendList = " + disableFriendList.ToString());
                    sw.WriteLine("DisableLeaderboard = " + disableLeaderboard.ToString());
                    sw.WriteLine("DisableGC = " + app.DisableGC.ToString());
                    sw.WriteLine("SecuredServer = " + securedServer.ToString());
                    sw.WriteLine("VR = " + enableVR.ToString());
                    sw.WriteLine("Offline = " + offline.ToString());
                    sw.WriteLine("QuickJoinHotkey = " + quickJoinHotkey);
                    sw.WriteLine("MasterServer = " + masterServerAddress);
                    sw.WriteLine("MasterServerGoldSrc = " + masterServerAddress);
                    sw.WriteLine(app.Extras);
                    sw.WriteLine("");

                    sw.WriteLine("[Achievements]");
                    sw.WriteLine("FailOnNonExistenceStats = " + Convert.ToBoolean(app.FailOnNonExistenceStats).ToString());
                    sw.WriteLine("");

                    sw.WriteLine("[SSEOverlay]");
                    sw.WriteLine("DisableOverlay = " + (!Convert.ToBoolean(enableOverlay)).ToString());
                    sw.WriteLine("OnlineMode = " + Convert.ToBoolean(enableOnlinePlay).ToString());
                    sw.WriteLine("Language = " + overlayLanguage);
                    sw.WriteLine("ScreenshotHotkey = " + gconf.OverlayScreenshotHotkey);
                    sw.WriteLine("HookRefCount = " + app.EnableHookRefCount.ToString());
                    sw.WriteLine("OnlineKey = " + (string.IsNullOrWhiteSpace(app.OnlineKey) ? gconf.OnlineKey : app.OnlineKey));
                    sw.WriteLine("");

                    sw.WriteLine("[DirectPatch]");
                    foreach (var patch in app.DirectPatchList)
                    {
                        sw.WriteLine(patch);
                    }
                    sw.WriteLine("");


                    sw.WriteLine("[Debug]");
                    sw.WriteLine("EnableLog = " + enableLog.ToString());
                    sw.WriteLine("MarkLogHotkey = " + gconf.MarkLogHotkey);
                    sw.WriteLine("LogFilter = " + gconf.LogFilter);
                    sw.WriteLine("Minidump = " + gconf.Minidump.ToString());
                    sw.WriteLine("");


                    sw.WriteLine("[DLC]");
                    sw.WriteLine("Default = " + app.DefaultDlcSubscribed.ToString());
                    foreach (var dlc in app.DlcList)
                    {
                        if (!dlc.Disabled)
                        {
                            sw.WriteLine(dlc.DlcId + " = " + dlc.DlcName);
                        }
                    }
                    sw.WriteLine("");
                    string broadcastAddress = "";
                    if (app.ListenPort == -1)
                    {
                        foreach (var address in gconf.BroadcastAddress)
                        {
                            broadcastAddress += address + " ";
                        }
                    }
                    else
                    {
                        foreach (var address in app.BroadcastAddress)
                        {
                            broadcastAddress += address + " ";
                        }
                    }
                    int num2 = (app.ListenPort == -1) ? gconf.ListenPort : app.ListenPort;
                    int num3 = (app.MaximumPort == -1) ? gconf.MaximumPort : app.MaximumPort;
                    int num4 = (app.DiscoveryInterval == -1) ? gconf.DiscoveryInterval : app.DiscoveryInterval;
                    int num5 = (app.MaximumConnection == -1) ? gconf.MaximumConnection : app.MaximumConnection;
                    sw.WriteLine("[Networking]");
                    sw.WriteLine("BroadcastAddress = " + broadcastAddress);
                    sw.WriteLine("ListenPort = " + num2);
                    sw.WriteLine("MaximumPort = " + num3);
                    sw.WriteLine("DiscoveryInterval = " + num4);
                    sw.WriteLine("MaximumConnection = " + num5);
                    sw.WriteLine("");
                    sw.WriteLine("[PlayerManagement]");
                    sw.WriteLine("AllowAnyoneConnect = " + gconf.AllowAnyoneConnect.ToString());
                    sw.WriteLine("AdminPassword = " + gconf.AdminPass);
                    foreach (string current6 in gconf.BanList)
                    {
                        sw.WriteLine(current6 + " = 0");
                    }
                    sw.WriteLine("");
                    using (Process.Start(new ProcessStartInfo
                    {
                        CreateNoWindow = false,
                        UseShellExecute = false,
                        FileName = launcherExecutable,
                        WindowStyle = ProcessWindowStyle.Normal,
                        Arguments = "\"" + launcherSettings + "\""
                    }))
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Concat(new string[]
                {
                    ex.Message,
                    "\n\nPath search by launcher:\n\n",
                    launcherSettings,
                    "\n\n",
                    launcherExecutable
                }), "Unable to launch games");
            }
        }

        private void DoRefreshCategories(CApp app)
        {
            if (!string.IsNullOrEmpty(app.Category))
            {
                bool CatDup = false;
                foreach (string s in AvailableCategories)
                {
                    if (s == app.Category)
                    {
                        CatDup = true;
                        break;
                    }
                }

                if (CatDup == false)
                {
                    AvailableCategories.Add(app.Category);
                }
            }
        }

        private void DoEditGame()
        {
            if (lstApps.SelectedItems.Count == 0) return;

            ListViewItem lvi = lstApps.SelectedItems[0];
            CApp app = m_AppList.GetApp(lvi);

            if (app == null)
            {
                return;
            }

            FrmAppSetting appSetting = new FrmAppSetting();
            appSetting.CategoryList = AvailableCategories;
            appSetting.SetEditApp(app, Conf);
            DialogResult res = appSetting.ShowDialog();
            appSetting.Dispose();
            DoRefreshCategories(app);

            if (res == DialogResult.OK)
            {
                lvi.Text = app.GameName;

                // TODO: Unused icon will be stored in memory. It should be removed.
                lvi.ImageKey = app.GetIconHash();
                if (imgList.Images[app.GetIconHash()] == null)
                {
                    try
                    {
                        BackgroundWorker bwWorker = new BackgroundWorker();
                        bwWorker.DoWork += bwWorker_DoWork;
                        bwWorker.RunWorkerCompleted += bwWorker_RunWorkerCompleted;
                        bwWorker.RunWorkerAsync(new BgParam(lvi.ImageKey, app));
                    }
                    catch
                    {

                    }
                }

                SetListViewItemGroup(app, lvi);
                EditTrayLaunchMenu(app, false);
            }

            m_AppList.Save();
            lstApps.Sort();
        }

        private void DoDeleteGame()
        {
            if (lstApps.SelectedItems.Count == 0) return;

            if (MessageBox.Show("Are you sure you want to delete this item(s)?", "Delete Game From List", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            foreach (ListViewItem lvi in lstApps.SelectedItems)
            {
                m_AppList.DeleteApp(lvi);
            }
        }

        private void DoSorting()
        {
            switch (Conf.SortBy)
            {
                case CConfig.ESortBy.SortByName:
                    lstApps.Sorting = SortOrder.Ascending;
                    break;
                case CConfig.ESortBy.SortByDateAdded:
                    lstApps.Sorting = SortOrder.None;
                    break;
            }

            lstApps.Sort();
        }

        private void SetListViewItemGroup(CApp app, ListViewItem lvi)
        {
            switch (Conf.GroupBy)
            {
                case CConfig.EGroupBy.GroupByType:
                    {
                        ListViewGroup grpSteam = null, grpNonSteam = null;
                        if (lstApps.Groups.Count == 0)
                        {
                            grpSteam = new ListViewGroup("Steam", HorizontalAlignment.Left);
                            grpNonSteam = new ListViewGroup("Non-steam", HorizontalAlignment.Left);

                            lstApps.Groups.Add(grpSteam);
                            lstApps.Groups.Add(grpNonSteam);
                        }

                        foreach (ListViewGroup lvg in lstApps.Groups)
                        {
                            if (lvg.Header == "Steam")
                            {
                                grpSteam = lvg;
                            }
                            else
                            {
                                grpNonSteam = lvg;
                            }
                        }

                        lvi.Group = (app.AppId >= 0 ? grpSteam : grpNonSteam);
                    }
                    break;
                case CConfig.EGroupBy.GroupByCategory:
                    {
                        ListViewGroup grpCat = null;

                        if (!string.IsNullOrEmpty(app.Category))
                        {
                            foreach (ListViewGroup lvg in lstApps.Groups)
                            {
                                if (lvg.Header == app.Category)
                                {
                                    grpCat = lvg;
                                }
                            }

                            if (grpCat == null)
                            {
                                grpCat = new ListViewGroup(app.Category, HorizontalAlignment.Left);
                                lstApps.Groups.Add(grpCat);
                            }

                            lvi.Group = grpCat;
                        }
                    }
                    break;
            }
        }

        private void OnAddGame()
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Game executables (*.exe)|*.exe;*.bat;*.cmd;*.lnk|All Files|*.*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CApp app = new CApp();

                if (ofd.FileName.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using (ShellLink link = new ShellLink(ofd.FileName))
                        {
                            app.Path = CApp.MakeRelativePath(link.Target);
                            app.GameName = Path.GetFileNameWithoutExtension(app.Path);
                            app.StartIn = CApp.MakeRelativePath(link.WorkingDirectory);
                            app.CommandLine = link.Arguments;
                        }
                    }
                    catch
                    { }
                }
                else
                {
                    app.Path = CApp.MakeRelativePath(ofd.FileName);
                    app.GameName = Path.GetFileNameWithoutExtension(app.Path);
                    app.StartIn = CApp.MakeRelativePath(Path.GetDirectoryName(ofd.FileName));
                }

                if (File.Exists(Path.Combine(CApp.GetAbsolutePath(app.StartIn), "bin\\launcher.dll")) ||
                    File.Exists(Path.Combine(CApp.GetAbsolutePath(app.StartIn), "hl.exe")) ||
                    File.Exists(Path.Combine(CApp.GetAbsolutePath(app.StartIn), "hlds.exe")) ||
                    File.Exists(Path.Combine(CApp.GetAbsolutePath(app.StartIn), "hltv.exe")))
                {
                    app.CommandLine = "-steam";
                }

                AutoAppConfig(CApp.GetAbsolutePath(app.Path), app);
            }
        }

        void AutoAppConfig(string path, CApp app)
        {
            // Attempt to load app id from steam_appid.txt
            if (app.AppId == 0)
            {
                var appidFile = Path.Combine(app.StartIn, "steam_appid.txt");

                if (File.Exists(appidFile))
                {
                    try
                    {
                        var text = File.ReadAllText(appidFile);

                        if (text.Length > 0)
                        {
                            if (Int32.TryParse(text, out int id))
                            {
                                app.AppId = id;
                            }
                        }
                    }
                    catch
                    {
                        // Don't care about errors
                    }
                }
            }

            if (appinfoVDF == null)
            {
                FrmAppSetting appSetting = new FrmAppSetting();
                appSetting.CategoryList = AvailableCategories;
                appSetting.SetEditApp(app, Conf);

                DialogResult res = appSetting.ShowDialog();

                appSetting.Dispose();

                DoRefreshCategories(app);

                if (res == DialogResult.OK)
                {
                    m_AppList.AddApp(app);
                }

                m_AppList.Save();

                return;
            }

            List<CApp> SSEList = new List<CApp>();
            List<AppInfoItem> AIIList = new List<AppInfoItem>();
            FileInfo fi = new FileInfo(path);

            if (fi.Exists)
            {
                //Work out the directory the exe in is, it may not necessarily be the root install folder so we'll go back 3 dirs if possible!
                //note: i'm not assuming anything is in steampps\\common

                String DirName = fi.DirectoryName;
                List<AppConfig> AppConfigs = new List<AppConfig>();

                Int32 IDX = DirName.LastIndexOf("\\");
                if (IDX > -1)
                {
                    AppConfig ac = new AppConfig(DirName, DirName.Substring(IDX + 1), fi.FullName.Substring(DirName.Length + 1));
                    AppConfigs.Add(ac);
                    DirName = DirName.Substring(0, IDX);
                }

                IDX = DirName.LastIndexOf("\\");
                if (IDX > -1)
                {
                    AppConfig ac = new AppConfig(DirName, DirName.Substring(IDX + 1), fi.FullName.Substring(DirName.Length + 1));
                    AppConfigs.Add(ac);
                    DirName = DirName.Substring(0, IDX);
                }

                IDX = DirName.LastIndexOf("\\");
                if (IDX > -1)
                {
                    AppConfig ac = new AppConfig(DirName, DirName.Substring(IDX + 1), fi.FullName.Substring(DirName.Length + 1));
                    AppConfigs.Add(ac);
                    DirName = DirName.Substring(0, IDX);
                }

                foreach (AppConfig ac in AppConfigs)
                {
                    List<AppInfoItem> matchingApps = appinfoVDF.GetAppInfoItem(ac.Folder, ac.Exe);
                    if (matchingApps.Count > 0)
                    {
                        ac.Matched = true;
                        AIIList.AddRange(matchingApps);
                    }
                }

                foreach (AppInfoItem AII in AIIList)
                {
                    String gamedir = AII.AppInfoKey.GetKeyValue("gamedir");

                    foreach (AppConfig ac in AppConfigs)
                    {
                        if (ac.Matched)
                        {
                            AppInfoItemKey AIIK = AII.AppInfoKey.GetKey("launch", AII.AppInfoKey);
                            String Name = AII.AppInfoKey.GetKeyValue("name");
                            bool HasGameDir = Directory.Exists(Path.Combine(ac.Path, gamedir));

                            if (AIIK != null)
                            {
                                //launch options exist for this app
                                foreach (AppInfoItemKey AIIKL in AIIK.keys)
                                {
                                    String os = AIIKL.GetKeyValue("oslist").ToLower();

                                    if (os == "" || os.ToLower() == "windows")
                                    {
                                        String AppPath = ac.Path;
                                        CApp SSELO = new CApp(app);
                                        SSELO.AppId = (int)AII.AppID;
                                        //SSELO.DLCList = AII.AppInfoKey.GetKeyValue("listofdlc"); TODO

                                        //we could filter for windows os here if required
                                        //for now i'm just loading the key if the exe matches
                                        //this handles things like duke3d

                                        String WorkDir = AIIKL.GetKeyValue("workingdir");
                                        if (WorkDir.Length > 0)
                                            WorkDir = Path.Combine(AppPath, WorkDir);

                                        String Description = AIIKL.GetKeyValue("description");
                                        String NameAsDesc = Name;
                                        if (Description.Length > 0)
                                            NameAsDesc = Description;

                                        SSELO.Path = Path.Combine(AppPath, AIIKL.GetKeyValue("executable"));
                                        SSELO.CommandLine = AIIKL.GetKeyValue("arguments");
                                        SSELO.GameName = NameAsDesc;
                                        SSELO.StartIn = (String.IsNullOrEmpty(WorkDir) ? AppPath : WorkDir);
                                        SSELO.HasGameDir = HasGameDir;
                                        SSEList.Add(SSELO);
                                    }
                                }

                                if (SSEList.Count == 1)
                                {
                                    SSEList[0].GameName = Name;
                                }
                            }
                        }
                    }
                }
            }

            if (SSEList.Count <= 1)
            {
                try
                {
                    app.Copy(SSEList[0]);
                }
                catch
                { }

                FrmAppSetting appSetting = new FrmAppSetting();
                appSetting.CategoryList = AvailableCategories;
                appSetting.SetEditApp(app, Conf);
                DialogResult res = appSetting.ShowDialog();
                appSetting.Dispose();
                DoRefreshCategories(app);

                if (res == DialogResult.OK)
                {
                    m_AppList.AddApp(app);
                }
            }
            else
            {
                DialogResult res;
                FrmAppMulti appMulti = new FrmAppMulti();
                appMulti.Apps = SSEList;
                res = appMulti.ShowDialog();

                if (res == DialogResult.Yes)
                {
                    foreach (CApp selectedApp in appMulti.SelectedApps)
                    {
                        m_AppList.AddApp(selectedApp);
                    }
                }
                else if (res == DialogResult.No)
                {
                    if (appMulti.SelectedApp != null)
                    {
                        app.Copy(appMulti.SelectedApp);
                    }

                    FrmAppSetting appSetting = new FrmAppSetting();
                    appSetting.CategoryList = AvailableCategories;
                    appSetting.SetEditApp(app, Conf);
                    res = appSetting.ShowDialog();
                    appSetting.Dispose();
                    DoRefreshCategories(app);

                    if (res == DialogResult.OK)
                    {
                        m_AppList.AddApp(app);
                    }
                }

                appMulti.Dispose();
            }

            m_AppList.Save();
        }

        private void addGamesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OnAddGame();
        }

        private void editGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoEditGame();
        }

        private void deleteGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoDeleteGame();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSettings settings = new FrmSettings();
            settings.SetConfig(Conf);
            settings.ShowDialog();
            settings.Dispose();

            m_AppList.Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoEditGame();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoDeleteGame();
        }

        private void launchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LaunchApp();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAbout about = new FrmAbout();
            about.ShowDialog();
            about.Dispose();
        }

        private void addGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void launchNormallywithoutEmuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstApps.SelectedItems.Count == 0) return;

            ListViewItem lvi = lstApps.SelectedItems[0];
            CApp app = m_AppList.GetApp(lvi);

            LaunchWithoutEmu(app);
        }

        private void LaunchWithoutEmu(CApp app)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = CApp.GetAbsolutePath(app.Path);
            startInfo.WorkingDirectory = CApp.GetAbsolutePath(app.StartIn);
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = app.CommandLine;
            //startInfo.Verb = "runas";

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {

                }
            }
            catch
            {

            }
        }

        private void largeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstApps.View = View.LargeIcon;
        }

        private void smallIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstApps.View = View.SmallIcon;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstApps.View = View.List;
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstApps.View = View.Tile;
        }

        private void lstApps_OnMouseUp(object sender, MouseEventArgs e)
        {
            OnSelectedAppChanged();
        }

        private void lstApps_KeyUp(object sender, KeyEventArgs e)
        {
            OnSelectedAppChanged();
        }

        private void addGameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OnAddGame();
        }

        private void createDesktopShortcutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem lvi = lstApps.SelectedItems[0];
                CApp app = m_AppList.GetApp(lvi);

                Regex pattern = new Regex("[\\\\/?:*?\"<>|]");

                string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string sanitizedName = pattern.Replace(app.GameName, "");
                string shortcutPath = Path.Combine(deskDir, sanitizedName + ".lnk");

                using (ShellLink link = new ShellLink())
                {
                    if (app.AppId != -1)
                    {
                        link.Target = Application.ExecutablePath;
                        link.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                        link.Arguments = "-appid " + app.AppId;
                        link.IconPath = CApp.GetAbsolutePath(String.IsNullOrEmpty(app.IconPath) ? app.Path : app.IconPath);
                        link.Description = "Play " + app.GameName;
                        link.Save(shortcutPath);
                    }
                    else
                    {
                        link.Target = CApp.GetAbsolutePath(app.Path);
                        link.WorkingDirectory = Path.GetDirectoryName(CApp.GetAbsolutePath(app.Path));
                        link.Arguments = app.CommandLine;
                        link.IconPath = CApp.GetAbsolutePath(String.IsNullOrEmpty(app.IconPath) ? app.Path : app.IconPath);
                        link.Description = "Run " + app.GameName;
                        link.Save(shortcutPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error creating shortcut");
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstApps.SelectedItems.Count > 0)
            {
                lstApps.SelectedItems[0].BeginEdit();
            }
        }

        delegate void sort();
        private void lstApps_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            CApp app = m_AppList.GetApp(lstApps.Items[e.Item]);
            if (app == null) return;

            if (String.IsNullOrEmpty(e.Label) || String.IsNullOrWhiteSpace(e.Label))
            {
                lstApps.Items[e.Item].Text = app.GameName;
                return;
            }

            app.GameName = e.Label;

            EditTrayLaunchMenu(app, false);

            lstApps.BeginInvoke(new sort(lstApps.Sort));
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_AppList.Refresh();
        }

        private void nameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Conf.SortBy = CConfig.ESortBy.SortByName;
            DoSorting();
        }

        private void dateAddedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Conf.SortBy = CConfig.ESortBy.SortByDateAdded;
            DoSorting();
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Conf.GroupBy = CConfig.EGroupBy.GroupByNone;
            m_AppList.Refresh();
        }

        private void typeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Conf.GroupBy = CConfig.EGroupBy.GroupByType;
            m_AppList.Refresh();
        }

        private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Conf.GroupBy = CConfig.EGroupBy.GroupByCategory;
            m_AppList.Refresh();
        }

        private void FrmMain_Closing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Icon = null; //ensures icon will be removed correctly from the tray

            if (this.WindowState == FormWindowState.Normal)
            {
                Conf.WindowSizeX = this.Size.Width;
                Conf.WindowSizeY = this.Size.Height;
                Conf.WindowPosX = this.Location.X;
                Conf.WindowPosY = this.Location.Y;
            }
            else
            {
                Conf.WindowSizeX = this.RestoreBounds.Size.Width;
                Conf.WindowSizeY = this.RestoreBounds.Size.Height;
                Conf.WindowPosX = this.RestoreBounds.Location.X;
                Conf.WindowPosY = this.RestoreBounds.Location.Y;
            }
        }

        private void hideMissingShortcutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Conf.HideMissingShortcut = !Conf.HideMissingShortcut;
            m_AppList.Refresh();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Visible = true;
            WindowState = LastWindowState;
            Activate();
        }

        private void NotifyMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            string mnuText = menu.Text;

            switch (mnuText)
            {
                case "About":
                    aboutToolStripMenuItem_Click(sender, e);
                    break;
                case "Exit":
                    Application.Exit();
                    break;
                case "Settings":
                    settingsToolStripMenuItem_Click(sender, e);
                    break;
                case "Open":
                    Visible = true;
                    WindowState = LastWindowState;
                    Activate();
                    break;
                default:
                    //launch app
                    TrayLaunchApp(menu);
                    break;
            }

        }

        private void TrayLaunchApp(ToolStripMenuItem menuObject)
        {
            CApp app = m_AppList.GetApp(menuObject.Tag);

            if (app != null)
            {
                if (app.AppId == -1)
                {
                    LaunchWithoutEmu(app);
                }
                else
                {
                    WriteIniAndLaunch(app, Conf);
                }
            }
        }

        private void AddTrayLaunchMenu(CApp app)
        {
            ToolStripMenuItem mnuTrayContent;

            if (!string.IsNullOrWhiteSpace(app.Category)) //add by category
            {
                ToolStripItem[] mnuItmCategory = mnuTrayMenu.Items.Find(app.Category, true);
                if (mnuItmCategory.Length < 1)
                {
                    mnuTrayContent = new ToolStripMenuItem();
                    mnuTrayContent.Name = app.Category;
                    mnuTrayContent.Text = app.Category;
                    mnuTrayMenu.Items.Add(mnuTrayContent);
                }
                else
                {
                    mnuTrayContent = (ToolStripMenuItem)mnuItmCategory[0];
                }

                ToolStripMenuItem mnuItemContent = new ToolStripMenuItem();
                mnuItemContent.Name = app.GameName;
                mnuItemContent.Text = app.GameName;
                mnuItemContent.Tag = app.Tag;
                mnuItemContent.Image = imgList.Images[app.GetIconHash()];
                mnuItemContent.Click += new System.EventHandler(this.NotifyMenu_Click);
                mnuTrayContent.DropDownItems.Add(mnuItemContent);

                ResortToolStripItemCollection(mnuTrayContent.DropDownItems);
            }
            else
            {
                if (app.AppId == -1) //non steam
                {
                    ToolStripItem[] mnuItmNonSteam = mnuTrayMenu.Items.Find("Non-Steam", true);
                    if (mnuItmNonSteam.Length < 1)
                    {
                        mnuTrayContent = new ToolStripMenuItem();
                        mnuTrayContent.Name = "Non-Steam";
                        mnuTrayContent.Text = "Non-Steam";
                        mnuTrayMenu.Items.Add(mnuTrayContent);
                        mnuTrayMenu.Refresh();
                    }
                    else
                    {
                        mnuTrayContent = (ToolStripMenuItem)mnuItmNonSteam[0];
                    }

                    ToolStripMenuItem mnuItemContent = new ToolStripMenuItem();
                    mnuItemContent.Name = app.GameName;
                    mnuItemContent.Text = app.GameName;
                    mnuItemContent.Tag = app.Tag;
                    mnuItemContent.Image = imgList.Images[app.GetIconHash()];
                    mnuItemContent.Click += new System.EventHandler(this.NotifyMenu_Click);
                    mnuTrayContent.DropDownItems.Add(mnuItemContent);

                    ResortToolStripItemCollection(mnuTrayContent.DropDownItems);
                }
                else //steam
                {
                    ToolStripItem[] mnuItmSteam = mnuTrayMenu.Items.Find("Steam", true);
                    if (mnuItmSteam.Length < 1)
                    {
                        mnuTrayContent = new ToolStripMenuItem();
                        mnuTrayContent.Name = "Steam";
                        mnuTrayContent.Text = "Steam";
                        mnuTrayMenu.Items.Add(mnuTrayContent);
                    }
                    else
                    {
                        mnuTrayContent = (ToolStripMenuItem)mnuItmSteam[0];
                    }

                    ToolStripMenuItem mnuItemContent = new ToolStripMenuItem();
                    mnuItemContent.Name = app.GameName;
                    mnuItemContent.Text = app.GameName;
                    mnuItemContent.Tag = app.Tag;
                    mnuItemContent.Image = imgList.Images[app.GetIconHash()];
                    mnuItemContent.Click += new System.EventHandler(this.NotifyMenu_Click);
                    mnuTrayContent.DropDownItems.Add(mnuItemContent);

                    ResortToolStripItemCollection(mnuTrayContent.DropDownItems);
                }
            }

            if (!MenuFirstInit) SortTrayMenu();
        }

        private void ResortToolStripItemCollection(ToolStripItemCollection coll)
        {
            if (MenuFirstInit) return;

            System.Collections.ArrayList oAList = new System.Collections.ArrayList(coll);
            oAList.Sort(new ToolStripItemComparer());
            coll.Clear();

            foreach (ToolStripItem oItem in oAList)
            {
                coll.Add(oItem);
            }
        }

        public class ToolStripItemComparer : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                ToolStripItem oItem1 = (ToolStripItem)x;
                ToolStripItem oItem2 = (ToolStripItem)y;
                return string.Compare(oItem1.Text, oItem2.Text, true);
            }
        }

        private void SortTrayMenu()
        {
            System.Collections.ArrayList mnuList = new System.Collections.ArrayList();
            ToolStripItem steamMenu = null, nonSteamMenu = null;

            foreach (ToolStripItem mnu in mnuTrayMenu.Items)
            {
                if (mnu.GetType() == typeof(ToolStripSeparator))
                {
                    mnuList.Add(mnu);
                    continue;
                }

                switch (mnu.Text)
                {
                    case "Steam":
                        steamMenu = mnu;
                        break;
                    case "Non-Steam":
                        nonSteamMenu = mnu;
                        break;
                    case "About":
                    case "Exit":
                    case "Settings":
                    case "Open":
                        mnuList.Add(mnu);
                        break;
                }
            }

            // Add Steam and Non-Steam first
            if (steamMenu != null)
                mnuTrayMenu.Items.Add(steamMenu);
            if (nonSteamMenu != null)
                mnuTrayMenu.Items.Add(nonSteamMenu);

            foreach (ToolStripItem mnu in mnuList)
            {
                mnuTrayMenu.Items.Add(mnu);
            }
        }

        private void EditTrayLaunchMenu(CApp app, bool backgroundLoadingOnly)
        {
            var mnu = FindMenuByTag(app.Tag, mnuTrayMenu.Items);
            if (mnu == null)
                return;

            if (imgList.Images[app.GetIconHash()] != null)
                mnu.Image = imgList.Images[app.GetIconHash()];

            // The worker thread only loads background icon.
            if (backgroundLoadingOnly) return;

            mnu.Text = app.GameName;
            mnu.Name = app.GameName;

            if (mnu.OwnerItem != null)
            {
                ToolStripMenuItem parentMenu = (ToolStripMenuItem)mnu.OwnerItem;
                parentMenu.DropDownItems.Remove(mnu);

                if (parentMenu.DropDownItems.Count < 1)
                    mnuTrayMenu.Items.Remove(parentMenu);
            }

            // TODO: Duplicates code from AddTrayLaunchMenu
            ToolStripMenuItem mnuTrayContent;
            if (app.Category != null && app.Category != "")
            {
                ToolStripItem[] mnuItmCategory = mnuTrayMenu.Items.Find(app.Category, true);

                if (mnuItmCategory.Length < 1)
                {
                    mnuTrayContent = new ToolStripMenuItem();
                    mnuTrayContent.Name = app.Category;
                    mnuTrayContent.Text = app.Category;
                    mnuTrayMenu.Items.Add(mnuTrayContent);
                }
                else
                {
                    mnuTrayContent = (ToolStripMenuItem)mnuItmCategory[0];
                }
            }
            else
            {
                if (app.AppId == -1) //non steam
                {
                    ToolStripItem[] mnuItmNonSteam = mnuTrayMenu.Items.Find("Non-Steam", true);
                    if (mnuItmNonSteam.Length < 1)
                    {
                        mnuTrayContent = new ToolStripMenuItem();
                        mnuTrayContent.Name = "Non-Steam";
                        mnuTrayContent.Text = "Non-Steam";
                        mnuTrayMenu.Items.Add(mnuTrayContent);
                        mnuTrayMenu.Refresh();
                    }
                    else
                    {
                        mnuTrayContent = (ToolStripMenuItem)mnuItmNonSteam[0];
                    }
                }
                else //steam
                {
                    ToolStripItem[] mnuItmSteam = mnuTrayMenu.Items.Find("Steam", true);
                    if (mnuItmSteam.Length < 1)
                    {
                        mnuTrayContent = new ToolStripMenuItem();
                        mnuTrayContent.Name = "Steam";
                        mnuTrayContent.Text = "Steam";
                        mnuTrayMenu.Items.Add(mnuTrayContent);
                    }
                    else
                    {
                        mnuTrayContent = (ToolStripMenuItem)mnuItmSteam[0];
                    }
                }
            }

            if (mnuTrayContent != null)
                mnuTrayContent.DropDownItems.Add(mnu);

            if (mnu.OwnerItem != null)
                ResortToolStripItemCollection(((ToolStripMenuItem)mnu.OwnerItem).DropDownItems);

            SortTrayMenu();
        }

        private void DeleteTrayLaunchMenu(Object tag)
        {
            ToolStripItem mnuItem = FindMenuByTag(tag, mnuTrayMenu.Items);
            if (mnuItem == null)
                return;

            var parentItem = mnuItem.OwnerItem;

            if (parentItem == null)
            {
                mnuTrayMenu.Items.Remove(mnuItem);
            }
            else
            {
                ToolStripMenuItem parentMenu = (ToolStripMenuItem)parentItem;
                parentMenu.DropDownItems.Remove(mnuItem);
                if (parentMenu.DropDownItems.Count < 1)
                    mnuTrayMenu.Items.Remove(parentMenu);
            }
        }

        private void PopulateTrayLaunchMenu()
        {
            mnuTrayMenu.Items.Add("-");

            ToolStripMenuItem mnuTrayContent = new ToolStripMenuItem();
            mnuTrayContent.Text = "Open";
            mnuTrayContent.Click += new System.EventHandler(this.NotifyMenu_Click);
            mnuTrayMenu.Items.Add(mnuTrayContent);

            mnuTrayContent = new ToolStripMenuItem();
            mnuTrayContent.Text = "Settings";
            mnuTrayContent.Click += new System.EventHandler(this.NotifyMenu_Click);
            mnuTrayMenu.Items.Add(mnuTrayContent);

            mnuTrayMenu.Items.Add("-");
            mnuTrayContent = new ToolStripMenuItem();
            mnuTrayContent.Text = "About";
            mnuTrayContent.Click += new System.EventHandler(this.NotifyMenu_Click);
            mnuTrayMenu.Items.Add(mnuTrayContent);

            mnuTrayMenu.Items.Add("-");

            mnuTrayContent = new ToolStripMenuItem();
            mnuTrayContent.Text = "Exit";
            mnuTrayContent.Click += new System.EventHandler(this.NotifyMenu_Click);
            mnuTrayMenu.Items.Add(mnuTrayContent);

            notifyIcon1.ContextMenuStrip = mnuTrayMenu;
        }

        private ToolStripMenuItem FindMenuByTag(Object tag, ToolStripItemCollection menuItems)
        {
            foreach (ToolStripItem mnu in menuItems)
            {
                if (mnu.GetType() == typeof(ToolStripMenuItem))
                {
                    ToolStripMenuItem mnuItem = (ToolStripMenuItem)mnu;

                    if (mnuItem.Tag != null && mnuItem.Tag == tag)
                    {
                        return mnuItem;
                    }

                    if (mnuItem.DropDownItems.Count > 0)
                    {
                        ToolStripMenuItem childMenu = FindMenuByTag(tag, mnuItem.DropDownItems);
                        if (childMenu != null)
                            return childMenu;
                    }
                }
            }

            return null;
        }

        private void lstApps_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DoDeleteGame();
            }
        }

        private void openFileLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstApps.SelectedItems.Count == 0)
            {
                return;
            }

            ListViewItem tag = lstApps.SelectedItems[0];
            Process.Start(Path.GetDirectoryName(m_AppList.GetApp(tag).Path));
        }

        private string GetAppStoragePath(CApp app)
        {
            bool storageOnAppdata = (app.StorageOnAppdata == -1) ? Conf.StorageOnAppdata : Convert.ToBoolean(app.StorageOnAppdata);
            bool separateStorageByName = (app.SeparateStorageByName == -1) ? Conf.SeparateStorageByName : Convert.ToBoolean(app.SeparateStorageByName);

            var basePath = storageOnAppdata ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SmartSteamEmu");

            var path = Path.Combine(basePath, "SmartSteamEmu");

            if (separateStorageByName)
            {
                path = Path.Combine(path, Conf.PersonaName);
            }

            path = Path.Combine(path, app.AppId.ToString());

            return path;
        }

        private void openStorageLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstApps.SelectedItems.Count == 0)
            {
                return;
            }

            var tag = lstApps.SelectedItems[0];
            var app = m_AppList.GetApp(tag);

            var path = GetAppStoragePath(app);

            if (Directory.Exists(path))
            {
                Process.Start(path);
            }
            else
            {
                MessageBox.Show("SSE storage location doesn't exist (yet), have you ever started this game before?", "Error");
            }
        }

        private void openSaveLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstApps.SelectedItems.Count == 0)
            {
                return;
            }

            var tag = lstApps.SelectedItems[0];
            var app = m_AppList.GetApp(tag);

            var path = String.Empty;

            if (!string.IsNullOrWhiteSpace(app.RemoteStoragePath))
            {
                path = app.RemoteStoragePath;
            }
            else
            {
                path = Path.Combine(GetAppStoragePath(app), "remote");
            }

            if (Directory.Exists(path))
            {
                Process.Start(path);
            }
            else
            {
                MessageBox.Show("SSE save location doesn't exist (yet), have you ever started this game before?", "Error");
            }

        }
    }
}

