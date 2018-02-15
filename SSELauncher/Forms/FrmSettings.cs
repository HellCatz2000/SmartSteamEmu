using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SSELauncher
{
	public partial class FrmSettings : Form
	{
        CConfig m_Conf;
        string m_TempAvatarPath;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        static extern uint GetPrivateProfileSectionNamesW(IntPtr lpszReturnBuffer, uint nSize, string lpFileName);

        public FrmSettings()
		{
            InitializeComponent();
		}

		public void SetConfig(CConfig config)
		{
            m_Conf = config;

            RepopulateLanguage();

            // Basic Settings

            cmbEmuSteamId.SelectedIndex = -1;
            cmbEmuSteamId.Text = (String.Equals(config.SteamIdGeneration, "Manual", StringComparison.OrdinalIgnoreCase) ? config.ManualSteamId.ToString() : config.SteamIdGeneration);
            cmbEmuPersonaName.SelectedIndex = -1;
            cmbEmuPersonaName.Text = config.PersonaName;
            cmbEmuQuickJoin.SelectedIndex = -1;
            cmbEmuQuickJoin.Text = config.QuickJoinHotkey;
            m_TempAvatarPath = CApp.GetAbsolutePath(config.AvatarPath);
            cmbParanoidMode.Text = m_Conf.ParanoidMode.ToString();
            chkHideToTray.Checked = m_Conf.HideToTray;
            cmbLang.Text = m_Conf.Language;
            cmbOverlay.Text = m_Conf.EnableOverlay.ToString();
            cmbOnlinePlay.Text = m_Conf.EnableOnlinePlay.ToString();
            cmbOverlayLang.Text = m_Conf.OverlayLanguage;
            cmbOverlayScreenshot.SelectedIndex = -1;
            cmbOverlayScreenshot.Text = m_Conf.OverlayScreenshotHotkey;

            try
            {
				if (config.AvatarPath == "avatar.png")
				{
                    pbAvatar.Image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"SmartSteamEmu\SmartSteamEmu\Common\" + config.AvatarPath));
				}
				else
				{
                    pbAvatar.Image = Image.FromFile(m_TempAvatarPath);
				}
			}
			catch
			{
			}

            // Advanced Settings

            cmbAutoJoinInvite.Text = config.AutomaticallyJoinInvite.ToString();
            cmbStorageOnAppData.Text = config.StorageOnAppdata.ToString();
            cmbSeparateStorageByName.Text = config.SeparateStorageByName.ToString();
            cmbSecuredServer.Text = config.SecuredServer.ToString();
            cmbDisableFriendList.Text = config.DisableFriendList.ToString();
            cmbDisableLeaderboard.Text = config.DisableLeaderboard.ToString();
            cmbEnableHttp.Text = config.EnableHTTP.ToString();
            cmbEnableIngameVoice.Text = config.EnableInGameVoice.ToString();
            cmbEnableLobbyFilter.Text = config.EnableLobbyFilter.ToString();
            cmbEnableVR.Text = config.EnableVR.ToString();
            cmbOffline.Text = config.Offline.ToString();
            txtOnlineKey.Text = config.OnlineKey;
            foreach (string ipport in config.MasterServerAddress)
            {
                lstMasterServer.Items.Add(ipport);
            }

            // Debugging

            cmbEnableLog.Text = config.EnableLog.ToString();
            chkCleanLog.Checked = config.CleanLog;

            // Networking

            numNetListenPort.Value = config.ListenPort;
            numNetMaxPort.Value = config.MaximumPort;
            numNetDiscoveryInterval.Value = config.DiscoveryInterval;
            numNetMaxConn.Value = config.MaximumConnection;
            foreach (string addr in config.BroadcastAddress)
            {
                lstNetBroadcast.Items.Add(addr);
            }

            // Player Management

            chkAllowAnyToConnect.Checked = config.AllowAnyoneConnect;
            txtAdminPass.Text = config.AdminPass;
            foreach (string steamid in config.BanList)
            {
                lstBan.Items.Add(steamid);
            }
        }

		private void RepopulateLanguage()
		{
            string AppPath = AppDomain.CurrentDomain.BaseDirectory;
            string langFilePath = Path.Combine(AppPath, @"SmartSteamEmu\SmartSteamEmu\Plugins\SSEOverlay\Language.ini");

            const uint MAX_BUFFER = 32767;
            IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)MAX_BUFFER * 2);
            uint bytesReturned = GetPrivateProfileSectionNamesW(pReturnedString, MAX_BUFFER, langFilePath);
            if (bytesReturned == 0 || bytesReturned == MAX_BUFFER - 2)
            {
                Marshal.FreeCoTaskMem(pReturnedString);
                return;
            }

            string local = Marshal.PtrToStringUni(pReturnedString, (int)bytesReturned).ToString();
            Marshal.FreeCoTaskMem(pReturnedString);

            string[] langList = local.Substring(0, local.Length - 1).Split('\0');
            if (langList.Count() > 0)
            {
                cmbOverlayLang.Items.Clear();
                cmbOverlayLang.Items.Add("");
                cmbOverlayLang.Items.Add("English");
                foreach (string s in langList)
                {
                    if (!s.Equals("English", StringComparison.OrdinalIgnoreCase))
                    {
                        cmbOverlayLang.Items.Add(s);
                    }
                }
            }
        }

		private void btnOK_Click(object sender, EventArgs e)
		{
            // Basic Settings

            if (String.Equals(cmbEmuSteamId.Text, "Static", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(cmbEmuSteamId.Text, "Random", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(cmbEmuSteamId.Text, "PersonaName", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(cmbEmuSteamId.Text, "ip", StringComparison.OrdinalIgnoreCase) ||
                String.Equals(cmbEmuSteamId.Text, "GenerateRandom", StringComparison.OrdinalIgnoreCase))
            {
                m_Conf.SteamIdGeneration = cmbEmuSteamId.Text;
            }
            else
            {
                try
                {
                    m_Conf.ManualSteamId = Convert.ToInt64(cmbEmuSteamId.Text);
                    m_Conf.SteamIdGeneration = "Manual";
                }
                catch
                {
                    MessageBox.Show("Invalid steam id!", "Invalid input");
                }
            }

            m_Conf.PersonaName = cmbEmuPersonaName.Text;
            m_Conf.AvatarPath = CApp.MakeRelativePath(m_TempAvatarPath, false);
            m_Conf.QuickJoinHotkey = cmbEmuQuickJoin.Text;
            m_Conf.ParanoidMode = Convert.ToBoolean(cmbParanoidMode.Text);
            m_Conf.HideToTray = chkHideToTray.Checked;
            m_Conf.Language = cmbLang.Text;
            m_Conf.EnableOverlay = Convert.ToBoolean(cmbOverlay.Text);
            m_Conf.EnableOnlinePlay = Convert.ToBoolean(cmbOnlinePlay.Text);
            m_Conf.OverlayLanguage = cmbOverlayLang.Text;
            m_Conf.OverlayScreenshotHotkey = cmbOverlayScreenshot.Text;

            // Advanced Setting

            m_Conf.AutomaticallyJoinInvite = Convert.ToBoolean(cmbAutoJoinInvite.Text);
            m_Conf.StorageOnAppdata = Convert.ToBoolean(cmbStorageOnAppData.Text);
            m_Conf.SeparateStorageByName = Convert.ToBoolean(cmbSeparateStorageByName.Text);
            m_Conf.SecuredServer = Convert.ToBoolean(cmbSecuredServer.Text);
            m_Conf.DisableFriendList = Convert.ToBoolean(cmbDisableFriendList.Text);
            m_Conf.DisableLeaderboard = Convert.ToBoolean(cmbDisableLeaderboard.Text);
            m_Conf.EnableHTTP = Convert.ToBoolean(cmbEnableHttp.Text);
            m_Conf.EnableInGameVoice = Convert.ToBoolean(cmbEnableIngameVoice.Text);
            m_Conf.EnableLobbyFilter = Convert.ToBoolean(cmbEnableLobbyFilter.Text);
            m_Conf.EnableVR = Convert.ToBoolean(cmbEnableVR.Text);
            m_Conf.Offline = Convert.ToBoolean(cmbOffline.Text);
            m_Conf.OnlineKey = (String.IsNullOrWhiteSpace(txtOnlineKey.Text) ? null : txtOnlineKey.Text);
            m_Conf.MasterServerAddress.Clear();
            foreach (string ipport in lstMasterServer.Items)
            {
                m_Conf.MasterServerAddress.Add(ipport);
            }

            // Debugging

            m_Conf.EnableLog = Convert.ToBoolean(cmbEnableLog.Text);
            m_Conf.CleanLog = chkCleanLog.Checked;

            // Network Setting

            m_Conf.ListenPort = Convert.ToInt32(numNetListenPort.Value);
            m_Conf.MaximumPort = Convert.ToInt32(numNetMaxPort.Value);
            m_Conf.DiscoveryInterval = Convert.ToInt32(numNetDiscoveryInterval.Value);
            m_Conf.MaximumConnection = Convert.ToInt32(numNetMaxConn.Value);
            m_Conf.BroadcastAddress.Clear();
            foreach (string address in lstNetBroadcast.Items)
            {
                m_Conf.BroadcastAddress.Add(address);
            }

            // Player Management
            m_Conf.AllowAnyoneConnect = chkAllowAnyToConnect.Checked;
            m_Conf.AdminPass = txtAdminPass.Text;
            m_Conf.BanList.Clear();
            foreach (string steamid in lstBan.Items)
            {
                m_Conf.BanList.Add(steamid);
            }

            this.DialogResult = DialogResult.OK;
        }

		private void btnCancel_Click(object sender, EventArgs e)
		{
			base.DialogResult = DialogResult.Cancel;
		}

		private void FrmSettings_Load(object sender, EventArgs e)
		{
			base.AcceptButton = btnOK;
			base.CancelButton = btnCancel;
		}

		private void DoShowAvatarDlg()
		{
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Portable Network Graphics (*.png)|*.png|All Files|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				try
				{
                    m_TempAvatarPath = openFileDialog.FileName;
                    pbAvatar.Image = Image.FromFile(openFileDialog.FileName);
				}
				catch (Exception e)
				{
					MessageBox.Show(e.Message, "Unable to show image");
				}
			}
		}

		private void lblChangeAvatar_Click(object sender, EventArgs e)
		{
            DoShowAvatarDlg();
		}

		private void pbAvatar_Click(object sender, EventArgs e)
		{
            DoShowAvatarDlg();
		}

		private void btnNetAddIp_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtNetIp.Text) || string.IsNullOrWhiteSpace(txtNetIp.Text))
			{
				return;
			}

            lstNetBroadcast.Items.Add(txtNetIp.Text);
            txtNetIp.Text = "";
		}

		private void btnNetDelIp_Click(object sender, EventArgs e)
		{
			if (lstNetBroadcast.SelectedIndex == -1)
			{
				return;
			}

            lstNetBroadcast.Items.RemoveAt(lstNetBroadcast.SelectedIndex);
		}

		private void btnAddMasterServer_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtMasterServerIp.Text) || string.IsNullOrWhiteSpace(txtMasterServerIp.Text))
			{
				return;
			}

            lstMasterServer.Items.Add(txtMasterServerIp.Text);
            txtMasterServerIp.Text = "";
		}

		private void btnDelMasterServer_Click(object sender, EventArgs e)
		{
			if (lstMasterServer.SelectedIndex == -1)
			{
				return;
			}

            lstMasterServer.Items.RemoveAt(lstMasterServer.SelectedIndex);
		}

		private void btnBanAdd_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtBan.Text) || string.IsNullOrWhiteSpace(txtBan.Text))
			{
				return;
			}

            lstBan.Items.Add(txtBan.Text);
            txtBan.Text = "";
		}

		private void btnBanDel_Click(object sender, EventArgs e)
		{
			if (lstBan.SelectedIndex == -1)
			{
				return;
			}

            lstBan.Items.RemoveAt(lstBan.SelectedIndex);
		}

		private void enableDisableToolStripMenuItem_Click(object sender, EventArgs e)
		{
            Control control = null;

            if (sender is ToolStripItem toolStripItem)
            {
                if (toolStripItem.Owner is ContextMenuStrip contextMenuStrip)
                {
                    control = contextMenuStrip.SourceControl;
                }
            }

            if (control == null)
			{
				return;
			}

			ListBox listBox = (ListBox)control;

			if (listBox.SelectedIndex == -1)
			{
				return;
			}

			string text = listBox.Items[listBox.SelectedIndex].ToString();

			if (text.Length > 0 && text[0] == ';')
			{
				text = text.Substring(1);
			}
			else
			{
				text = ";" + text;
			}

			listBox.Items[listBox.SelectedIndex] = text;
			listBox.Refresh();
		}

		private void lstBan_DrawItem(object sender, DrawItemEventArgs e)
		{
			try
			{
				e.DrawBackground();
				Graphics graphics = e.Graphics;
				string text = lstBan.Items[e.Index].ToString();
				if (text.Length > 0 && text[0] == ';')
				{
					graphics.FillRectangle(new SolidBrush(Color.FromArgb(-31108)), e.Bounds);
				}
				graphics.DrawString(lstBan.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), new PointF((float)e.Bounds.X, (float)e.Bounds.Y));
				e.DrawFocusRectangle();
			}
			catch (Exception)
			{
			}
		}

		private void lstMasterServer_DrawItem(object sender, DrawItemEventArgs e)
		{
			try
			{
				e.DrawBackground();
				Graphics graphics = e.Graphics;
				string text = lstMasterServer.Items[e.Index].ToString();
				if (text.Length > 0 && text[0] == ';')
				{
					graphics.FillRectangle(new SolidBrush(Color.FromArgb(-31108)), e.Bounds);
				}
				graphics.DrawString(lstMasterServer.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), new PointF((float)e.Bounds.X, (float)e.Bounds.Y));
				e.DrawFocusRectangle();
			}
			catch (Exception)
			{
			}
		}

		private void lstNetBroadcast_DrawItem(object sender, DrawItemEventArgs e)
		{
			try
			{
				e.DrawBackground();
				Graphics graphics = e.Graphics;
				string text = lstNetBroadcast.Items[e.Index].ToString();
				if (text.Length > 0 && text[0] == ';')
				{
					graphics.FillRectangle(new SolidBrush(Color.FromArgb(-31108)), e.Bounds);
				}
				graphics.DrawString(lstNetBroadcast.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), new PointF((float)e.Bounds.X, (float)e.Bounds.Y));
				e.DrawFocusRectangle();
			}
			catch (Exception)
			{
			}
		}
	}
}
