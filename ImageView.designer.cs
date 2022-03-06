﻿
namespace BioImage
{
    partial class ImageView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timeBar = new System.Windows.Forms.TrackBar();
            this.timePlayMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playTimeToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.stopTimeToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.playSpeedToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.setValueRangeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.strechToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.normalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.centerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTimeTrackbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openChannelsToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoContrastChannelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.planeModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zBar = new System.Windows.Forms.TrackBar();
            this.zPlayMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playSpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setValueRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.channelBoxB = new System.Windows.Forms.ComboBox();
            this.channelBoxG = new System.Windows.Forms.ComboBox();
            this.labelRGB = new System.Windows.Forms.Label();
            this.channelBoxR = new System.Windows.Forms.ComboBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.rgbPictureBox = new AForge.Controls.PictureBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.rgbBoxsPanel = new System.Windows.Forms.Panel();
            this.cPanel = new System.Windows.Forms.Panel();
            this.cLabel = new System.Windows.Forms.Label();
            this.cBar = new System.Windows.Forms.TrackBar();
            this.cPlayMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CPlaySpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setCValueRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timelineTimer = new System.Windows.Forms.Timer(this.components);
            this.zTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openImagesDialog = new System.Windows.Forms.OpenFileDialog();
            this.cTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).BeginInit();
            this.timePlayMenuStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zBar)).BeginInit();
            this.zPlayMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rgbPictureBox)).BeginInit();
            this.rgbBoxsPanel.SuspendLayout();
            this.cPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cBar)).BeginInit();
            this.cPlayMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // timeBar
            // 
            this.timeBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.timeBar.AutoSize = false;
            this.timeBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(98)))));
            this.timeBar.ContextMenuStrip = this.timePlayMenuStrip;
            this.timeBar.LargeChange = 1;
            this.timeBar.Location = new System.Drawing.Point(9, 28);
            this.timeBar.Margin = new System.Windows.Forms.Padding(0);
            this.timeBar.Name = "timeBar";
            this.timeBar.Size = new System.Drawing.Size(422, 26);
            this.timeBar.TabIndex = 0;
            this.timeBar.Visible = false;
            this.timeBar.ValueChanged += new System.EventHandler(this.timeBar_ValueChanged);
            // 
            // timePlayMenuStrip
            // 
            this.timePlayMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playTimeToolStripMenu,
            this.stopTimeToolStripMenu,
            this.playSpeedToolStripMenuItem1,
            this.setValueRangeToolStripMenuItem1});
            this.timePlayMenuStrip.Name = "zPlayMenuStrip";
            this.timePlayMenuStrip.Size = new System.Drawing.Size(158, 92);
            // 
            // playTimeToolStripMenu
            // 
            this.playTimeToolStripMenu.Name = "playTimeToolStripMenu";
            this.playTimeToolStripMenu.Size = new System.Drawing.Size(157, 22);
            this.playTimeToolStripMenu.Text = "Play";
            this.playTimeToolStripMenu.Click += new System.EventHandler(this.playTimeToolStripMenu_Click);
            // 
            // stopTimeToolStripMenu
            // 
            this.stopTimeToolStripMenu.Checked = true;
            this.stopTimeToolStripMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stopTimeToolStripMenu.Name = "stopTimeToolStripMenu";
            this.stopTimeToolStripMenu.Size = new System.Drawing.Size(157, 22);
            this.stopTimeToolStripMenu.Text = "Stop";
            this.stopTimeToolStripMenu.Click += new System.EventHandler(this.stopTimeToolStripMenu_Click);
            // 
            // playSpeedToolStripMenuItem1
            // 
            this.playSpeedToolStripMenuItem1.Name = "playSpeedToolStripMenuItem1";
            this.playSpeedToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
            this.playSpeedToolStripMenuItem1.Text = "Play Speed";
            this.playSpeedToolStripMenuItem1.Click += new System.EventHandler(this.playSpeedToolStripMenuItem1_Click);
            // 
            // setValueRangeToolStripMenuItem1
            // 
            this.setValueRangeToolStripMenuItem1.Name = "setValueRangeToolStripMenuItem1";
            this.setValueRangeToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
            this.setValueRangeToolStripMenuItem1.Text = "Set Value Range";
            this.setValueRangeToolStripMenuItem1.Click += new System.EventHandler(this.setValueRangeToolStripMenuItem1_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyImageToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.showControlsToolStripMenuItem,
            this.showTimeTrackbarToolStripMenuItem,
            this.openChannelsToolToolStripMenuItem,
            this.autoContrastChannelsToolStripMenuItem,
            this.planeModeToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(208, 158);
            // 
            // copyImageToolStripMenuItem
            // 
            this.copyImageToolStripMenuItem.Name = "copyImageToolStripMenuItem";
            this.copyImageToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.copyImageToolStripMenuItem.Text = "Copy Image";
            this.copyImageToolStripMenuItem.Click += new System.EventHandler(this.copyImageToolStripMenuItem_Click);
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomToolStripMenuItem,
            this.strechToolStripMenuItem,
            this.normalToolStripMenuItem,
            this.centerToolStripMenuItem,
            this.autoSizeToolStripMenuItem});
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            this.imageToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.imageToolStripMenuItem.Text = "Size Mode";
            // 
            // zoomToolStripMenuItem
            // 
            this.zoomToolStripMenuItem.Checked = true;
            this.zoomToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            this.zoomToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.zoomToolStripMenuItem.Text = "Zoom";
            this.zoomToolStripMenuItem.Click += new System.EventHandler(this.zoomToolStripMenuItem_Click);
            // 
            // strechToolStripMenuItem
            // 
            this.strechToolStripMenuItem.Name = "strechToolStripMenuItem";
            this.strechToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.strechToolStripMenuItem.Text = "Strech";
            this.strechToolStripMenuItem.Click += new System.EventHandler(this.strechToolStripMenuItem_Click);
            // 
            // normalToolStripMenuItem
            // 
            this.normalToolStripMenuItem.Name = "normalToolStripMenuItem";
            this.normalToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.normalToolStripMenuItem.Text = "Normal";
            this.normalToolStripMenuItem.Click += new System.EventHandler(this.normalToolStripMenuItem_Click);
            // 
            // centerToolStripMenuItem
            // 
            this.centerToolStripMenuItem.Name = "centerToolStripMenuItem";
            this.centerToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.centerToolStripMenuItem.Text = "Center";
            this.centerToolStripMenuItem.Click += new System.EventHandler(this.centerToolStripMenuItem_Click);
            // 
            // autoSizeToolStripMenuItem
            // 
            this.autoSizeToolStripMenuItem.Name = "autoSizeToolStripMenuItem";
            this.autoSizeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.autoSizeToolStripMenuItem.Text = "Auto Size";
            this.autoSizeToolStripMenuItem.Click += new System.EventHandler(this.autoSizeToolStripMenuItem_Click);
            // 
            // showControlsToolStripMenuItem
            // 
            this.showControlsToolStripMenuItem.Name = "showControlsToolStripMenuItem";
            this.showControlsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.showControlsToolStripMenuItem.Text = "Hide Controls";
            this.showControlsToolStripMenuItem.Click += new System.EventHandler(this.showControlsToolStripMenuItem_Click);
            // 
            // showTimeTrackbarToolStripMenuItem
            // 
            this.showTimeTrackbarToolStripMenuItem.Name = "showTimeTrackbarToolStripMenuItem";
            this.showTimeTrackbarToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.showTimeTrackbarToolStripMenuItem.Text = "Show Time Trackbar";
            this.showTimeTrackbarToolStripMenuItem.Visible = false;
            this.showTimeTrackbarToolStripMenuItem.Click += new System.EventHandler(this.showTimeTrackbarToolStripMenuItem_Click);
            // 
            // openChannelsToolToolStripMenuItem
            // 
            this.openChannelsToolToolStripMenuItem.Name = "openChannelsToolToolStripMenuItem";
            this.openChannelsToolToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.openChannelsToolToolStripMenuItem.Text = "Open Channels Tool";
            this.openChannelsToolToolStripMenuItem.Click += new System.EventHandler(this.openChannelsToolToolStripMenuItem_Click);
            // 
            // autoContrastChannelsToolStripMenuItem
            // 
            this.autoContrastChannelsToolStripMenuItem.Name = "autoContrastChannelsToolStripMenuItem";
            this.autoContrastChannelsToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.autoContrastChannelsToolStripMenuItem.Text = "Auto Threshold Channels";
            this.autoContrastChannelsToolStripMenuItem.Click += new System.EventHandler(this.autoContrastChannelsToolStripMenuItem_Click);
            // 
            // planeModeToolStripMenuItem
            // 
            this.planeModeToolStripMenuItem.CheckOnClick = true;
            this.planeModeToolStripMenuItem.Name = "planeModeToolStripMenuItem";
            this.planeModeToolStripMenuItem.Size = new System.Drawing.Size(207, 22);
            this.planeModeToolStripMenuItem.Text = "Plane Mode";
            this.planeModeToolStripMenuItem.Click += new System.EventHandler(this.planeModeToolStripMenuItem_Click);
            // 
            // zBar
            // 
            this.zBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zBar.AutoSize = false;
            this.zBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(98)))));
            this.zBar.ContextMenuStrip = this.zPlayMenuStrip;
            this.zBar.LargeChange = 1;
            this.zBar.Location = new System.Drawing.Point(9, 2);
            this.zBar.Margin = new System.Windows.Forms.Padding(0);
            this.zBar.Name = "zBar";
            this.zBar.Size = new System.Drawing.Size(422, 26);
            this.zBar.TabIndex = 12;
            this.zBar.ValueChanged += new System.EventHandler(this.zBar_ValueChanged);
            // 
            // zPlayMenuStrip
            // 
            this.zPlayMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playZToolStripMenuItem,
            this.stopZToolStripMenuItem,
            this.playSpeedToolStripMenuItem,
            this.setValueRangeToolStripMenuItem});
            this.zPlayMenuStrip.Name = "zPlayMenuStrip";
            this.zPlayMenuStrip.Size = new System.Drawing.Size(158, 92);
            // 
            // playZToolStripMenuItem
            // 
            this.playZToolStripMenuItem.Name = "playZToolStripMenuItem";
            this.playZToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.playZToolStripMenuItem.Text = "Play";
            this.playZToolStripMenuItem.Click += new System.EventHandler(this.playZToolStripMenuItem_Click);
            // 
            // stopZToolStripMenuItem
            // 
            this.stopZToolStripMenuItem.Checked = true;
            this.stopZToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stopZToolStripMenuItem.Name = "stopZToolStripMenuItem";
            this.stopZToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.stopZToolStripMenuItem.Text = "Stop";
            this.stopZToolStripMenuItem.Click += new System.EventHandler(this.stopZToolStripMenuItem_Click);
            // 
            // playSpeedToolStripMenuItem
            // 
            this.playSpeedToolStripMenuItem.Name = "playSpeedToolStripMenuItem";
            this.playSpeedToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.playSpeedToolStripMenuItem.Text = "Play Speed";
            this.playSpeedToolStripMenuItem.Click += new System.EventHandler(this.playSpeedToolStripMenuItem_Click);
            // 
            // setValueRangeToolStripMenuItem
            // 
            this.setValueRangeToolStripMenuItem.Name = "setValueRangeToolStripMenuItem";
            this.setValueRangeToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.setValueRangeToolStripMenuItem.Text = "Set Value Range";
            this.setValueRangeToolStripMenuItem.Click += new System.EventHandler(this.setValueRangeToolStripMenuItem_Click);
            // 
            // channelBoxB
            // 
            this.channelBoxB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.channelBoxB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(56)))), ((int)(((byte)(76)))));
            this.channelBoxB.DropDownWidth = 150;
            this.channelBoxB.ForeColor = System.Drawing.Color.White;
            this.channelBoxB.FormattingEnabled = true;
            this.channelBoxB.Location = new System.Drawing.Point(297, 4);
            this.channelBoxB.Name = "channelBoxB";
            this.channelBoxB.Size = new System.Drawing.Size(125, 21);
            this.channelBoxB.TabIndex = 8;
            this.channelBoxB.SelectedIndexChanged += new System.EventHandler(this.channelBoxB_SelectedIndexChanged);
            // 
            // channelBoxG
            // 
            this.channelBoxG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.channelBoxG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(56)))), ((int)(((byte)(76)))));
            this.channelBoxG.DropDownWidth = 150;
            this.channelBoxG.ForeColor = System.Drawing.Color.White;
            this.channelBoxG.FormattingEnabled = true;
            this.channelBoxG.Location = new System.Drawing.Point(171, 4);
            this.channelBoxG.Name = "channelBoxG";
            this.channelBoxG.Size = new System.Drawing.Size(120, 21);
            this.channelBoxG.TabIndex = 6;
            this.channelBoxG.SelectedIndexChanged += new System.EventHandler(this.channelBoxG_SelectedIndexChanged);
            // 
            // labelRGB
            // 
            this.labelRGB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRGB.AutoSize = true;
            this.labelRGB.ForeColor = System.Drawing.Color.White;
            this.labelRGB.Location = new System.Drawing.Point(6, 7);
            this.labelRGB.Name = "labelRGB";
            this.labelRGB.Size = new System.Drawing.Size(33, 13);
            this.labelRGB.TabIndex = 5;
            this.labelRGB.Text = "RGB:";
            // 
            // channelBoxR
            // 
            this.channelBoxR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.channelBoxR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(56)))), ((int)(((byte)(76)))));
            this.channelBoxR.DropDownWidth = 150;
            this.channelBoxR.ForeColor = System.Drawing.Color.White;
            this.channelBoxR.FormattingEnabled = true;
            this.channelBoxR.Location = new System.Drawing.Point(45, 4);
            this.channelBoxR.Name = "channelBoxR";
            this.channelBoxR.Size = new System.Drawing.Size(120, 21);
            this.channelBoxR.TabIndex = 4;
            this.channelBoxR.SelectedIndexChanged += new System.EventHandler(this.channelBoxR_SelectedIndexChanged);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.rgbPictureBox);
            this.splitContainer.Panel1.Controls.Add(this.statusLabel);
            this.splitContainer.Panel1MinSize = 0;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.ContextMenuStrip = this.contextMenuStrip;
            this.splitContainer.Panel2.Controls.Add(this.rgbBoxsPanel);
            this.splitContainer.Panel2.Controls.Add(this.cPanel);
            this.splitContainer.Panel2.Controls.Add(this.timeLabel);
            this.splitContainer.Panel2.Controls.Add(this.label1);
            this.splitContainer.Panel2.Controls.Add(this.zBar);
            this.splitContainer.Panel2.Controls.Add(this.timeBar);
            this.splitContainer.Panel2MinSize = 0;
            this.splitContainer.Size = new System.Drawing.Size(432, 368);
            this.splitContainer.SplitterDistance = 280;
            this.splitContainer.SplitterWidth = 3;
            this.splitContainer.TabIndex = 3;
            // 
            // rgbPictureBox
            // 
            this.rgbPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rgbPictureBox.ContextMenuStrip = this.contextMenuStrip;
            this.rgbPictureBox.Image = null;
            this.rgbPictureBox.Location = new System.Drawing.Point(-1, 21);
            this.rgbPictureBox.Name = "rgbPictureBox";
            this.rgbPictureBox.Size = new System.Drawing.Size(432, 261);
            this.rgbPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.rgbPictureBox.TabIndex = 4;
            this.rgbPictureBox.TabStop = false;
            this.rgbPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rgbPictureBox_MouseMove);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.Color.White;
            this.statusLabel.Location = new System.Drawing.Point(9, 6);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 13);
            this.statusLabel.TabIndex = 3;
            // 
            // rgbBoxsPanel
            // 
            this.rgbBoxsPanel.Controls.Add(this.channelBoxR);
            this.rgbBoxsPanel.Controls.Add(this.channelBoxB);
            this.rgbBoxsPanel.Controls.Add(this.labelRGB);
            this.rgbBoxsPanel.Controls.Add(this.channelBoxG);
            this.rgbBoxsPanel.ForeColor = System.Drawing.Color.White;
            this.rgbBoxsPanel.Location = new System.Drawing.Point(0, 57);
            this.rgbBoxsPanel.Name = "rgbBoxsPanel";
            this.rgbBoxsPanel.Size = new System.Drawing.Size(432, 27);
            this.rgbBoxsPanel.TabIndex = 13;
            // 
            // cPanel
            // 
            this.cPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cPanel.Controls.Add(this.cLabel);
            this.cPanel.Controls.Add(this.cBar);
            this.cPanel.Location = new System.Drawing.Point(0, 61);
            this.cPanel.Name = "cPanel";
            this.cPanel.Size = new System.Drawing.Size(432, 26);
            this.cPanel.TabIndex = 16;
            // 
            // cLabel
            // 
            this.cLabel.AutoSize = true;
            this.cLabel.ForeColor = System.Drawing.Color.White;
            this.cLabel.Location = new System.Drawing.Point(2, 5);
            this.cLabel.Name = "cLabel";
            this.cLabel.Size = new System.Drawing.Size(14, 13);
            this.cLabel.TabIndex = 15;
            this.cLabel.Text = "C";
            // 
            // cBar
            // 
            this.cBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cBar.AutoSize = false;
            this.cBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(98)))));
            this.cBar.ContextMenuStrip = this.cPlayMenuStrip;
            this.cBar.LargeChange = 1;
            this.cBar.Location = new System.Drawing.Point(9, -2);
            this.cBar.Margin = new System.Windows.Forms.Padding(0);
            this.cBar.Name = "cBar";
            this.cBar.Size = new System.Drawing.Size(422, 26);
            this.cBar.TabIndex = 15;
            this.cBar.ValueChanged += new System.EventHandler(this.cBar_ValueChanged);
            // 
            // cPlayMenuStrip
            // 
            this.cPlayMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playCToolStripMenuItem,
            this.stopCToolStripMenuItem,
            this.CPlaySpeedToolStripMenuItem,
            this.setCValueRangeToolStripMenuItem});
            this.cPlayMenuStrip.Name = "zPlayMenuStrip";
            this.cPlayMenuStrip.Size = new System.Drawing.Size(158, 92);
            // 
            // playCToolStripMenuItem
            // 
            this.playCToolStripMenuItem.Name = "playCToolStripMenuItem";
            this.playCToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.playCToolStripMenuItem.Text = "Play";
            this.playCToolStripMenuItem.Click += new System.EventHandler(this.playCToolStripMenuItem_Click);
            // 
            // stopCToolStripMenuItem
            // 
            this.stopCToolStripMenuItem.Checked = true;
            this.stopCToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stopCToolStripMenuItem.Name = "stopCToolStripMenuItem";
            this.stopCToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.stopCToolStripMenuItem.Text = "Stop";
            this.stopCToolStripMenuItem.Click += new System.EventHandler(this.stopCToolStripMenuItem_Click);
            // 
            // CPlaySpeedToolStripMenuItem
            // 
            this.CPlaySpeedToolStripMenuItem.Name = "CPlaySpeedToolStripMenuItem";
            this.CPlaySpeedToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.CPlaySpeedToolStripMenuItem.Text = "Play Speed";
            this.CPlaySpeedToolStripMenuItem.Click += new System.EventHandler(this.CPlaySpeedToolStripMenuItem_Click);
            // 
            // setCValueRangeToolStripMenuItem
            // 
            this.setCValueRangeToolStripMenuItem.Name = "setCValueRangeToolStripMenuItem";
            this.setCValueRangeToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.setCValueRangeToolStripMenuItem.Text = "Set Value Range";
            this.setCValueRangeToolStripMenuItem.Click += new System.EventHandler(this.setCValueRangeToolStripMenuItem_Click);
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.ForeColor = System.Drawing.Color.White;
            this.timeLabel.Location = new System.Drawing.Point(0, 33);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(14, 13);
            this.timeLabel.TabIndex = 14;
            this.timeLabel.Text = "T";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(1, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Z";
            // 
            // timelineTimer
            // 
            this.timelineTimer.Interval = 32;
            this.timelineTimer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // zTimer
            // 
            this.zTimer.Interval = 32;
            this.zTimer.Tick += new System.EventHandler(this.zTimer_Tick);
            // 
            // openImagesDialog
            // 
            this.openImagesDialog.SupportMultiDottedExtensions = true;
            this.openImagesDialog.Title = "Open Image File";
            // 
            // cTimer
            // 
            this.cTimer.Interval = 1000;
            this.cTimer.Tick += new System.EventHandler(this.cTimer_Tick);
            // 
            // ImageView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(98)))));
            this.Controls.Add(this.splitContainer);
            this.Name = "ImageView";
            this.Size = new System.Drawing.Size(432, 368);
            this.SizeChanged += new System.EventHandler(this.ImageView_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImageView_KeyDown);
            this.Resize += new System.EventHandler(this.ImageView_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).EndInit();
            this.timePlayMenuStrip.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.zBar)).EndInit();
            this.zPlayMenuStrip.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rgbPictureBox)).EndInit();
            this.rgbBoxsPanel.ResumeLayout(false);
            this.rgbBoxsPanel.PerformLayout();
            this.cPanel.ResumeLayout(false);
            this.cPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cBar)).EndInit();
            this.cPlayMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar timeBar;
        private System.Windows.Forms.ComboBox channelBoxR;
        private System.Windows.Forms.ComboBox channelBoxB;
        private System.Windows.Forms.ComboBox channelBoxG;
        private System.Windows.Forms.Label labelRGB;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem strechToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem normalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem centerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.TrackBar zBar;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ToolStripMenuItem showControlsToolStripMenuItem;
        private System.Windows.Forms.Panel rgbBoxsPanel;
        private System.Windows.Forms.ToolStripMenuItem showTimeTrackbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openChannelsToolToolStripMenuItem;
        private System.Windows.Forms.Timer timelineTimer;
        private System.Windows.Forms.ContextMenuStrip zPlayMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem playZToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopZToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip timePlayMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem playTimeToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem stopTimeToolStripMenu;
        private System.Windows.Forms.Timer zTimer;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem autoContrastChannelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playSpeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playSpeedToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem copyImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setValueRangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setValueRangeToolStripMenuItem1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openImagesDialog;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Label label1;
        private AForge.Controls.PictureBox rgbPictureBox;
        private System.Windows.Forms.ToolStripMenuItem planeModeToolStripMenuItem;
        private System.Windows.Forms.Panel cPanel;
        private System.Windows.Forms.TrackBar cBar;
        private System.Windows.Forms.Label cLabel;
        private System.Windows.Forms.Timer cTimer;
        private System.Windows.Forms.ContextMenuStrip cPlayMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem playCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CPlaySpeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setCValueRangeToolStripMenuItem;
    }
}