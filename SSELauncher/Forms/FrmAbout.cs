using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SSELauncher
{
    public partial class FrmAbout : Form
	{
        public FrmAbout()
        {
            InitializeComponent();
        }

        private void FrmAbout_Load(object sender, EventArgs e)
		{
            try
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "SmartSteamEmu\\SmartSteamEmu.dll"));
                lblVersionx86.Text = String.Format("SmartSteamEmu.dll version {0}.{1}.{2}.{3}", versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart);
            }
            catch
            {
                lblVersionx86.Text = "Unable to retrieve SmartSteamEmu.dll version";
            }

            try
            {
                var versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "SmartSteamEmu\\SmartSteamEmu64.dll"));
                lblVersionx64.Text = String.Format("SmartSteamEmu64.dll version {0}.{1}.{2}.{3}", versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart);
            }
            catch
            {
                lblVersionx64.Text = "Unable to retrieve SmartSteamEmu64.dll version";
            }
        }

		private void btnClose_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		private void lblVisit_Click(object sender, EventArgs e)
		{
			Process.Start("http://cs.rin.ru/forum/viewtopic.php?f=29&t=62935");
		}

        private void lblVisitComfySource_Click(object sender, EventArgs e)
        {
            Process.Start("https://gitgud.io/softashell/SSELauncher-comfy-edition");
        }
    }
}
