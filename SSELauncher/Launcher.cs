using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace SSELauncher
{
    class Launcher
    {
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
    }
}
