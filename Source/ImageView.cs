﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Bio
{
    public partial class ImageView : UserControl, IDisposable
    {
        public ImageView(BioImage im)
        {
            string file = im.ID.Replace("\\", "/");
            InitializeComponent();
            serie = im.series;
            selectedImage = im;
            tools = new Tools();
            Dock = DockStyle.Fill;
            Images.Add(im);
            UpdateImages();
            App.viewer = this;
            if (file == "" || file == null)
                return;
            SetCoordinate(0, 0, 0);
            InitGUI();
            //Buf = image.GetBufByCoord(GetCoordinate());
            MouseWheel += new System.Windows.Forms.MouseEventHandler(ImageView_MouseWheel);
            zBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(ZTrackBar_MouseWheel);
            cBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(CTrackBar_MouseWheel);
            tBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(TimeTrackBar_MouseWheel);
            //We set the trackbar event to handled so that it only scrolls one tick not the default multiple.
            zBar.MouseWheel += (sender, e) => ((HandledMouseEventArgs)e).Handled = true;
            tBar.MouseWheel += (sender, e) => ((HandledMouseEventArgs)e).Handled = true;
            cBar.MouseWheel += (sender, e) => ((HandledMouseEventArgs)e).Handled = true;
            TimeFps = 60;
            ZFps = 60;
            CFps = 1;
            // Change parent for overlay PictureBox.
            overlayPictureBox.Parent = pictureBox;
            overlayPictureBox.Location = new Point(0, 0);
            GoToImage();
            UpdateView();
        }
        ~ImageView()
        {

        }

        List<Bitmap> Bitmaps = new List<Bitmap>();
        public static List<ROI> selectedAnnotations = new List<ROI>();
        private static BioImage selectedImage = null;
        public static BioImage SelectedImage
        {
            get
            {
                return selectedImage;
            }
        }
        public static PointD mouseDown;
        public static bool down;
        public static PointD mouseUp;
        public static bool up;
        public static bool Ctrl
        {
            get
            {
                return Win32.GetKeyState(Keys.LControlKey);
            }
        }
        private bool x1State = false;
        private bool x2State = false;
        public static MouseButtons mouseUpButtons;
        public static MouseButtons mouseDownButtons;
        private PointD pd;
        public static bool showBounds = true;
        public static bool showText = true;
        public Image Buf = null;
        public bool init = false;
        public List<BioImage> Images = new List<BioImage>();
        private int selIndex = 0;
        public int SelectedIndex
        {
            get
            {
                return selIndex;
            }
            set
            {
                selIndex = value;
                selectedImage = Images[selIndex];
                InitGUI();
            }
        }

        public string filepath = "";
        public int serie = 0;
        public int minSizeX = 50;
        public int minSizeY = 20;
        public bool loopZ = true;
        public bool loopT = true;
        public bool loopC = true;
        private double pxWmicron = 0.004;
        private double pxHmicron = 0.004;
        public static SizeF scale = new SizeF(1, 1);
        public void SetCoordinate(int z, int c, int t)
        {
            if (SelectedImage == null)
                return;
            if (z >= SelectedImage.SizeZ)
                zBar.Value = zBar.Maximum;
            if (c >= SelectedImage.SizeC)
                cBar.Value = cBar.Maximum;
            if (t >= SelectedImage.SizeT)
                tBar.Value = tBar.Maximum;
            zBar.Value = z;
            cBar.Value = c;
            tBar.Value = t;
            UpdateImage();
        }
        public ZCT GetCoordinate()
        {
            return SelectedImage.Coordinate;
        }
        public void AddImage(BioImage im)
        {
            Images.Add(im);
            SelectedIndex = Images.Count - 1;
            InitGUI();
            UpdateImages();
            GoToImage(Images.Count - 1);
        }
        private bool showControls = true;
        public bool ShowControls
        {
            get { return trackBarPanel.Visible; }
            set
            {
                showControls = value;
                if (!value)
                {
                    trackBarPanel.Hide();
                    if (ShowStatus)
                    {
                        panel.Top = 25;
                        panel.Height += 75;
                    }
                    else
                    {
                        panel.Top = 0;
                        panel.Height += 75;
                    }
                    hideControlsToolStripMenuItem1.Text = "Show Controls";
                }
                else
                {
                    trackBarPanel.Show();
                    if (ShowStatus)
                    {
                        panel.Top = 25;
                        panel.Height -= 75;
                    }
                    else
                    {
                        panel.Top = 0;
                        panel.Height += 75;
                    }
                    
                    panel.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                    hideControlsToolStripMenuItem1.Text = "Hide Controls";
                }
                UpdateView();
            }
        }
        private bool showStatus = true;
        public bool ShowStatus
        {
            get { return showStatus; }
            set
            {
                showStatus = true;
                if (!value)
                {
                    statusPanel.Hide();
                    panel.Top = 0;
                    panel.Height += 25;
                    showControlsToolStripMenuItem.Visible = true;
                    hideControlsToolStripMenuItem.Text = "Show Status";
                }
                else
                {
                    statusPanel.Show();
                    statusPanel.Visible = value;
                    panel.Top = 25;
                    showControlsToolStripMenuItem.Visible = false;
                    hideControlsToolStripMenuItem.Text = "Hide Status";
                }
            }
        }
        public double PxWmicron
        {
            get
            {
                return pxWmicron;
            }
            set
            {
                pxWmicron = value;
            }
        }
        public double PxHmicron
        {
            get
            {
                return pxHmicron;
            }
            set
            {
                pxHmicron = value;
            }
        }
        public enum ViewMode
        {
            Raw,
            Filtered,
            RGBImage
        }
        private ViewMode viewMode = ViewMode.Filtered;
        public ViewMode Mode
        {
            get
            {
                return viewMode;
            }
            set
            {
                viewMode = value;
                //If view mode is changed we update.
                App.tabsView.UpdateViewMode(viewMode);
                UpdateView();
                UpdateOverlay();
                UpdateImages();
                if (viewMode == ViewMode.RGBImage)
                {
                    cBar.Value = 0;
                    rgbBoxsPanel.BringToFront();
                    cBar.SendToBack();
                    cLabel.SendToBack();
                }
                else
                if (viewMode == ViewMode.Filtered)
                {
                    rgbBoxsPanel.SendToBack();
                    cBar.BringToFront();
                    cLabel.BringToFront();
                }
                else
                if (viewMode == ViewMode.Raw)
                {
                    rgbBoxsPanel.SendToBack();
                    cBar.BringToFront();
                    cLabel.BringToFront();
                }
            }
        }
        public string Path
        {
            get
            {
                return filepath;
            }
        }
        public Channel RChannel
        {
            get
            {
                return SelectedImage.Channels[SelectedImage.rgbChannels[0]];
            }
        }
        public Channel GChannel
        {
            get
            {
                return SelectedImage.Channels[SelectedImage.rgbChannels[1]];
            }
        }
        public Channel BChannel
        {
            get
            {
                return SelectedImage.Channels[SelectedImage.rgbChannels[2]];
            }
        }
        private PointD origin = new PointD(0, 0);
        public PointD Origin
        {
            get { return origin; }
            set
            {
                origin = value;
            }
        }

        public Tools tools;
        public void UpdateRGBChannels()
        {
            //Buf = image.GetBufByCoord(GetCoordinate());
            if (channelBoxR.SelectedIndex == -1)
                SelectedImage.rgbChannels[0] = 0;
            else
                SelectedImage.rgbChannels[1] = channelBoxR.SelectedIndex;
            if (channelBoxG.SelectedIndex == -1)
                SelectedImage.rgbChannels[1] = 0;
            else
                SelectedImage.rgbChannels[1] = channelBoxG.SelectedIndex;
            if (channelBoxB.SelectedIndex == -1)
                SelectedImage.rgbChannels[2] = 0;
            else
                SelectedImage.rgbChannels[2] = channelBoxB.SelectedIndex;

        }
        private bool timeEnabled = false;
        private int zfps;
        public int ZFps
        {
            get
            {
                return zfps;
            }
            set
            {
                zfps = value;
                float f = value;
                zTimer.Interval = (int)Math.Floor(1000 / f);
            }
        }
        private int timefps;
        public int TimeFps
        {
            get
            {
                return timefps;
            }
            set
            {
                timefps = value;
                float f = value;
                timelineTimer.Interval = (int)Math.Floor(1000 / f);
            }
        }
        private int cfps;
        public int CFps
        {
            get
            {
                return cfps;
            }
            set
            {
                cfps = value;
                float f = value;
                cTimer.Interval = (int)Math.Floor(1000 / f);
            }
        }
        public void InitGUI()
        {
            zBar.Maximum = SelectedImage.SizeZ - 1;
            if(SelectedImage.Buffers[0].RGBChannelsCount == 3)
                cBar.Maximum = 0;
            else
                cBar.Maximum = SelectedImage.SizeC - 1;
            if (SelectedImage.SizeT > 1)
            {
                tBar.Maximum = SelectedImage.SizeT - 1;
                timeEnabled = true;
            }
            else
            {
                tBar.Enabled = false;
                timeEnabled = false;
                tBar.Maximum = SelectedImage.SizeT - 1;
            }
            //rgbPictureBox.Image = image.plane.GetBitmap();
            //we clear the channel comboboxes incase we have channels from previous loaded image.
            channelBoxR.Items.Clear();
            channelBoxG.Items.Clear();
            channelBoxB.Items.Clear();

            foreach (Channel ch in SelectedImage.Channels)
            {
                channelBoxR.Items.Add(ch);
                channelBoxG.Items.Add(ch);
                channelBoxB.Items.Add(ch);
            }
            if (SelectedImage.Channels.Count > 2)
            {
                channelBoxR.SelectedIndex = 0;
                channelBoxG.SelectedIndex = 1;
                channelBoxB.SelectedIndex = 2;
            }
            else
            if (SelectedImage.Channels.Count == 2)
            {
                channelBoxR.SelectedIndex = 0;
                channelBoxG.SelectedIndex = 1;
            }
            UpdateRGBChannels();
            init = true;
        }
        public void UpdateSelectBoxSize(float size)
        {
            if (SelectedImage == null)
                return;
            foreach (ROI item in SelectedImage.Annotations)
            {
                item.selectBoxSize = size;
            }
        }
        public void UpdateOverlay()
        {
            overlayPictureBox.Invalidate();
        }
        public void UpdateStatus()
        {
            if (SelectedImage == null)
                return;
            if (Mode == ViewMode.RGBImage)
            {
                //Since combining 3 planes to one image takes time we show the image load time.
                float ms = SelectedImage.loadTimeMS;
                ticksLabel.Text = " Ticks: " + SelectedImage.loadTimeTicks;

                if (timeEnabled)
                {
                    statusLabel.Text = (zBar.Value + 1) + "/" + (zBar.Maximum + 1) + ", " + (tBar.Value + 1) + "/" + (tBar.Maximum + 1) + ", " + 
                        mousePoint + mouseColor + ", " + SelectedImage.Buffers[0].PixelFormat.ToString() + ", (" + -Origin.X + ", " + -Origin.Y + "), (" + SelectedImage.Volume.Location.X + ", " + SelectedImage.Volume.Location.Y + ")";
                }
                else
                {
                    statusLabel.Text = (zBar.Value + 1) + "/" + (cBar.Maximum + 1) + ", " + mousePoint + mouseColor + ", " + SelectedImage.Buffers[0].PixelFormat.ToString()
                        + ", (" + -Origin.X + ", " + -Origin.Y + "), (" + SelectedImage.Volume.Location.X + ", " + SelectedImage.Volume.Location.Y + ")";
                }

            }
            else
            {
                if (timeEnabled)
                {
                    statusLabel.Text = (zBar.Value + 1) + "/" + (zBar.Maximum + 1) + ", " + (cBar.Value + 1) + "/" + (cBar.Maximum + 1) + ", " + (tBar.Value + 1) + "/" + (tBar.Maximum + 1) + ", " +
                        mousePoint + mouseColor + ", " + SelectedImage.Buffers[0].PixelFormat.ToString() + ", (" + -Origin.X + ", " + -Origin.Y + "), (" + SelectedImage.Volume.Location.X + ", " + SelectedImage.Volume.Location.Y + ")";
                }
                else
                {
                    statusLabel.Text = (zBar.Value + 1) + "/" + (zBar.Maximum + 1) + ", " + (cBar.Value + 1) + "/" + (cBar.Maximum + 1) + ", " + mousePoint + mouseColor + ", " +
                        SelectedImage.Buffers[0].PixelFormat.ToString() + ", (" + -Origin.X + ", " + -Origin.Y + "), (" + SelectedImage.Volume.Location.X + ", " + SelectedImage.Volume.Location.Y + ")";
                }
            }
        }
        public void UpdateView()
        {
            UpdateStatus();
            pictureBox.Invalidate();
            overlayPictureBox.Invalidate();
        }
        public void UpdateImages()
        {
            for (int i = 0; i < Bitmaps.Count; i++)
            {
                Bitmaps[i] = null;
            }
            GC.Collect();
            Bitmaps.Clear();
            foreach (BioImage b in Images)
            {
                ZCT coords = new ZCT(zBar.Value, cBar.Value, tBar.Value);
                Bitmap bitmap;

                int index = b.Coords[zBar.Value, cBar.Value, tBar.Value];
                if (Mode == ViewMode.Filtered)
                {
                    bitmap = b.GetFiltered(coords, b.RChannel.range, b.GChannel.range, b.BChannel.range);
                }
                else if (Mode == ViewMode.RGBImage)
                {
                    PixelFormat px = SelectedImage.Buffers[index].PixelFormat;
                    if (px == PixelFormat.Format8bppIndexed || px == PixelFormat.Format16bppGrayScale)
                    {
                        bitmap = b.GetRGBBitmap(coords, b.RChannel.range, b.GChannel.range, b.BChannel.range);
                    }
                    else
                    {
                        bitmap = (Bitmap)b.Buffers[index].Image;
                    }
                }
                else
                {
                    bitmap = (Bitmap)b.Buffers[index].Image;
                }
                if (bitmap.PixelFormat == PixelFormat.Format16bppGrayScale || bitmap.PixelFormat == PixelFormat.Format48bppRgb)
                    bitmap = AForge.Imaging.Image.Convert16bppTo8bpp((Bitmap)bitmap);
                Bitmaps.Add(bitmap);
            }
        }
        Bitmap bitmap;
        public void UpdateImage()
        {
            if (Bitmaps.Count == 0)
                return;
            ZCT coords = new ZCT(zBar.Value, cBar.Value, tBar.Value);
            bitmap = null;
            GC.Collect();
            if (zBar.Maximum != SelectedImage.SizeZ - 1 || cBar.Maximum != SelectedImage.SizeC - 1 || tBar.Maximum != SelectedImage.SizeT - 1)
            {
                zBar.Value = 0;
                cBar.Value = 0;
                tBar.Value = 0;
                InitGUI();
            }
            int index = SelectedImage.Coords[zBar.Value, cBar.Value, tBar.Value];
            if (Mode == ViewMode.Filtered)
            {
                bitmap = SelectedImage.GetFiltered(coords, RChannel.range, GChannel.range, BChannel.range);
            }
            else if (Mode == ViewMode.RGBImage)
            {
                PixelFormat px = SelectedImage.Buffers[index].PixelFormat;
                if (px == PixelFormat.Format8bppIndexed || px == PixelFormat.Format16bppGrayScale)
                {
                    bitmap = SelectedImage.GetRGBBitmap(coords, RChannel.range, GChannel.range, BChannel.range);
                }
                else
                {
                    bitmap = (Bitmap)SelectedImage.Buffers[index].Image;
                }
            }
            else
            {
                bitmap = (Bitmap)SelectedImage.Buffers[index].Image;
            }
            if (bitmap.PixelFormat == PixelFormat.Format16bppGrayScale || bitmap.PixelFormat == PixelFormat.Format48bppRgb)
                bitmap = AForge.Imaging.Image.Convert16bppTo8bpp((Bitmap)bitmap);
            if (SelectedIndex < Bitmaps.Count)
                Bitmaps[SelectedIndex] = bitmap;
            else
                Bitmaps.Add(bitmap);
        }
        private void channelBoxR_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxR.SelectedIndex == -1)
                return;
            SelectedImage.rgbChannels[0] = channelBoxR.SelectedIndex;
            UpdateImage();
            UpdateView();
        }
        private void channelBoxG_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxG.SelectedIndex == -1)
                return;
            SelectedImage.rgbChannels[1] = channelBoxG.SelectedIndex;
            UpdateImage();
            UpdateView();
        }
        private void channelBoxB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxB.SelectedIndex == -1)
                return;
            SelectedImage.rgbChannels[2] = channelBoxB.SelectedIndex;
            UpdateImage();
            UpdateView();
        }
        private void showControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (trackBarPanel.Visible)
            {
                trackBarPanel.Hide();
                trackBarPanel.Height = 0;
                Application.DoEvents();
                pictureBox.Height += 75;
                overlayPictureBox.Height += 75;
                showControlsToolStripMenuItem.Text = "Show Controls";
                pictureBox.Dock = DockStyle.Fill;
            }
            else
            {
                trackBarPanel.Show();
                pictureBox.Height -= 75;
                Application.DoEvents();
                trackBarPanel.Height = 75;
                overlayPictureBox.Height += trackBarPanel.Height;
                showControlsToolStripMenuItem.Text = "Hide Controls";
                pictureBox.Dock = DockStyle.Fill;
            }

        }
        private void playZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (playZToolStripMenuItem.Checked)
            {
                //We stop
                playZToolStripMenuItem.Checked = false;
                stopZToolStripMenuItem.Checked = true;
                zTimer.Stop();
            }
            else
            {
                //We start
                playZToolStripMenuItem.Checked = true;
                stopZToolStripMenuItem.Checked = false;
                zTimer.Start();
            }
        }
        private void stopZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stopZToolStripMenuItem.Checked)
            {
                //We start
                playZToolStripMenuItem.Checked = true;
                stopZToolStripMenuItem.Checked = false;
                zTimer.Start();

            }
            else
            {
                //We stop
                playZToolStripMenuItem.Checked = false;
                stopZToolStripMenuItem.Checked = true;
                zTimer.Stop();
            }
        }
        private void playTimeToolStripMenu_Click(object sender, EventArgs e)
        {
            if (playTimeToolStripMenu.Checked)
            {
                //We stop
                playTimeToolStripMenu.Checked = false;
                stopTimeToolStripMenu.Checked = true;
                timelineTimer.Stop();
            }
            else
            {
                //We start
                playTimeToolStripMenu.Checked = true;
                stopTimeToolStripMenu.Checked = false;
                timelineTimer.Start();
            }
        }
        private void stopTimeToolStripMenu_Click(object sender, EventArgs e)
        {
            playTimeToolStripMenu.Checked = false;
            stopTimeToolStripMenu.Checked = true;
            timelineTimer.Stop();
        }
        private void playCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (playCToolStripMenuItem.Checked)
            {
                //We stop
                playCToolStripMenuItem.Checked = false;
                stopCToolStripMenuItem.Checked = true;
                cTimer.Stop();
            }
            else
            {
                //We start
                playCToolStripMenuItem.Checked = true;
                stopCToolStripMenuItem.Checked = false;
                cTimer.Start();
            }
        }
        private void stopCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopCToolStripMenuItem.Checked = false;
            playCToolStripMenuItem.Checked = true;
            cTimer.Stop();
        }
        private string mousePoint = "";
        private string mouseColor = "";
        private void playSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = null;
            if (Mode == ViewMode.RGBImage)
                sp = new PlaySpeed(timeEnabled, false, ZFps, TimeFps, CFps);
            else
                sp = new PlaySpeed(timeEnabled, true, ZFps, TimeFps, CFps);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            zTimer.Interval = sp.TimePlayspeed;
            cTimer.Interval = sp.CPlayspeed;
            timelineTimer.Interval = sp.TimePlayspeed;
        }
        private void playSpeedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = null;
            if (Mode == ViewMode.RGBImage)
                sp = new PlaySpeed(timeEnabled, false, ZFps, TimeFps, CFps);
            else
                sp = new PlaySpeed(timeEnabled, true, ZFps, TimeFps, CFps);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            zTimer.Interval = sp.TimePlayspeed;
            cTimer.Interval = sp.CPlayspeed;
            timelineTimer.Interval = sp.TimePlayspeed;
        }
        private void CPlaySpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = null;
            if (Mode == ViewMode.RGBImage)
                sp = new PlaySpeed(timeEnabled, false, ZFps, TimeFps, CFps);
            else
                sp = new PlaySpeed(timeEnabled, true, ZFps, TimeFps, CFps);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            zTimer.Interval = sp.TimePlayspeed;
            cTimer.Interval = sp.CPlayspeed;
            timelineTimer.Interval = sp.TimePlayspeed;
        }
        public void GetRange()
        {
            RangeTool t;
            if (Mode == ViewMode.Filtered)
                t = new RangeTool(timeEnabled, true, zBar.Minimum, zBar.Maximum, tBar.Minimum, tBar.Maximum, cBar.Minimum, cBar.Maximum);
            else
                t = new RangeTool(timeEnabled, false, zBar.Minimum, zBar.Maximum, tBar.Minimum, tBar.Maximum, cBar.Minimum, cBar.Maximum);
            if (t.ShowDialog() != DialogResult.OK)
                return;
            zBar.Minimum = t.ZMin;
            zBar.Maximum = t.ZMax;
            tBar.Minimum = t.TimeMin;
            tBar.Maximum = t.TimeMax;
            cBar.Minimum = t.CMin;
            cBar.Maximum = t.CMax;
        }
        private void setValueRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetRange();
        }
        private void setValueRangeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GetRange();
        }
        private void setCValueRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetRange();
        }     
        private void ImageView_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            float dx = scale.Width/50;
            float dy = scale.Height/50;
            if (Ctrl)
            {
                if (e.Delta > 0)
                {
                    scale.Width += dx;
                    scale.Height += dy;
                }
                else
                {
                    scale.Width -= dx;
                    scale.Height -= dy;
                }
                UpdateView();
            }
            else
            if (e.Delta > 0)
            {
                if (zBar.Value + 1 <= zBar.Maximum)
                    zBar.Value += 1;
            }
            else
            {
                if (zBar.Value - 1 >= zBar.Minimum)
                    zBar.Value -= 1;
            }
        }
        private void ZTrackBar_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (zBar.Value + 1 <= zBar.Maximum)
                    zBar.Value += 1;
            }
            else
            {
                if (zBar.Value - 1 >= zBar.Minimum)
                    zBar.Value -= 1;
            }

        }
        private void TimeTrackBar_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!timeEnabled)
                return;
            if (e.Delta > 0)
            {
                if (tBar.Value + 1 <= tBar.Maximum)
                    tBar.Value += 1;
            }
            else
            {
                if (tBar.Value - 1 >= tBar.Minimum)
                    tBar.Value -= 1;
            }
        }
        private void CTrackBar_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (cBar.Value + 1 <= cBar.Maximum)
                    cBar.Value += 1;
            }
            else
            {
                if (cBar.Value - 1 >= cBar.Minimum)
                    cBar.Value -= 1;
            }
        }

        private void cTimer_Tick(object sender, EventArgs e)
        {
            if (playCToolStripMenuItem.Checked)
            {
                if (cBar.Maximum >= cBar.Value + 1)
                    cBar.Value++;
                else
                {
                    if (loopC)
                        cBar.Value = cBar.Minimum;
                }
            }
        }
        private void zTimer_Tick(object sender, EventArgs e)
        {
            if (playZToolStripMenuItem.Checked)
            {
                if (zBar.Maximum >= zBar.Value + 1)
                    zBar.Value++;
                else
                {
                    if (loopZ)
                        zBar.Value = zBar.Minimum;
                }
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (playTimeToolStripMenu.Checked)
            {
                if (tBar.Maximum >= tBar.Value + 1)
                    tBar.Value++;
                else
                {
                    if (loopT)
                        tBar.Value = tBar.Minimum;
                }
            }
        }
        private void zBar_ValueChanged(object sender, EventArgs e)
        {
            SelectedImage.Coordinate = new ZCT(zBar.Value, SelectedImage.Coordinate.C, SelectedImage.Coordinate.T);
            UpdateImage();
            UpdateView();
        }
        private void timeBar_ValueChanged(object sender, EventArgs e)
        {
            SelectedImage.Coordinate = new ZCT(SelectedImage.Coordinate.Z, SelectedImage.Coordinate.C, tBar.Value);
            UpdateImage();
            UpdateView();
        }
        private void cBar_ValueChanged(object sender, EventArgs e)
        {
            SelectedImage.Coordinate = new ZCT(SelectedImage.Coordinate.Z, cBar.Value, SelectedImage.Coordinate.T);
            UpdateImage();
            UpdateView();
        }

        private void loopTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loopT = loopTimeToolStripMenuItem.Checked;
        }
        private void loopZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loopZ = loopZToolStripMenuItem.Checked;
        }
        private void loopCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loopC = loopCToolStripMenuItem.Checked;
        }

        public bool showRROIs = true;
        public bool showGROIs = true;
        public bool showBROIs = true;

        private List<ROI> annotationsR = new List<ROI>();
        public List<ROI> AnnotationsR
        {
            get
            {
                return SelectedImage.GetAnnotations(SelectedImage.Coordinate.Z, SelectedImage.RChannel.Index, SelectedImage.Coordinate.T);
            }
        }
        private List<ROI> annotationsG = new List<ROI>();
        public List<ROI> AnnotationsG
        {
            get
            {
                return SelectedImage.GetAnnotations(SelectedImage.Coordinate.Z, SelectedImage.GChannel.Index, SelectedImage.Coordinate.T);
            }
        }
        private List<ROI> annotationsB = new List<ROI>();
        public List<ROI> AnnotationsB
        {
            get
            {
                return SelectedImage.GetAnnotations(SelectedImage.Coordinate.Z, SelectedImage.BChannel.Index, SelectedImage.Coordinate.T);
            }
        }
        public List<ROI> AnnotationsRGB
        {
            get
            {
                if (SelectedImage == null)
                    return null;
                List<ROI> ans = new List<ROI>();
                if (Mode == ViewMode.RGBImage)
                {
                    if (showRROIs)
                        ans.AddRange(AnnotationsR);
                    if (showGROIs)
                        ans.AddRange(AnnotationsG);
                    if (showBROIs)
                        ans.AddRange(AnnotationsB);
                }
                else
                {
                    ans.AddRange(SelectedImage.GetAnnotations(SelectedImage.Coordinate));
                }
                return ans;
            }
        }
        private void DrawOverlay(Graphics g)
        {
            if (SelectedImage == null)
                return;
            SetCoordinate(zBar.Value, cBar.Value, tBar.Value);
            Pen pen = null;
            Pen red = null;
            Pen green = null;
            Pen mag = null;
            Pen blue = null;
            Brush b = null;
            
            bool bounds = showBounds;
            bool labels = showText;

            ZCT cor = GetCoordinate();
            List<ROI> ans = SelectedImage.GetAnnotations(cor);
            foreach (ROI an in ans)
            {
                pen = new Pen(an.strokeColor, (float)an.strokeWidth / scale.Width);
                red = new Pen(Brushes.Red, (float)an.strokeWidth / scale.Width);
                mag = new Pen(Brushes.Magenta, (float)an.strokeWidth / scale.Width);
                green = new Pen(Brushes.Green, (float)an.strokeWidth / scale.Width);
                blue = new Pen(Brushes.Blue, (float)an.strokeWidth / scale.Width);
                Font fo = new Font(an.font.FontFamily, (float)an.strokeWidth / scale.Width);
                if (an.selected)
                {
                    b = new SolidBrush(Color.Magenta);
                }
                else
                    b = new SolidBrush(an.strokeColor);
                PointF pc = new PointF((float)(an.BoundingBox.X + (an.BoundingBox.W / 2)), (float)(an.BoundingBox.Y + (an.BoundingBox.H / 2)));
                if (an.type == ROI.Type.Point)
                {
                    g.DrawLine(pen, ToScreenSpace(an.Point.ToPointF()), ToScreenSpace(new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1)));
                    g.DrawRectangles(red, ToScreenSpace(an.GetSelectBoxes(scale.Width)));
                }
                else
                if (an.type == ROI.Type.Line)
                {
                    g.DrawLine(pen, ToScreenSpace(an.GetPoint(0).ToPointF()), ToScreenSpace(an.GetPoint(1).ToPointF()));
                    g.DrawRectangles(red, ToScreenSpace(an.GetSelectBoxes(scale.Width)));
                }
                else
                if (an.type == ROI.Type.Rectangle && an.Rect.W > 0 && an.Rect.H > 0)
                {
                    RectangleF[] rects = new RectangleF[1];
                    rects[0] = an.Rect.ToRectangleF();
                    g.DrawRectangles(pen, ToScreenSpace(rects));
                    g.DrawRectangles(red, ToScreenSpace(an.GetSelectBoxes(scale.Width)));
                }
                else
                if (an.type == ROI.Type.Ellipse)
                {
                    g.DrawEllipse(pen, ToScreenSpace(an.Rect.ToRectangleF()));
                    g.DrawRectangles(red, ToScreenSpace(an.GetSelectBoxes(scale.Width)));
                }
                else
                if (an.type == ROI.Type.Polygon && an.closed)
                {
                    g.DrawPolygon(pen, ToScreenSpace(an.GetPointsF()));
                    g.DrawRectangles(red, ToScreenSpace(an.GetSelectBoxes(scale.Width)));
                }
                else
                if (an.type == ROI.Type.Polygon && !an.closed)
                {
                    PointF[] points = an.GetPointsF();
                    if (points.Length == 1)
                    {
                        g.DrawLine(pen, ToScreenSpace(an.Point.ToPointF()), ToScreenSpace(new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1)));
                    }
                    else
                        g.DrawLines(pen, ToScreenSpace(points));
                    g.DrawRectangles(red, ToScreenSpace(an.GetSelectBoxes(scale.Width)));
                }
                else
                if (an.type == ROI.Type.Polyline)
                {
                    g.DrawLines(pen, ToScreenSpace(an.GetPointsF()));
                    g.DrawRectangles(red, ToScreenSpace(an.GetSelectBoxes(scale.Width)));
                }

                else
                if (an.type == ROI.Type.Freeform && an.closed)
                {
                    PointF[] points = an.GetPointsF();
                    if (points.Length > 1)
                        if (points.Length == 1)
                        {
                            g.DrawLine(pen, ToScreenSpace(an.Point.ToPointF()), ToScreenSpace(new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1)));
                        }
                        else
                            g.DrawPolygon(pen, ToScreenSpace(an.GetPointsF()));
                }
                else
                if (an.type == ROI.Type.Freeform && !an.closed)
                {
                    PointF[] points = an.GetPointsF();
                    if (points.Length > 1)
                        if (points.Length == 1)
                        {
                            g.DrawLine(pen, ToScreenSpace(an.Point.ToPointF()), ToScreenSpace(new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1)));
                        }
                        else
                            g.DrawLines(pen, ToScreenSpace(points));
                }
                if (an.type == ROI.Type.Label)
                {
                    g.DrawString(an.Text, fo, b, ToScreenSpace(an.Point.ToPointF()));
                    g.DrawRectangles(red, ToScreenSpace(an.GetSelectBoxes(scale.Width)));
                }
                if (labels)
                {
                    //Lets draw the text of this ROI in the middle of the RO
                    float fw = ((float)an.Rect.X + ((float)an.Rect.W / 2)) - ((float)an.TextSize.Width / 2);
                    float fh = ((float)an.Rect.Y + ((float)an.Rect.H / 2)) - ((float)an.TextSize.Height / 2);
                    g.DrawString(an.Text, fo, b, ToScreenSpace(new PointF(fw, fh)));
                }
                if (bounds)
                {
                    RectangleF[] rects = new RectangleF[1];
                    rects[0] = an.BoundingBox.ToRectangleF();
                    g.DrawRectangles(green, ToScreenSpace(rects));
                }
                if (an.selected)
                {
                    //Lets draw the bounding box.
                    RectangleF[] bo = new RectangleF[1];
                    bo[0] = an.BoundingBox.ToRectangleF();
                    g.DrawRectangles(mag, ToScreenSpace(bo));
                    //Lets draw the selectBoxes.
                    List<RectangleF> rects = new List<RectangleF>();
                    RectangleF[] sels = an.GetSelectBoxes(scale.Width);
                    for (int i = 0; i < an.selectedPoints.Count; i++)
                    {
                        if (an.selectedPoints[i] < an.GetPointCount())
                        {
                            rects.Add(sels[an.selectedPoints[i]]);
                        }
                    }
                    if (rects.Count > 0)
                        g.DrawRectangles(blue, ToScreenSpace(rects.ToArray()));
                    rects.Clear();
                    //Lets draw the text of this ROI in the middle of the ROI
                    float fw = ((float)an.Rect.X + ((float)an.Rect.W / 2)) - ((float)an.TextSize.Width / 2);
                    float fh = ((float)an.Rect.Y + ((float)an.Rect.H / 2)) - ((float)an.TextSize.Height / 2);
                    g.DrawString(an.Text, fo, b, ToScreenSpace(new PointF(fw, fh)));
                }
                pen.Dispose();
                red.Dispose();
                mag.Dispose();
                green.Dispose();
                blue.Dispose();
                b.Dispose();
            }

        }
        private void overlayPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TranslateTransform(pictureBox.Width / 2, pictureBox.Height / 2);
            if (scale.Width == 0)
                scale = new SizeF(0.00001f, 0.00001f);
            g.ScaleTransform(scale.Width, scale.Height);
            DrawOverlay(g);
            TabsView.graphics = g;
            Point3D p = Microscope.GetPosition();
            
            if ((Tools.currentTool.type == Tools.Tool.Type.rectSel && down) || (Tools.currentTool.type == Tools.Tool.Type.magic && down))
            {
                Pen mag = new Pen(Brushes.Magenta, (float)1/scale.Width);
                RectangleF[] fs = new RectangleF[1];
                fs[0] = Tools.GetTool(Tools.Tool.Type.rectSel).RectangleF;
                g.DrawRectangles(mag, ToScreenSpace(fs));
                mag.Dispose();
            }
            else
                Tools.GetTool(Tools.Tool.Type.rectSel).Rectangle = new RectangleD(0, 0, 0, 0);
        }

        private void DrawView(Graphics g)
        {
            g.TranslateTransform(pictureBox.Width / 2, pictureBox.Height / 2);
            if (scale.Width == 0 || float.IsInfinity(scale.Width))
                scale = new SizeF(0.00001f, 0.00001f);
            g.ScaleTransform(scale.Width, scale.Height);
            g.FillRectangle(Brushes.LightGray, ToScreenRectF(PointD.MinX, PointD.MinY, PointD.MaxX - PointD.MinX, PointD.MaxY - PointD.MinY));
            RectangleF[] rf = new RectangleF[1];
            Pen blue = new Pen(Brushes.Blue, 1 / scale.Width);
            int i = 0;
            foreach (BioImage im in Images)
            {
                RectangleF r = ToScreenRectF(im.Volume.Location.X, im.Volume.Location.Y, im.Volume.Width, im.Volume.Height);
                g.DrawImage(Bitmaps[i], r.X, r.Y, r.Width, r.Height);
                if (i == SelectedIndex)
                {
                    rf[0] = r;
                    g.DrawRectangles(blue, rf);
                }
                i++;
            }
            blue.Dispose();
        }
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            DrawView(e.Graphics);
        }

        private void rgbPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (SelectedImage == null)
                return;
            selectedImage = SelectedImage;
            PointD p = ToViewSpace(e.Location.X,e.Location.Y);
            Point ip = SelectedImage.ToImageSpace(p);
            mousePoint = "(" + p.X + ", " + p.Y + ")";
            if (Mode != ViewMode.RGBImage)
            {
                if (e.Button == MouseButtons.XButton1 && !x1State)
                {
                    if (cBar.Value < cBar.Maximum)
                        cBar.Value++;
                    x1State = true;
                }
                if (e.Button != MouseButtons.XButton1)
                    x1State = false;

                if (e.Button == MouseButtons.XButton2 && !x2State)
                {
                    if (cBar.Value > cBar.Minimum)
                        cBar.Value--;
                    x2State = true;
                }
                if (e.Button != MouseButtons.XButton2)
                    x2State = false;
            }

            if (Tools.currentTool.type == Tools.Tool.Type.move && e.Button == MouseButtons.Left)
            {
                foreach (ROI an in selectedAnnotations)
                {
                    if (an.selectedPoints.Count > 0 && an.selectedPoints.Count < an.GetPointCount())
                    {
                        if (an.type == ROI.Type.Rectangle || an.type == ROI.Type.Ellipse)
                        {
                            RectangleD d = an.Rect;
                            if (an.selectedPoints[0] == 0)
                            {
                                double dw = d.X - p.X;
                                double dh = d.Y - p.Y;
                                d.X = p.X;
                                d.Y = p.Y;
                                d.W += dw;
                                d.H += dh;
                            }
                            else
                            if (an.selectedPoints[0] == 1)
                            {
                                double dw = p.X - (d.W + d.X);
                                double dh = d.Y - p.Y;
                                d.W += dw;
                                d.H += dh;
                                d.Y -= dh;
                            }
                            else
                            if (an.selectedPoints[0] == 2)
                            {
                                double dw = d.X - p.X;
                                double dh = p.Y - (d.Y + d.H);
                                d.W += dw;
                                d.H += dh;
                                d.X -= dw;
                            }
                            else
                            if (an.selectedPoints[0] == 3)
                            {
                                double dw = d.X - p.X;
                                double dh = d.Y - p.Y;
                                d.W = p.X - an.X;
                                d.H = p.Y - an.Y;
                            }
                            an.Rect = d;
                        }
                        else
                        {
                            PointD pod = new PointD(p.X - pd.X, p.Y - pd.Y);
                            for (int i = 0; i < an.selectedPoints.Count; i++)
                            {
                                PointD poid = an.GetPoint(an.selectedPoints[i]);
                                an.UpdatePoint(new PointD(poid.X + pod.X, poid.Y + pod.Y), an.selectedPoints[i]);
                            }
                        }
                    }
                    else
                    {
                        PointD pod = new PointD(p.X - pd.X, p.Y - pd.Y);
                        for (int i = 0; i < an.GetPointCount(); i++)
                        {
                            PointD poid = an.PointsD[i];
                            an.UpdatePoint(new PointD(poid.X + pod.X, poid.Y + pod.Y), i);
                        }
                    }
                }
                UpdateOverlay();
            }

            if (Tools.currentTool != null)
                if (Tools.currentTool.type == Tools.Tool.Type.pencil && e.Button == MouseButtons.Left)
                    if (Mode == ViewMode.RGBImage)
                    {
                        BioImage b = ImageView.SelectedImage;
                        ZCT co = SelectedImage.Coordinate;
                        if (b.RGBChannelCount > 1)
                        {
                            ZCTXY cor = new ZCTXY(co.Z, co.C, co.T, ip.X, ip.Y);
                            Tools.Tool tool = Tools.currentTool;
                            if (Tools.rEnabled)
                                b.SetValueRGB(cor, 0, tool.Color.R);
                            if (Tools.gEnabled)
                                b.SetValueRGB(cor, 0, tool.Color.G);
                            if (Tools.bEnabled)
                                b.SetValueRGB(cor, 0, tool.Color.B);
                        }
                        else
                        if (Mode == ViewMode.RGBImage)
                        {
                            if (Tools.rEnabled)
                                SelectedImage.SetValue(ip.X, ip.Y, RChannel.Index, Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                            if (Tools.gEnabled)
                                SelectedImage.SetValue(ip.X,ip.Y, GChannel.Index, Tools.GetTool(Tools.Tool.Type.pencil).Color.G);
                            if (Tools.bEnabled)
                                SelectedImage.SetValue(ip.X, ip.Y, BChannel.Index, Tools.GetTool(Tools.Tool.Type.pencil).Color.B);
                        }
                        else
                        {
                            SelectedImage.SetValue(ip.X, ip.Y, GetCoordinate(), Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                        }
                        UpdateView();
                    }
                    else
                    if (Mode == ViewMode.Filtered)
                    {
                        if (SelectedImage.RGBChannelCount > 1)
                        {
                            SelectedImage.SetValue(ip.X, ip.Y, GetCoordinate(), Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                        }
                        else
                        {
                            SelectedImage.SetValue(ip.X, ip.Y, GetCoordinate(), Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                        }
                        UpdateView();
                    }
                    else
                    if (Mode == ViewMode.Raw)
                    {
                        if (SelectedImage.RGBChannelCount > 1)
                        {
                            SelectedImage.SetValue(ip.X, ip.Y, GetCoordinate(), Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                        }
                        else
                        {
                            SelectedImage.SetValue(ip.X, ip.Y, GetCoordinate(), Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                        }
                        UpdateView();
                    }

            UpdateStatus();
            tools.ToolMove(p, mouseDownButtons);
            pd = p;
        }
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (SelectedImage == null)
                return;
            App.viewer = this;
            PointD p = ToViewSpace(e.Location.X,e.Location.Y);
            if (e.Button == MouseButtons.Middle)
            {
                PointD pd = new PointD(p.X - mouseDown.X, p.Y - mouseDown.Y);
                origin = new PointD(origin.X + pd.X, origin.Y + pd.Y);
            }
            mouseUpButtons = e.Button;
            mouseDownButtons = MouseButtons.None;
            mouseUp = p;
            down = false;
            up = true;
            tools.ToolUp(p, e.Button);

        }
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (SelectedImage == null)
                return;
            App.viewer = this;
            selectedImage = SelectedImage;
            mouseDownButtons = e.Button;
            mouseUpButtons = MouseButtons.None;
            PointD p = ToViewSpace(e.Location.X, e.Location.Y);
            Point ip = SelectedImage.ToImageSpace(p);
            pd = new PointD(p.X,p.Y);
            mouseDown = pd;
            down = true;
            up = false;
            tools.BringToFront();
            int ind = 0;
            foreach (BioImage b in Images)
            {
                RectangleD r = new RectangleD(b.Volume.Location.X, b.Volume.Location.Y, b.Volume.Width, b.Volume.Height);
                if (r.IntersectsWith(p))
                {
                    SelectedIndex = ind;
                    break;
                }
                ind++;
            }
            if (!Ctrl)
            {
                foreach (ROI item in selectedAnnotations)
                {
                    if (item.selected)
                        item.selectedPoints.Clear();
                }
                selectedAnnotations.Clear();
            }

            if (Tools.currentTool.type == Tools.Tool.Type.move)
            {
                foreach (ROI an in AnnotationsRGB)
                {
                    if (an.GetSelectBound().IntersectsWith(p.X, p.Y))
                    {
                        selectedAnnotations.Add(an);
                        an.selected = true;

                        RectangleF r = new RectangleF((float)p.X, (float)p.Y, 1, 1);
                        RectangleF[] sels = an.GetSelectBoxes(scale.Width);
                        for (int i = 0; i < sels.Length; i++)
                        {
                            if (sels[i].IntersectsWith(r))
                            {
                                an.selectedPoints.Add(i);
                            }
                        }
                    }
                    else
                        if (!Ctrl)
                        an.selected = false;
                }
                UpdateOverlay();
            }

            if (e.Button == MouseButtons.Left)
            {
                Point s = new Point(SelectedImage.SizeX, SelectedImage.SizeY);
                if ((ip.X < s.X && ip.Y < s.Y) || (ip.X >= 0 && ip.Y >= 0))
                {
                    int zc = SelectedImage.Coordinate.Z;
                    int cc = SelectedImage.Coordinate.C;
                    int tc = SelectedImage.Coordinate.T;
                    if (Mode == ViewMode.RGBImage)
                    {
                        int r = SelectedImage.GetValueRGB(zc, RChannel.Index, tc, ip.X, ip.Y, 0);
                        int g = SelectedImage.GetValueRGB(zc, GChannel.Index, tc, ip.X, ip.Y, 1);
                        int b = SelectedImage.GetValueRGB(zc, BChannel.Index, tc, ip.X, ip.Y, 2);
                        mouseColor = ", " + r + "," + g + "," + b;
                    }
                    else
                    {
                        if (SelectedImage.isRGB)
                        {
                            int r = SelectedImage.GetValueRGB(zc, RChannel.Index, tc, ip.X, ip.Y, 0);
                            int g = SelectedImage.GetValueRGB(zc, GChannel.Index, tc, ip.X, ip.Y, 1);
                            int b = SelectedImage.GetValueRGB(zc, BChannel.Index, tc, ip.X, ip.Y, 2);
                            mouseColor = ", " + r + "," + g + "," + b;
                        }
                        else
                        {
                            int r = SelectedImage.GetValueRGB(zc, 0, tc, ip.X, ip.Y, 0);
                            mouseColor = ", " + r;
                        }
                    }
                }
            }
            UpdateStatus();
            tools.ToolDown(mouseDown, e.Button);
        }
        private void pictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (SelectedImage == null)
                return;
            App.viewer = this;
            selectedImage = SelectedImage;
            PointD p = ToViewSpace(e.Location.X,e.Location.Y);
            tools.ToolDown(p, e.Button);
            Origin = new PointD(-p.X, -p.Y);
        }
        private void deleteROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedImage == null)
                return;
            foreach (ROI item in AnnotationsRGB)
            {
                if (item.selected && (item.selectedPoints.Count == 0 || item.selectedPoints.Count == item.GetPointCount()))
                {
                    SelectedImage.Annotations.Remove(item);
                }
                else
                {
                    if ((item.type == ROI.Type.Polygon || item.type == ROI.Type.Freeform || item.type == ROI.Type.Polyline) && item.selectedPoints.Count > 0)
                    {
                        item.RemovePoints(item.selectedPoints.ToArray());

                    }
                    item.selectedPoints.Clear();
                }
            }
            UpdateOverlay();
        }
        private void setTextSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedImage == null)
                return;
            foreach (ROI item in AnnotationsRGB)
            {
                if (item.selected)
                {
                    TextInput input = new TextInput(item.Text);
                    if (input.ShowDialog() != DialogResult.OK)
                        return;
                    item.Text = input.textInput;
                    item.font = input.font;
                    item.strokeColor = input.color;
                    UpdateOverlay();
                }
            }
        }

        private void copyViewToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                //g.CopyFromScreen(PointToScreen(new Point(pictureBox.Left, pictureBox.Top + 25)), Point.Empty, bm.Size);
                g.DrawImage(Bitmaps[SelectedIndex], 0, 0);
                DrawOverlay(g);
            }
            Clipboard.SetImage(bmp);
        }
        List<ROI> copys = new List<ROI>();

        public void CopySelection()
        {
            copys.Clear();
            string s = "";
            foreach (ROI item in AnnotationsRGB)
            {
                if (item.selected)
                {
                    copys.Add(item);
                    s += BioImage.ROIToString(item);
                }
            }
            Clipboard.SetText(s);
        }
        public void PasteSelection()
        {
            string[] sts = Clipboard.GetText().Split(BioImage.NewLine);
            foreach (string line in sts)
            {
                if (line.Length > 8)
                {
                    ROI an = BioImage.StringToROI(line);
                    //We set the coordinates of the ROI's we are pasting
                    an.coord = GetCoordinate();
                    SelectedImage.Annotations.Add(an);
                }
            }
            UpdateOverlay();
        }
        private void copyROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelection();
        }

        private void pasteROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSelection();
        }

        private void hideControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowControls)
                ShowControls = false;
            else
                ShowControls = true;
        }

        private void showControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowControls)
                ShowControls = false;
            else
                ShowControls = true;
        }

        private void HideStatusMenuItem_Click(object sender, EventArgs e)
        {
            ShowStatus = false;
        }
        private void rGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = ViewMode.RGBImage;
        }

        private void filteredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = ViewMode.Filtered;
        }

        private void rawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = ViewMode.Raw;
        }

        public PointF ToViewSpace(Point p)
        {
            PointD d = ToViewSpace(p.X, p.Y);
            return new PointF((float)d.X, (float)d.Y);
        }
        public PointF ToViewSpace(PointF p)
        {
            PointD d = ToViewSpace(p.X, p.Y);
            return new PointF((float)d.X, (float)d.Y);
        }
        public PointD ToViewSpace(PointD p)
        {
            return ToViewSpace(p.X, p.Y);;
        }
        public PointD ToViewSpace(double x, double y)
        {
            double dx = (ToViewSizeW(x - (pictureBox.Width / 2))/scale.Width) - Origin.X;
            double dy = (ToViewSizeH(y - (pictureBox.Height / 2))/scale.Height) - Origin.Y;
            return new PointD(dx, dy);
        }
        public double ToViewSizeW(double d)
        {
            double x = (double)(d / PxWmicron);
            return x;
        }
        public double ToViewSizeH(double d)
        {
            double y = (double)(d / PxHmicron);
            return y;
        }

        public PointD ToScreenSpace(double x, double y)
        {
            double fx = ToScreenScaleW(Origin.X + x);
            double fy = ToScreenScaleH(Origin.Y + y);
            return new PointD(fx, fy);
        }
        public PointD ToScreenSpace(PointD p)
        {
            return ToScreenSpace(p.X,p.Y);
        }
        public PointF ToScreenSpace(PointF p)
        {
            PointD pd = ToScreenSpace(p.X, p.Y);
            return new PointF((float)pd.X, (float)pd.Y);
        }
        public PointF[] ToScreenSpace(PointF[] p)
        {
            PointF[] pf = new PointF[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                pf[i] = ToScreenSpace(p[i]);
            }
            return pf;
        }
        public PointF ToScreenSpace(Point3D p)
        {
            PointD pd =  ToScreenSpace(p.X, p.Y);
            return new PointF((float)pd.X, (float)pd.Y);
        }
        public float ToScreenScaleW(double x)
        {
            return (float)(x * PxWmicron);
        }
        public float ToScreenScaleH(double y)
        {
            return (float)(y * PxHmicron);
        }
        public PointF ToScreenScale(PointD p)
        {
            float x = ToScreenScaleW((float)p.X);
            float y = ToScreenScaleH((float)p.Y);
            return new PointF(x, y);
        }
        public RectangleF ToScreenRectF(double x, double y, double w, double h)
        {
            PointD pf = ToScreenSpace(x, y);
            return new RectangleF((float)pf.X, (float)pf.Y, ToScreenScaleW(w), ToScreenScaleH(h));
        }
        public RectangleF ToScreenSpace(RectangleD p)
        {
            PointD pf = ToScreenSpace(p.X, p.Y);
            return new RectangleF((float)pf.X, (float)pf.Y, (float)ToScreenScaleW(p.W), (float)ToScreenScaleH(p.H));
        }
        public RectangleF ToScreenSpace(RectangleF p)
        {
            PointD pf = ToScreenSpace(p.X, p.Y);
            return new RectangleF((float)pf.X, (float)pf.Y, (float)ToScreenScaleW(p.Width), (float)ToScreenScaleH(p.Height));
        }
        public RectangleF[] ToScreenSpace(RectangleD[] p)
        {
            RectangleF[] rs = new RectangleF[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                rs[i] = ToScreenSpace(p[i]);
            }
            return rs;
        }
        public RectangleF[] ToScreenSpace(RectangleF[] p)
        {
            RectangleF[] rs = new RectangleF[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                rs[i] = ToScreenSpace(p[i]);
            }
            return rs;
        }
        public PointF[] ToScreenSpace(PointD[] p)
        {
            PointF[] rs = new PointF[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                PointD pd = ToScreenSpace(p[i]);
                rs[i] = new PointF((float)pd.X, (float)pd.Y);
            }
            return rs;
        }

        private void ImageView_KeyDown(object sender, KeyEventArgs e)
        {
            double moveAmount = 5 * scale.Width;
            if (e.KeyCode == Keys.C && e.Control)
            {
                CopySelection();
                return;
            }
            if (e.KeyCode == Keys.V && e.Control)
            {
                PasteSelection();
                return;
            }
            if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.NumPad7)
            {
                scale.Width -= 0.1f;
                scale.Height -= 0.1f;
                UpdateOverlay();
            }
            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.NumPad9)
            {
                scale.Width += 0.1f;
                scale.Height += 0.1f;
                UpdateOverlay();
            }
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.NumPad8)
            {
                Origin = new PointD(Origin.X, Origin.Y + moveAmount);
                UpdateView();
            }
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.NumPad2)
            {
                Origin = new PointD(Origin.X, Origin.Y - moveAmount);
                UpdateView();
            }
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.NumPad4)
            {
                Origin = new PointD(Origin.X + moveAmount, Origin.Y);
                UpdateView();
            }
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.NumPad6)
            {
                Origin = new PointD(Origin.X - moveAmount, Origin.Y);
                UpdateView();
            }
        }

        public void GoToImage()
        {
            if (SelectedImage == null)
                return;
            double dx = SelectedImage.Volume.Width / 2;
            double dy = SelectedImage.Volume.Height / 2;
            Origin = new PointD(-(SelectedImage.Volume.Location.X + dx),-(SelectedImage.Volume.Location.Y + dy));
            double wx = pictureBox.Width / ToScreenScaleW(SelectedImage.Volume.Width);
            double wy = pictureBox.Height / ToScreenScaleH(SelectedImage.Volume.Height);
            scale.Width = (float)wy;
            scale.Height = (float)wy;
            UpdateView();
        }
        public void GoToImage(int i)
        {
            double dx = Images[i].Volume.Width / 2;
            double dy = Images[i].Volume.Height / 2;
            Origin = new PointD(-(Images[i].Volume.Location.X + dx), -(Images[i].Volume.Location.Y + dy));
            double wx = pictureBox.Width / ToScreenScaleW(SelectedImage.Volume.Width);
            double wy = pictureBox.Height / ToScreenScaleH(SelectedImage.Volume.Height);
            scale.Width = (float)wy;
            scale.Height = (float)wy;
            UpdateView();
        }
        private void goToImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoToImage();
        }
        private void goToToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Origin = new PointD(mouseDown.X,mouseDown.Y);
        }

        private void goToImageToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.DropDownItems.Clear();
            foreach (BioImage im in Images)
            {
                ToolStripMenuItem it = new ToolStripMenuItem();
                it.Text = System.IO.Path.GetFileName(im.ID);
                item.DropDownItems.Add(it);
            }
        }

        private void goToImageToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int i = 0;
            foreach (var item in Images)
            {
                if(item.ID.Contains(e.ClickedItem.Text))
                {
                    GoToImage(i);
                    return;
                }
                i++;
            }
        }
        public new void Dispose()
        {
            for (int i = 0; i < Bitmaps.Count; i++)
            {
                Bitmaps[i].Dispose();
            }
            foreach (BioImage item in Images)
            {
                Bio.Images.RemoveImage(item);
            }
        }

        private void goToToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Microscope.Stage.SetPosition(mouseDown.X, mouseDown.Y);
        }
    }
}
