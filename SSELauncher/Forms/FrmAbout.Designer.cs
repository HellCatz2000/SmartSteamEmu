namespace SSELauncher
{
    partial class FrmAbout
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblVisit = new System.Windows.Forms.Label();
            this.lblVersionx86 = new System.Windows.Forms.Label();
            this.lblVersionx64 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.lblVisitComfySource = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 115);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(254, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "SmartSteam Emu && Launcher";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(9, 134);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(254, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "visit";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVisit
            // 
            this.lblVisit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblVisit.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblVisit.Location = new System.Drawing.Point(11, 153);
            this.lblVisit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblVisit.Name = "lblVisit";
            this.lblVisit.Size = new System.Drawing.Size(250, 19);
            this.lblVisit.TabIndex = 2;
            this.lblVisit.Text = "Project forum @ cs.rin.ru";
            this.lblVisit.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVisit.Click += new System.EventHandler(this.lblVisit_Click);
            // 
            // lblVersionx86
            // 
            this.lblVersionx86.Location = new System.Drawing.Point(7, 213);
            this.lblVersionx86.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblVersionx86.Name = "lblVersionx86";
            this.lblVersionx86.Size = new System.Drawing.Size(254, 19);
            this.lblVersionx86.TabIndex = 2;
            this.lblVersionx86.Text = "VERSION x86";
            this.lblVersionx86.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblVersionx64
            // 
            this.lblVersionx64.Location = new System.Drawing.Point(7, 232);
            this.lblVersionx64.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblVersionx64.Name = "lblVersionx64";
            this.lblVersionx64.Size = new System.Drawing.Size(254, 19);
            this.lblVersionx64.TabIndex = 2;
            this.lblVersionx64.Text = "VERSION x64";
            this.lblVersionx64.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 194);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(254, 19);
            this.label3.TabIndex = 2;
            this.label3.Text = "Version Info";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 291);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.panel1.Size = new System.Drawing.Size(272, 36);
            this.panel1.TabIndex = 3;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(94, 2);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 26);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pbLogo
            // 
            this.pbLogo.Image = global::Properties.Resources.launcher_big;
            this.pbLogo.Location = new System.Drawing.Point(9, 10);
            this.pbLogo.Margin = new System.Windows.Forms.Padding(2);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(254, 103);
            this.pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbLogo.TabIndex = 0;
            this.pbLogo.TabStop = false;
            // 
            // lblVisitComfySource
            // 
            this.lblVisitComfySource.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblVisitComfySource.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblVisitComfySource.Location = new System.Drawing.Point(11, 172);
            this.lblVisitComfySource.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblVisitComfySource.Name = "lblVisitComfySource";
            this.lblVisitComfySource.Size = new System.Drawing.Size(250, 19);
            this.lblVisitComfySource.TabIndex = 4;
            this.lblVisitComfySource.Text = "Comfy edition source @ gitgud.io";
            this.lblVisitComfySource.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVisitComfySource.Click += new System.EventHandler(this.lblVisitComfySource_Click);
            // 
            // FrmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 327);
            this.ControlBox = false;
            this.Controls.Add(this.lblVisitComfySource);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblVisit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblVersionx64);
            this.Controls.Add(this.lblVersionx86);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbLogo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAbout";
            this.ShowInTaskbar = false;
            this.Text = "About";
            this.Load += new System.EventHandler(this.FrmAbout_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblVisit;
        private System.Windows.Forms.Label lblVisitComfySource;
        private System.Windows.Forms.Label lblVersionx86;
        private System.Windows.Forms.Label lblVersionx64;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
    }
}