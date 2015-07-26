namespace SharpNews
{
    partial class Main
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
            System.Windows.Forms.ToolStripMenuItem quitMenuItem;
            System.Windows.Forms.ToolStripMenuItem stopMenuItem;
            System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
            System.Windows.Forms.SplitContainer groupsDetailsSplit;
            System.Windows.Forms.TabPage groupContentsPage;
            System.Windows.Forms.TabPage statusPage;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.serverMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prefMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupsTree = new System.Windows.Forms.TreeView();
            this.groupTabs = new System.Windows.Forms.TabControl();
            this.groupList = new System.Windows.Forms.ListView();
            this.DownloadsPage = new System.Windows.Forms.TabPage();
            this.downloadsList = new System.Windows.Forms.ListView();
            this.statusLog = new System.Windows.Forms.TextBox();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nZBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolbar = new System.Windows.Forms.ToolStrip();
            this.updateButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.pauseButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.serverButton = new System.Windows.Forms.ToolStripButton();
            this.groupButton = new System.Windows.Forms.ToolStripButton();
            this.prefButton = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.generalStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.speedStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.downloadedStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.pendingStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.timeRemainigStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.filterPanel = new System.Windows.Forms.Panel();
            this.groupActiveSplit = new System.Windows.Forms.SplitContainer();
            quitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            stopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            groupsDetailsSplit = new System.Windows.Forms.SplitContainer();
            groupContentsPage = new System.Windows.Forms.TabPage();
            statusPage = new System.Windows.Forms.TabPage();
            groupsDetailsSplit.Panel1.SuspendLayout();
            groupsDetailsSplit.Panel2.SuspendLayout();
            groupsDetailsSplit.SuspendLayout();
            this.groupTabs.SuspendLayout();
            groupContentsPage.SuspendLayout();
            this.DownloadsPage.SuspendLayout();
            statusPage.SuspendLayout();
            this.menu.SuspendLayout();
            this.toolbar.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.groupActiveSplit.Panel1.SuspendLayout();
            this.groupActiveSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // quitMenuItem
            // 
            quitMenuItem.Name = "quitMenuItem";
            quitMenuItem.Size = new System.Drawing.Size(94, 22);
            quitMenuItem.Text = "Quit";
            // 
            // stopMenuItem
            // 
            stopMenuItem.Name = "stopMenuItem";
            stopMenuItem.Size = new System.Drawing.Size(158, 22);
            stopMenuItem.Text = "Stop Update";
            // 
            // settingsMenuItem
            // 
            settingsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.serverMenuItem,
            this.groupMenuItem,
            this.prefMenuItem});
            settingsMenuItem.Name = "settingsMenuItem";
            settingsMenuItem.Size = new System.Drawing.Size(58, 20);
            settingsMenuItem.Text = "Settings";
            // 
            // serverMenuItem
            // 
            this.serverMenuItem.Name = "serverMenuItem";
            this.serverMenuItem.Size = new System.Drawing.Size(132, 22);
            this.serverMenuItem.Text = "Server";
            this.serverMenuItem.Click += new System.EventHandler(this.serverToolStripMenuItem_Click);
            // 
            // groupMenuItem
            // 
            this.groupMenuItem.Name = "groupMenuItem";
            this.groupMenuItem.Size = new System.Drawing.Size(132, 22);
            this.groupMenuItem.Text = "Group";
            this.groupMenuItem.Click += new System.EventHandler(this.groupMenuItem_Click);
            // 
            // prefMenuItem
            // 
            this.prefMenuItem.Name = "prefMenuItem";
            this.prefMenuItem.Size = new System.Drawing.Size(132, 22);
            this.prefMenuItem.Text = "Preferences";
            // 
            // groupsDetailsSplit
            // 
            groupsDetailsSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            groupsDetailsSplit.Location = new System.Drawing.Point(0, 0);
            groupsDetailsSplit.Name = "groupsDetailsSplit";
            // 
            // groupsDetailsSplit.Panel1
            // 
            groupsDetailsSplit.Panel1.Controls.Add(this.groupsTree);
            // 
            // groupsDetailsSplit.Panel2
            // 
            groupsDetailsSplit.Panel2.Controls.Add(this.groupTabs);
            groupsDetailsSplit.Size = new System.Drawing.Size(1015, 340);
            groupsDetailsSplit.SplitterDistance = 214;
            groupsDetailsSplit.TabIndex = 0;
            groupsDetailsSplit.Text = "splitContainer2";
            // 
            // groupsTree
            // 
            this.groupsTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupsTree.Location = new System.Drawing.Point(0, 0);
            this.groupsTree.Name = "groupsTree";
            this.groupsTree.Size = new System.Drawing.Size(214, 340);
            this.groupsTree.TabIndex = 0;
            // 
            // groupTabs
            // 
            this.groupTabs.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.groupTabs.Controls.Add(groupContentsPage);
            this.groupTabs.Controls.Add(this.DownloadsPage);
            this.groupTabs.Controls.Add(statusPage);
            this.groupTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupTabs.Location = new System.Drawing.Point(0, 0);
            this.groupTabs.Name = "groupTabs";
            this.groupTabs.SelectedIndex = 0;
            this.groupTabs.Size = new System.Drawing.Size(797, 340);
            this.groupTabs.TabIndex = 0;
            // 
            // groupContentsPage
            // 
            groupContentsPage.Controls.Add(this.groupList);
            groupContentsPage.Location = new System.Drawing.Point(4, 4);
            groupContentsPage.Name = "groupContentsPage";
            groupContentsPage.Padding = new System.Windows.Forms.Padding(3);
            groupContentsPage.Size = new System.Drawing.Size(789, 314);
            groupContentsPage.TabIndex = 0;
            groupContentsPage.Text = "Group";
            // 
            // groupList
            // 
            this.groupList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupList.Location = new System.Drawing.Point(3, 3);
            this.groupList.Name = "groupList";
            this.groupList.Size = new System.Drawing.Size(783, 308);
            this.groupList.TabIndex = 0;
            this.groupList.UseCompatibleStateImageBehavior = false;
            // 
            // DownloadsPage
            // 
            this.DownloadsPage.Controls.Add(this.downloadsList);
            this.DownloadsPage.Location = new System.Drawing.Point(4, 4);
            this.DownloadsPage.Name = "DownloadsPage";
            this.DownloadsPage.Padding = new System.Windows.Forms.Padding(3);
            this.DownloadsPage.Size = new System.Drawing.Size(789, 314);
            this.DownloadsPage.TabIndex = 1;
            this.DownloadsPage.Text = "Downloads";
            // 
            // downloadsList
            // 
            this.downloadsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.downloadsList.Location = new System.Drawing.Point(3, 3);
            this.downloadsList.Name = "downloadsList";
            this.downloadsList.Size = new System.Drawing.Size(783, 308);
            this.downloadsList.TabIndex = 0;
            this.downloadsList.UseCompatibleStateImageBehavior = false;
            // 
            // statusPage
            // 
            statusPage.Controls.Add(this.statusLog);
            statusPage.Location = new System.Drawing.Point(4, 4);
            statusPage.Name = "statusPage";
            statusPage.Padding = new System.Windows.Forms.Padding(3);
            statusPage.Size = new System.Drawing.Size(789, 314);
            statusPage.TabIndex = 2;
            statusPage.Text = "Status";
            // 
            // statusLog
            // 
            this.statusLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusLog.Location = new System.Drawing.Point(3, 3);
            this.statusLog.Multiline = true;
            this.statusLog.Name = "statusLog";
            this.statusLog.ReadOnly = true;
            this.statusLog.Size = new System.Drawing.Size(783, 308);
            this.statusLog.TabIndex = 0;
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.actionsMenuItem,
            settingsMenuItem});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(1015, 24);
            this.menu.TabIndex = 0;
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nZBToolStripMenuItem,
            quitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileMenuItem.Text = "File";
            // 
            // nZBToolStripMenuItem
            // 
            this.nZBToolStripMenuItem.Name = "nZBToolStripMenuItem";
            this.nZBToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.nZBToolStripMenuItem.Text = "NZB";
            this.nZBToolStripMenuItem.Click += new System.EventHandler(this.nZBToolStripMenuItem_Click);
            // 
            // actionsMenuItem
            // 
            this.actionsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateMenuItem,
            stopMenuItem,
            this.refreshMenuItem,
            this.pauseMenuItem});
            this.actionsMenuItem.Name = "actionsMenuItem";
            this.actionsMenuItem.Size = new System.Drawing.Size(54, 20);
            this.actionsMenuItem.Text = "Actions";
            // 
            // updateMenuItem
            // 
            this.updateMenuItem.Name = "updateMenuItem";
            this.updateMenuItem.Size = new System.Drawing.Size(158, 22);
            this.updateMenuItem.Text = "Update All";
            // 
            // refreshMenuItem
            // 
            this.refreshMenuItem.Name = "refreshMenuItem";
            this.refreshMenuItem.Size = new System.Drawing.Size(158, 22);
            this.refreshMenuItem.Text = "Refresh";
            // 
            // pauseMenuItem
            // 
            this.pauseMenuItem.Name = "pauseMenuItem";
            this.pauseMenuItem.Size = new System.Drawing.Size(158, 22);
            this.pauseMenuItem.Text = "Pause Downloads";
            // 
            // toolbar
            // 
            this.toolbar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateButton,
            this.stopButton,
            this.pauseButton,
            this.toolStripSeparator1,
            this.serverButton,
            this.prefButton,
            this.groupButton});
            this.toolbar.Location = new System.Drawing.Point(0, 24);
            this.toolbar.Name = "toolbar";
            this.toolbar.Size = new System.Drawing.Size(1015, 25);
            this.toolbar.TabIndex = 1;
            this.toolbar.Text = "toolStrip1";
            // 
            // updateButton
            // 
            this.updateButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.updateButton.Image = ((System.Drawing.Image)(resources.GetObject("updateButton.Image")));
            this.updateButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(23, 22);
            this.updateButton.Text = "toolStripButton1";
            this.updateButton.ToolTipText = "Update All";
            // 
            // stopButton
            // 
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(23, 22);
            this.stopButton.Text = "Stop Update";
            // 
            // pauseButton
            // 
            this.pauseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseButton.Image = ((System.Drawing.Image)(resources.GetObject("pauseButton.Image")));
            this.pauseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(23, 22);
            this.pauseButton.Text = "Pause Downloads";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // serverButton
            // 
            this.serverButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.serverButton.Image = ((System.Drawing.Image)(resources.GetObject("serverButton.Image")));
            this.serverButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.serverButton.Name = "serverButton";
            this.serverButton.Size = new System.Drawing.Size(23, 22);
            this.serverButton.Text = "Server Settings";
            // 
            // groupButton
            // 
            this.groupButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.groupButton.Image = ((System.Drawing.Image)(resources.GetObject("groupButton.Image")));
            this.groupButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.groupButton.Name = "groupButton";
            this.groupButton.Size = new System.Drawing.Size(23, 22);
            this.groupButton.Text = "Group Settings";
            // 
            // prefButton
            // 
            this.prefButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.prefButton.Image = ((System.Drawing.Image)(resources.GetObject("prefButton.Image")));
            this.prefButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.prefButton.Name = "prefButton";
            this.prefButton.Size = new System.Drawing.Size(23, 22);
            this.prefButton.Text = "Preferences";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.generalStatus,
            this.speedStatus,
            this.downloadedStatus,
            this.pendingStatus,
            this.timeRemainigStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 585);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1015, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "statusStrip1";
            // 
            // generalStatus
            // 
            this.generalStatus.Name = "generalStatus";
            this.generalStatus.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.generalStatus.Size = new System.Drawing.Size(115, 17);
            this.generalStatus.Text = "Ready                        ";
            this.generalStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // speedStatus
            // 
            this.speedStatus.Name = "speedStatus";
            this.speedStatus.Size = new System.Drawing.Size(36, 17);
            this.speedStatus.Text = "0KiB/s";
            // 
            // downloadedStatus
            // 
            this.downloadedStatus.Name = "downloadedStatus";
            this.downloadedStatus.Size = new System.Drawing.Size(29, 17);
            this.downloadedStatus.Text = "0MiB";
            // 
            // pendingStatus
            // 
            this.pendingStatus.Name = "pendingStatus";
            this.pendingStatus.Size = new System.Drawing.Size(29, 17);
            this.pendingStatus.Text = "0MiB";
            // 
            // timeRemainigStatus
            // 
            this.timeRemainigStatus.Name = "timeRemainigStatus";
            this.timeRemainigStatus.Size = new System.Drawing.Size(45, 17);
            this.timeRemainigStatus.Text = "0:00:00";
            // 
            // filterPanel
            // 
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterPanel.Location = new System.Drawing.Point(0, 49);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Size = new System.Drawing.Size(1015, 67);
            this.filterPanel.TabIndex = 3;
            // 
            // groupActiveSplit
            // 
            this.groupActiveSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupActiveSplit.Location = new System.Drawing.Point(0, 116);
            this.groupActiveSplit.Name = "groupActiveSplit";
            this.groupActiveSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // groupActiveSplit.Panel1
            // 
            this.groupActiveSplit.Panel1.Controls.Add(groupsDetailsSplit);
            this.groupActiveSplit.Size = new System.Drawing.Size(1015, 469);
            this.groupActiveSplit.SplitterDistance = 340;
            this.groupActiveSplit.TabIndex = 4;
            this.groupActiveSplit.Text = "splitContainer1";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1015, 607);
            this.Controls.Add(this.groupActiveSplit);
            this.Controls.Add(this.filterPanel);
            this.Controls.Add(this.toolbar);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menu);
            this.MainMenuStrip = this.menu;
            this.Name = "Main";
            this.Text = "SharpNews - 0.1Alpha";
            groupsDetailsSplit.Panel1.ResumeLayout(false);
            groupsDetailsSplit.Panel2.ResumeLayout(false);
            groupsDetailsSplit.ResumeLayout(false);
            this.groupTabs.ResumeLayout(false);
            groupContentsPage.ResumeLayout(false);
            this.DownloadsPage.ResumeLayout(false);
            statusPage.ResumeLayout(false);
            statusPage.PerformLayout();
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.toolbar.ResumeLayout(false);
            this.toolbar.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupActiveSplit.Panel1.ResumeLayout(false);
            this.groupActiveSplit.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStrip toolbar;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionsMenuItem;
        private System.Windows.Forms.ToolStripButton updateButton;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ToolStripButton pauseButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton serverButton;
        private System.Windows.Forms.ToolStripMenuItem updateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem refreshMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem serverMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groupMenuItem;
        private System.Windows.Forms.ToolStripMenuItem prefMenuItem;
        private System.Windows.Forms.Panel filterPanel;
        private System.Windows.Forms.SplitContainer groupActiveSplit;
        private System.Windows.Forms.TreeView groupsTree;
        private System.Windows.Forms.TabControl groupTabs;
        private System.Windows.Forms.TabPage DownloadsPage;
        private System.Windows.Forms.TextBox statusLog;
        private System.Windows.Forms.ListView groupList;
        private System.Windows.Forms.ListView downloadsList;
        private System.Windows.Forms.ToolStripButton groupButton;
        private System.Windows.Forms.ToolStripButton prefButton;
        private System.Windows.Forms.ToolStripStatusLabel generalStatus;
        private System.Windows.Forms.ToolStripStatusLabel speedStatus;
        private System.Windows.Forms.ToolStripStatusLabel downloadedStatus;
        private System.Windows.Forms.ToolStripStatusLabel pendingStatus;
        private System.Windows.Forms.ToolStripStatusLabel timeRemainigStatus;
        private System.Windows.Forms.ToolStripMenuItem nZBToolStripMenuItem;
    }
}

