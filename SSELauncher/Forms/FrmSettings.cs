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
		private CConfig m_Conf;

		private string m_TempAvatarPath;


		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
		private static extern uint GetPrivateProfileSectionNamesW(IntPtr lpszReturnBuffer, uint nSize, string lpFileName);

		public FrmSettings()
		{
            InitializeComponent();
		}

		public void SetConfig(CConfig config)
		{
            m_Conf = config;
            RepopulateLanguage();
            cmbEmuSteamId.SelectedIndex = -1;
            cmbEmuSteamId.Text = (string.Equals(config.SteamIdGeneration, "Manual", StringComparison.OrdinalIgnoreCase) ? config.ManualSteamId.ToString() : config.SteamIdGeneration);
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
                    pbAvatar.Image = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SmartSteamEmu\\SmartSteamEmu\\Common\\" + config.AvatarPath));
				}
				else
				{
                    pbAvatar.Image = Image.FromFile(m_TempAvatarPath);
				}
			}
			catch
			{
			}
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
			foreach (string current in config.MasterServerAddress)
			{
                lstMasterServer.Items.Add(current);
			}
            cmbEnableLog.Text = config.EnableLog.ToString();
            chkCleanLog.Checked = config.CleanLog;

            numNetListenPort.Value = config.ListenPort;
            numNetMaxPort.Value = config.MaximumPort;
            numNetDiscoveryInterval.Value = config.DiscoveryInterval;
            numNetMaxConn.Value = config.MaximumConnection;
			foreach (string current2 in config.BroadcastAddress)
			{
                lstNetBroadcast.Items.Add(current2);
			}
            chkAllowAnyToConnect.Checked = config.AllowAnyoneConnect;
            txtAdminPass.Text = config.AdminPass;
			foreach (string current3 in config.BanList)
			{
                lstBan.Items.Add(current3);
			}
		}

		private void RepopulateLanguage()
		{
			string lpFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SmartSteamEmu\\SmartSteamEmu\\Plugins\\SSEOverlay\\Language.ini");
			IntPtr intPtr = Marshal.AllocCoTaskMem(65534);
			uint privateProfileSectionNamesW = FrmSettings.GetPrivateProfileSectionNamesW(intPtr, 32767u, lpFileName);
			if (privateProfileSectionNamesW == 0u || privateProfileSectionNamesW == 32765u)
			{
				Marshal.FreeCoTaskMem(intPtr);
				return;
			}
			string text = Marshal.PtrToStringUni(intPtr, (int)privateProfileSectionNamesW).ToString();
			Marshal.FreeCoTaskMem(intPtr);
			string[] array = text.Substring(0, text.Length - 1).Split(new char[1]);
			if (array.Count<string>() > 0)
			{
                cmbOverlayLang.Items.Clear();
                cmbOverlayLang.Items.Add("");
                cmbOverlayLang.Items.Add("English");
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i];
					if (!text2.Equals("English", StringComparison.OrdinalIgnoreCase))
					{
                        cmbOverlayLang.Items.Add(text2);
					}
				}
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (string.Equals(cmbEmuSteamId.Text, "Static", StringComparison.OrdinalIgnoreCase) || string.Equals(cmbEmuSteamId.Text, "Random", StringComparison.OrdinalIgnoreCase) || string.Equals(cmbEmuSteamId.Text, "PersonaName", StringComparison.OrdinalIgnoreCase) || string.Equals(cmbEmuSteamId.Text, "ip", StringComparison.OrdinalIgnoreCase) || string.Equals(cmbEmuSteamId.Text, "GenerateRandom", StringComparison.OrdinalIgnoreCase))
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
            m_Conf.OnlineKey = (string.IsNullOrWhiteSpace(txtOnlineKey.Text) ? null : txtOnlineKey.Text);
            m_Conf.MasterServerAddress.Clear();
			foreach (string item in lstMasterServer.Items)
			{
                m_Conf.MasterServerAddress.Add(item);
			}
            m_Conf.EnableLog = Convert.ToBoolean(cmbEnableLog.Text);
            m_Conf.CleanLog = chkCleanLog.Checked;
            m_Conf.ListenPort = Convert.ToInt32(numNetListenPort.Value);
            m_Conf.MaximumPort = Convert.ToInt32(numNetMaxPort.Value);
            m_Conf.DiscoveryInterval = Convert.ToInt32(numNetDiscoveryInterval.Value);
            m_Conf.MaximumConnection = Convert.ToInt32(numNetMaxConn.Value);
            m_Conf.BroadcastAddress.Clear();
			foreach (string item2 in lstNetBroadcast.Items)
			{
                m_Conf.BroadcastAddress.Add(item2);
			}
            m_Conf.AllowAnyoneConnect = chkAllowAnyToConnect.Checked;
            m_Conf.AdminPass = txtAdminPass.Text;
            m_Conf.BanList.Clear();
			foreach (string item3 in lstBan.Items)
			{
                m_Conf.BanList.Add(item3);
			}
			base.DialogResult = DialogResult.OK;
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
