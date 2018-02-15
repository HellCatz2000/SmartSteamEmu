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
				FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "SmartSteamEmu\\SmartSteamEmu.dll"));
                lblVersionx86.Text = "SmartSteamEmu.dll version " + string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					versionInfo.FileMajorPart,
					versionInfo.FileMinorPart,
					versionInfo.FileBuildPart,
					versionInfo.FilePrivatePart
				});
			}
			catch
			{
                lblVersionx86.Text = "Unable to retrieve SmartSteamEmu.dll version";
			}
			try
			{
				FileVersionInfo versionInfo2 = FileVersionInfo.GetVersionInfo(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "SmartSteamEmu\\SmartSteamEmu64.dll"));
                lblVersionx64.Text = "SmartSteamEmu64.dll version " + string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					versionInfo2.FileMajorPart,
					versionInfo2.FileMinorPart,
					versionInfo2.FileBuildPart,
					versionInfo2.FilePrivatePart
				});
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
