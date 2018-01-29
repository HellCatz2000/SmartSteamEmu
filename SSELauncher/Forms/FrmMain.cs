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
using vbAccelerator.Components.Shell;

namespace SSELauncher
{
    public partial class FrmMain : Form
    {
        private delegate void sort();

        public class ToolStripItemComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                ToolStripItem arg_0D_0 = (ToolStripItem)x;
                ToolStripItem toolStripItem = (ToolStripItem)y;
                return string.Compare(arg_0D_0.Text, toolStripItem.Text, true);
            }
        }

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
            base.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            notifyIcon1.Icon = base.Icon;
            notifyIcon1.Text = Text;
            m_AppList.EventAppClear += new EventHandler<AppModifiedEventArgs>(OnAppClear);
            m_AppList.EventAppAdded += new EventHandler<AppModifiedEventArgs>(OnAppAdded);
            m_AppList.EventAppDeleted += new EventHandler<AppModifiedEventArgs>(OnAppDeleted);
            BackgroundWorker expr_D1 = new BackgroundWorker();
            expr_D1.DoWork += new DoWorkEventHandler(bwWorker_DoWorkInitVDF);
            expr_D1.RunWorkerAsync();
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
            Size = new Size((Conf.WindowSizeX == 0) ? Size.Width : Conf.WindowSizeX, (Conf.WindowSizeY == 0) ? Size.Height : Conf.WindowSizeY);
            Location = new Point((Conf.WindowPosX < 0) ? Location.X : Conf.WindowPosX, (Conf.WindowPosY < 0) ? Location.Y : Conf.WindowPosY);
            pbDrop.Visible = true;
            DragEnter += new DragEventHandler(lstApps_DragEnter);
            DragDrop += new DragEventHandler(lstApps_DragDrop);
            m_AppList.Refresh();
            OnSelectedAppChanged();
            MenuFirstInit = false;
            SortTrayMenu();
            FormFullyLoaded = true;

            AdjustSize();
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            if (!FormFullyLoaded)
            {
                return;
            }

            if (WindowState != FormWindowState.Minimized)
            {
                LastWindowState = WindowState;
            }

            if (Conf.HideToTray)
            {
                Visible = (WindowState != FormWindowState.Minimized);
            }

            AdjustSize();
        }

        private void AdjustSize() => pbDrop.SetBounds(-10, -25, Width + 10, Height + 25);

        private void lstApps_DoubleClick(object sender, EventArgs e) => LaunchApp();

        private void lstApps_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnSelectedAppChanged();
            if (e.KeyChar == '\r')
            {
                LaunchApp();
            }
        }

        private void OnSelectedAppChanged()
        {
            if (lstApps.SelectedItems.Count == 0)
            {
                largeIconToolStripMenuItem.Checked = (lstApps.View == View.LargeIcon);
                smallIconToolStripMenuItem.Checked = (lstApps.View == View.SmallIcon);
                listToolStripMenuItem.Checked = (lstApps.View == View.List);
                tileToolStripMenuItem.Checked = (lstApps.View == View.Tile);
                nameToolStripMenuItem.Checked = (Conf.SortBy == CConfig.ESortBy.SortByName);
                dateAddedToolStripMenuItem.Checked = (Conf.SortBy == CConfig.ESortBy.SortByDateAdded);
                noneToolStripMenuItem.Checked = (Conf.GroupBy == CConfig.EGroupBy.GroupByNone);
                typeToolStripMenuItem.Checked = (Conf.GroupBy == CConfig.EGroupBy.GroupByType);
                categoryToolStripMenuItem.Checked = (Conf.GroupBy == CConfig.EGroupBy.GroupByCategory);
                hideMissingShortcutToolStripMenuItem.Checked = Conf.HideMissingShortcut;
                lstApps.ContextMenuStrip = ctxMenuViewStrip;
                editGameToolStripMenuItem.Enabled = false;
                deleteGameToolStripMenuItem.Enabled = false;
                return;
            }
            lstApps.ContextMenuStrip = ctxMenuStrip;
            editGameToolStripMenuItem.Enabled = true;
            deleteGameToolStripMenuItem.Enabled = true;
            ListViewItem tag = lstApps.SelectedItems[0];
            CApp app = m_AppList.GetApp(tag);
            if (app != null)
            {
                launchNormallywithoutEmuToolStripMenuItem.Visible = (app.AppId != -1);
            }
        }

        private void lstApps_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void lstApps_DragDrop(object sender, DragEventArgs e)
        {
            string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                CApp cApp = new CApp
                {
                    Path = CApp.MakeRelativePath(text, true)
                };
                cApp.GameName = Path.GetFileNameWithoutExtension(cApp.Path);
                cApp.StartIn = CApp.MakeRelativePath(Path.GetDirectoryName(cApp.Path), true);
                if (text.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using (ShellLink shellLink = new ShellLink(text))
                        {
                            cApp.Path = CApp.MakeRelativePath(shellLink.Target, true);
                            cApp.GameName = Path.GetFileNameWithoutExtension(cApp.Path);
                            cApp.StartIn = CApp.MakeRelativePath(shellLink.WorkingDirectory, true);
                            cApp.CommandLine = shellLink.Arguments;
                        }
                    }
                    catch
                    {
                    }
                }
                if (File.Exists(Path.Combine(CApp.GetAbsolutePath(cApp.StartIn), "bin\\launcher.dll")) || File.Exists(Path.Combine(CApp.GetAbsolutePath(cApp.StartIn), "hl.exe")) || File.Exists(Path.Combine(CApp.GetAbsolutePath(cApp.StartIn), "hlds.exe")) || File.Exists(Path.Combine(CApp.GetAbsolutePath(cApp.StartIn), "hltv.exe")))
                {
                    cApp.CommandLine = "-steam";
                }

                AutoAppConfig(text, cApp);
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
            if (lstApps.SelectedItems.Count == 0)
            {
                return;
            }

            var tag = lstApps.SelectedItems[0];
            CApp app = m_AppList.GetApp(tag);

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

                return;
            }

            FrmMain.WriteIniAndLaunch(app, Conf, null);
        }

        private void OnAppClear(object sender, AppModifiedEventArgs e)
        {
            mnuTrayMenu.Items.Clear();
            PopulateTrayLaunchMenu();
            lstApps.Groups.Clear();
            lstApps.Clear();
            CConfig.ESortBy sortBy = Conf.SortBy;
            if (sortBy == CConfig.ESortBy.SortByName)
            {
                lstApps.Sorting = SortOrder.Ascending;
                return;
            }
            if (sortBy != CConfig.ESortBy.SortByDateAdded)
            {
                return;
            }
            lstApps.Sorting = SortOrder.None;
        }

        private void OnAppAdded(object sender, AppModifiedEventArgs e)
        {
            if (Conf.HideMissingShortcut && !new FileInfo(CApp.GetAbsolutePath(e.app.Path)).Exists)
            {
                return;
            }
            ListViewItem listViewItem = new ListViewItem(e.app.GameName);
            SetListViewItemGroup(e.app, listViewItem);
            DoRefreshCategories(e.app);
            try
            {
                listViewItem.ImageKey = e.app.GetIconHash();
                BackgroundWorker expr_6A = new BackgroundWorker();
                expr_6A.DoWork += new DoWorkEventHandler(bwWorker_DoWork);
                expr_6A.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwWorker_RunWorkerCompleted);
                expr_6A.RunWorkerAsync(new BgParam(listViewItem.GetHashCode().ToString(), e.app));
            }
            catch
            {
            }
            e.tag = lstApps.Items.Add(listViewItem);
            e.app.Tag = e.tag;
            AddTrayLaunchMenu(e.app);
            pbDrop.Visible = false;
            lstApps.Sort();
        }

        private void bwWorker_DoWorkInitVDF(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    if (registryKey != null)
                    {
                        RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Valve\\Steam");
                        if (registryKey2 != null)
                        {
                            SteamInstallPath = registryKey2.GetValue("InstallPath").ToString();
                            appinfoVDF = new AppInfoVDF(Path.Combine(SteamInstallPath, "appcache\\appinfo.vdf"));
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void bwWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BgParam bgParam = (BgParam)e.Result;
            if (bgParam.AppIcon != null)
            {
                imgList.Images.Add(bgParam.App.GetIconHash(), bgParam.AppIcon);
            }
            EditTrayLaunchMenu(bgParam.App, true);
        }

        private void bwWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BgParam bgParam = (BgParam)e.Argument;
            bgParam.AppIcon = CAppList.GetIcon(bgParam.App);
            e.Result = bgParam;
        }

        private void OnAppDeleted(object sender, AppModifiedEventArgs e)
        {
            DeleteTrayLaunchMenu(e.tag);
            lstApps.Items.Remove((ListViewItem)e.tag);
            if (lstApps.Items.Count > 0)
            {
                pbDrop.Visible = false;
                return;
            }
            pbDrop.Visible = true;
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
                bool flag = false;
                using (List<string>.Enumerator enumerator = AvailableCategories.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current == app.Category)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    AvailableCategories.Add(app.Category);
                }
            }
        }

        private void DoEditGame()
        {
            if (lstApps.SelectedItems.Count == 0)
            {
                return;
            }

            var listViewItem = lstApps.SelectedItems[0];

            CApp app = m_AppList.GetApp(listViewItem);

            if (app == null)
            {
                return;
            }

            var frmAppSetting = new FrmAppSetting
            {
                CategoryList = AvailableCategories
            };

            frmAppSetting.SetEditApp(app, Conf);

            DoRefreshCategories(app);

            if (frmAppSetting.ShowDialog() == DialogResult.OK)
            {
                listViewItem.Text = app.GameName;
                listViewItem.ImageKey = app.GetIconHash();
                if (imgList.Images[app.GetIconHash()] == null)
                {
                    try
                    {
                        var bw = new BackgroundWorker();

                        bw.DoWork += new DoWorkEventHandler(bwWorker_DoWork);
                        bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwWorker_RunWorkerCompleted);
                        bw.RunWorkerAsync(new BgParam(listViewItem.ImageKey, app));
                    }
                    catch
                    {
                    }
                }
                SetListViewItemGroup(app, listViewItem);
                EditTrayLaunchMenu(app, false);
            }
            m_AppList.Save();
            lstApps.Sort();
        }

        private void DoDeleteGame()
        {
            if (lstApps.SelectedItems.Count == 0)
            {
                return;
            }
            if (MessageBox.Show("Are you sure you want to delete this item(s)?", "Delete Game From List", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                return;
            }
            foreach (ListViewItem tag in lstApps.SelectedItems)
            {
                m_AppList.DeleteApp(tag);
            }
        }

        private void DoSorting()
        {
            CConfig.ESortBy sortBy = Conf.SortBy;
            if (sortBy != CConfig.ESortBy.SortByName)
            {
                if (sortBy == CConfig.ESortBy.SortByDateAdded)
                {
                    lstApps.Sorting = SortOrder.None;
                }
            }
            else
            {
                lstApps.Sorting = SortOrder.Ascending;
            }
            lstApps.Sort();
        }

        private void SetListViewItemGroup(CApp app, ListViewItem lvi)
        {
            CConfig.EGroupBy groupBy = Conf.GroupBy;
            if (groupBy == CConfig.EGroupBy.GroupByType)
            {
                ListViewGroup listViewGroup = null;
                ListViewGroup listViewGroup2 = null;
                if (lstApps.Groups.Count == 0)
                {
                    listViewGroup = new ListViewGroup("Steam", HorizontalAlignment.Left);
                    listViewGroup2 = new ListViewGroup("Non-steam", HorizontalAlignment.Left);
                    lstApps.Groups.Add(listViewGroup);
                    lstApps.Groups.Add(listViewGroup2);
                }
                foreach (ListViewGroup listViewGroup3 in lstApps.Groups)
                {
                    if (listViewGroup3.Header == "Steam")
                    {
                        listViewGroup = listViewGroup3;
                    }
                    else
                    {
                        listViewGroup2 = listViewGroup3;
                    }
                }
                lvi.Group = ((app.AppId >= 0) ? listViewGroup : listViewGroup2);
                return;
            }
            if (groupBy != CConfig.EGroupBy.GroupByCategory)
            {
                return;
            }
            ListViewGroup listViewGroup4 = null;
            if (!string.IsNullOrEmpty(app.Category))
            {
                foreach (ListViewGroup listViewGroup5 in lstApps.Groups)
                {
                    if (listViewGroup5.Header == app.Category)
                    {
                        listViewGroup4 = listViewGroup5;
                    }
                }
                if (listViewGroup4 == null)
                {
                    listViewGroup4 = new ListViewGroup(app.Category, HorizontalAlignment.Left);
                    lstApps.Groups.Add(listViewGroup4);
                }
                lvi.Group = listViewGroup4;
            }
        }

        private void OnAddGame()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Game executables (*.exe)|*.exe;*.bat;*.cmd;*.lnk|All Files|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                CApp cApp = new CApp();

                if (openFileDialog.FileName.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        using (ShellLink shellLink = new ShellLink(openFileDialog.FileName))
                        {
                            cApp.Path = CApp.MakeRelativePath(shellLink.Target, true);
                            cApp.GameName = Path.GetFileNameWithoutExtension(cApp.Path);
                            cApp.StartIn = CApp.MakeRelativePath(shellLink.WorkingDirectory, true);
                            cApp.CommandLine = shellLink.Arguments;
                        }
                        goto IL_DC;
                    }
                    catch
                    {
                        goto IL_DC;
                    }
                }

                cApp.Path = CApp.MakeRelativePath(openFileDialog.FileName, true);
                cApp.GameName = Path.GetFileNameWithoutExtension(cApp.Path);
                cApp.StartIn = CApp.MakeRelativePath(Path.GetDirectoryName(openFileDialog.FileName), true);

            IL_DC:
                if (File.Exists(Path.Combine(CApp.GetAbsolutePath(cApp.StartIn), "bin\\launcher.dll")) || File.Exists(Path.Combine(CApp.GetAbsolutePath(cApp.StartIn), "hl.exe")) || File.Exists(Path.Combine(CApp.GetAbsolutePath(cApp.StartIn), "hlds.exe")) || File.Exists(Path.Combine(CApp.GetAbsolutePath(cApp.StartIn), "hltv.exe")))
                {
                    cApp.CommandLine = "-steam";
                }

                AutoAppConfig(CApp.GetAbsolutePath(cApp.Path), cApp);
            }
        }

        private void AutoAppConfig(string path, CApp app)
        {
            if (appinfoVDF == null)
            {
                var frmAppSetting = new FrmAppSetting
                {
                    CategoryList = AvailableCategories
                };

                frmAppSetting.SetEditApp(app, Conf);

                DoRefreshCategories(app);

                if (frmAppSetting.ShowDialog() == DialogResult.OK)
                {
                    m_AppList.AddApp(app);
                }

                m_AppList.Save();

                return;
            }

            List<CApp> list = new List<CApp>();
            List<AppInfoItem> list2 = new List<AppInfoItem>();
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Exists)
            {
                string text = fileInfo.DirectoryName;
                List<AppConfig> list3 = new List<AppConfig>();
                int num = text.LastIndexOf("\\");
                if (num > -1)
                {
                    AppConfig item = new AppConfig(text, text.Substring(num + 1), fileInfo.FullName.Substring(text.Length + 1));
                    list3.Add(item);
                    text = text.Substring(0, num);
                }
                num = text.LastIndexOf("\\");
                if (num > -1)
                {
                    AppConfig item2 = new AppConfig(text, text.Substring(num + 1), fileInfo.FullName.Substring(text.Length + 1));
                    list3.Add(item2);
                    text = text.Substring(0, num);
                }
                num = text.LastIndexOf("\\");
                if (num > -1)
                {
                    AppConfig item3 = new AppConfig(text, text.Substring(num + 1), fileInfo.FullName.Substring(text.Length + 1));
                    list3.Add(item3);
                    text = text.Substring(0, num);
                }
                foreach (AppConfig current in list3)
                {
                    List<AppInfoItem> appInfoItem = appinfoVDF.GetAppInfoItem(current.Folder, current.Exe);
                    if (appInfoItem.Count > 0)
                    {
                        current.Matched = true;
                        list2.AddRange(appInfoItem);
                    }
                }
                foreach (AppInfoItem current2 in list2)
                {
                    string keyValue = current2.AppInfoKey.GetKeyValue("gamedir");
                    foreach (AppConfig current3 in list3)
                    {
                        if (current3.Matched)
                        {
                            AppInfoItemKey key = current2.AppInfoKey.GetKey("launch", current2.AppInfoKey);
                            string keyValue2 = current2.AppInfoKey.GetKeyValue("name");
                            bool hasGameDir = Directory.Exists(Path.Combine(current3.Path, keyValue));
                            if (key != null)
                            {
                                foreach (AppInfoItemKey current4 in key.keys)
                                {
                                    string text2 = current4.GetKeyValue("oslist").ToLower();
                                    if (text2 == "" || text2.ToLower() == "windows")
                                    {
                                        string path2 = current3.Path;
                                        CApp cApp = new CApp(app)
                                        {
                                            AppId = (int)current2.AppID
                                        };
                                        string text3 = current4.GetKeyValue("workingdir");
                                        if (text3.Length > 0)
                                        {
                                            text3 = Path.Combine(path2, text3);
                                        }
                                        string keyValue3 = current4.GetKeyValue("description");
                                        string gameName = keyValue2;
                                        if (keyValue3.Length > 0)
                                        {
                                            gameName = keyValue3;
                                        }
                                        cApp.Path = Path.Combine(path2, current4.GetKeyValue("executable"));
                                        cApp.CommandLine = current4.GetKeyValue("arguments");
                                        cApp.GameName = gameName;
                                        cApp.StartIn = (string.IsNullOrEmpty(text3) ? path2 : text3);
                                        cApp.HasGameDir = hasGameDir;
                                        list.Add(cApp);
                                    }
                                }
                                if (list.Count == 1)
                                {
                                    list[0].GameName = keyValue2;
                                }
                            }
                        }
                    }
                }
            }
            if (list.Count <= 1)
            {
                try
                {
                    app.Copy(list[0]);
                }
                catch
                {
                }

                var frmAppSetting = new FrmAppSetting
                {
                    CategoryList = AvailableCategories
                };

                frmAppSetting.SetEditApp(app, Conf);

                DoRefreshCategories(app);

                if (frmAppSetting.ShowDialog() == DialogResult.OK)
                {
                    m_AppList.AddApp(app);
                }
            }
            else
            {
                var frmAppMulti = new FrmAppMulti
                {
                    Apps = list
                };

                var dialogResult3 = frmAppMulti.ShowDialog();

                if (dialogResult3 == DialogResult.Yes)
                {
                    foreach (var currentApp in frmAppMulti.SelectedApps)
                    {
                        m_AppList.AddApp(currentApp);
                    }
                }
                if (dialogResult3 == DialogResult.No)
                {
                    if (frmAppMulti.SelectedApp != null)
                    {
                        app.Copy(frmAppMulti.SelectedApp);
                    }

                    var frmAppSetting = new FrmAppSetting
                    {
                        CategoryList = AvailableCategories
                    };

                    frmAppSetting.SetEditApp(app, Conf);

                    dialogResult3 = frmAppSetting.ShowDialog();

                    DoRefreshCategories(app);

                    if (dialogResult3 == DialogResult.OK)
                    {
                        m_AppList.AddApp(app);
                    }
                }
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
            FrmSettings expr_05 = new FrmSettings();
            expr_05.SetConfig(Conf);
            expr_05.ShowDialog();
            expr_05.Dispose();
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
            FrmAbout expr_05 = new FrmAbout();
            expr_05.ShowDialog();
            expr_05.Dispose();
        }

        private void addGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void launchNormallywithoutEmuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstApps.SelectedItems.Count == 0)
            {
                return;
            }
            ListViewItem tag = lstApps.SelectedItems[0];
            CApp app = m_AppList.GetApp(tag);
            LaunchWithoutEmu(app);
        }

        private void LaunchWithoutEmu(CApp app)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = true,
                FileName = CApp.GetAbsolutePath(app.Path),
                WorkingDirectory = CApp.GetAbsolutePath(app.StartIn),
                WindowStyle = ProcessWindowStyle.Normal,
                Arguments = app.CommandLine
            };
            try
            {
                using (Process.Start(processStartInfo))
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
                ListViewItem tag = lstApps.SelectedItems[0];
                CApp app = m_AppList.GetApp(tag);
                Regex arg_3C_0 = new Regex("[\\\\/?:*?\"<>|]");
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string str = arg_3C_0.Replace(app.GameName, "");
                string linkFile = Path.Combine(folderPath, str + ".lnk");
                using (ShellLink shellLink = new ShellLink())
                {
                    if (app.AppId != -1)
                    {
                        shellLink.Target = Application.ExecutablePath;
                        shellLink.WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                        shellLink.Arguments = "-appid " + app.AppId;
                        shellLink.IconPath = CApp.GetAbsolutePath(string.IsNullOrEmpty(app.IconPath) ? app.Path : app.IconPath);
                        shellLink.Description = "Play " + app.GameName;
                        shellLink.Save(linkFile);
                    }
                    else
                    {
                        shellLink.Target = CApp.GetAbsolutePath(app.Path);
                        shellLink.WorkingDirectory = Path.GetDirectoryName(CApp.GetAbsolutePath(app.Path));
                        shellLink.Arguments = app.CommandLine;
                        shellLink.IconPath = CApp.GetAbsolutePath(string.IsNullOrEmpty(app.IconPath) ? app.Path : app.IconPath);
                        shellLink.Description = "Run " + app.GameName;
                        shellLink.Save(linkFile);
                    }
                }
            }
            catch (Exception arg_177_0)
            {
                MessageBox.Show(arg_177_0.Message, "Error creating shortcut");
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstApps.SelectedItems.Count > 0)
            {
                lstApps.SelectedItems[0].BeginEdit();
            }
        }

        private void lstApps_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            CApp app = m_AppList.GetApp(lstApps.Items[e.Item]);
            if (app == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(e.Label) || string.IsNullOrWhiteSpace(e.Label))
            {
                lstApps.Items[e.Item].Text = app.GameName;
                return;
            }
            app.GameName = e.Label;
            EditTrayLaunchMenu(app, false);
            lstApps.BeginInvoke(new FrmMain.sort(lstApps.Sort));
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
            notifyIcon1.Icon = null;
            if (WindowState == FormWindowState.Normal)
            {
                Conf.WindowSizeX = Size.Width;
                Conf.WindowSizeY = Size.Height;
                Conf.WindowPosX = Location.X;
                Conf.WindowPosY = Location.Y;
                return;
            }
            Conf.WindowSizeX = base.RestoreBounds.Size.Width;
            Conf.WindowSizeY = base.RestoreBounds.Size.Height;
            Conf.WindowPosX = base.RestoreBounds.Location.X;
            Conf.WindowPosY = base.RestoreBounds.Location.Y;
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
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)sender;
            string text = toolStripMenuItem.Text;
            if (text == "About")
            {
                aboutToolStripMenuItem_Click(sender, e);
                return;
            }
            if (text == "Exit")
            {
                Application.Exit();
                return;
            }
            if (text == "Settings")
            {
                settingsToolStripMenuItem_Click(sender, e);
                return;
            }
            if (!(text == "Open"))
            {
                TrayLaunchApp(toolStripMenuItem);
                return;
            }
            Visible = true;
            WindowState = LastWindowState;
            Activate();
        }

        private void TrayLaunchApp(ToolStripMenuItem menuObject)
        {
            CApp app = m_AppList.GetApp(menuObject.Tag);
            if (app != null)
            {
                if (app.AppId == -1)
                {
                    LaunchWithoutEmu(app);
                    return;
                }
                FrmMain.WriteIniAndLaunch(app, Conf, null);
            }
        }

        private void AddTrayLaunchMenu(CApp app)
        {
            if (!string.IsNullOrWhiteSpace(app.Category))
            {
                ToolStripItem[] array = mnuTrayMenu.Items.Find(app.Category, true);
                ToolStripMenuItem toolStripMenuItem;
                if (array.Length < 1)
                {
                    toolStripMenuItem = new ToolStripMenuItem
                    {
                        Name = app.Category,
                        Text = app.Category
                    };

                    mnuTrayMenu.Items.Add(toolStripMenuItem);
                }
                else
                {
                    toolStripMenuItem = (ToolStripMenuItem)array[0];
                }
                ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem
                {
                    Name = app.GameName,
                    Text = app.GameName,
                    Tag = app.Tag,
                    Image = imgList.Images[app.GetIconHash()]
                };

                toolStripMenuItem2.Click += new EventHandler(NotifyMenu_Click);
                toolStripMenuItem.DropDownItems.Add(toolStripMenuItem2);
                ResortToolStripItemCollection(toolStripMenuItem.DropDownItems);
            }
            else if (app.AppId == -1)
            {
                ToolStripItem[] array2 = mnuTrayMenu.Items.Find("Non-Steam", true);
                ToolStripMenuItem toolStripMenuItem;
                if (array2.Length < 1)
                {
                    toolStripMenuItem = new ToolStripMenuItem();
                    toolStripMenuItem.Name = "Non-Steam";
                    toolStripMenuItem.Text = "Non-Steam";
                    mnuTrayMenu.Items.Add(toolStripMenuItem);
                    mnuTrayMenu.Refresh();
                }
                else
                {
                    toolStripMenuItem = (ToolStripMenuItem)array2[0];
                }
                ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem();
                toolStripMenuItem3.Name = app.GameName;
                toolStripMenuItem3.Text = app.GameName;
                toolStripMenuItem3.Tag = app.Tag;
                toolStripMenuItem3.Image = imgList.Images[app.GetIconHash()];
                toolStripMenuItem3.Click += new EventHandler(NotifyMenu_Click);
                toolStripMenuItem.DropDownItems.Add(toolStripMenuItem3);
                ResortToolStripItemCollection(toolStripMenuItem.DropDownItems);
            }
            else
            {
                ToolStripItem[] array3 = mnuTrayMenu.Items.Find("Steam", true);
                ToolStripMenuItem toolStripMenuItem;
                if (array3.Length < 1)
                {
                    toolStripMenuItem = new ToolStripMenuItem();
                    toolStripMenuItem.Name = "Steam";
                    toolStripMenuItem.Text = "Steam";
                    mnuTrayMenu.Items.Add(toolStripMenuItem);
                }
                else
                {
                    toolStripMenuItem = (ToolStripMenuItem)array3[0];
                }
                ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem();
                toolStripMenuItem4.Name = app.GameName;
                toolStripMenuItem4.Text = app.GameName;
                toolStripMenuItem4.Tag = app.Tag;
                toolStripMenuItem4.Image = imgList.Images[app.GetIconHash()];
                toolStripMenuItem4.Click += new EventHandler(NotifyMenu_Click);
                toolStripMenuItem.DropDownItems.Add(toolStripMenuItem4);
                ResortToolStripItemCollection(toolStripMenuItem.DropDownItems);
            }
            if (!MenuFirstInit)
            {
                SortTrayMenu();
            }
        }

        private void ResortToolStripItemCollection(ToolStripItemCollection coll)
        {
            if (MenuFirstInit)
            {
                return;
            }
            ArrayList expr_0F = new ArrayList(coll);
            expr_0F.Sort(new FrmMain.ToolStripItemComparer());
            coll.Clear();
            foreach (ToolStripItem value in expr_0F)
            {
                coll.Add(value);
            }
        }

        private void SortTrayMenu()
        {
            ArrayList arrayList = new ArrayList();
            ToolStripItem toolStripItem = null;
            ToolStripItem toolStripItem2 = null;
            foreach (ToolStripItem toolStripItem3 in mnuTrayMenu.Items)
            {
                if (toolStripItem3.GetType() == typeof(ToolStripSeparator))
                {
                    arrayList.Add(toolStripItem3);
                }
                else
                {
                    string text = toolStripItem3.Text;
                    if (!(text == "Steam"))
                    {
                        if (!(text == "Non-Steam"))
                        {
                            if (text == "About" || text == "Exit" || text == "Settings" || text == "Open")
                            {
                                arrayList.Add(toolStripItem3);
                            }
                        }
                        else
                        {
                            toolStripItem2 = toolStripItem3;
                        }
                    }
                    else
                    {
                        toolStripItem = toolStripItem3;
                    }
                }
            }
            if (toolStripItem != null)
            {
                mnuTrayMenu.Items.Add(toolStripItem);
            }
            if (toolStripItem2 != null)
            {
                mnuTrayMenu.Items.Add(toolStripItem2);
            }
            foreach (ToolStripItem value in arrayList)
            {
                mnuTrayMenu.Items.Add(value);
            }
        }

        private void EditTrayLaunchMenu(CApp app, bool backgroundLoadingOnly)
        {
            ToolStripMenuItem toolStripMenuItem = FindMenuByTag(app.Tag, mnuTrayMenu.Items);
            if (toolStripMenuItem == null)
            {
                return;
            }
            if (imgList.Images[app.GetIconHash()] != null)
            {
                toolStripMenuItem.Image = imgList.Images[app.GetIconHash()];
            }
            if (backgroundLoadingOnly)
            {
                return;
            }
            toolStripMenuItem.Text = app.GameName;
            toolStripMenuItem.Name = app.GameName;
            if (toolStripMenuItem.OwnerItem != null)
            {
                ToolStripMenuItem toolStripMenuItem2 = (ToolStripMenuItem)toolStripMenuItem.OwnerItem;
                toolStripMenuItem2.DropDownItems.Remove(toolStripMenuItem);
                if (toolStripMenuItem2.DropDownItems.Count < 1)
                {
                    mnuTrayMenu.Items.Remove(toolStripMenuItem2);
                }
            }
            ToolStripMenuItem toolStripMenuItem3;
            if (app.Category != null && app.Category != "")
            {
                ToolStripItem[] array = mnuTrayMenu.Items.Find(app.Category, true);
                if (array.Length < 1)
                {
                    toolStripMenuItem3 = new ToolStripMenuItem();
                    toolStripMenuItem3.Name = app.Category;
                    toolStripMenuItem3.Text = app.Category;
                    mnuTrayMenu.Items.Add(toolStripMenuItem3);
                }
                else
                {
                    toolStripMenuItem3 = (ToolStripMenuItem)array[0];
                }
            }
            else if (app.AppId == -1)
            {
                ToolStripItem[] array2 = mnuTrayMenu.Items.Find("Non-Steam", true);
                if (array2.Length < 1)
                {
                    toolStripMenuItem3 = new ToolStripMenuItem();
                    toolStripMenuItem3.Name = "Non-Steam";
                    toolStripMenuItem3.Text = "Non-Steam";
                    mnuTrayMenu.Items.Add(toolStripMenuItem3);
                    mnuTrayMenu.Refresh();
                }
                else
                {
                    toolStripMenuItem3 = (ToolStripMenuItem)array2[0];
                }
            }
            else
            {
                ToolStripItem[] array3 = mnuTrayMenu.Items.Find("Steam", true);
                if (array3.Length < 1)
                {
                    toolStripMenuItem3 = new ToolStripMenuItem();
                    toolStripMenuItem3.Name = "Steam";
                    toolStripMenuItem3.Text = "Steam";
                    mnuTrayMenu.Items.Add(toolStripMenuItem3);
                }
                else
                {
                    toolStripMenuItem3 = (ToolStripMenuItem)array3[0];
                }
            }
            if (toolStripMenuItem3 != null)
            {
                toolStripMenuItem3.DropDownItems.Add(toolStripMenuItem);
            }
            if (toolStripMenuItem.OwnerItem != null)
            {
                ResortToolStripItemCollection(((ToolStripMenuItem)toolStripMenuItem.OwnerItem).DropDownItems);
            }
            SortTrayMenu();
        }

        private void DeleteTrayLaunchMenu(object tag)
        {
            ToolStripItem toolStripItem = FindMenuByTag(tag, mnuTrayMenu.Items);
            if (toolStripItem == null)
            {
                return;
            }
            ToolStripItem ownerItem = toolStripItem.OwnerItem;
            if (ownerItem == null)
            {
                mnuTrayMenu.Items.Remove(toolStripItem);
                return;
            }
            ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)ownerItem;
            toolStripMenuItem.DropDownItems.Remove(toolStripItem);
            if (toolStripMenuItem.DropDownItems.Count < 1)
            {
                mnuTrayMenu.Items.Remove(toolStripMenuItem);
            }
        }

        private void PopulateTrayLaunchMenu()
        {
            mnuTrayMenu.Items.Add("-");
            ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem.Text = "Open";
            toolStripMenuItem.Click += new EventHandler(NotifyMenu_Click);
            mnuTrayMenu.Items.Add(toolStripMenuItem);
            toolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem.Text = "Settings";
            toolStripMenuItem.Click += new EventHandler(NotifyMenu_Click);
            mnuTrayMenu.Items.Add(toolStripMenuItem);
            mnuTrayMenu.Items.Add("-");
            toolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem.Text = "About";
            toolStripMenuItem.Click += new EventHandler(NotifyMenu_Click);
            mnuTrayMenu.Items.Add(toolStripMenuItem);
            mnuTrayMenu.Items.Add("-");
            toolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem.Text = "Exit";
            toolStripMenuItem.Click += new EventHandler(NotifyMenu_Click);
            mnuTrayMenu.Items.Add(toolStripMenuItem);
            notifyIcon1.ContextMenuStrip = mnuTrayMenu;
        }

        private ToolStripMenuItem FindMenuByTag(object tag, ToolStripItemCollection menuItems)
        {
            foreach (ToolStripItem toolStripItem in menuItems)
            {
                if (toolStripItem.GetType() == typeof(ToolStripMenuItem))
                {
                    ToolStripMenuItem toolStripMenuItem = (ToolStripMenuItem)toolStripItem;
                    if (toolStripMenuItem.Tag != null && toolStripMenuItem.Tag == tag)
                    {
                        ToolStripMenuItem result = toolStripMenuItem;
                        return result;
                    }
                    if (toolStripMenuItem.DropDownItems.Count > 0)
                    {
                        ToolStripMenuItem toolStripMenuItem2 = FindMenuByTag(tag, toolStripMenuItem.DropDownItems);
                        if (toolStripMenuItem2 != null)
                        {
                            ToolStripMenuItem result = toolStripMenuItem2;
                            return result;
                        }
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
            var path = String.Empty;

            bool storageOnAppdata = (app.StorageOnAppdata == -1) ? Conf.StorageOnAppdata : Convert.ToBoolean(app.StorageOnAppdata);
            bool separateStorageByName = (app.SeparateStorageByName == -1) ? Conf.SeparateStorageByName : Convert.ToBoolean(app.SeparateStorageByName);

            var basePath = storageOnAppdata ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SmartSteamEmu");

            path = Path.Combine(basePath, "SmartSteamEmu");

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
