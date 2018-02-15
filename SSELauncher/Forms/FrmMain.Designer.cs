namespace SSELauncher
{
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            this.lstApps = new System.Windows.Forms.ListView();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addGamesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.launchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchNormallywithoutEmuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createDesktopShortcutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openFileLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openStorageLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSaveLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editGameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteGameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMenuViewStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addGameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallIconToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.hideMissingShortcutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dateAddedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.typeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.categoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.pbDrop = new System.Windows.Forms.PictureBox();
            this.menuStrip.SuspendLayout();
            this.ctxMenuStrip.SuspendLayout();
            this.ctxMenuViewStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDrop)).BeginInit();
            this.SuspendLayout();
            // 
            // lstApps
            // 
            this.lstApps.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstApps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstApps.LabelEdit = true;
            this.lstApps.LargeImageList = this.imgList;
            this.lstApps.Location = new System.Drawing.Point(0, 24);
            this.lstApps.Margin = new System.Windows.Forms.Padding(2);
            this.lstApps.Name = "lstApps";
            this.lstApps.Size = new System.Drawing.Size(323, 388);
            this.lstApps.SmallImageList = this.imgList;
            this.lstApps.TabIndex = 0;
            this.lstApps.UseCompatibleStateImageBehavior = false;
            this.lstApps.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.lstApps_AfterLabelEdit);
            this.lstApps.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lstApps_OnItemSelectionChanged);
            this.lstApps.SelectedIndexChanged += new System.EventHandler(this.lstApps_SelectedIndexChanged);
            this.lstApps.DoubleClick += new System.EventHandler(this.lstApps_DoubleClick);
            this.lstApps.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstApps_KeyDown);
            this.lstApps.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstApps_KeyPress);
            this.lstApps.KeyUp += new System.Windows.Forms.KeyEventHandler(this.lstApps_KeyUp);
            this.lstApps.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lstApps_OnMouseUp);
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imgList.ImageSize = new System.Drawing.Size(32, 32);
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(323, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addGamesToolStripMenuItem,
            this.editGameToolStripMenuItem,
            this.deleteGameToolStripMenuItem,
            this.toolStripMenuItem1,
            this.settingsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // addGamesToolStripMenuItem
            // 
            this.addGamesToolStripMenuItem.Name = "addGamesToolStripMenuItem";
            this.addGamesToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.addGamesToolStripMenuItem.Text = "&Add Game";
            this.addGamesToolStripMenuItem.Click += new System.EventHandler(this.addGamesToolStripMenuItem_Click);
            // 
            // editGameToolStripMenuItem
            // 
            this.editGameToolStripMenuItem.Name = "editGameToolStripMenuItem";
            this.editGameToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.editGameToolStripMenuItem.Text = "&Edit Game";
            this.editGameToolStripMenuItem.Click += new System.EventHandler(this.editGameToolStripMenuItem_Click);
            // 
            // deleteGameToolStripMenuItem
            // 
            this.deleteGameToolStripMenuItem.Name = "deleteGameToolStripMenuItem";
            this.deleteGameToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.deleteGameToolStripMenuItem.Text = "&Delete Game";
            this.deleteGameToolStripMenuItem.Click += new System.EventHandler(this.deleteGameToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(138, 6);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(138, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // ctxMenuStrip
            // 
            this.ctxMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.launchToolStripMenuItem,
            this.launchNormallywithoutEmuToolStripMenuItem,
            this.createDesktopShortcutToolStripMenuItem,
            this.toolStripSeparator1,
            this.openFileLocationToolStripMenuItem,
            this.openStorageLocationToolStripMenuItem,
            this.openSaveLocationToolStripMenuItem,
            this.toolStripMenuItem3,
            this.deleteToolStripMenuItem,
            this.renameToolStripMenuItem,
            this.toolStripMenuItem7,
            this.editToolStripMenuItem});
            this.ctxMenuStrip.Name = "ctxMenuStrip";
            this.ctxMenuStrip.Size = new System.Drawing.Size(245, 220);
            // 
            // launchToolStripMenuItem
            // 
            this.launchToolStripMenuItem.Name = "launchToolStripMenuItem";
            this.launchToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.launchToolStripMenuItem.Text = "&Launch";
            this.launchToolStripMenuItem.Click += new System.EventHandler(this.launchToolStripMenuItem_Click);
            // 
            // launchNormallywithoutEmuToolStripMenuItem
            // 
            this.launchNormallywithoutEmuToolStripMenuItem.Name = "launchNormallywithoutEmuToolStripMenuItem";
            this.launchNormallywithoutEmuToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.launchNormallywithoutEmuToolStripMenuItem.Text = "Launch &Normally (without Emu)";
            this.launchNormallywithoutEmuToolStripMenuItem.Click += new System.EventHandler(this.launchNormallywithoutEmuToolStripMenuItem_Click);
            // 
            // createDesktopShortcutToolStripMenuItem
            // 
            this.createDesktopShortcutToolStripMenuItem.Name = "createDesktopShortcutToolStripMenuItem";
            this.createDesktopShortcutToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.createDesktopShortcutToolStripMenuItem.Text = "&Create Desktop Shortcut";
            this.createDesktopShortcutToolStripMenuItem.Click += new System.EventHandler(this.createDesktopShortcutToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(241, 6);
            // 
            // openFileLocationToolStripMenuItem
            // 
            this.openFileLocationToolStripMenuItem.Name = "openFileLocationToolStripMenuItem";
            this.openFileLocationToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.openFileLocationToolStripMenuItem.Text = "&Open File Location";
            this.openFileLocationToolStripMenuItem.Click += new System.EventHandler(this.openFileLocationToolStripMenuItem_Click);
            // 
            // openStorageLocationToolStripMenuItem
            // 
            this.openStorageLocationToolStripMenuItem.Name = "openStorageLocationToolStripMenuItem";
            this.openStorageLocationToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.openStorageLocationToolStripMenuItem.Text = "Open Storage Location";
            this.openStorageLocationToolStripMenuItem.Click += new System.EventHandler(this.openStorageLocationToolStripMenuItem_Click);
            // 
            // openSaveLocationToolStripMenuItem
            // 
            this.openSaveLocationToolStripMenuItem.Name = "openSaveLocationToolStripMenuItem";
            this.openSaveLocationToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.openSaveLocationToolStripMenuItem.Text = "Open Save Location";
            this.openSaveLocationToolStripMenuItem.Click += new System.EventHandler(this.openSaveLocationToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(241, 6);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.renameToolStripMenuItem.Text = "&Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(241, 6);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(244, 22);
            this.editToolStripMenuItem.Text = "&Properties";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // fileToolStripMenuItem1
            // 
            this.fileToolStripMenuItem1.Name = "fileToolStripMenuItem1";
            this.fileToolStripMenuItem1.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem1.Text = "&File";
            // 
            // addGameToolStripMenuItem
            // 
            this.addGameToolStripMenuItem.Name = "addGameToolStripMenuItem";
            this.addGameToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.addGameToolStripMenuItem.Text = "&Add Game";
            this.addGameToolStripMenuItem.Click += new System.EventHandler(this.addGameToolStripMenuItem_Click);
            // 
            // editGameToolStripMenuItem1
            // 
            this.editGameToolStripMenuItem1.Name = "editGameToolStripMenuItem1";
            this.editGameToolStripMenuItem1.Size = new System.Drawing.Size(175, 24);
            this.editGameToolStripMenuItem1.Text = "&Edit Game";
            // 
            // deleteGameToolStripMenuItem1
            // 
            this.deleteGameToolStripMenuItem1.Name = "deleteGameToolStripMenuItem1";
            this.deleteGameToolStripMenuItem1.Size = new System.Drawing.Size(175, 24);
            this.deleteGameToolStripMenuItem1.Text = "&Delete Game";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(172, 6);
            // 
            // settingToolStripMenuItem
            // 
            this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
            this.settingToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.settingToolStripMenuItem.Text = "&Setting";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(172, 6);
            // 
            // exitToolStripMenuItem1
            // 
            this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
            this.exitToolStripMenuItem1.Size = new System.Drawing.Size(175, 24);
            this.exitToolStripMenuItem1.Text = "E&xit";
            // 
            // helpToolStripMenuItem1
            // 
            this.helpToolStripMenuItem1.Name = "helpToolStripMenuItem1";
            this.helpToolStripMenuItem1.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem1.Text = "&Help";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(175, 24);
            this.aboutToolStripMenuItem1.Text = "&About";
            // 
            // ctxMenuViewStrip
            // 
            this.ctxMenuViewStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxMenuViewStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addGameToolStripMenuItem1,
            this.toolStripMenuItem6,
            this.viewToolStripMenuItem,
            this.sortByToolStripMenuItem,
            this.groupByToolStripMenuItem,
            this.refreshToolStripMenuItem});
            this.ctxMenuViewStrip.Name = "ctxMenuViewStrip";
            this.ctxMenuViewStrip.Size = new System.Drawing.Size(131, 120);
            // 
            // addGameToolStripMenuItem1
            // 
            this.addGameToolStripMenuItem1.Name = "addGameToolStripMenuItem1";
            this.addGameToolStripMenuItem1.Size = new System.Drawing.Size(130, 22);
            this.addGameToolStripMenuItem1.Text = "Add Game";
            this.addGameToolStripMenuItem1.Click += new System.EventHandler(this.addGameToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(127, 6);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.largeIconToolStripMenuItem,
            this.smallIconToolStripMenuItem,
            this.listToolStripMenuItem,
            this.tileToolStripMenuItem,
            this.toolStripMenuItem8,
            this.hideMissingShortcutToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // largeIconToolStripMenuItem
            // 
            this.largeIconToolStripMenuItem.Name = "largeIconToolStripMenuItem";
            this.largeIconToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.largeIconToolStripMenuItem.Text = "L&arge Icon";
            this.largeIconToolStripMenuItem.Click += new System.EventHandler(this.largeIconToolStripMenuItem_Click);
            // 
            // smallIconToolStripMenuItem
            // 
            this.smallIconToolStripMenuItem.Name = "smallIconToolStripMenuItem";
            this.smallIconToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.smallIconToolStripMenuItem.Text = "&Small Icon";
            this.smallIconToolStripMenuItem.Click += new System.EventHandler(this.smallIconToolStripMenuItem_Click);
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            this.listToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.listToolStripMenuItem.Text = "&List";
            this.listToolStripMenuItem.Click += new System.EventHandler(this.listToolStripMenuItem_Click);
            // 
            // tileToolStripMenuItem
            // 
            this.tileToolStripMenuItem.Name = "tileToolStripMenuItem";
            this.tileToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.tileToolStripMenuItem.Text = "&Tile";
            this.tileToolStripMenuItem.Click += new System.EventHandler(this.tileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(187, 6);
            // 
            // hideMissingShortcutToolStripMenuItem
            // 
            this.hideMissingShortcutToolStripMenuItem.Name = "hideMissingShortcutToolStripMenuItem";
            this.hideMissingShortcutToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.hideMissingShortcutToolStripMenuItem.Text = "Hide missing shortcut";
            this.hideMissingShortcutToolStripMenuItem.Click += new System.EventHandler(this.hideMissingShortcutToolStripMenuItem_Click);
            // 
            // sortByToolStripMenuItem
            // 
            this.sortByToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nameToolStripMenuItem,
            this.dateAddedToolStripMenuItem});
            this.sortByToolStripMenuItem.Name = "sortByToolStripMenuItem";
            this.sortByToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.sortByToolStripMenuItem.Text = "S&ort by";
            // 
            // nameToolStripMenuItem
            // 
            this.nameToolStripMenuItem.Name = "nameToolStripMenuItem";
            this.nameToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.nameToolStripMenuItem.Text = "Name";
            this.nameToolStripMenuItem.Click += new System.EventHandler(this.nameToolStripMenuItem_Click);
            // 
            // dateAddedToolStripMenuItem
            // 
            this.dateAddedToolStripMenuItem.Name = "dateAddedToolStripMenuItem";
            this.dateAddedToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.dateAddedToolStripMenuItem.Text = "(None)";
            this.dateAddedToolStripMenuItem.Click += new System.EventHandler(this.dateAddedToolStripMenuItem_Click);
            // 
            // groupByToolStripMenuItem
            // 
            this.groupByToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem,
            this.typeToolStripMenuItem,
            this.categoryToolStripMenuItem});
            this.groupByToolStripMenuItem.Name = "groupByToolStripMenuItem";
            this.groupByToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.groupByToolStripMenuItem.Text = "Grou&p by";
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.noneToolStripMenuItem.Text = "(&None)";
            this.noneToolStripMenuItem.Click += new System.EventHandler(this.noneToolStripMenuItem_Click);
            // 
            // typeToolStripMenuItem
            // 
            this.typeToolStripMenuItem.Name = "typeToolStripMenuItem";
            this.typeToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.typeToolStripMenuItem.Text = "Type";
            this.typeToolStripMenuItem.Click += new System.EventHandler(this.typeToolStripMenuItem_Click);
            // 
            // categoryToolStripMenuItem
            // 
            this.categoryToolStripMenuItem.Name = "categoryToolStripMenuItem";
            this.categoryToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.categoryToolStripMenuItem.Text = "Category";
            this.categoryToolStripMenuItem.Click += new System.EventHandler(this.categoryToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.refreshToolStripMenuItem.Text = "R&efresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // pbDrop
            // 
            this.pbDrop.BackColor = System.Drawing.Color.White;
            this.pbDrop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbDrop.Image = global::Properties.Resources.drop_here;
            this.pbDrop.Location = new System.Drawing.Point(3, 63);
            this.pbDrop.Margin = new System.Windows.Forms.Padding(2);
            this.pbDrop.Name = "pbDrop";
            this.pbDrop.Size = new System.Drawing.Size(314, 318);
            this.pbDrop.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbDrop.TabIndex = 2;
            this.pbDrop.TabStop = false;
            // 
            // FrmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(323, 414);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.pbDrop);
            this.Controls.Add(this.lstApps);
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(339, 414);
            this.Name = "FrmMain";
            this.Text = "SmartSteamEmu Comfy Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_Closing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.Resize += new System.EventHandler(this.FrmMain_Resize);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ctxMenuStrip.ResumeLayout(false);
            this.ctxMenuViewStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbDrop)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lstApps;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addGamesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem editGameToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ctxMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem launchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editGameToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteGameToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem settingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.PictureBox pbDrop;
        private System.Windows.Forms.ToolStripMenuItem launchNormallywithoutEmuToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ctxMenuViewStrip;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeIconToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallIconToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addGameToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem createDesktopShortcutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dateAddedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groupByToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem typeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem categoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem8;
        private System.Windows.Forms.ToolStripMenuItem hideMissingShortcutToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem openFileLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openStorageLocationToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openSaveLocationToolStripMenuItem;

    }
}

