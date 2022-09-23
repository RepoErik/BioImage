﻿using AForge;
using AForge.Imaging.Filters;
using BitMiracle.LibTiff.Classic;
using loci.common.services;
using loci.formats;
using loci.formats.services;
using ome.xml.model.primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using loci.formats.meta;

namespace Bio
{
    public static class Images
    {
        public static List<BioImage> images = new List<BioImage>();
        public static BioImage GetImage(string ids)
        {
            for (int i = 0; i < images.Count; i++)
            {
                if (images[i].ID.Contains(ids))
                    return images[i];
            }
            return null;
        }
        public static void AddImage(BioImage im)
        {
            im.ID = GetImageName(im.ID);
            images.Add(im);
            //App.tabsView.AddTab(im);
            //App.Image = im;
            //NodeView.viewer.AddTab(im);
        }
        public static int GetImageCountByName(string s)
        {
            int i = 0;
            string name = Path.GetFileNameWithoutExtension(s);
            for (int im = 0; im < images.Count; im++)
            {
                if (images[im].ID.Contains(name))
                    i++;
            }
            return i;
        }
        public static string GetImageName(string s)
        {
            //Here we create a unique ID for an image.
            int i = Images.GetImageCountByName(s);
            if (i == 0)
                return s;
            string test = Path.GetFileName(s);
            string name = Path.GetFileNameWithoutExtension(s);
            string ext = Path.GetExtension(s);
            int sti = name.LastIndexOf("-");
            if (sti == -1)
            {
                return name + "-" + i + ext;

            }
            else
            {
                string stb = name.Substring(0, sti);
                string sta = name.Substring(sti + 1, name.Length - sti - 1);
                int ind;
                if (int.TryParse(sta, out ind))
                {
                    return stb + "-" + (ind + 1).ToString() + ext;
                }
                else
                    return name + "-" + i + ext;
            }
            //
        }
        public static void RemoveImage(BioImage im)
        {
            RemoveImage(im.ID);
        }
        public static void RemoveImage(string id)
        {
            BioImage im = GetImage(id);
            if (im == null)
                return;
            images.Remove(im);
            im.Dispose();
            im = null;
            GC.Collect();
            Recorder.AddLine("Bio.Table.RemoveImage(" + '"' + id + '"' + ");");
        }
    }
    public struct ZCT
    {
        public int Z, C, T;
        public ZCT(int z, int c, int t)
        {
            Z = z;
            C = c;
            T = t;
        }
        public static bool operator ==(ZCT c1, ZCT c2)
        {
            if (c1.Z == c2.Z && c1.C == c2.C && c1.T == c2.T)
                return true;
            else
                return false;
        }
        public static bool operator !=(ZCT c1, ZCT c2)
        {
            if (c1.Z != c2.Z || c1.C != c2.C || c1.T != c2.T)
                return false;
            else
                return true;
        }
        public override string ToString()
        {
            return Z + "," + C + "," + T;
        }
    }
    public struct ZCTXY
    {
        public int Z, C, T, X, Y;
        public ZCTXY(int z, int c, int t, int x, int y)
        {
            Z = z;
            C = c;
            T = t;
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return Z + "," + C + "," + T + "," + X + "," + Y;
        }

        public static bool operator ==(ZCTXY c1, ZCTXY c2)
        {
            if (c1.Z == c2.Z && c1.C == c2.C && c1.T == c2.T && c1.X == c2.X && c1.Y == c2.Y)
                return true;
            else
                return false;
        }
        public static bool operator !=(ZCTXY c1, ZCTXY c2)
        {
            if (c1.Z != c2.Z || c1.C != c2.C || c1.T != c2.T || c1.X != c2.X || c1.Y != c2.Y)
                return false;
            else
                return true;
        }
    }
    public enum RGB
    {
        R,
        G,
        B,
        Gray
    }
    public class ColorS
    {
        public ushort R = 0;
        public ushort G = 0;
        public ushort B = 0;
        public ColorS()
        {

        }
        public ColorS(ushort s)
        {
            R = s;
            G = s;
            B = s;
        }
        public ColorS(ushort r, ushort g, ushort b)
        {
            R = r;
            G = g;
            B = b;
        }
        public static ColorS FromColor(System.Drawing.Color col)
        {
            float r = (((float)col.R) / 255) * ushort.MaxValue;
            float g = (((float)col.G) / 255) * ushort.MaxValue;
            float b = (((float)col.B) / 255) * ushort.MaxValue;
            ColorS color = new ColorS();
            color.R = (ushort)r;
            color.G = (ushort)g;
            color.B = (ushort)b;
            return color;
        }
        public static System.Drawing.Color ToColor(ColorS col)
        {
            float r = ((float)(col.R) / 65535) * 255;
            float g = ((float)(col.G) / 65535) * 255;
            float b = ((float)(col.B) / 65535) * 255;
            System.Drawing.Color c = System.Drawing.Color.FromArgb((byte)r, (byte)g, (byte)b);
            return c;
        }
        public override string ToString()
        {
            return R + "," + G + "," + B;
        }
    }
    public struct RectangleD
    {
        private double x;
        private double y;
        private double w;
        private double h;
        public double X { get { return x; } set { x = value; } }
        public double Y { get { return y; } set { y = value; } }
        public double W { get { return w; } set { w = value; } }
        public double H { get { return h; } set { h = value; } }

        public RectangleD(double X, double Y, double W, double H)
        {
            x = X;
            y = Y;
            w = W;
            h = H;
        }
        public System.Drawing.Rectangle ToRectangleInt()
        {
            return new System.Drawing.Rectangle((int)X, (int)Y, (int)W, (int)H);
        }
        public bool IntersectsWith(PointD p)
        {
            if (X <= p.X && (X + W) >= p.X && Y <= p.Y && (Y + H) >= p.Y)
                return true;
            else
                return false;
        }
        public bool IntersectsWith(double x, double y)
        {
            if (X <= x && (X + W) >= x && Y <= y && (Y + H) >= y)
                return true;
            else
                return false;
        }
        public RectangleF ToRectangleF()
        {
            return new RectangleF((float)X, (float)Y, (float)W, (float)H);
        }
        public override string ToString()
        {
            return X.ToString() + ", " + Y.ToString() + ", " + W.ToString() + ", " + H.ToString();
        }

    }
    public class ROI
    {
        public enum Type
        {
            Rectangle,
            Point,
            Line,
            Polygon,
            Polyline,
            Freeform,
            Ellipse,
            Label
        }
        public PointD Point
        {
            get
            {
                if (Points.Count == 0)
                    return new PointD(0, 0);
                if (type == Type.Line || type == Type.Ellipse || type == Type.Label || type == Type.Freeform)
                    return new PointD(BoundingBox.X, BoundingBox.Y);
                return Points[0];
            }
            set
            {
                if (Points.Count == 0)
                {
                    AddPoint(value);
                }
                else
                    UpdatePoint(value, 0);
                UpdateSelectBoxs();
                UpdateBoundingBox();
            }
        }
        public RectangleD Rect
        {
            get
            {
                if (Points.Count == 0)
                    return new RectangleD(0, 0, 0, 0);
                if (type == Type.Line || type == Type.Polyline || type == Type.Polygon || type == Type.Freeform || type == Type.Label)
                    return BoundingBox;
                if (type == Type.Rectangle || type == Type.Ellipse)
                    return new RectangleD(Points[0].X, Points[0].Y, Points[1].X - Points[0].X, Points[2].Y - Points[0].Y);
                else
                    return new RectangleD(Points[0].X, Points[0].Y, 1, 1);
            }
            set
            {
                if (type == Type.Line || type == Type.Polyline || type == Type.Polygon || type == Type.Freeform)
                {
                    BoundingBox = value;
                }
                else
                if (Points.Count < 4 && (type == Type.Rectangle || type == Type.Ellipse))
                {
                    AddPoint(new PointD(value.X, value.Y));
                    AddPoint(new PointD(value.X + value.W, value.Y));
                    AddPoint(new PointD(value.X, value.Y + value.H));
                    AddPoint(new PointD(value.X + value.W, value.Y + value.H));
                }
                else
                if (type == Type.Rectangle || type == Type.Ellipse)
                {
                    Points[0] = new PointD(value.X, value.Y);
                    Points[1] = new PointD(value.X + value.W, value.Y);
                    Points[2] = new PointD(value.X, value.Y + value.H);
                    Points[3] = new PointD(value.X + value.W, value.Y + value.H);
                }
                UpdateSelectBoxs();
                UpdateBoundingBox();
            }
        }
        public double X
        {
            get
            {
                return Point.X;
            }
            set
            {
                Rect = new RectangleD(value, Y, W, H);
                Recorder.AddLine("App.AddROI(" + BioImage.ROIToString(this) + ");");
            }
        }
        public double Y
        {
            get
            {
                return Point.Y;
            }
            set
            {
                Rect = new RectangleD(X, value, W, H);
                Recorder.AddLine("App.AddROI(" + BioImage.ROIToString(this) + ");");
            }
        }
        public double W
        {
            get
            {
                if (type == Type.Point)
                    return strokeWidth;
                else
                    return BoundingBox.W;
            }
            set
            {
                Rect = new RectangleD(X, Y, value, H);
                Recorder.AddLine("App.AddROI(" + BioImage.ROIToString(this) + ");");
            }
        }
        public double H
        {
            get
            {
                if (type == Type.Point)
                    return strokeWidth;
                else
                    return BoundingBox.H;
            }
            set
            {
                Rect = new RectangleD(X, Y, W, value);
                Recorder.AddLine("App.AddROI(" + BioImage.ROIToString(this) + ");");
            }
        }

        public Type type;
        public float selectBoxSize = 1.5f;
        private List<PointD> Points = new List<PointD>();
        public List<PointD> PointsD
        {
            get
            {
                return Points;
            }
        }
        private List<RectangleF> selectBoxs = new List<RectangleF>();
        public List<int> selectedPoints = new List<int>();
        public RectangleD BoundingBox;
        public Font font = System.Drawing.SystemFonts.DefaultFont;
        public ZCT coord;
        public System.Drawing.Color strokeColor;
        public System.Drawing.Color fillColor;
        public bool isFilled = false;
        public string id = "";
        public string roiID = "";
        public string roiName = "";
        private string text = "";

        public double strokeWidth = 1;
        public int shapeIndex = 0;
        public bool closed = false;
        public bool selected = false;

        public ROI Copy()
        {
            ROI copy = new ROI();
            copy.id = id;
            copy.roiID = roiID;
            copy.roiName = roiName;
            copy.text = text;
            copy.strokeWidth = strokeWidth;
            copy.strokeColor = strokeColor;
            copy.fillColor = fillColor;
            copy.Points = Points;
            copy.selected = selected;
            copy.shapeIndex = shapeIndex;
            copy.closed = closed;
            copy.font = font;
            copy.selectBoxs = selectBoxs;
            copy.BoundingBox = BoundingBox;
            copy.isFilled = isFilled;
            copy.coord = coord;
            copy.selectedPoints = selectedPoints;

            return copy;
        }
        public ROI Copy(ZCT cord)
        {
            ROI copy = new ROI();
            copy.type = type;
            copy.selectBoxSize = selectBoxSize;
            copy.id = id;
            copy.roiID = roiID;
            copy.roiName = roiName;
            copy.text = text;
            copy.strokeWidth = strokeWidth;
            copy.strokeColor = strokeColor;
            copy.fillColor = fillColor;
            copy.Points.AddRange(Points);
            copy.selected = selected;
            copy.shapeIndex = shapeIndex;
            copy.closed = closed;
            copy.font = font;
            copy.selectBoxs.AddRange(selectBoxs);
            copy.BoundingBox = BoundingBox;
            copy.isFilled = isFilled;
            copy.coord = cord;
            copy.selectedPoints = selectedPoints;
            return copy;
        }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                if (type == Type.Label)
                {
                    UpdateBoundingBox();
                    UpdateSelectBoxs();
                }
            }
        }
        public Size TextSize
        {
            get
            {
                return TextRenderer.MeasureText(text, font);
            }
        }
        public RectangleD GetSelectBound()
        {
            return new RectangleD(BoundingBox.X - selectBoxSize, BoundingBox.Y - selectBoxSize, BoundingBox.W + selectBoxSize, BoundingBox.H + selectBoxSize);
        }
        public ROI()
        {
            coord = new ZCT(0, 0, 0);
            strokeColor = System.Drawing.Color.Yellow;
            font = SystemFonts.DefaultFont;
            BoundingBox = new RectangleD(0, 0, 1, 1);
        }

        public RectangleF[] GetSelectBoxes(float size)
        {
            float f = (selectBoxSize) / 2;
            selectBoxs.Clear();
            for (int i = 0; i < Points.Count; i++)
            {
                selectBoxs.Add(new RectangleF((float)Points[i].X - f, (float)Points[i].Y - f, selectBoxSize, selectBoxSize));
            }
            return selectBoxs.ToArray();
        }
        public static ROI CreatePoint(ZCT coord, double x, double y)
        {
            ROI an = new ROI();
            an.coord = coord;
            an.AddPoint(new PointD(x, y));
            an.type = Type.Point;
            Recorder.AddLine("ROI.CreatePoint(new ZCT(" + coord.Z + "," + coord.C + "," + coord.T + "), " + x + "," + y + ");");
            return an;
        }
        public static ROI CreatePoint(int z, int c, int t, double x, double y)
        {
            return CreatePoint(new ZCT(z, c, t), x, y);
        }
        public static ROI CreateLine(ZCT coord, PointD x1, PointD x2)
        {
            ROI an = new ROI();
            an.coord = coord;
            an.type = Type.Line;
            an.AddPoint(x1);
            an.AddPoint(x2);
            Recorder.AddLine("ROI.CreateLine(new ZCT(" + coord.Z + "," + coord.C + "," + coord.T + "), new PointD(" + x1.X + "," + x1.Y + "), new PointD(" + x2.X + "," + x2.Y + "));");
            return an;
        }
        public static ROI CreateRectangle(ZCT coord, double x, double y, double w, double h)
        {
            ROI an = new ROI();
            an.coord = coord;
            an.type = Type.Rectangle;
            an.Rect = new RectangleD(x, y, w, h);
            Recorder.AddLine("ROI.CreateRectangle(new ZCT(" + coord.Z + "," + coord.C + "," + coord.T + "), new RectangleD(" + x + "," + y + "," + w + "," + h + ");");
            return an;
        }
        public static ROI CreateEllipse(ZCT coord, double x, double y, double w, double h)
        {
            ROI an = new ROI();
            an.coord = coord;
            an.type = Type.Ellipse;
            an.Rect = new RectangleD(x, y, w, h);
            Recorder.AddLine("ROI.CreateEllipse(new ZCT(" + coord.Z + "," + coord.C + "," + coord.T + "), new RectangleD(" + x + "," + y + "," + w + "," + h + ");");
            return an;
        }
        public static ROI CreatePolygon(ZCT coord, PointD[] pts)
        {
            ROI an = new ROI();
            an.coord = coord;
            an.type = Type.Polygon;
            an.AddPoints(pts);
            an.closed = true;
            return an;
        }
        public static ROI CreateFreeform(ZCT coord, PointD[] pts)
        {
            ROI an = new ROI();
            an.coord = coord;
            an.type = Type.Freeform;
            an.AddPoints(pts);
            an.closed = true;
            return an;
        }

        public void UpdatePoint(PointD p, int i)
        {
            if (i < Points.Count)
            {
                Points[i] = p;
            }
            UpdateBoundingBox();
            UpdateSelectBoxs();
        }
        public PointD GetPoint(int i)
        {
            return Points[i];
        }
        public PointD[] GetPoints()
        {
            return Points.ToArray();
        }
        public PointF[] GetPointsF()
        {
            PointF[] pfs = new PointF[Points.Count];
            for (int i = 0; i < Points.Count; i++)
            {
                pfs[i].X = (float)Points[i].X;
                pfs[i].Y = (float)Points[i].Y;
            }
            return pfs;
        }
        public void AddPoint(PointD p)
        {
            Points.Add(p);
            UpdateSelectBoxs();
            UpdateBoundingBox();
        }
        public void AddPoints(PointD[] p)
        {
            Points.AddRange(p);
            UpdateSelectBoxs();
            UpdateBoundingBox();
        }
        public void RemovePoints(int[] indexs)
        {
            List<PointD> inds = new List<PointD>();
            for (int i = 0; i < Points.Count; i++)
            {
                bool found = false;
                for (int ind = 0; ind < indexs.Length; ind++)
                {
                    if (indexs[ind] == i)
                        found = true;
                }
                if (!found)
                    inds.Add(Points[i]);
            }
            Points = inds;
            UpdateBoundingBox();
            UpdateSelectBoxs();
        }
        public int GetPointCount()
        {
            return Points.Count;
        }
        public PointD[] stringToPoints(string s)
        {
            List<PointD> pts = new List<PointD>();
            string[] ints = s.Split(' ');
            for (int i = 0; i < ints.Length; i++)
            {
                string[] sints = ints[i].Split(',');
                double x = double.Parse(sints[0]);
                double y = double.Parse(sints[1]);
                pts.Add(new PointD(x, y));
            }
            return pts.ToArray();
        }
        public string PointsToString()
        {
            string pts = "";
            for (int j = 0; j < Points.Count; j++)
            {
                if (j == Points.Count - 1)
                    pts += Points[j].X.ToString() + "," + Points[j].Y.ToString();
                else
                    pts += Points[j].X.ToString() + "," + Points[j].Y.ToString() + " ";
            }
            return pts;
        }
        public string PointsToString(PointD[] Points)
        {
            string pts = "";
            for (int j = 0; j < Points.Length; j++)
            {
                if (j == Points.Length - 1)
                    pts += Points[j].X.ToString() + "," + Points[j].Y.ToString();
                else
                    pts += Points[j].X.ToString() + "," + Points[j].Y.ToString() + " ";
            }
            return pts;
        }
        public void UpdateSelectBoxs()
        {
            float f = selectBoxSize / 2;
            selectBoxs.Clear();
            for (int i = 0; i < Points.Count; i++)
            {
                selectBoxs.Add(new RectangleF((float)Points[i].X - f, (float)Points[i].Y - f, selectBoxSize, selectBoxSize));
            }
        }
        public void UpdateBoundingBox()
        {
            if (type == Type.Label)
            {
                if (text != "")
                {
                    Size s = TextSize;
                    BoundingBox = new RectangleD(Points[0].X, Points[0].Y, s.Width, s.Height);
                }
            }
            else
            {
                PointD min = new PointD(double.MaxValue, double.MaxValue);
                PointD max = new PointD(double.MinValue, double.MinValue);
                foreach (PointD p in Points)
                {
                    if (min.X > p.X)
                        min.X = p.X;
                    if (min.Y > p.Y)
                        min.Y = p.Y;

                    if (max.X < p.X)
                        max.X = p.X;
                    if (max.Y < p.Y)
                        max.Y = p.Y;
                }
                RectangleD r = new RectangleD();
                r.X = min.X;
                r.Y = min.Y;
                r.W = max.X - min.X;
                r.H = max.Y - min.Y;
                if (r.W == 0)
                    r.W = 1;
                if (r.H == 0)
                    r.H = 1;
                BoundingBox = r;
            }
        }
        public override string ToString()
        {
            return type.ToString() + ", " + Text + " (" + W + ", " + H + "); " + " (" + Point.X + ", " + Point.Y + ") " + coord.ToString();
        }
    }
    public class Channel : IDisposable
    {
        public IntRange range;
        public ChannelInfo info = new ChannelInfo();
        public Statistics statistics;
        private ome.xml.model.enums.ContrastMethod contrastMethod;
        private ome.xml.model.enums.IlluminationType illuminationType;
        [Serializable]
        public class ChannelInfo
        {
            internal string name = "";
            internal string ID = "";
            internal int index = 0;
            internal string fluor = "";
            internal int samplesPerPixel;
            internal System.Drawing.Color? color;
            internal int emission = -1;
            internal int excitation = -1;
            internal int exposure = -1;
            internal string lightSource = "";
            internal double lightSourceIntensity = -1;
            internal int lightSourceWavelength = -1;
            internal string contrastMethod = "";
            internal string illuminationType = "";
            internal int bitsPerPixel;
            public string Name
            {
                get { return name; }
                set { name = value; }
            }
            public int Index
            {
                get
                {
                    return index;
                }
                set
                {
                    index = value;
                }

            }
            public string Fluor
            {
                get { return fluor; }
                set { fluor = value; }
            }
            public int SamplesPerPixel
            {
                get { return samplesPerPixel; }
                set { samplesPerPixel = value; }
            }
            public System.Drawing.Color? Color
            {
                get { return color; }
                set { color = value; }
            }
            public int Emission
            {
                get { return emission; }
                set { emission = value; }
            }
            public int Excitation
            {
                get { return excitation; }
                set { excitation = value; }
            }
            public int Exposure
            {
                get { return exposure; }
                set { exposure = value; }
            }
            public string LightSource
            {
                get { return lightSource; }
                set { lightSource = value; }
            }
            public double LightSourceIntensity
            {
                get { return lightSourceIntensity; }
                set { lightSourceIntensity = value; }
            }
            public int LightSourceWavelength
            {
                get { return lightSourceWavelength; }
                set { lightSourceWavelength = value; }
            }
            public string ContrastMethod
            {
                get { return contrastMethod.ToString(); }
                set
                {
                    contrastMethod = value.ToString();
                }
            }
            public string IlluminationType
            {
                get { return illuminationType.ToString(); }
                set
                {
                    illuminationType = value.ToString();
                }
            }
        }

        public string Name
        {
            get { return info.name; }
            set { info.name = value; }
        }
        public int Index
        {
            get
            {
                return info.index;
            }
            set
            {
                info.index = value;
            }

        }
        public int Max
        {
            get
            {
                return range.Max;
            }
            set
            {
                range.Max = value;
                Recorder.AddLine("App.Channels[" + Index + "].Max = " + value + ";");
            }
        }
        public int Min
        {
            get
            {
                return range.Min;
            }
            set
            {
                range.Min = value;
                Recorder.AddLine("App.Channels[" + Index + "].Min = " + value + ";");
            }
        }
        public string Fluor
        {
            get { return info.fluor; }
            set { info.fluor = value; }
        }
        public int SamplesPerPixel
        {
            get { return info.samplesPerPixel; }
            set { info.samplesPerPixel = value; }
        }
        public System.Drawing.Color? Color
        {
            get { return info.color; }
            set { info.color = value; }
        }
        public int Emission
        {
            get { return info.emission; }
            set { info.emission = value; }
        }
        public int Excitation
        {
            get { return info.excitation; }
            set { info.excitation = value; }
        }
        public int Exposure
        {
            get { return info.exposure; }
            set { info.exposure = value; }
        }
        public string LightSource
        {
            get { return info.lightSource; }
            set { info.lightSource = value; }
        }
        public double LightSourceIntensity
        {
            get { return info.lightSourceIntensity; }
            set { info.lightSourceIntensity = value; }
        }
        public int LightSourceWavelength
        {
            get { return info.lightSourceWavelength; }
            set { info.lightSourceWavelength = value; }
        }
        public ome.xml.model.enums.ContrastMethod ContrastMethod
        {
            get { return contrastMethod; }
            set
            {
                contrastMethod = value;
            }
        }
        public ome.xml.model.enums.IlluminationType IlluminationType
        {
            get { return illuminationType; }
            set { illuminationType = value; }
        }
        public int BitsPerPixel
        {
            get { return info.bitsPerPixel; }
            set { info.bitsPerPixel = value; }
        }
        public Channel(int ind, int bitsPerPixel)
        {
            if (bitsPerPixel == 16)
                Max = 65535;
            if (bitsPerPixel == 14)
                Max = 16383;
            if (bitsPerPixel == 12)
                Max = 4096;
            if (bitsPerPixel == 10)
                Max = 1024;
            if (bitsPerPixel == 8)
                Max = byte.MaxValue;
            range = new IntRange(0, Max);
            info = new ChannelInfo();
            Min = 0;
            info.index = ind;
        }
        public Channel Copy()
        {
            Channel c = new Channel(info.index, info.bitsPerPixel);
            c.Name = Name;
            c.info.ID = info.ID;
            c.range = range;
            c.info.color = info.color;
            c.Fluor = Fluor;
            c.SamplesPerPixel = SamplesPerPixel;
            c.Emission = Emission;
            c.Excitation = Excitation;
            c.Exposure = Exposure;
            c.LightSource = LightSource;
            c.LightSourceIntensity = LightSourceIntensity;
            c.LightSourceWavelength = LightSourceWavelength;
            c.contrastMethod = contrastMethod;
            c.illuminationType = illuminationType;
            return c;
        }
        public override string ToString()
        {
            if (Name == "")
                return info.index.ToString();
            else
                return info.index + ", " + Name;
        }
        public void Dispose()
        {
            if (statistics != null)
                statistics.Dispose();
        }
    }
    public class BufferInfo : IDisposable
    {
        public ushort GetValueRGB(int ix, int iy, int index)
        {
            int i = -1;
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = ix;
            int y = iy;
            if (BitsPerPixel > 8)
            {
                int index2 = (y * stridex + x) * 2 * index;
                i = BitConverter.ToUInt16(bytes, index2);
                return (ushort)i;
            }
            else
            {
                int stride = SizeX;
                int indexb = (y * stridex + x) * index;
                i = bytes[indexb];
                return (ushort)i;
            }
        }
        public ushort GetValue(int ix, int iy)
        {
            int i = 0;
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = ix;
            int y = iy;
            if (ix < 0)
                x = 0;
            if (iy < 0)
                y = 0;
            if (ix >= SizeX)
                x = SizeX - 1;
            if (iy >= SizeY)
                y = SizeY - 1;

            if (BitsPerPixel > 8)
            {
                int index2 = (y * stridex + x) * 2 * RGBChannelsCount;
                i = BitConverter.ToUInt16(bytes, index2);
                return (ushort)i;
            }
            else
            {
                int index = (y * stridex + x) * RGBChannelsCount;
                i = bytes[index];
                return (ushort)i;
            }

        }
        public void SetValue(int ix, int iy, ushort value)
        {
            byte[] bts = bytes;
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = ix;
            int y = iy;
            if (BitsPerPixel > 8)
            {
                int index2 = ((y * stridex + x) * 2 * RGBChannelsCount);
                byte upper = (byte)(value >> 8);
                byte lower = (byte)(value & 0xff);
                bytes[index2] = lower;
                bytes[index2 + 1] = upper;
            }
            else
            {
                int index = (y * stridex + x) * RGBChannelsCount;
                bytes[index] = (byte)value;
            }
        }
        public void SetValueRGB(int ix, int iy, int RGBChannel, ushort value)
        {
            int stride = SizeX;
            int x = ix;
            int y = iy;
            if (BitsPerPixel > 8)
            {
                int index2 = ((y * stride + x) * 2 * RGBChannelsCount);
                byte upper = (byte)(value >> 8);
                byte lower = (byte)(value & 0xff);
                bytes[index2] = lower;
                bytes[index2 + 1] = upper;
            }
            else
            {
                int index = ((y * stride + x) * RGBChannelsCount) + (RGBChannel);
                bytes[index] = (byte)value;
            }
        }
        public long GetIndex(int x, int y)
        {
            if (BitsPerPixel > 8)
            {
                return (y * Stride + x) * 2 * RGBChannelsCount;
            }
            else
            {
                return (y * Stride + x) * RGBChannelsCount;
            }
        }
        public static string CreateID(string filepath, int index)
        {
            const char sep = '/';
            filepath = filepath.Replace("\\", "/");
            string s = filepath + sep + 'i' + sep + index;
            return s;
        }
        public string ID;
        public string File
        {
            get { return file; }
            set { file = value; }
        }
        public int HashID
        {
            get
            {
                return ID.GetHashCode();
            }
        }
        public int SizeX, SizeY;
        public int Stride
        {
            get
            {
                int s = 0;
                if (pixelFormat == PixelFormat.Format8bppIndexed)
                    s = SizeX;
                else
                if (pixelFormat == PixelFormat.Format16bppGrayScale)
                    s = SizeX * 2;
                else
                if (pixelFormat == PixelFormat.Format24bppRgb)
                    s = SizeX * 3;
                else
                    if (pixelFormat == PixelFormat.Format32bppRgb || pixelFormat == PixelFormat.Format32bppArgb)
                    s = SizeX * 4;
                else
                    s = SizeX * 3 * 2;
                return s;
            }
        }
        public int PaddedStride
        {
            get
            {
                return GetStridePadded(Stride);
            }
        }
        public byte[] PaddedBuffer
        {
            get
            {
                return GetPaddedBuffer(Bytes, SizeX, SizeY, Stride, PixelFormat);
            }
        }
        public bool LittleEndian
        {
            get
            {
                return BitConverter.IsLittleEndian;
            }
        }
        public int Length
        {
            get
            {
                return bytes.Length;
            }
        }
        public int RGBChannelsCount
        {
            get
            {
                if (PixelFormat == PixelFormat.Format24bppRgb || PixelFormat == PixelFormat.Format48bppRgb)
                    return 3;
                else
                if (PixelFormat == PixelFormat.Format8bppIndexed || PixelFormat == PixelFormat.Format16bppGrayScale)
                    return 1;
                else
                    return 4;
            }
        }
        public int BitsPerPixel
        {
            get
            {
                if (PixelFormat == PixelFormat.Format16bppGrayScale || PixelFormat == PixelFormat.Format48bppRgb)
                {
                    return 16;
                }
                else
                    return 8;
            }
        }
        public ZCT Coordinate;
        public PixelFormat PixelFormat
        {
            get
            {
                return pixelFormat;
            }
            set
            {
                pixelFormat = value;
            }
        }
        public byte[] Bytes
        {
            get { return bytes; }
            set
            {
                bytes = value;
            }
        }
        public byte[] PaddedBytes
        {
            get
            {
                return GetPaddedBuffer(bytes, SizeX, SizeY, Stride, PixelFormat);
            }
        }
        public Image Image
        {
            get
            {
                return GetBitmap(SizeX, SizeY, Stride, PixelFormat, Bytes);
            }
            set
            {
                Bitmap bitmap;
                bitmap = (Bitmap)value;
                if (!LittleEndian)
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                PixelFormat = value.PixelFormat;
                SizeX = value.Width;
                SizeY = value.Height;
                bytes = GetBuffer(bitmap, Stride);
                if (LittleEndian)
                    Array.Reverse(bytes);
            }
        }
        private PixelFormat pixelFormat;
        public Statistics Statistics
        {
            get { return statistics; }
            set { statistics = value; }
        }
        private Statistics statistics;
        private byte[] bytes;
        private string file;

        public void SetImage(Bitmap bitmap, bool switchRGB)
        {
            if (switchRGB)
                bitmap = BufferInfo.SwitchRedBlue(bitmap);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            PixelFormat = bitmap.PixelFormat;
            SizeX = bitmap.Width;
            SizeY = bitmap.Height;
            bytes = GetBuffer((Bitmap)bitmap, Stride);
        }
        private static int GetStridePadded(int stride)
        {
            if (stride % 4 == 0)
                return stride;
            int newstride = stride + 2;
            if (newstride % 4 != 0)
            {
                newstride = stride + 1;
                if (newstride % 4 != 0)
                    newstride = stride + 3;
            }
            if (newstride % 4 != 0)
            {
                throw new InvalidOperationException("Stride padding failed");
            }
            return newstride;
        }
        private static byte[] GetPaddedBuffer(byte[] bts, int w, int h, int stride, PixelFormat px)
        {
            int newstride = GetStridePadded(stride);
            if (newstride == stride)
                return bts;
            byte[] newbts = new byte[newstride * h];
            if (px == PixelFormat.Format24bppRgb || px == PixelFormat.Format32bppArgb || px == PixelFormat.Format8bppIndexed)
            {
                /*
                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w; ++x)
                    {
                        int index = (y * stride) + x;
                        int index2 = (y * newstride) + x;
                        newbts[index2] = bts[index];
                    }
                }
                */
                return bts;
            }
            else
            {
                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w * 2; ++x)
                    {
                        int index = (y * stride) + x;
                        int index2 = (y * newstride) + x;
                        newbts[index2] = bts[index];
                    }
                }
            }
            return newbts;
        }
        public static BufferInfo[] RGB48To16(string file, int w, int h, int stride, byte[] bts, ZCT coord, int index)
        {
            BufferInfo[] bfs = new BufferInfo[3];
            Bitmap bmpr = new Bitmap(w, h, PixelFormat.Format16bppGrayScale);
            Bitmap bmpg = new Bitmap(w, h, PixelFormat.Format16bppGrayScale);
            Bitmap bmpb = new Bitmap(w, h, PixelFormat.Format16bppGrayScale);
            //creating the bitmapdata and lock bits
            System.Drawing.Rectangle rec = new System.Drawing.Rectangle(0, 0, w, h);
            BitmapData bmdr = bmpr.LockBits(rec, ImageLockMode.ReadWrite, bmpr.PixelFormat);
            BitmapData bmdg = bmpg.LockBits(rec, ImageLockMode.ReadWrite, bmpg.PixelFormat);
            BitmapData bmdb = bmpb.LockBits(rec, ImageLockMode.ReadWrite, bmpb.PixelFormat);
            unsafe
            {
                //iterating through all the pixels in y direction
                for (int y = 0; y < h; y++)
                {
                    //getting the pixels of current row
                    byte* rowr = (byte*)bmdr.Scan0 + (y * bmdr.Stride);
                    byte* rowg = (byte*)bmdg.Scan0 + (y * bmdg.Stride);
                    byte* rowb = (byte*)bmdb.Scan0 + (y * bmdb.Stride);
                    int rowRGB = y * stride;
                    //iterating through all the pixels in x direction
                    for (int x = 0; x < w; x++)
                    {
                        int indexRGB = x * 6;
                        int index16 = x * 2;
                        //R
                        rowr[index16 + 1] = bts[rowRGB + indexRGB];
                        rowr[index16] = bts[rowRGB + indexRGB + 1];
                        //G
                        rowg[index16 + 1] = bts[rowRGB + indexRGB + 2];
                        rowg[index16] = bts[rowRGB + indexRGB + 3];
                        //B
                        rowb[index16 + 1] = bts[rowRGB + indexRGB + 4];
                        rowb[index16] = bts[rowRGB + indexRGB + 5];

                    }
                }
            }
            bmpr.UnlockBits(bmdr);
            bmpg.UnlockBits(bmdg);
            bmpb.UnlockBits(bmdb);
            bfs[2] = new BufferInfo(file, bmpr, new ZCT(coord.Z, 0, coord.T), index);
            bfs[2].RotateFlip(RotateFlipType.Rotate180FlipNone);
            bfs[1] = new BufferInfo(file, bmpg, new ZCT(coord.Z, 0, coord.T), index + 1);
            bfs[1].RotateFlip(RotateFlipType.Rotate180FlipNone);
            bfs[0] = new BufferInfo(file, bmpb, new ZCT(coord.Z, 0, coord.T), index + 2);
            bfs[0].RotateFlip(RotateFlipType.Rotate180FlipNone);
            return bfs;
        }
        public static Bitmap[] RGB24To8(Bitmap info)
        {
            Bitmap[] bfs = new Bitmap[3];
            ExtractChannel cr = new ExtractChannel((short)0);
            ExtractChannel cg = new ExtractChannel((short)1);
            ExtractChannel cb = new ExtractChannel((short)2);
            bfs[0] = cr.Apply(info);
            bfs[1] = cg.Apply(info);
            bfs[2] = cb.Apply(info);
            bfs[0].RotateFlip(RotateFlipType.Rotate180FlipNone);
            bfs[1].RotateFlip(RotateFlipType.Rotate180FlipNone);
            bfs[2].RotateFlip(RotateFlipType.Rotate180FlipNone);
            return bfs;
        }
        public static unsafe Bitmap GetBitmap(int w, int h, int stride, PixelFormat px, byte[] bts)
        {
            fixed (byte* numPtr1 = bts)
            {
                if (stride % 4 == 0)
                {
                    return new Bitmap(w, h, stride, px, new IntPtr((void*)numPtr1));
                }
                int newstride = GetStridePadded(stride);
                byte[] newbts = GetPaddedBuffer(bts, w, h, stride, px);
                fixed (byte* numPtr2 = newbts)
                {
                    return new Bitmap(w, h, newstride, px, new IntPtr((void*)numPtr2));
                }
            }
        }
        public static unsafe Bitmap GetFiltered(int w, int h, int stride, PixelFormat px, byte[] bts, IntRange rr, IntRange rg, IntRange rb)
        {
            if (px == PixelFormat.Format24bppRgb)
            {
                //opening a 8 bit per pixel jpg image
                Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                //creating the bitmapdata and lock bits
                System.Drawing.Rectangle rec = new System.Drawing.Rectangle(0, 0, w, h);
                BitmapData bmd = bmp.LockBits(rec, ImageLockMode.ReadWrite, bmp.PixelFormat);
                unsafe
                {
                    //iterating through all the pixels in y direction
                    for (int y = 0; y < h; y++)
                    {
                        //getting the pixels of current row
                        byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                        int rowRGB = y * stride;
                        //iterating through all the pixels in x direction
                        for (int x = 0; x < w; x++)
                        {
                            int indexRGB = x * 3;
                            int indexRGBA = x * 4;
                            row[indexRGBA + 3] = byte.MaxValue;//byte A
                            row[indexRGBA + 2] = bts[rowRGB + indexRGB + 2];//byte R
                            row[indexRGBA + 1] = bts[rowRGB + indexRGB + 1];//byte G
                            row[indexRGBA] = bts[rowRGB + indexRGB];//byte Bfloat ri = ((float)BitConverter.ToUInt16(bts, rowRGB + indexRGB) - rr.Min);
                            float ri = ((float)bts[rowRGB + indexRGB] - rr.Min);
                            if (ri < 0)
                                ri = 0;
                            ri = ri / rr.Max;
                            float gi = ((float)bts[rowRGB + indexRGB + 1] - rg.Min);
                            if (gi < 0)
                                gi = 0;
                            gi = gi / rg.Max;
                            float bi = ((float)bts[rowRGB + indexRGB + 2] - rb.Min);
                            if (bi < 0)
                                bi = 0;
                            bi = bi / rb.Max;
                            int b = (int)(ri * 255);
                            int g = (int)(gi * 255);
                            int r = (int)(bi * 255);
                            row[indexRGBA + 3] = 255;//byte A
                            row[indexRGBA + 2] = (byte)(b);//byte R
                            row[indexRGBA + 1] = (byte)(g);//byte G
                            row[indexRGBA] = (byte)(r);//byte B
                        }
                    }
                }
                //unlocking bits and disposing image
                bmp.UnlockBits(bmd);
                return bmp;
            }
            else
            if (px == PixelFormat.Format48bppRgb)
            {
                //opening a 8 bit per pixel jpg image
                Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                //creating the bitmapdata and lock bits
                System.Drawing.Rectangle rec = new System.Drawing.Rectangle(0, 0, w, h);
                BitmapData bmd = bmp.LockBits(rec, ImageLockMode.ReadWrite, bmp.PixelFormat);
                unsafe
                {
                    //iterating through all the pixels in y direction
                    for (int y = 0; y < h; y++)
                    {
                        //getting the pixels of current row
                        byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                        int rowRGB = y * stride;
                        //iterating through all the pixels in x direction
                        for (int x = 0; x < w; x++)
                        {
                            int indexRGB = x * 6;
                            int indexRGBA = x * 4;
                            float ri = ((float)BitConverter.ToUInt16(bts, rowRGB + indexRGB) - rr.Min);
                            if (ri < 0)
                                ri = 0;
                            ri = ri / rr.Max;
                            float gi = ((float)BitConverter.ToUInt16(bts, rowRGB + indexRGB + 2) - rg.Min);
                            if (gi < 0)
                                gi = 0;
                            gi = gi / rg.Max;
                            float bi = ((float)BitConverter.ToUInt16(bts, rowRGB + indexRGB + 4) - rb.Min);
                            if (bi < 0)
                                bi = 0;
                            bi = bi / rb.Max;
                            int b = (int)(ri * 255);
                            int g = (int)(gi * 255);
                            int r = (int)(bi * 255);
                            row[indexRGBA + 3] = 255;//byte A
                            row[indexRGBA + 2] = (byte)(b);//byte R
                            row[indexRGBA + 1] = (byte)(g);//byte G
                            row[indexRGBA] = (byte)(r);//byte B
                        }
                    }
                }

                bmp.UnlockBits(bmd);
                return bmp;
            }
            throw new InvalidDataException("Get Filtered only supports 24bit & 48 bit images.");
        }
        public Bitmap GetFiltered(IntRange rr, IntRange rg, IntRange rb)
        {
            return BufferInfo.GetFiltered(SizeX, SizeY, Stride, PixelFormat, Bytes, rr, rg, rb);
        }
        public void Crop(Rectangle r)
        {
            //This crop function supports 16 bit images unlike Bitmap class.
            if (BitsPerPixel > 8)
            {
                if (RGBChannelsCount == 1)
                {
                    byte[] bts = null;
                    int bytesPer = 2;
                    int stridenew = r.Width * bytesPer;
                    int strideold = Stride;
                    bts = new byte[(stridenew * r.Height)];
                    for (int y = 0; y < r.Height; y++)
                    {
                        for (int x = 0; x < stridenew; x += bytesPer)
                        {
                            int indexnew = (y * stridenew + x);
                            int indexold = ((y + r.Y) * strideold + (x + (r.X * bytesPer)));// + r.X;
                            bts[indexnew] = bytes[indexold];
                            bts[indexnew + 1] = bytes[indexold + 1];
                        }
                    }
                    bytes = bts;
                }
                else
                {
                    byte[] bts = null;
                    int bytesPer = 6;
                    int stridenew = r.Width * bytesPer;
                    int strideold = Stride;
                    bts = new byte[(stridenew * r.Height)];
                    for (int y = 0; y < r.Height; y++)
                    {
                        for (int x = 0; x < stridenew; x += bytesPer)
                        {
                            int indexnew = (y * stridenew + x);
                            int indexold = ((y + r.Y) * strideold + (x + (r.X * bytesPer)));// + r.X;
                            bts[indexnew] = bytes[indexold];
                            bts[indexnew + 1] = bytes[indexold + 1];
                            bts[indexnew + 2] = bytes[indexold + 2];
                            bts[indexnew + 3] = bytes[indexold + 3];
                            bts[indexnew + 4] = bytes[indexold + 4];
                            bts[indexnew + 5] = bytes[indexold + 5];
                        }
                    }
                    bytes = bts;
                }
            }
            else
            {
                Image = ((Bitmap)Image).Clone(r, PixelFormat);
            }
            SizeX = r.Width;
            SizeY = r.Height;
        }
        public Bitmap GetCropBitmap(Rectangle r)
        {
            //This crop function supports 16 bit images unlike Bitmap class.
            if (BitsPerPixel > 8)
            {
                byte[] bts = null;
                if (RGBChannelsCount == 1)
                {
                    int bytesPer = 2;
                    int stridenew = r.Width * bytesPer;
                    int strideold = Stride;
                    bts = new byte[(stridenew * r.Height)];
                    for (int y = 0; y < r.Height; y++)
                    {
                        for (int x = 0; x < stridenew; x += bytesPer)
                        {
                            int indexnew = (y * stridenew + x) * RGBChannelsCount;
                            int indexold = (((y + r.Y) * strideold + (x + (r.X * bytesPer))) * RGBChannelsCount);// + r.X;
                            bts[indexnew] = bytes[indexold];
                            bts[indexnew + 1] = bytes[indexold + 1];
                        }
                    }
                    return new Bitmap(r.Width, r.Height, stridenew, PixelFormat.Format16bppGrayScale, Marshal.UnsafeAddrOfPinnedArrayElement(bts, 0));
                }
                else
                {
                    int bytesPer = 6;
                    int stridenew = r.Width * bytesPer;
                    int strideold = Stride;
                    bts = new byte[(stridenew * r.Height)];
                    for (int y = 0; y < r.Height; y++)
                    {
                        for (int x = 0; x < stridenew; x += bytesPer)
                        {
                            int indexnew = (y * stridenew + x);
                            int indexold = ((y + r.Y) * strideold + (x + (r.X * bytesPer)));// + r.X;
                            bts[indexnew] = bytes[indexold];
                            bts[indexnew + 1] = bytes[indexold + 1];
                            bts[indexnew + 2] = bytes[indexold + 2];
                            bts[indexnew + 3] = bytes[indexold + 3];
                            bts[indexnew + 4] = bytes[indexold + 4];
                            bts[indexnew + 5] = bytes[indexold + 5];
                        }
                    }
                    //bytes = bts;
                    return new Bitmap(r.Width, r.Height, stridenew, PixelFormat.Format48bppRgb, Marshal.UnsafeAddrOfPinnedArrayElement(bts, 0));
                }
            }
            else
            {
                return ((Bitmap)Image).Clone(r, PixelFormat);
            }

        }
        public BufferInfo GetCropBuffer(Rectangle r)
        {
            BufferInfo inf = null;
            //This crop function supports 16 bit images unlike Bitmap class.
            if (BitsPerPixel > 8)
            {
                byte[] bts = null;
                if (RGBChannelsCount == 1)
                {
                    int bytesPer = 2;
                    int stridenew = r.Width * bytesPer;
                    int strideold = Stride;
                    bts = new byte[(stridenew * r.Height)];
                    for (int y = 0; y < r.Height; y++)
                    {
                        for (int x = 0; x < stridenew; x += bytesPer)
                        {
                            int indexnew = (y * stridenew + x) * RGBChannelsCount;
                            int indexold = (((y + r.Y) * strideold + (x + (r.X * bytesPer))) * RGBChannelsCount);// + r.X;
                            bts[indexnew] = bytes[indexold];
                            bts[indexnew + 1] = bytes[indexold + 1];
                        }
                    }
                    BufferInfo bf = new BufferInfo(r.Width, r.Height, PixelFormat.Format16bppGrayScale, bts, Coordinate, ID);
                    return bf;
                }
                else
                {
                    int bytesPer = 6;
                    int stridenew = r.Width * bytesPer;
                    int strideold = Stride;
                    bts = new byte[(stridenew * r.Height)];
                    for (int y = 0; y < r.Height; y++)
                    {
                        for (int x = 0; x < stridenew; x += bytesPer)
                        {
                            int indexnew = (y * stridenew + x);
                            int indexold = ((y + r.Y) * strideold + (x + (r.X * bytesPer)));// + r.X;
                            bts[indexnew] = bytes[indexold];
                            bts[indexnew + 1] = bytes[indexold + 1];
                            bts[indexnew + 2] = bytes[indexold + 2];
                            bts[indexnew + 3] = bytes[indexold + 3];
                            bts[indexnew + 4] = bytes[indexold + 4];
                            bts[indexnew + 5] = bytes[indexold + 5];
                        }
                    }
                    BufferInfo bf = new BufferInfo(r.Width, r.Height, PixelFormat.Format48bppRgb, bts, Coordinate, ID);
                    return bf;
                }
            }
            else
            {
                Bitmap bmp = ((Bitmap)Image).Clone(r, PixelFormat);
                return new BufferInfo(ID, bmp, Coordinate, 0);
            }
        }
        public BufferInfo(string file, int w, int h, PixelFormat px, byte[] bts, ZCT coord, int index)
        {
            ID = CreateID(file, index);
            SizeX = w;
            SizeY = h;
            pixelFormat = px;
            Coordinate = coord;
            Bytes = bts;
            if (isRGB)
                SwitchRedBlue();
            Bitmap b = (Bitmap)Image.Clone();
            b.RotateFlip(RotateFlipType.Rotate180FlipNone);
            Image = b;
        }
        public BufferInfo(string file, Image im, ZCT coord, int index)
        {
            ID = CreateID(file, index);
            SizeX = im.Width;
            SizeY = im.Height;
            pixelFormat = im.PixelFormat;
            Coordinate = coord;
            Image = im;
            if (isRGB)
                SwitchRedBlue();
            Bitmap b = (Bitmap)Image.Clone();
            b.RotateFlip(RotateFlipType.Rotate180FlipNone);
            Image = b;
        }
        public BufferInfo(int w, int h, PixelFormat px, byte[] bts, ZCT coord, string id)
        {
            ID = id;
            SizeX = w;
            SizeY = h;
            pixelFormat = px;
            Coordinate = coord;
            Bytes = bts;
            if (isRGB)
                SwitchRedBlue();
            Bitmap b = (Bitmap)Image.Clone();
            b.RotateFlip(RotateFlipType.Rotate180FlipNone);
            Image = b;
        }
        public Statistics UpdateStatistics()
        {
            statistics = Statistics.FromBytes(bytes, SizeX, SizeY, RGBChannelsCount, BitsPerPixel, Stride);
            return statistics;
        }
        /*
        private static int count = 0;
        private static List<BufferInfo> bufferInfos = new List<BufferInfo>();
        public static void AddBuffer(BufferInfo b, int Count)
        {
            index = 0;
            bufferInfos.Add(b);
            count = Count;
        }
        public static void CalculateStatistics()
        {
            Thread th = new Thread(CalcStats);
            th.Start();
        }
        public static void ClearStatsBuffer()
        {
            bufferInfos.Clear();
        }
        */
        public static Bitmap SwitchRedBlue(Bitmap image)
        {
            ExtractChannel cr = new ExtractChannel(AForge.Imaging.RGB.R);
            ExtractChannel cb = new ExtractChannel(AForge.Imaging.RGB.B);
            // apply the filter
            Bitmap rImage = cr.Apply(image);
            Bitmap bImage = cb.Apply(image);

            ReplaceChannel replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, bImage);
            replaceRFilter.ApplyInPlace(image);

            ReplaceChannel replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, rImage);
            replaceBFilter.ApplyInPlace(image);
            rImage.Dispose();
            bImage.Dispose();
            return image;
        }
        public void SwitchRedBlue()
        {
            if (PixelFormat == PixelFormat.Format8bppIndexed || PixelFormat == PixelFormat.Format16bppGrayScale)
                return;
            //BufferInfo bf = new BufferInfo(SizeX, SizeY,PixelFormat, bytes, Coordinate, ID);
            if (PixelFormat == PixelFormat.Format24bppRgb)
                for (int y = 0; y < SizeY; y++)
                {
                    for (int x = 0; x < Stride; x += 3)
                    {
                        int i = y * Stride + x;
                        byte bb = bytes[i + 2];
                        bytes[i + 2] = bytes[i];
                        bytes[i] = bb;
                    }
                }
            if (PixelFormat == PixelFormat.Format32bppArgb)
                for (int y = 0; y < SizeY; y++)
                {
                    for (int x = 0; x < Stride; x += 4)
                    {
                        int i = y * Stride + x;
                        byte bb = bytes[i + 2];
                        bytes[i + 2] = bytes[i];
                        bytes[i] = bb;
                    }
                }
            /*
            Bitmap b = (Bitmap)Image;
            ExtractChannel cr = new ExtractChannel(AForge.Imaging.RGB.R);
            ExtractChannel cb = new ExtractChannel(AForge.Imaging.RGB.B);
            // apply the filter
            Bitmap rImage = cr.Apply(b);
            Bitmap bImage = cb.Apply(b);

            ReplaceChannel replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, bImage);
            replaceRFilter.ApplyInPlace(b);

            ReplaceChannel replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, rImage);
            replaceBFilter.ApplyInPlace(b);
            rImage.Dispose();
            bImage.Dispose();
            */
        }
        public byte[] GetSaveBytes(bool littleEndian)
        {
            Bitmap bitmap = (Bitmap)Image.Clone();
            if (littleEndian)
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, PixelFormat);
            IntPtr ptr = data.Scan0;
            int length = this.bytes.Length;
            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);
            if (littleEndian)
                Array.Reverse(bytes);
            bitmap.UnlockBits(data);
            bitmap.Dispose();
            return bytes;
        }
        public static byte[] GetBuffer(Bitmap bmp, int stride)
        {
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            IntPtr ptr = data.Scan0;
            int length = data.Stride * bmp.Height;
            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);
            Array.Reverse(bytes);
            bmp.UnlockBits(data);
            return bytes;
        }
        public static Bitmap To24Bit(Bitmap b)
        {
            Bitmap bm = new Bitmap(b.Width, b.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bm);
            if (b.PixelFormat == PixelFormat.Format16bppGrayScale || b.PixelFormat == PixelFormat.Format48bppRgb)
            {
                g.DrawImage(AForge.Imaging.Image.Convert16bppTo8bpp(b), 0, 0);
            }
            else
            {
                g.DrawImage(b, 0, 0);
            }
            g.Dispose();
            return bm;
        }
        public static Bitmap To32Bit(Bitmap b)
        {
            Bitmap bm = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppArgb);
            if (b.PixelFormat == PixelFormat.Format16bppGrayScale || b.PixelFormat == PixelFormat.Format48bppRgb)
            {
                bm = AForge.Imaging.Image.Convert16bppTo8bpp(b);
            }
            Graphics g = Graphics.FromImage(bm);
            g.DrawImage(b, 0, 0);
            return bm;
        }
        public void RGBTo32Bit()
        {
            Bitmap bm = new Bitmap(SizeX, SizeY, PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(bm);
            g.DrawImage((Bitmap)Image, 0, 0);
            Image = bm;
        }
        public static Bitmap SwitchChannels(Bitmap image, int c1, int c2)
        {
            ExtractChannel cr = new ExtractChannel((short)c1);
            ExtractChannel cb = new ExtractChannel((short)c2);
            // apply the filter
            Bitmap rImage = cr.Apply(image);
            Bitmap bImage = cb.Apply(image);
            ReplaceChannel replaceRFilter = new ReplaceChannel((short)c1, bImage);
            replaceRFilter.ApplyInPlace(image);
            ReplaceChannel replaceBFilter = new ReplaceChannel((short)c2, rImage);
            replaceBFilter.ApplyInPlace(image);
            rImage.Dispose();
            bImage.Dispose();
            return image;
        }

        public BufferInfo Copy()
        {
            byte[] bt = new byte[Bytes.Length];
            for (int i = 0; i < bt.Length; i++)
            {
                bt[i] = bytes[i];
            }
            BufferInfo bf = new BufferInfo(SizeX, SizeY, PixelFormat, bt, Coordinate, ID);
            if (LittleEndian)
                bf.RotateFlip(RotateFlipType.Rotate180FlipNone);
            return bf;
        }
        public void To8Bit()
        {
            Bitmap bm = AForge.Imaging.Image.Convert16bppTo8bpp((Bitmap)Image);
            bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
            Image = bm;
        }
        public void To16Bit()
        {
            Bitmap bm = AForge.Imaging.Image.Convert8bppTo16bpp((Bitmap)Image);
            bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
            Image = bm;
        }
        public void RotateFlip(RotateFlipType rot)
        {
            Bitmap fl = (Bitmap)Image.Clone();
            fl.RotateFlip(rot);
            Image = fl;
            fl.Dispose();
        }
        public bool isRGB
        {
            get
            {
                if (pixelFormat == PixelFormat.Format8bppIndexed || pixelFormat == PixelFormat.Format16bppGrayScale)
                    return false;
                else
                    return true;
            }
        }
        public override string ToString()
        {
            return ID;
        }
        public void Dispose()
        {
            bytes = null;
            if (statistics != null)
                statistics.Dispose();
            ID = null;
            file = null;
            GC.Collect();
        }
    }
    public class Filt
    {
        public enum Type
        {
            Base,
            Base2,
            InPlace,
            InPlace2,
            InPlacePartial,
            Resize,
            Rotate,
            Transformation,
            Copy
        }
        public string name;
        public IFilter filt;
        public Type type;
        public Filt(string s, IFilter f, Type t)
        {
            name = s;
            filt = f;
            type = t;
        }
    }
    public static class Filters
    {
        public static Filt GetFilter(string name)
        {
            return filters[name];
        }
        public static Dictionary<string, Filt> filters = new Dictionary<string, Filt>();
        public static BioImage Base(string id, string name, bool inPlace)
        {
            BioImage img = Images.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseFilter fi = (BaseFilter)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].SetImage(fi.Apply((Bitmap)img.Buffers[i].Image), false); ;
                }
                if (!inPlace)
                {
                    Images.AddImage(img);
                    ImageView iv = new ImageView(img);
                    iv.Show();
                }
                Recorder.AddLine("Bio.Filters.Base(" + '"' + id +
                    '"' + "," + '"' + name + '"' + "," + inPlace.ToString().ToLower() + ");");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage Base2(string id, string id2, string name, bool inPlace)
        {
            BioImage c2 = Images.GetImage(id);
            BioImage img = Images.GetImage(id2);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseFilter2 fi = (BaseFilter2)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    fi.OverlayImage = (Bitmap)c2.Buffers[i].Image;
                    img.Buffers[i].SetImage(fi.Apply((Bitmap)img.Buffers[i].Image), false);
                }
                if (!inPlace)
                {
                    Images.AddImage(img);
                    ImageView iv = new ImageView(img);
                    iv.Show();
                }
                Recorder.AddLine("Bio.Filters.Base2(" + '"' + id + '"' + "," +
                   '"' + id2 + '"' + "," + '"' + name + '"' + "," + inPlace.ToString().ToLower() + ");");
                return img;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage InPlace(string id, string name, bool inPlace)
        {
            BioImage img = Images.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseInPlaceFilter fi = (BaseInPlaceFilter)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].SetImage(fi.Apply((Bitmap)img.Buffers[i].Image), false);
                }
                if (!inPlace)
                {
                    Images.AddImage(img);
                    ImageView iv = new ImageView(img);
                    iv.Show();
                }
                Recorder.AddLine("Bio.Filters.InPlace(" + '"' + id +
                    '"' + "," + '"' + name + '"' + "," + inPlace.ToString().ToLower() + ");");
                return img;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage InPlace2(string id, string id2, string name, bool inPlace)
        {
            BioImage c2 = Images.GetImage(id);
            BioImage img = Images.GetImage(id2);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseInPlaceFilter2 fi = (BaseInPlaceFilter2)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    fi.OverlayImage = (Bitmap)c2.Buffers[i].Image;
                    img.Buffers[i].SetImage(fi.Apply((Bitmap)img.Buffers[i].Image), false);
                }
                if (!inPlace)
                {
                    Images.AddImage(img);
                    ImageView iv = new ImageView(img);
                    iv.Show();
                }
                Recorder.AddLine("Bio.Filters.InPlace2(" + '"' + id + '"' + "," +
                   '"' + id2 + '"' + "," + '"' + name + '"' + "," + inPlace.ToString().ToLower() + ");");
                return img;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage InPlacePartial(string id, string name, bool inPlace)
        {
            BioImage img = Images.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseInPlacePartialFilter fi = (BaseInPlacePartialFilter)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].SetImage(fi.Apply((Bitmap)img.Buffers[i].Image), false);
                }
                if (!inPlace)
                {
                    Images.AddImage(img);
                    ImageView iv = new ImageView(img);
                    iv.Show();
                }
                Recorder.AddLine("Bio.Filters.InPlacePartial(" + '"' + id +
                    '"' + "," + '"' + name + '"' + "," + inPlace.ToString().ToLower() + ");");
                return img;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage Resize(string id, string name, bool inPlace, int w, int h)
        {
            BioImage img = Images.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseResizeFilter fi = (BaseResizeFilter)f.filt;
                fi.NewHeight = h;
                fi.NewWidth = w;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].SetImage(fi.Apply((Bitmap)img.Buffers[i].Image), false);
                }
                if (!inPlace)
                {
                    Images.AddImage(img);
                    ImageView iv = new ImageView(img);
                    iv.Show();
                }
                Recorder.AddLine("Bio.Filters.Resize(" + '"' + id +
                    '"' + "," + '"' + name + '"' + "," + inPlace.ToString().ToLower() + "," + w + "," + h + ");");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage Rotate(string id, string name, bool inPlace, float angle, int a, int r, int g, int b)
        {
            BioImage img = Images.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(Images.GetImage(id));
            try
            {
                Filt f = filters[name];
                BaseRotateFilter fi = (BaseRotateFilter)f.filt;
                fi.Angle = angle;
                fi.FillColor = System.Drawing.Color.FromArgb(a, r, g, b);
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].SetImage(fi.Apply((Bitmap)img.Buffers[i].Image), false);
                }
                if (!inPlace)
                {
                    Images.AddImage(img);
                    ImageView iv = new ImageView(img);
                    iv.Show();
                }
                Recorder.AddLine("Bio.Filters.Rotate(" + '"' + id +
                    '"' + "," + '"' + name + '"' + "," + inPlace.ToString().ToLower() + "," + angle.ToString() + "," +
                    a + "," + r + "," + g + "," + b + ");");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;

        }
        public static BioImage Transformation(string id, string name, bool inPlace, float angle)
        {
            BioImage img = Images.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseTransformationFilter fi = (BaseTransformationFilter)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].SetImage(fi.Apply((Bitmap)img.Buffers[i].Image), false);
                }
                if (!inPlace)
                {
                    Images.AddImage(img);
                    ImageView iv = new ImageView(img);
                    iv.Show();
                }
                Recorder.AddLine("Bio.Filters.Transformation(" + '"' + id +
                        '"' + "," + '"' + name + '"' + "," + inPlace.ToString().ToLower() + "," + angle + ");");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage Copy(string id, string name, bool inPlace)
        {
            BioImage img = Images.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseUsingCopyPartialFilter fi = (BaseUsingCopyPartialFilter)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].Image = fi.Apply((Bitmap)img.Buffers[i].Image);
                }
                if (!inPlace)
                {
                    Images.AddImage(img);
                    ImageView iv = new ImageView(img);
                    iv.Show();
                }
                Recorder.AddLine("Bio.Filters.Copy(" + '"' + id +
                        '"' + "," + '"' + name + '"' + "," + inPlace.ToString().ToLower() + ");");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage Crop(string id, double x, double y, double w, double h)
        {
            BioImage c = Images.GetImage(id);
            Rectangle r = c.ToImageSpace(new RectangleD(x, y, w, h));
            BioImage img = BioImage.Copy(c,false);
            for (int i = 0; i < img.Buffers.Count; i++)
            {
                img.Buffers[i].Crop(r);
            }
            Images.AddImage(img);
            Recorder.AddLine("Bio.Filters.Crop(" + '"' + id + '"' + "," + x + "," + y + "," + w + "," + h + ");");
            App.tabsView.AddTab(img);
            return img;
        }
        public static BioImage Crop(string id, RectangleD r)
        {
            return Crop(id, r.X, r.Y, r.W, r.H);
        }
        public static void Init()
        {
            //Base Filters
            Filt f = new Filt("AdaptiveSmoothing", new AdaptiveSmoothing(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("BayerFilter", new BayerFilter(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("BayerFilterOptimized", new BayerFilterOptimized(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("BayerDithering", new BayerDithering(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("ConnectedComponentsLabeling", new ConnectedComponentsLabeling(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("ExtractChannel", new ExtractChannel(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("ExtractNormalizedRGBChannel", new ExtractNormalizedRGBChannel(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("Grayscale", new Grayscale(0.2125, 0.7154, 0.0721), Filt.Type.Base);
            filters.Add(f.name, f);
            //f = new Filt("TexturedFilter", new TexturedFilter());
            //filters.Add(f.name, f);
            f = new Filt("WaterWave", new WaterWave(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("YCbCrExtractChannel", new YCbCrExtractChannel(), Filt.Type.Base);
            filters.Add(f.name, f);

            //BaseFilter2
            f = new Filt("ThresholdedDifference", new ThresholdedDifference(), Filt.Type.Base2);
            filters.Add(f.name, f);
            f = new Filt("ThresholdedEuclideanDifference", new ThresholdedDifference(), Filt.Type.Base2);
            filters.Add(f.name, f);


            //BaseInPlaceFilter
            f = new Filt("BackwardQuadrilateralTransformation", new BackwardQuadrilateralTransformation(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("BlobsFiltering", new BlobsFiltering(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("BottomHat", new BottomHat(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("BradleyLocalThresholding", new BradleyLocalThresholding(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("CanvasCrop", new CanvasCrop(Rectangle.Empty), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("CanvasFill", new CanvasFill(Rectangle.Empty), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("CanvasMove", new CanvasMove(new IntPoint()), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("FillHoles", new FillHoles(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("FlatFieldCorrection", new FlatFieldCorrection(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("TopHat", new TopHat(), Filt.Type.InPlace);
            filters.Add(f.name, f);

            //BaseInPlaceFilter2
            f = new Filt("Add", new Add(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Difference", new Difference(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Intersect", new Intersect(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Merge", new Merge(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Morph", new Morph(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("MoveTowards", new MoveTowards(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("StereoAnaglyph", new StereoAnaglyph(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Subtract", new Subtract(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            //f = new Filt("Add", new TexturedMerge(), Filt.Type.InPlace2);
            //filters.Add(f.name, f);

            //BaseInPlacePartialFilter
            f = new Filt("AdditiveNoise", new AdditiveNoise(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);

            //f = new Filt("ApplyMask", new ApplyMask(), Filt.Type.InPlacePartial2);
            //filters.Add(f.name, f);
            f = new Filt("BrightnessCorrection", new BrightnessCorrection(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ChannelFiltering", new ChannelFiltering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ColorFiltering", new ColorFiltering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ColorRemapping", new ColorRemapping(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ContrastCorrection", new ContrastCorrection(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ContrastStretch", new ContrastStretch(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            //f = new Filt("ErrorDiffusionDithering", new ErrorDiffusionDithering(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);
            f = new Filt("EuclideanColorFiltering", new EuclideanColorFiltering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("GammaCorrection", new GammaCorrection(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("HistogramEqualization", new HistogramEqualization(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("HorizontalRunLengthSmoothing", new HorizontalRunLengthSmoothing(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("HSLFiltering", new HSLFiltering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("HueModifier", new HueModifier(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("Invert", new Invert(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("LevelsLinear", new LevelsLinear(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("LevelsLinear16bpp", new LevelsLinear16bpp(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            //f = new Filt("MaskedFilter", new MaskedFilter(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);
            //f = new Filt("Mirror", new Mirror(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);
            f = new Filt("OrderedDithering", new OrderedDithering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("OtsuThreshold", new OtsuThreshold(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("Pixellate", new Pixellate(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("PointedColorFloodFill", new PointedColorFloodFill(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("PointedMeanFloodFill", new PointedMeanFloodFill(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ReplaceChannel", new Invert(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("RotateChannels", new LevelsLinear(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("SaltAndPepperNoise", new LevelsLinear16bpp(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("SaturationCorrection", new SaturationCorrection(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("Sepia", new Sepia(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("SimplePosterization", new SimplePosterization(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("SISThreshold", new SISThreshold(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            //f = new Filt("Texturer", new Texturer(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);
            //f = new Filt("Threshold", new Threshold(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);
            f = new Filt("ThresholdWithCarry", new ThresholdWithCarry(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("VerticalRunLengthSmoothing", new VerticalRunLengthSmoothing(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("YCbCrFiltering", new YCbCrFiltering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("YCbCrLinear", new YCbCrLinear(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            //f = new Filt("YCbCrReplaceChannel", new YCbCrReplaceChannel(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);

            //BaseResizeFilter
            f = new Filt("ResizeBicubic", new ResizeBicubic(0, 0), Filt.Type.Resize);
            filters.Add(f.name, f);
            f = new Filt("ResizeBilinear", new ResizeBilinear(0, 0), Filt.Type.Resize);
            filters.Add(f.name, f);
            f = new Filt("ResizeNearestNeighbor", new ResizeNearestNeighbor(0, 0), Filt.Type.Resize);
            filters.Add(f.name, f);
            //BaseRotateFilter
            f = new Filt("RotateBicubic", new RotateBicubic(0), Filt.Type.Rotate);
            filters.Add(f.name, f);
            f = new Filt("RotateBilinear", new RotateBilinear(0), Filt.Type.Rotate);
            filters.Add(f.name, f);
            f = new Filt("RotateNearestNeighbor", new RotateNearestNeighbor(0), Filt.Type.Rotate);
            filters.Add(f.name, f);

            //Transformation
            f = new Filt("Crop", new Crop(Rectangle.Empty), Filt.Type.Transformation);
            filters.Add(f.name, f);

            f = new Filt("QuadrilateralTransformation", new QuadrilateralTransformation(), Filt.Type.Transformation);
            filters.Add(f.name, f);
            //f = new Filt("QuadrilateralTransformationBilinear", new QuadrilateralTransformationBilinear(), Filt.Type.Transformation);
            //filters.Add(f.name, f);
            //f = new Filt("QuadrilateralTransformationNearestNeighbor", new QuadrilateralTransformationNearestNeighbor(), Filt.Type.Transformation);
            //filters.Add(f.name, f);
            f = new Filt("Shrink", new Shrink(), Filt.Type.Transformation);
            filters.Add(f.name, f);
            f = new Filt("SimpleQuadrilateralTransformation", new SimpleQuadrilateralTransformation(), Filt.Type.Transformation);
            filters.Add(f.name, f);
            f = new Filt("TransformFromPolar", new TransformFromPolar(), Filt.Type.Transformation);
            filters.Add(f.name, f);
            f = new Filt("TransformToPolar", new TransformToPolar(), Filt.Type.Transformation);
            filters.Add(f.name, f);

            //BaseUsingCopyPartialFilter 
            f = new Filt("BinaryDilatation3x3", new BinaryDilatation3x3(), Filt.Type.Copy);
            filters.Add(f.name, f);
            f = new Filt("BilateralSmoothing ", new BilateralSmoothing(), Filt.Type.Copy);
            filters.Add(f.name, f);
            f = new Filt("BinaryErosion3x3 ", new BinaryErosion3x3(), Filt.Type.Copy);
            filters.Add(f.name, f);

        }
    }
    public class Statistics
    {
        private int[] values = null;
        public int[] Values
        {
            get { return values; }
            set { values = value; }
        }
        private int bitsPerPixel;
        private int min = ushort.MaxValue;
        private int max = ushort.MinValue;
        private float stackMin = ushort.MaxValue;
        private float stackMax = ushort.MinValue;
        private float stackMean = 0;
        private float stackMedian = 0;
        private float mean = 0;
        private float median = 0;
        private float meansum = 0;
        private float[] stackValues;
        private int count = 0;
        public int Min
        {
            get { return min; }
        }
        public int Max
        {
            get { return max; }
        }
        public double Mean
        {
            get { return mean; }
        }
        public int BitsPerPixel
        {
            get { return bitsPerPixel; }
        }
        public float Median
        {
            get
            {
                return median;
            }
        }
        public float StackMedian
        {
            get
            {
                return stackMedian;
            }
        }
        public float StackMean
        {
            get
            {
                return stackMean;
            }
        }
        public float StackMax
        {
            get
            {
                return stackMax;
            }
        }
        public float StackMin
        {
            get
            {
                return stackMin;
            }
        }
        public float[] StackValues
        {
            get { return stackValues; }
        }
        public Statistics(bool bit16)
        {
            if (bit16)
            {
                values = new int[ushort.MaxValue + 1];
                bitsPerPixel = 16;
            }
            else
            {
                values = new int[byte.MaxValue + 1];
                bitsPerPixel = 8;
            }
        }
        public static Statistics FromBytes(byte[] bts, int w, int h, int rGBChannels, int BitsPerPixel, int stride)
        {
            Statistics st;
            st = new Statistics(true);
            st.max = ushort.MinValue;
            st.min = ushort.MaxValue;
            st.bitsPerPixel = BitsPerPixel;
            float sum = 0;
            if (BitsPerPixel > 8)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w * 2; x += 2)
                    {
                        ushort s = BitConverter.ToUInt16(bts, (y * stride) + x);
                        if (st.max < s)
                            st.max = s;
                        if (st.min > s)
                            st.min = s;
                        st.values[s]++;
                        sum += s;
                    }
                }
            }
            else
            {
                for (int i = 0; i < bts.Length; i++)
                {
                    byte s = bts[i];
                    if (st.max < s)
                        st.max = s;
                    if (st.min > s)
                        st.min = s;
                    st.values[s]++;
                }
            }
            st.mean = sum / (float)(w * h);
            int median = 0;
            for (int i = 0; i < st.values.Length; i++)
            {
                if (median < st.values[i])
                    median = st.values[i];
            }
            st.median = median;
            return st;
        }
        public static Statistics FromBytes(BufferInfo bf)
        {
            return FromBytes(bf.Bytes, bf.SizeX, bf.SizeY, bf.RGBChannelsCount, bf.BitsPerPixel, bf.Stride);
        }
        public static BioImage b = null;
        public static Dictionary<string, BufferInfo> list = new Dictionary<string, BufferInfo>();
        public static void FromBytes()
        {
            string name = Thread.CurrentThread.Name;
            list[name].Statistics = FromBytes(list[name]);
            list.Remove(name);
        }
        public static void CalcStatistics(BufferInfo bf)
        {
            Thread th = new Thread(FromBytes);
            th.Name = bf.ID;
            list.Add(th.Name.ToString(), bf);
            th.Start();
        }
        public static void ClearCalcBuffer()
        {
            list.Clear();
        }
        public void AddStatistics(Statistics s)
        {
            if (stackValues == null)
            {
                if (bitsPerPixel > 8)
                    stackValues = new float[ushort.MaxValue + 1];
                else
                    stackValues = new float[byte.MaxValue + 1];
            }
            if (stackMax < s.max)
                stackMax = s.max;
            if (stackMin > s.min)
                stackMin = s.min;
            meansum += s.mean;
            for (int i = 0; i < stackValues.Length; i++)
            {
                stackValues[i] += s.values[i];
            }
            values = s.values;
            count++;
        }
        public void MeanHistogram()
        {
            for (int i = 0; i < stackValues.Length; i++)
            {
                stackValues[i] /= (float)count;
            }
            stackMean = (float)meansum / (float)count;

            for (int i = 0; i < stackValues.Length; i++)
            {
                if (stackMedian < stackValues[i])
                    stackMedian = (float)stackValues[i];
            }

        }
        public void Dispose()
        {
            stackValues = null;
            values = null;
        }
        public void DisposeHistogram()
        {
            stackValues = null;
            values = null;
        }
    }
    public class BioImageInfo
    {
        bool HasPhysicalXY = false;
        bool HasPhysicalXYZ = false;
        private double physicalSizeX = -1;
        private double physicalSizeY = -1;
        private double physicalSizeZ = -1;
        public double PhysicalSizeX
        {
            get { return physicalSizeX; }
            set
            {
                physicalSizeX = value;
                HasPhysicalXY = true;
            }
        }
        public double PhysicalSizeY
        {
            get { return physicalSizeY; }
            set
            {
                physicalSizeY = value;
                HasPhysicalXY = true;
            }
        }
        public double PhysicalSizeZ
        {
            get { return physicalSizeZ; }
            set
            {
                physicalSizeZ = value;
                HasPhysicalXYZ = true;
            }
        }

        bool HasStageXY = false;
        bool HasStageXYZ = false;
        public double stageSizeX = -1;
        public double stageSizeY = -1;
        public double stageSizeZ = -1;
        public double StageSizeX
        {
            get { return stageSizeX; }
            set
            {
                stageSizeX = value;
                HasStageXY = true;
            }
        }
        public double StageSizeY
        {
            get { return stageSizeY; }
            set
            {
                stageSizeY = value;
                HasStageXY = true;
            }
        }
        public double StageSizeZ
        {
            get { return stageSizeZ; }
            set
            {
                stageSizeZ = value;
                HasStageXYZ = true;
            }
        }

        private int series = 0;
        public int Series
        {
            get { return series; }
            set { series = value; }
        }

        public BioImageInfo Copy()
        {
            BioImageInfo inf = new BioImageInfo();
            inf.PhysicalSizeX = PhysicalSizeX;
            inf.PhysicalSizeY = PhysicalSizeY;
            inf.PhysicalSizeZ = PhysicalSizeZ;
            inf.StageSizeX = StageSizeX;
            inf.StageSizeY = StageSizeY;
            inf.StageSizeZ = StageSizeZ;
            inf.HasPhysicalXY = HasPhysicalXY;
            inf.HasPhysicalXYZ = HasPhysicalXYZ;
            inf.StageSizeX = StageSizeX;
            inf.StageSizeY = StageSizeY;
            inf.StageSizeZ = StageSizeZ;
            inf.HasStageXY = HasStageXY;
            inf.HasStageXYZ = HasStageXYZ;
            return inf;
        }

    }
    public class BioImage : IDisposable
    {
        public int[,,] Coords;
        private ZCT coordinate;
        public ZCT Coordinate
        {
            get
            {
                return coordinate;
            }
            set
            {
                coordinate = value;
            }
        }

        private string id;
        public List<Channel> Channels = new List<Channel>();
        public List<BufferInfo> Buffers = new List<BufferInfo>();
        public VolumeD Volume;
        public List<ROI> Annotations = new List<ROI>();
        public string filename = "";
        public string script = "";
        public string Filename
        {
            get
            {
                return Path.GetFileName(id);
            }
            set
            {
                filename = value;
            }
        }
        public int[] rgbChannels = new int[3];
        public int RGBChannelCount
        {
            get
            {
                return Buffers[0].RGBChannelsCount;
            }
        }
        public int bitsPerPixel;
        public int imagesPerSeries = 0;
        public int seriesCount = 1;
        public double frameInterval = 0;
        public bool littleEndian = false;
        public bool isGroup = false;
        public long loadTimeMS = 0;
        public long loadTimeTicks = 0;
        public bool selected = false;
        public Statistics Statistics
        {
            get
            {
                return statistics;
            }
            set
            {
                statistics = value;
            }
        }
        private int sizeZ, sizeC, sizeT;
        private Statistics statistics;
        private Bitmap rgbBitmap8 = null;
        private Bitmap rgbBitmap16 = null;

        BioImageInfo imageInfo = new BioImageInfo();
        public static BioImage Copy(BioImage b, bool rois)
        {
            BioImage bi = new BioImage(b.ID);
            if(rois)
            foreach (ROI an in b.Annotations)
            {
                bi.Annotations.Add(an);
            }
            foreach (BufferInfo bf in b.Buffers)
            {
                bi.Buffers.Add(bf.Copy());
            }
            foreach (Channel c in b.Channels)
            {
                bi.Channels.Add(c);
            }
            bi.Volume = b.Volume;
            bi.Coords = b.Coords;
            bi.sizeZ = b.sizeZ;
            bi.sizeC = b.sizeC;
            bi.sizeT = b.sizeT;
            bi.series = b.series;
            bi.seriesCount = b.seriesCount;
            bi.frameInterval = b.frameInterval;
            bi.littleEndian = b.littleEndian;
            bi.isGroup = b.isGroup;
            bi.imageInfo = b.imageInfo;
            bi.bitsPerPixel = b.bitsPerPixel;
            bi.rgbBitmap16 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
            bi.rgbBitmap8 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            return bi;
        }
        public static BioImage Copy(BioImage b)
        {
            return Copy(b, true);
        }
        public BioImage Copy(bool rois)
        {
            return BioImage.Copy(this,rois);
        }
        public BioImage Copy()
        {
            return BioImage.Copy(this, true);
        }
        public static BioImage CopyInfo(BioImage b, bool copyAnnotations, bool copyChannels)
        {
            BioImage bi = new BioImage(b.ID);
            if (copyAnnotations)
                foreach (ROI an in b.Annotations)
                {
                    bi.Annotations.Add(an);
                }
            if (copyChannels)
                foreach (Channel c in b.Channels)
                {
                    bi.Channels.Add(c.Copy());
                }

            bi.Coords = b.Coords;
            bi.Volume = b.Volume;
            bi.sizeZ = b.sizeZ;
            bi.sizeC = b.sizeC;
            bi.sizeT = b.sizeT;
            bi.series = b.series;
            bi.seriesCount = b.seriesCount;
            bi.frameInterval = b.frameInterval;
            bi.littleEndian = b.littleEndian;
            bi.isGroup = b.isGroup;
            bi.imageInfo = b.imageInfo;
            bi.bitsPerPixel = b.bitsPerPixel;
            bi.rgbBitmap16 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
            bi.rgbBitmap8 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            return bi;
        }
        public string ID
        {
            get { return id; }
            set
            {
                id = value;
            }
        }
        public int ImageCount
        {
            get
            {
                return Buffers.Count;
            }
        }
        public double physicalSizeX
        {
            get { return imageInfo.PhysicalSizeX; }
            set { imageInfo.PhysicalSizeX = value; }
        }
        public double physicalSizeY
        {
            get { return imageInfo.PhysicalSizeY; }
            set { imageInfo.PhysicalSizeY = value; }
        }
        public double physicalSizeZ
        {
            get { return imageInfo.PhysicalSizeZ; }
            set { imageInfo.PhysicalSizeZ = value; }
        }
        public double stageSizeX
        {
            get { return imageInfo.StageSizeX; }
            set { imageInfo.StageSizeX = value; }
        }
        public double stageSizeY
        {
            get { return imageInfo.StageSizeY; }
            set { imageInfo.StageSizeY = value; }
        }
        public double stageSizeZ
        {
            get { return imageInfo.StageSizeZ; }
            set { imageInfo.StageSizeZ = value; }
        }

        public int series
        {
            get
            {
                return imageInfo.Series;
            }
            set
            {
                imageInfo.Series = value;
            }
        }

        static bool initialized = false;
        public Channel RChannel
        {
            get
            {
                return Channels[rgbChannels[0]];
            }
        }
        public Channel GChannel
        {
            get
            {
                return Channels[rgbChannels[1]];
            }
        }
        public Channel BChannel
        {
            get
            {
                return Channels[rgbChannels[2]];
            }
        }
        public class ImageJDesc
        {
            public string ImageJ;
            public int images = 0;
            public int channels = 0;
            public int slices = 0;
            public int frames = 0;
            public bool hyperstack;
            public string mode;
            public string unit;
            public double finterval = 0;
            public double spacing = 0;
            public bool loop;
            public double min = 0;
            public double max = 0;
            public int count;
            public bool bit8color = false;

            public ImageJDesc FromImage(BioImage b)
            {
                ImageJ = "";
                images = b.ImageCount;
                channels = b.SizeC;
                slices = b.SizeZ;
                frames = b.SizeT;
                hyperstack = true;
                mode = "grayscale";
                unit = "micron";
                finterval = b.frameInterval;
                spacing = b.physicalSizeZ;
                loop = false;
                /*
                double dmax = double.MinValue;
                double dmin = double.MaxValue;
                foreach (Channel c in b.Channels)
                {
                    if(dmax < c.Max)
                        dmax = c.Max;
                    if(dmin > c.Min)
                        dmin = c.Min;
                }
                min = dmin;
                max = dmax;
                */
                min = b.Channels[0].Min;
                max = b.Channels[0].Max;
                return this;
            }
            public string GetString()
            {
                string s = "";
                s += "ImageJ=" + ImageJ + "\n";
                s += "images=" + images + "\n";
                s += "channels=" + channels.ToString() + "\n";
                s += "slices=" + slices.ToString() + "\n";
                s += "frames=" + frames.ToString() + "\n";
                s += "hyperstack=" + hyperstack.ToString() + "\n";
                s += "mode=" + mode.ToString() + "\n";
                s += "unit=" + unit.ToString() + "\n";
                s += "finterval=" + finterval.ToString() + "\n";
                s += "spacing=" + spacing.ToString() + "\n";
                s += "loop=" + loop.ToString() + "\n";
                s += "min=" + min.ToString() + "\n";
                s += "max=" + max.ToString() + "\n";
                return s;
            }
            public void SetString(string desc)
            {
                string[] lines = desc.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                int maxlen = 20;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i < maxlen)
                    {
                        string[] sp = lines[i].Split('=');
                        if (sp[0] == "ImageJ")
                            ImageJ = sp[1];
                        if (sp[0] == "images")
                            images = int.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "channels")
                            channels = int.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "slices")
                            slices = int.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "frames")
                            frames = int.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "hyperstack")
                            hyperstack = bool.Parse(sp[1]);
                        if (sp[0] == "mode")
                            mode = sp[1];
                        if (sp[0] == "unit")
                            unit = sp[1];
                        if (sp[0] == "finterval")
                            finterval = double.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "spacing")
                            spacing = double.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "loop")
                            loop = bool.Parse(sp[1]);
                        if (sp[0] == "min")
                            min = double.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "max")
                            max = double.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "8bitcolor")
                            bit8color = bool.Parse(sp[1]);
                    }
                    else
                        return;
                }

            }
        }
        public int SizeX
        {
            get
            {
                if (Buffers.Count > 0)
                    return Buffers[0].SizeX;
                else return 0;
            }
        }
        public int SizeY
        {
            get
            {
                if (Buffers.Count > 0)
                    return Buffers[0].SizeY;
                else return 0;
            }
        }
        public int SizeZ
        {
            get { return sizeZ; }
        }
        public int SizeC
        {
            get { return sizeC; }
        }
        public int SizeT
        {
            get { return sizeT; }
        }
        public Stopwatch watch = new Stopwatch();
        public bool isRGB
        {
            get
            {
                if (RGBChannelCount == 3 || RGBChannelCount == 4)
                    return true;
                else
                    return false;
            }
        }
        public bool isTime
        {
            get
            {
                if (SizeT > 1)
                    return true;
                else
                    return false;
            }
        }
        public static bool Initialized
        {
            get
            {
                return initialized;
            }
        }
        public void To8Bit()
        {
            if (Buffers[0].PixelFormat == PixelFormat.Format48bppRgb)
            {
                To16Bit();
                for (int i = 0; i < Buffers.Count; i++)
                {
                    Bitmap b = AForge.Imaging.Image.Convert16bppTo8bpp((Bitmap)Buffers[i].Image);
                    Buffers[i].Image = b;
                    Statistics.CalcStatistics(Buffers[i]);
                }
            }
            else if (Buffers[0].PixelFormat == PixelFormat.Format24bppRgb)
            {
                sizeC = 3;
                List<BufferInfo> bfs = new List<BufferInfo>();
                int index = 0;
                for (int i = 0; i < Buffers.Count; i++)
                {
                    Bitmap[] bs = BufferInfo.RGB24To8(GetFiltered(i, RChannel.range, GChannel.range, BChannel.range));
                    BufferInfo br = new BufferInfo(ID, bs[2], new ZCT(Buffers[i].Coordinate.Z, 0, Buffers[i].Coordinate.T), index);
                    BufferInfo bg = new BufferInfo(ID, bs[1], new ZCT(Buffers[i].Coordinate.Z, 1, Buffers[i].Coordinate.T), index + 1);
                    BufferInfo bb = new BufferInfo(ID, bs[0], new ZCT(Buffers[i].Coordinate.Z, 2, Buffers[i].Coordinate.T), index + 2);
                    bfs.Add(br);
                    bfs.Add(bg);
                    bfs.Add(bb);
                    index += 3;
                }
                Buffers = bfs;
                UpdateCoords();
            }
            else
                for (int i = 0; i < Buffers.Count; i++)
                {
                    Bitmap b = GetFiltered(i, RChannel.range, GChannel.range, BChannel.range);
                    b = AForge.Imaging.Image.Convert16bppTo8bpp(b);
                    Buffers[i].Image = b;
                    Statistics.CalcStatistics(Buffers[i]);
                }
            foreach (Channel c in Channels)
            {
                c.Min = (int)((((float)c.Min / (float)ushort.MaxValue)) * 255);
                c.Max = (int)((((float)c.Max / (float)ushort.MaxValue)) * 255);
                c.BitsPerPixel = 8;
            }
            bitsPerPixel = 8;
            AutoThreshold(this, true);
            Recorder.AddLine("Bio.Table.GetImage(" + '"' + ID + '"' + ")" + "." + "To8Bit();");
        }
        public void To16Bit()
        {
            foreach (Channel c in Channels)
            {
                c.Min = 0;
                c.Max = ushort.MaxValue;
                c.BitsPerPixel = 16;
            }
            if (Buffers[0].PixelFormat == PixelFormat.Format48bppRgb)
            {
                sizeC = 3;
                List<BufferInfo> bfs = new List<BufferInfo>();
                int index = 0;
                for (int i = 0; i < Buffers.Count; i++)
                {
                    BufferInfo[] bs = BufferInfo.RGB48To16(ID, SizeX, SizeY, Buffers[i].Stride, Buffers[i].Bytes, Buffers[i].Coordinate, index);
                    bfs.AddRange(bs);
                    index += 3;
                }
                Buffers = bfs;
                UpdateCoords();
            }
            else
                for (int i = 0; i < Buffers.Count; i++)
                {
                    Bitmap b = AForge.Imaging.Image.Convert8bppTo16bpp((Bitmap)Buffers[i].Image);
                    Buffers[i].Image = b;
                    Statistics.CalcStatistics(Buffers[i]);
                }
            bitsPerPixel = 16;
            AutoThreshold(this, true);
            Recorder.AddLine("Bio.Table.GetImage(" + '"' + ID + '"' + ")" + "." + "To16Bit();");
        }
        public void To24Bit()
        {
            if (Buffers[0].PixelFormat == PixelFormat.Format48bppRgb)
            {
                //We run 8bit so we get 24 bit rgb.
                for (int i = 0; i < Buffers.Count; i++)
                {
                    Bitmap b = AForge.Imaging.Image.Convert16bppTo8bpp((Bitmap)Buffers[i].Image);
                    Buffers[i].Image = b;
                    Statistics.CalcStatistics(Buffers[i]);
                }
            }
            else
            if (Buffers[0].PixelFormat == PixelFormat.Format16bppGrayScale)
            {
                To8Bit();
                int index = 0;
                List<BufferInfo> buffers = new List<BufferInfo>();
                for (int i = 0; i < Buffers.Count; i += 3)
                {
                    Bitmap b = GetRGBBitmap(i, RChannel.range, GChannel.range, BChannel.range);
                    BufferInfo inf = new BufferInfo(ID, b, Buffers[i].Coordinate, index);
                    inf.SwitchRedBlue();
                    inf.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    buffers.Add(inf);
                    Statistics.CalcStatistics(buffers[index]);
                    index++;
                }
                Buffers.Clear();
                Buffers.AddRange(buffers);
            }
            else
            if (Buffers[0].PixelFormat == PixelFormat.Format32bppArgb)
            {
                for (int i = 0; i < Buffers.Count; i++)
                {
                    Buffers[i].Image = BufferInfo.To24Bit((Bitmap)Buffers[i].Image);
                    Statistics.CalcStatistics(Buffers[i]);
                }
            }
            else
            {
                int index = 0;
                List<BufferInfo> buffers = new List<BufferInfo>();
                for (int i = 0; i < Buffers.Count; i += 3)
                {
                    Bitmap b = GetRGBBitmap(i, RChannel.range, GChannel.range, BChannel.range);
                    BufferInfo inf = new BufferInfo(ID, b, Buffers[i].Coordinate, index);
                    inf.SwitchRedBlue();
                    inf.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    buffers.Add(inf);
                    Statistics.CalcStatistics(buffers[index]);
                    index++;
                }
                Buffers.Clear();
                Buffers.AddRange(buffers);
            }
            bitsPerPixel = 8;
            foreach (Channel c in Channels)
            {
                c.Min = 0;
                c.Max = 255;
                c.BitsPerPixel = 8;
            }
            AutoThreshold(this, true);
            Recorder.AddLine("Bio.Table.GetImage(" + '"' + ID + '"' + ")" + "." + "To24Bit();");
        }
        public void To32Bit()
        {
            if (bitsPerPixel > 8)
                return;
            for (int i = 0; i < Buffers.Count; i++)
            {
                Bitmap b = BufferInfo.To32Bit((Bitmap)Buffers[i].Image);
                Buffers[i].Image = b;
                Statistics.CalcStatistics(Buffers[i]);
                //BufferInfo.AddBuffer(Buffers[i], Buffers.Count);
                //BufferInfo.CalculateStatistics();
            }
            Recorder.AddLine("Bio.Table.GetImage(" + '"' + ID + '"' + ")" + "." + "To32Bit();");
        }
        public void To48Bit()
        {
            if (Buffers[0].PixelFormat == PixelFormat.Format24bppRgb)
            {
                for (int i = 0; i < Buffers.Count; i++)
                {
                    Bitmap b = AForge.Imaging.Image.Convert8bppTo16bpp((Bitmap)Buffers[i].Image);
                    Buffers[i].Image = b;
                    Statistics.CalcStatistics(Buffers[i]);
                }
            }
            else
            {
                int index = 0;
                List<BufferInfo> buffers = new List<BufferInfo>();
                for (int i = 0; i < Buffers.Count; i += 3)
                {
                    Bitmap b = GetRGBBitmap(i, RChannel.range, GChannel.range, BChannel.range);
                    BufferInfo inf = new BufferInfo(ID, b, Buffers[i].Coordinate, index);
                    inf.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    buffers.Add(inf);
                    Statistics.CalcStatistics(buffers[index]);
                    index++;
                }
                Buffers.Clear();
                Buffers.AddRange(buffers);
            }
            bitsPerPixel = 16;
            foreach (Channel c in Channels)
            {
                c.Min = 0;
                c.Max = ushort.MaxValue;
            }
            AutoThreshold(this, true);
            Recorder.AddLine("Bio.Table.GetImage(" + '"' + ID + '"' + ")" + "." + "To48Bit();");
        }
        public void RotateFlip(RotateFlipType rot)
        {
            for (int i = 0; i < Buffers.Count; i++)
            {
                Buffers[i].RotateFlip(rot);
            }
        }
        public void Bake(int rmin, int rmax, int gmin, int gmax, int bmin, int bmax)
        {
            Bake(new IntRange(rmin, rmax), new IntRange(gmin, gmax), new IntRange(bmin, bmax));
        }
        public void Bake(IntRange rf, IntRange gf, IntRange bf)
        {
            BioImage bm = new BioImage(Images.GetImageName(ID));
            bm = CopyInfo(this, true, true);
            for (int i = 0; i < Buffers.Count; i++)
            {
                ZCT co = Buffers[i].Coordinate;
                Bitmap b = GetFiltered(i, rf, gf, bf);
                BufferInfo inf = new BufferInfo(bm.ID, b, co, i);
                bm.Coords[co.Z, co.C, co.T] = i;
                bm.Buffers.Add(inf);
            }
            Images.AddImage(bm);
            Recorder.AddLine("App.Image.Bake(" + rf.Min + "," + rf.Max + "," + gf.Min + "," + gf.Max + "," + bf.Min + "," + bf.Max + ");");
        }
        public void UpdateCoords()
        {
            int z = 0;
            int c = 0;
            int t = 0;
            Coords = new int[SizeZ, SizeC, SizeT];
            for (int im = 0; im < Buffers.Count; im++)
            {
                ZCT co = new ZCT(z, c, t);
                Coords[co.Z, co.C, co.T] = im;
                Buffers[im].Coordinate = co;
                if (c < SizeC - 1)
                    c++;
                else
                {
                    c = 0;
                    if (z < SizeZ - 1)
                        z++;
                    else
                    {
                        z = 0;
                        if (t < SizeT - 1)
                            t++;
                        else
                            t = 0;
                    }
                }
            }
        }
        public System.Drawing.Point ToImageSpace(PointD p)
        {
            System.Drawing.Point pp = new System.Drawing.Point();
            pp.X = (int)((p.X - stageSizeX) / physicalSizeX);
            pp.Y = (int)((p.Y - stageSizeY) / physicalSizeY);
            return pp;
        }
        public System.Drawing.Rectangle ToImageSpace(RectangleD p)
        {
            System.Drawing.Rectangle r = new Rectangle();
            System.Drawing.Point pp = new System.Drawing.Point();
            r.X = (int)((p.X - stageSizeX) / physicalSizeX);
            r.Y = (int)((p.Y - stageSizeY) / physicalSizeY);
            r.Width = (int)(p.W / physicalSizeX);
            r.Height = (int)(p.H / physicalSizeY);
            return r;
        }

        public BioImage(string id)
        {
            ID = Images.GetImageName(id);
            rgbChannels[0] = 0;
            rgbChannels[1] = 0;
            rgbChannels[2] = 0;
        }
        public BioImage(string id, int SizeX, int SizeY)
        {
            ID = Images.GetImageName(id);
            rgbChannels[0] = 0;
            rgbChannels[1] = 0;
            rgbChannels[2] = 0;
            rgbBitmap16 = new Bitmap(SizeX, SizeY, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
            rgbBitmap8 = new Bitmap(SizeX, SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        }
        public static BioImage Substack(BioImage orig, int ser, int zs, int ze, int cs, int ce, int ts, int te)
        {
            BioImage b = CopyInfo(orig, false, true);
            b.ID = Images.GetImageName(orig.ID);
            int i = 0;
            b.Coords = new int[ze - zs, ce - cs, te - ts];
            b.sizeZ = ze - zs;
            b.sizeC = ce - cs;
            b.sizeT = te - ts;
            for (int ti = 0; ti < b.SizeT; ti++)
            {
                for (int zi = 0; zi < b.SizeZ; zi++)
                {
                    for (int ci = 0; ci < b.SizeC; ci++)
                    {
                        int ind = orig.Coords[zs + zi, cs + ci, ts + ti];
                        BufferInfo bf = new BufferInfo(Images.GetImageName(orig.id), orig.SizeX, orig.SizeY, orig.Buffers[0].PixelFormat, orig.Buffers[ind].Bytes, new ZCT(zi, ci, ti), i);
                        if (b.littleEndian)
                            bf.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        Statistics.CalcStatistics(bf);
                        b.Buffers.Add(bf);
                        b.Coords[zi, ci, ti] = i;
                        i++;
                    }
                }
            }
            Images.AddImage(b);
            b.rgbBitmap16 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
            b.rgbBitmap8 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Recorder.AddLine("Bio.BioImage.Substack(" + '"' + orig.Filename + '"' + "," + ser + "," + zs + "," + ze + "," + cs + "," + ce + "," + ts + "," + te + ");");
            return b;
        }
        public static BioImage MergeChannels(BioImage b2, BioImage b)
        {
            BioImage res = new BioImage(b2.ID, b2.SizeX, b2.SizeY);
            res.ID = Images.GetImageName(b2.ID);
            res.series = b2.series;
            res.sizeZ = b2.SizeZ;
            int cOrig = b2.SizeC;
            res.sizeC = b2.SizeC + b.SizeC;
            res.sizeT = b2.SizeT;
            res.bitsPerPixel = b2.bitsPerPixel;
            res.imageInfo = b2.imageInfo;
            res.littleEndian = b2.littleEndian;
            res.seriesCount = b2.seriesCount;
            res.imagesPerSeries = res.ImageCount / res.seriesCount;
            res.Coords = new int[res.SizeZ, res.SizeC, res.SizeT];

            int i = 0;
            int cc = 0;
            for (int ti = 0; ti < res.SizeT; ti++)
            {
                for (int zi = 0; zi < res.SizeZ; zi++)
                {
                    for (int ci = 0; ci < res.SizeC; ci++)
                    {
                        ZCT co = new ZCT(zi, ci, ti);
                        if (ci < cOrig)
                        {
                            //If this channel is part of the image b1 we add planes from it.
                            BufferInfo copy = new BufferInfo(b2.id, b2.SizeX, b2.SizeY, b2.Buffers[0].PixelFormat, b2.Buffers[i].Bytes, co, i);
                            if (b2.littleEndian)
                                copy.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            res.Coords[zi, ci, ti] = i;
                            res.Buffers.Add(b2.Buffers[i]);
                            res.Buffers.Add(copy);
                            //Lets copy the ROI's from the original image.
                            List<ROI> anns = b2.GetAnnotations(zi, ci, ti);
                            if (anns.Count > 0)
                                res.Annotations.AddRange(anns);
                        }
                        else
                        {
                            //This plane is not part of b1 so we add the planes from b2 channels.
                            BufferInfo copy = new BufferInfo(b.id, b.SizeX, b.SizeY, b.Buffers[0].PixelFormat, b.Buffers[i].Bytes, co, i);
                            if (b2.littleEndian)
                                copy.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            res.Coords[zi, ci, ti] = i;
                            res.Buffers.Add(b.Buffers[i]);
                            res.Buffers.Add(copy);

                            //Lets copy the ROI's from the original image.
                            List<ROI> anns = b.GetAnnotations(zi, cc, ti);
                            if (anns.Count > 0)
                                res.Annotations.AddRange(anns);
                        }
                        i++;
                    }
                }
            }
            for (int ci = 0; ci < res.SizeC; ci++)
            {
                if (ci < cOrig)
                {
                    res.Channels.Add(b2.Channels[ci].Copy());
                }
                else
                {
                    res.Channels.Add(b.Channels[cc].Copy());
                    res.Channels[cOrig + cc].Index = ci;
                    cc++;
                }
            }
            Images.AddImage(res);
            Recorder.AddLine("Bio.BioImage.MergeChannels(" + '"' + b.ID + '"' + "," + '"' + b2.ID + '"' + ");");
            return res;
        }
        public static BioImage MergeChannels(string bname, string b2name)
        {
            BioImage b = Images.GetImage(bname);
            BioImage b2 = Images.GetImage(b2name);
            return MergeChannels(b, b2);
        }
        public BioImage[] SplitChannels()
        {
            BioImage[] bms;
            if (isRGB)
            {
                bms = new BioImage[3];
                BioImage ri = new BioImage(Path.GetFileNameWithoutExtension(ID) + "-1" + Path.GetExtension(ID));
                BioImage gi = new BioImage(Path.GetFileNameWithoutExtension(ID) + "-2" + Path.GetExtension(ID));
                BioImage bi = new BioImage(Path.GetFileNameWithoutExtension(ID) + "-3" + Path.GetExtension(ID));

                ri.sizeC = 1;
                gi.sizeC = 1;
                bi.sizeC = 1;
                ri.sizeZ = SizeZ;
                gi.sizeZ = SizeZ;
                bi.sizeZ = SizeZ;
                ri.sizeT = SizeT;
                gi.sizeT = SizeT;
                bi.sizeT = SizeT;

                ri.Coords = new int[SizeZ, 1, SizeT];
                gi.Coords = new int[SizeZ, 1, SizeT];
                bi.Coords = new int[SizeZ, 1, SizeT];
                int ind = 0;
                for (int i = 0; i < ImageCount; i++)
                {
                    if (Buffers[i].PixelFormat == PixelFormat.Format48bppRgb)
                    {
                        //For 48bit images we need to use our own function as AForge won't give us a proper image.
                        BufferInfo[] bfs = BufferInfo.RGB48To16(ID, SizeX, SizeY, Buffers[i].Stride, Buffers[i].Bytes, Buffers[i].Coordinate, ind);
                        ind += 3;
                        ri.Buffers.Add(bfs[0]);
                        gi.Buffers.Add(bfs[1]);
                        bi.Buffers.Add(bfs[2]);
                        Statistics.CalcStatistics(bfs[0]);
                        Statistics.CalcStatistics(bfs[1]);
                        Statistics.CalcStatistics(bfs[2]);
                        ri.Coords[Buffers[i].Coordinate.Z, Buffers[i].Coordinate.C, Buffers[i].Coordinate.T] = i;
                        gi.Coords[Buffers[i].Coordinate.Z, Buffers[i].Coordinate.C, Buffers[i].Coordinate.T] = i;
                        bi.Coords[Buffers[i].Coordinate.Z, Buffers[i].Coordinate.C, Buffers[i].Coordinate.T] = i;
                    }
                    else
                    {

                        Bitmap rImage = extractR.Apply((Bitmap)Buffers[i].Image);
                        BufferInfo rbf = new BufferInfo(ri.ID, rImage, Buffers[i].Coordinate, ind++);
                        Statistics.CalcStatistics(rbf);
                        ri.Buffers.Add(rbf);
                        ri.Coords[Buffers[i].Coordinate.Z, Buffers[i].Coordinate.C, Buffers[i].Coordinate.T] = i;

                        Bitmap gImage = extractG.Apply((Bitmap)Buffers[i].Image);
                        BufferInfo gbf = new BufferInfo(gi.ID, gImage, Buffers[i].Coordinate, ind++);
                        Statistics.CalcStatistics(gbf);
                        gi.Buffers.Add(gbf);
                        gi.Coords[Buffers[i].Coordinate.Z, Buffers[i].Coordinate.C, Buffers[i].Coordinate.T] = i;

                        Bitmap bImage = extractB.Apply((Bitmap)Buffers[i].Image);
                        //Clipboard.SetImage(bImage);
                        BufferInfo bbf = new BufferInfo(bi.ID, bImage, Buffers[i].Coordinate, ind++);
                        Statistics.CalcStatistics(bbf);
                        bi.Buffers.Add(bbf);
                        bi.Coords[Buffers[i].Coordinate.Z, Buffers[i].Coordinate.C, Buffers[i].Coordinate.T] = i;

                    }
                }
                ri.Channels.Add(Channels[0].Copy());
                gi.Channels.Add(Channels[0].Copy());
                bi.Channels.Add(Channels[0].Copy());
                AutoThreshold(ri, false);
                AutoThreshold(gi, false);
                AutoThreshold(bi, false);
                Images.AddImage(ri);
                Images.AddImage(gi);
                Images.AddImage(bi);
                Statistics.ClearCalcBuffer();
                bms[0] = ri;
                bms[1] = gi;
                bms[2] = bi;
            }
            else
            {
                bms = new BioImage[SizeC];
                for (int c = 0; c < SizeC; c++)
                {
                    BioImage b = BioImage.Substack(this, 0, 0, SizeZ, c, c + 1, 0, SizeT);
                    bms[c] = b;
                }
            }
            Recorder.AddLine("Bio.BioImage.SplitChannels(" + '"' + Filename + '"' + ");");
            return bms;
        }
        public static BioImage[] SplitChannels(BioImage bb)
        {
            return bb.SplitChannels();
        }
        public static BioImage[] SplitChannels(string name)
        {
            return SplitChannels(Images.GetImage(name));
        }

        public static LevelsLinear filter8 = new LevelsLinear();
        public static LevelsLinear16bpp filter16 = new LevelsLinear16bpp();
        private ReplaceChannel replaceRFilter;
        private ReplaceChannel replaceGFilter;
        private ReplaceChannel replaceBFilter;
        private static ExtractChannel extractR = new ExtractChannel(AForge.Imaging.RGB.R);
        private static ExtractChannel extractG = new ExtractChannel(AForge.Imaging.RGB.G);
        private static ExtractChannel extractB = new ExtractChannel(AForge.Imaging.RGB.B);

        public Image GetImageByCoord(int z, int c, int t)
        {
            return Buffers[Coords[z, c, t]].Image;
        }
        public Bitmap GetBitmap(int z, int c, int t)
        {
            return (Bitmap)Buffers[Coords[z, c, t]].Image;
        }
        public int GetIndex(int ix, int iy)
        {
            if (ix > SizeX || iy > SizeY || ix < 0 || iy < 0)
                return 0;
            int stridex = SizeX;
            int x = ix;
            int y = iy;
            if (bitsPerPixel > 8)
            {
                return (y * stridex + x) * 2;
            }
            else
            {
                return (y * stridex + x);
            }
        }
        public int GetIndexRGB(int ix, int iy, int index)
        {
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = ix;
            int y = iy;
            if (bitsPerPixel > 8)
            {
                return (y * stridex + x) * 2 * index;
            }
            else
            {
                return (y * stridex + x) * index;
            }
        }
        public ushort GetValue(ZCTXY coord)
        {
            if (coord.X < 0 || coord.Y < 0 || coord.X > SizeX || coord.Y > SizeY)
            {
                return 0;
            }
            if (isRGB)
            {
                if (coord.C == 0)
                    return GetValueRGB(coord, 0);
                else if (coord.C == 1)
                    return GetValueRGB(coord, 1);
                else if (coord.C == 2)
                    return GetValueRGB(coord, 2);
            }
            else
                return GetValueRGB(coord, 0);
            return 0;
        }
        public ushort GetValueRGB(ZCTXY coord, int index)
        {
            if (coord.X > SizeX || coord.Y > SizeY || coord.X < 0 || coord.Y < 0)
                return 0;
            int i = -1;
            int ind;
            byte[] bytes;
            if (isRGB)
            {
                ind = Coords[coord.Z, 0, coord.T];
                bytes = Buffers[Coords[coord.Z, 0, coord.T]].Bytes;
            }
            else
            {
                bytes = Buffers[Coords[coord.Z, coord.C, coord.T]].Bytes;
                ind = Coords[coord.Z, coord.C, coord.T];
            }
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = coord.X;
            int y = coord.Y;
            if (bitsPerPixel > 8)
            {
                int index2 = (y * stridex + x) * 2 * index;
                i = BitConverter.ToUInt16(bytes, index2);
                return (ushort)i;
            }
            else
            {
                int stride = SizeX;
                System.Drawing.Color c = ((Bitmap)Buffers[ind].Image).GetPixel(x, y);
                if (index == 0)
                    return c.R;
                else
                if (index == 1)
                    return c.G;
                else
                if (index == 2)
                    return c.B;
                else
                    return c.A;
            }
        }
        public ushort GetValue(ZCT coord, int ix, int iy)
        {
            if (ix > SizeX || iy > SizeY)
                return 0;
            int ind = Coords[coord.Z, coord.C, coord.T];
            byte[] bytes = Buffers[ind].Bytes;
            int i = 0;
            int stridex = SizeX;

            int x = ix;
            int y = iy;
            if (ix < 0)
                x = 0;
            if (iy < 0)
                y = 0;
            if (ix >= SizeX)
                x = SizeX - 1;
            if (iy >= SizeY)
                y = SizeY - 1;
            if (bitsPerPixel > 8)
            {
                //For 16bit (2*8bit) images we multiply buffer index by 2
                int index2 = (y * stridex + x) * 2 * RGBChannelCount;
                i = BitConverter.ToUInt16(bytes, index2);
                return (ushort)i;
            }
            else
            {
                int index2 = (y * stridex + x) * RGBChannelCount;
                return bytes[index2];
            }
        }
        public ushort GetValue(int z, int c, int t, int x, int y)
        {
            return GetValue(new ZCTXY(z, c, t, x, y));
        }
        public ushort GetValueRGB(ZCT coord, int x, int y, int RGBindex)
        {
            ZCTXY c = new ZCTXY(coord.Z, coord.C, coord.T, x, y);
            if (isRGB)
            {
                return GetValueRGB(c, RGBindex);
            }
            else
                return GetValue(coord, x, y);
        }
        public ushort GetValueRGB(int z, int c, int t, int x, int y, int RGBindex)
        {
            return GetValueRGB(new ZCT(z, c, t), x, y, RGBindex);
        }
        public void SetValue(ZCTXY coord, ushort value)
        {
            int i = Coords[coord.Z, coord.C, coord.T];
            Buffers[i].SetValue(coord.X, coord.Y, value);
        }
        public void SetValue(int x, int y, int ind, ushort value)
        {
            Buffers[ind].SetValue(x, y, value);
        }
        public void SetValue(int x, int y, ZCT coord, ushort value)
        {
            SetValue(x, y, Coords[coord.Z, coord.C, coord.T], value);
        }
        public void SetValueRGB(ZCTXY coord, int RGBindex, ushort value)
        {
            int i = -1;
            int ind = Coords[coord.Z, coord.C, coord.T];
            Buffers[ind].SetValueRGB(coord.X, coord.Y, RGBindex, value);
        }
        public Bitmap GetBitmap(ZCT coord)
        {
            return (Bitmap)GetImageByCoord(coord.Z, coord.C, coord.T);
        }
        public Bitmap GetFiltered(ZCT coord, IntRange r, IntRange g, IntRange b)
        {
            int index = Coords[coord.Z, coord.C, coord.T];
            return GetFiltered(index, r, g, b);
        }
        public Bitmap GetFiltered(int ind, IntRange r, IntRange g, IntRange b)
        {
            if (Buffers[ind].BitsPerPixel > 8)
            {
                if (RGBChannelCount == 3)
                {
                    BioImage.filter16.InRed = r;
                    BioImage.filter16.InGreen = g;
                    BioImage.filter16.InBlue = b;
                }
                else
                {
                    BioImage.filter16.InRed = r;
                    BioImage.filter16.InGreen = r;
                    BioImage.filter16.InBlue = r;
                }
                return BioImage.filter16.Apply((Bitmap)Buffers[ind].Image);
            }
            else
            {
                // set ranges
                BioImage.filter8.InRed = r;
                BioImage.filter8.InGreen = g;
                BioImage.filter8.InBlue = b;
                return BioImage.filter8.Apply((Bitmap)Buffers[ind].Image);
            }
        }
        public Bitmap GetChannelImage(int ind, RGB rGB)
        {
            BufferInfo bf = Buffers[ind];
            if (bf.isRGB)
            {
                if (rGB == RGB.R)
                    return extractR.Apply((Bitmap)Buffers[ind].Image);
                else
                if (rGB == RGB.G)
                    return extractG.Apply((Bitmap)Buffers[ind].Image);
                else
                    return extractB.Apply((Bitmap)Buffers[ind].Image);
            }
            else
                throw new InvalidOperationException();
        }
        public Bitmap GetRGBBitmap(int index, IntRange rf, IntRange gf, IntRange bf)
        {
            if (bitsPerPixel > 8)
                return GetRGBBitmap16(index, rf, gf, bf);
            else
                return GetRGBBitmap8(index);
        }
        public Bitmap GetRGBBitmap(int index, IntRange rf, IntRange gf, IntRange bf, RGB rgb)
        {
            if (bitsPerPixel > 8)
                return GetRGBBitmap16(index, rf, gf, bf);
            else
                return GetRGBBitmap8(index);
        }
        public Bitmap GetRGBBitmap(ZCT coord, IntRange rf, IntRange gf, IntRange bf)
        {
            int index = Coords[coord.Z, coord.C, coord.T];
            if (bitsPerPixel > 8)
                return GetRGBBitmap16(index, rf, gf, bf);
            else
                return GetRGBBitmap8(index);
        }
        public Bitmap GetRGBBitmap16(int ri, IntRange rf, IntRange gf, IntRange bf)
        {
            watch.Restart();
            if (replaceRFilter == null || replaceGFilter == null || replaceBFilter == null)
            {
                replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, GetFiltered(ri + RChannel.Index, rf, gf, bf));
                replaceGFilter = new ReplaceChannel(AForge.Imaging.RGB.G, GetFiltered(ri + GChannel.Index, rf, gf, bf));
                replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, GetFiltered(ri + BChannel.Index, rf, gf, bf));
            }
            if (rgbBitmap16 == null)
                rgbBitmap16 = new Bitmap(SizeX, SizeY, PixelFormat.Format48bppRgb);
            if (rgbBitmap16.Width != SizeX || rgbBitmap16.Height != SizeY)
                rgbBitmap16 = new Bitmap(SizeX, SizeY, PixelFormat.Format48bppRgb);
            if (RGBChannelCount == 1)
            {
                replaceRFilter.ChannelImage = GetFiltered(ri + RChannel.Index, rf, gf, bf);
                replaceRFilter.ApplyInPlace(rgbBitmap16);
                replaceRFilter.ChannelImage.Dispose();
                replaceGFilter.ChannelImage = GetFiltered(ri + GChannel.Index, gf, gf, bf);
                replaceGFilter.ApplyInPlace(rgbBitmap16);
                replaceGFilter.ChannelImage.Dispose();
                replaceBFilter.ChannelImage = GetFiltered(ri + BChannel.Index, bf, gf, bf);
                replaceBFilter.ApplyInPlace(rgbBitmap16);
                replaceBFilter.ChannelImage.Dispose();
            }
            else
            {
                rgbBitmap16 = (Bitmap)Buffers[ri].Image;
            }
            watch.Stop();
            loadTimeMS = watch.ElapsedMilliseconds;
            loadTimeTicks = watch.ElapsedTicks;
            return rgbBitmap16;
        }
        public Bitmap GetRGBBitmap8(int ri)
        {
            watch.Restart();
            if (rgbBitmap8 == null)
                rgbBitmap8 = new Bitmap(SizeX, SizeY, PixelFormat.Format24bppRgb);
            if (rgbBitmap8.Width != SizeX || rgbBitmap8.Height != SizeY)
                rgbBitmap8 = new Bitmap(SizeX, SizeY, PixelFormat.Format24bppRgb);
            if (RGBChannelCount == 1)
            {
                if (replaceRFilter == null || replaceGFilter == null || replaceBFilter == null)
                {
                    replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, (Bitmap)Buffers[ri + RChannel.Index].Image);
                    replaceGFilter = new ReplaceChannel(AForge.Imaging.RGB.G, (Bitmap)Buffers[ri + GChannel.Index].Image);
                    replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, (Bitmap)Buffers[ri + BChannel.Index].Image);
                }
                replaceRFilter.ChannelImage = (Bitmap)Buffers[ri + RChannel.Index].Image;
                replaceRFilter.ApplyInPlace(rgbBitmap8);

                replaceGFilter.ChannelImage = (Bitmap)Buffers[ri + GChannel.Index].Image;
                replaceGFilter.ApplyInPlace(rgbBitmap8);

                replaceBFilter.ChannelImage = (Bitmap)Buffers[ri + BChannel.Index].Image;
                replaceBFilter.ApplyInPlace(rgbBitmap8);

            }
            else
            {
                rgbBitmap8 = (Bitmap)Buffers[ri].Image;
            }
            watch.Stop();
            loadTimeMS = watch.ElapsedMilliseconds;
            loadTimeTicks = watch.ElapsedTicks;
            return rgbBitmap8;
        }

        public static Stopwatch swatch = new Stopwatch();
        public List<ROI> GetAnnotations(ZCT coord)
        {
            List<ROI> annotations = new List<ROI>();
            foreach (ROI an in Annotations)
            {
                if (an == null)
                    continue;
                if (an.coord == coord)
                    annotations.Add(an);
            }
            return annotations;
        }
        public List<ROI> GetAnnotations(int Z, int C, int T)
        {
            List<ROI> annotations = new List<ROI>();
            foreach (ROI an in Annotations)
            {
                if (an.coord.Z == Z && an.coord.Z == Z && an.coord.C == C && an.coord.T == T)
                    annotations.Add(an);
            }
            return annotations;
        }

        private static ImageWriter wr;
        private static int serie;
        public bool Loading = false;
        public static void Initialize()
        {
            //We initialize OME on a seperate thread so the user doesn't have to wait for initialization to
            //view images. 
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(InitOME));
            t.Start();
        }
        private static void InitOME()
        {
            factory = new ServiceFactory();
            service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            reader = new ImageReader();
            writer = new ImageWriter();
            initialized = true;
        }
        public static void SaveFile(string file, string ID)
        {
            Progress pr = new Progress(file, "Saving");
            pr.Show();
            BioImage b = Images.GetImage(ID);
            string fn = Path.GetFileNameWithoutExtension(file);
            string dir = Path.GetDirectoryName(file);
            //Save ROIs to CSV file.
            if (b.Annotations.Count > 0)
            {
                string f = dir + "//" + fn + ".csv";
                ExportROIsCSV(f, b.Annotations);
            }
            ImageJDesc j = new ImageJDesc();
            j.FromImage(b);
            string desc = j.GetString();
            //Embed ROI's to image description.
            for (int i = 0; i < b.Annotations.Count; i++)
            {
                desc += "-ROI:" + b.series + ":" + ROIToString(b.Annotations[i]) + NewLine;
            }
            foreach (Channel c in b.Channels)
            {
                string cj = JsonConvert.SerializeObject(c.info, Formatting.None);
                desc += "-Channel:" + b.series + ":" + cj + NewLine;
            }
            string json = JsonConvert.SerializeObject(b.imageInfo, Formatting.None);
            desc += "-ImageInfo:" + b.series + ":" + json + NewLine;

            Tiff image = Tiff.Open(file, "w");
            int stride = b.Buffers[0].Stride;
            int im = 0;
            int sizec = 1;
            if (!b.isRGB)
            {
                sizec = b.SizeC;
            }
            byte[] buffer;
            for (int c = 0; c < sizec; c++)
            {
                for (int z = 0; z < b.SizeZ; z++)
                {
                    for (int t = 0; t < b.SizeT; t++)
                    {
                        image.SetDirectory((short)im);
                        image.SetField(TiffTag.IMAGEWIDTH, b.SizeX);
                        image.SetField(TiffTag.IMAGEDESCRIPTION, desc);
                        image.SetField(TiffTag.IMAGELENGTH, b.SizeY);
                        image.SetField(TiffTag.BITSPERSAMPLE, b.bitsPerPixel);
                        image.SetField(TiffTag.SAMPLESPERPIXEL, b.RGBChannelCount);
                        image.SetField(TiffTag.ROWSPERSTRIP, b.SizeY);
                        /*
                        if (im % 2 == 0)
                            image.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                        else
                            image.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISWHITE);
                        */
                        image.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        image.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        image.SetField(TiffTag.ROWSPERSTRIP, image.DefaultStripSize(0));
                        if (b.physicalSizeX != -1 && b.physicalSizeY != -1)
                        {
                            image.SetField(TiffTag.XRESOLUTION, (b.physicalSizeX * b.SizeX) / ((b.physicalSizeX * b.SizeX) * b.physicalSizeX));
                            image.SetField(TiffTag.YRESOLUTION, (b.physicalSizeY * b.SizeY) / ((b.physicalSizeY * b.SizeY) * b.physicalSizeY));
                            image.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.NONE);
                        }
                        else
                        {
                            image.SetField(TiffTag.XRESOLUTION, 100.0);
                            image.SetField(TiffTag.YRESOLUTION, 100.0);
                            image.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                        }
                        // specify that it's a page within the multipage file
                        image.SetField(TiffTag.SUBFILETYPE, FileType.PAGE);
                        // specify the page number
                        buffer = b.Buffers[im].GetSaveBytes(false);
                        image.SetField(TiffTag.PAGENUMBER, c, b.Buffers.Count);
                        for (int i = 0, offset = 0; i < b.SizeY; i++)
                        {
                            image.WriteScanline(buffer, offset, i, 0);
                            offset += stride;
                        }
                        image.WriteDirectory();
                        pr.UpdateProgressF((float)im / (float)b.ImageCount);
                        Application.DoEvents();
                        im++;
                    }
                }
            }
            //buffer = null;
            image.Dispose();
            Recorder.AddLine("Bio.BioImage.Save(" + '"' + file + '"' + "," + '"' + ID + '"' + ");");
            pr.Close();
        }
        public static void SaveSeries(string[] files, string ID)
        {
            string desc = "";
            int stride = 0;
            ImageJDesc j = new ImageJDesc();
            BioImage bi = Images.GetImage(files[0]);
            j.FromImage(bi);
            desc = j.GetString();
            for (int fi = 0; fi < files.Length; fi++)
            {
                string file = files[fi];

                BioImage b = Images.GetImage(file);
                string fn = Path.GetFileNameWithoutExtension(ID);
                string dir = Path.GetDirectoryName(ID);
                stride = b.Buffers[0].Stride;

                //Save ROIs to CSV file.
                if (b.Annotations.Count > 0)
                {
                    string f = dir + "//" + fn + ".csv";
                    ExportROIsCSV(f, b.Annotations);
                }

                //Embed ROI's to image description.
                for (int i = 0; i < b.Annotations.Count; i++)
                {
                    desc += "-ROI:" + b.series + ":" + ROIToString(b.Annotations[i]) + NewLine;
                }
                foreach (Channel c in b.Channels)
                {
                    string cj = JsonConvert.SerializeObject(c.info, Formatting.None);
                    desc += "-Channel:" + fi + ":" + cj + NewLine;
                }
                string json = JsonConvert.SerializeObject(b.imageInfo, Formatting.None);
                desc += "-ImageInfo:" + fi + ":" + json + NewLine;
            }
            Tiff image = Tiff.Open(ID, "w");

            for (int fi = 0; fi < files.Length; fi++)
            {
                int im = 0;
                string file = files[fi];
                Progress pr = new Progress(file, "Saving");
                pr.Show();
                Application.DoEvents();
                BioImage b = Images.GetImage(file);
                int sizec = 1;
                if (!b.isRGB)
                {
                    sizec = b.SizeC;
                }
                byte[] buffer;
                for (int c = 0; c < sizec; c++)
                {
                    for (int z = 0; z < b.SizeZ; z++)
                    {
                        for (int t = 0; t < b.SizeT; t++)
                        {
                            image.SetDirectory((short)(c + (b.Buffers.Count * fi)));
                            image.SetField(TiffTag.IMAGEWIDTH, b.SizeX);
                            image.SetField(TiffTag.IMAGEDESCRIPTION, desc);
                            image.SetField(TiffTag.IMAGELENGTH, b.SizeY);
                            image.SetField(TiffTag.BITSPERSAMPLE, b.bitsPerPixel);
                            image.SetField(TiffTag.SAMPLESPERPIXEL, b.RGBChannelCount);
                            image.SetField(TiffTag.ROWSPERSTRIP, b.SizeY);
                            image.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                            image.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                            image.SetField(TiffTag.ROWSPERSTRIP, image.DefaultStripSize(0));
                            if (b.physicalSizeX != -1 && b.physicalSizeY != -1)
                            {
                                image.SetField(TiffTag.XRESOLUTION, (b.physicalSizeX * b.SizeX) / ((b.physicalSizeX * b.SizeX) * b.physicalSizeX));
                                image.SetField(TiffTag.YRESOLUTION, (b.physicalSizeY * b.SizeY) / ((b.physicalSizeY * b.SizeY) * b.physicalSizeY));
                                image.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.NONE);
                            }
                            else
                            {
                                image.SetField(TiffTag.XRESOLUTION, 100.0);
                                image.SetField(TiffTag.YRESOLUTION, 100.0);
                                image.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                            }
                            // specify that it's a page within the multipage file
                            image.SetField(TiffTag.SUBFILETYPE, FileType.PAGE);
                            // specify the page number
                            buffer = b.Buffers[im].GetSaveBytes(false);
                            image.SetField(TiffTag.PAGENUMBER, c + (b.Buffers.Count * fi), b.Buffers.Count * files.Length);
                            for (int i = 0, offset = 0; i < b.SizeY; i++)
                            {
                                image.WriteScanline(buffer, offset, i, 0);
                                offset += stride;
                            }
                            image.WriteDirectory();
                            pr.UpdateProgressF((float)im / (float)b.ImageCount);
                            Application.DoEvents();
                            im++;
                        }
                    }
                }
                pr.Close();
            }
            //buffer = null;
            image.Dispose();
        }
        public static BioImage[] OpenSeries(string file)
        {
            Tiff image = Tiff.Open(file, "r");
            int pages = image.NumberOfDirectories();
            FieldValue[] f = image.GetField(TiffTag.IMAGEDESCRIPTION);
            int sp = image.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();
            ImageJDesc imDesc = new ImageJDesc();
            int count = 1;
            if (f != null)
            {
                string desc = f[0].ToString();
                if (desc.StartsWith("ImageJ"))
                {
                    imDesc.SetString(desc);
                    if (imDesc.channels != 0)
                        count = imDesc.channels;
                }
            }
            int scount = (pages * sp) / count;
            BioImage[] bs = new BioImage[pages];
            image.Close();
            for (int i = 0; i < pages; i++)
            {
                bs[i] = OpenFile(file, i);
            }
            return bs;
        }
        public static BioImage OpenFile(string file)
        {
            return OpenFile(file, 0);
        }
        public static BioImage OpenFile(string file, int series)
        {
            if (file.EndsWith("ome.tif") || file.EndsWith("OME.TIF"))
            {
                return OpenOME(file);
            }
            if (!((file.EndsWith("tif") || file.EndsWith("tiff") || file.EndsWith("TIF") || file.EndsWith("TIFF") ||
                file.EndsWith("png") || file.EndsWith("PNG") || file.EndsWith("jpg") || file.EndsWith("JPG") ||
                file.EndsWith("jpeg") || file.EndsWith("JPEG") || file.EndsWith("bmp") || file.EndsWith("BMP"))))
            {
                return OpenOME(file);
            }
            Stopwatch st = new Stopwatch();
            st.Start();
            Progress pr = new Progress(file, "Opening");
            pr.Show();
            Application.DoEvents();
            BioImage b = new BioImage(file);
            b.series = 0;
            string fn = Path.GetFileNameWithoutExtension(file);
            string dir = Path.GetDirectoryName(file);
            if (File.Exists(fn + ".csv"))
            {
                string f = dir + "//" + fn + ".csv";
                b.Annotations = BioImage.ImportROIsCSV(f);
            }
            if (file.EndsWith("tif") || file.EndsWith("tiff") || file.EndsWith("TIF") || file.EndsWith("TIFF"))
            {
                Tiff image = Tiff.Open(file, "r");
                int SizeX = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int SizeY = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                b.bitsPerPixel = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                b.littleEndian = image.IsBigEndian();
                int RGBChannelCount = image.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();
                string desc = "";

                FieldValue[] f = image.GetField(TiffTag.IMAGEDESCRIPTION);
                ImageJDesc imDesc = new ImageJDesc();
                b.sizeC = 1;
                b.sizeT = 1;
                b.sizeZ = 1;
                int count = 0;
                if (f != null)
                {
                    desc = f[0].ToString();
                    if (desc.StartsWith("ImageJ"))
                    {
                        imDesc.SetString(desc);
                        if (imDesc.channels != 0)
                            b.sizeC = imDesc.channels;
                        else
                            b.sizeC = 1;
                        if (imDesc.slices != 0)
                            b.sizeZ = imDesc.slices;
                        else
                            b.sizeZ = 1;
                        if (imDesc.frames != 0)
                            b.sizeT = imDesc.frames;
                        else
                            b.sizeT = 1;
                        if (imDesc.finterval != 0)
                            b.frameInterval = imDesc.finterval;
                        else
                            b.frameInterval = 1;
                        if (imDesc.spacing != 0)
                            b.physicalSizeZ = imDesc.spacing;
                        else
                            b.physicalSizeZ = 1;
                    }
                }
                int stride = 0;
                PixelFormat PixelFormat;
                if (RGBChannelCount == 1)
                {
                    if (b.bitsPerPixel > 8)
                    {
                        PixelFormat = PixelFormat.Format16bppGrayScale;
                        stride = SizeX * 2;
                    }
                    else
                    {
                        PixelFormat = PixelFormat.Format8bppIndexed;
                        stride = SizeX;
                    }
                }
                else
                if (RGBChannelCount == 3)
                {
                    b.sizeC = 1;
                    if (b.bitsPerPixel > 8)
                    {
                        PixelFormat = PixelFormat.Format48bppRgb;
                        stride = SizeX * 2 * 3;
                    }
                    else
                    {
                        PixelFormat = PixelFormat.Format24bppRgb;
                        stride = SizeX * 3;
                    }
                }
                else
                {
                    PixelFormat = PixelFormat.Format32bppArgb;
                    stride = SizeX * 4;
                }
                string unit = (string)image.GetField(TiffTag.RESOLUTIONUNIT)[0].ToString();
                if (unit == "CENTIMETER")
                {
                    if (image.GetField(TiffTag.XRESOLUTION) != null)
                        b.physicalSizeX = image.GetField(TiffTag.XRESOLUTION)[0].ToDouble() / 1000;
                    if (image.GetField(TiffTag.YRESOLUTION) != null)
                        b.physicalSizeY = image.GetField(TiffTag.YRESOLUTION)[0].ToDouble() / 1000;
                }
                else
                if (unit == "INCH")
                {
                    //inch to centimeter
                    if (image.GetField(TiffTag.XRESOLUTION) != null)
                        b.physicalSizeX = (2.54 / image.GetField(TiffTag.XRESOLUTION)[0].ToDouble()) / 1000;
                    if (image.GetField(TiffTag.YRESOLUTION) != null)
                        b.physicalSizeY = (2.54 / image.GetField(TiffTag.YRESOLUTION)[0].ToDouble()) / 1000;
                }
                else
                if (unit == "NONE")
                {
                    if (imDesc.unit == "micron")
                    {
                        //size micron
                        if (image.GetField(TiffTag.XRESOLUTION) != null)
                            b.physicalSizeX = (b.SizeX / image.GetField(TiffTag.XRESOLUTION)[0].ToDouble()) / b.SizeX;
                        if (image.GetField(TiffTag.YRESOLUTION) != null)
                            b.physicalSizeY = (b.SizeY / image.GetField(TiffTag.YRESOLUTION)[0].ToDouble()) / b.SizeY;
                    }
                }
                string[] sts = desc.Split('\n');
                int index = 0;
                for (int i = 0; i < sts.Length; i++)
                {
                    if (sts[i].StartsWith("-Channel"))
                    {
                        string val = sts[i].Substring(9);
                        val = val.Substring(0, val.IndexOf(':'));
                        int serie = int.Parse(val);
                        if (serie == series && sts[i].Length > 7)
                        {
                            string cht = sts[i].Substring(sts[i].IndexOf('{'), sts[i].Length - sts[i].IndexOf('{'));
                            Channel ch = new Channel(index, b.bitsPerPixel);
                            ch.info = JsonConvert.DeserializeObject<Channel.ChannelInfo>(cht);
                            b.Channels.Add(ch);
                            index++;
                        }
                    }
                    else
                    if (sts[i].StartsWith("-ROI"))
                    {
                        string val = sts[i].Substring(5);
                        val = val.Substring(0, val.IndexOf(':'));
                        int serie = int.Parse(val);
                        if (serie == series && sts[i].Length > 7)
                        {
                            string ro = sts[i].Substring(sts[i].LastIndexOf(':') + 1, sts[i].Length - (sts[i].LastIndexOf(':') + 1));
                            ROI roi = StringToROI(ro);
                            b.Annotations.Add(roi);
                        }
                    }
                    else
                    if (sts[i].StartsWith("-ImageInfo"))
                    {
                        string val = sts[i].Substring(11);
                        val = val.Substring(0, val.IndexOf(':'));
                        int serie = int.Parse(val);
                        if (serie == series && sts[i].Length > 10)
                        {
                            string cht = sts[i].Substring(sts[i].IndexOf('{'), sts[i].Length - sts[i].IndexOf('{'));
                            b.imageInfo = JsonConvert.DeserializeObject<BioImageInfo>(cht);
                        }
                    }
                }
                b.Coords = new int[b.SizeZ, b.SizeC, b.SizeT];

                //If this is a tiff file not made by Bio we init channels based on BitsPerPixel.
                if (b.Channels.Count == 0)
                    for (int i = 0; i < b.SizeC; i++)
                    {
                        Channel ch = new Channel(i, b.bitsPerPixel);
                        b.Channels.Add(ch);
                    }
                int z = 0;
                int c = 0;
                int t = 0;
                b.Buffers = new List<BufferInfo>();
                int pages = image.NumberOfDirectories();
                //int stride = image.ScanlineSize();
                int str = image.ScanlineSize();
                if (stride != str)
                    throw new InvalidDataException();
                for (int p = series * b.sizeC; p < (series + 1) * b.sizeC; p++)
                {
                    image.SetDirectory((short)p);
                    byte[] bytes = new byte[stride * SizeY];
                    for (int i = 0, offset = 0; i < SizeY; i++)
                    {
                        image.ReadScanline(bytes, offset, i, 0);
                        offset += stride;
                    }
                    if (!b.littleEndian)
                        Array.Reverse(bytes);
                    BufferInfo inf = new BufferInfo(file, SizeX, SizeY, PixelFormat, bytes, new ZCT(0, 0, 0), p);
                    if (!b.littleEndian)
                        inf.SwitchRedBlue();
                    b.Buffers.Add(inf);
                    Statistics.CalcStatistics(inf);
                    //b.Buffers[b.Buffers.Count - 1].Statistics = Statistics.FromBytes(inf);
                    pr.UpdateProgressF((float)((double)p / (double)pages));
                    Application.DoEvents();
                }

                for (int im = 0; im < b.Buffers.Count; im++)
                {
                    ZCT co = new ZCT(z, c, t);
                    int ind = b.Coords[z, c, t];
                    b.Coords[co.Z, co.C, co.T] = im;
                    b.Buffers[im].Coordinate = co;
                    if (c < b.SizeC - 1)
                        c++;
                    else
                    {
                        c = 0;
                        if (z < b.SizeZ - 1)
                            z++;
                        else
                        {
                            z = 0;
                            if (t < b.SizeT - 1)
                                t++;
                            else
                                t = 0;
                        }
                    }
                }
                image.Close();
            }
            else
            {
                b.bitsPerPixel = 8;
                b.littleEndian = BitConverter.IsLittleEndian;
                b.sizeZ = 1;
                b.sizeC = 1;
                b.sizeT = 1;
                BufferInfo inf = null;
                //We use a try block incase the user tried opening a OME file.
                try
                {
                    inf = new BufferInfo(file, Image.FromFile(file), new ZCT(0, 0, 0), 0);
                }
                catch (Exception)
                {
                    b.Dispose();
                    return OpenOME(file);
                }
                b.Buffers.Add(inf);
                Channel ch = new Channel(0, 8);
                b.Channels.Add(ch);
                b.Coords = new int[b.SizeZ, b.SizeC, b.sizeT];
            }
            if (b.stageSizeX == 0)
            {
                b.stageSizeX = 0.04 * b.SizeX;
                b.stageSizeY = 0.04 * b.SizeY;
                b.stageSizeZ = 0.04 * b.SizeY;
                b.physicalSizeX = 0.04;
                b.physicalSizeY = 0.04;
                b.physicalSizeZ = 0.04;
            }
            b.Volume = new VolumeD(new Point3D(b.stageSizeX, b.stageSizeY, b.stageSizeZ), new Point3D(b.physicalSizeX * b.SizeX, b.physicalSizeY * b.SizeY, b.physicalSizeZ * b.SizeZ));
            //We wait for histogram image statistics calculation
            do
            {
            } while (b.Buffers[b.Buffers.Count - 1].Statistics == null);

            Statistics.ClearCalcBuffer();
            AutoThreshold(b, false);
            Recorder.AddLine("Bio.BioImage.Open(" + '"' + file + '"' + ");");
            Images.AddImage(b);

            pr.Close();
            pr.Dispose();
            st.Stop();
            return b;
        }
        public static BioImage SaveOME(string file, string ID)
        {
            Progress pr = new Progress(file, "Saving");
            pr.Show();

            BioImage b = Images.GetImage(ID);
            int series = b.series;
            // create OME-XML metadata store
            loci.formats.meta.IMetadata omexml = service.createOMEXMLMetadata();
            omexml.setImageID("Image:0", series);
            omexml.setPixelsID("Pixels:0", series);
            if (!BitConverter.IsLittleEndian)
                omexml.setPixelsBinDataBigEndian(java.lang.Boolean.TRUE, serie, 0);
            else
                omexml.setPixelsBinDataBigEndian(java.lang.Boolean.FALSE, serie, 0);
            omexml.setPixelsDimensionOrder(ome.xml.model.enums.DimensionOrder.XYCZT, series);
            if (b.bitsPerPixel > 8)
                omexml.setPixelsType(ome.xml.model.enums.PixelType.UINT16, series);
            else
                omexml.setPixelsType(ome.xml.model.enums.PixelType.UINT8, series);
            omexml.setPixelsSizeX(new PositiveInteger(java.lang.Integer.valueOf(b.SizeX)), series);
            omexml.setPixelsSizeY(new PositiveInteger(java.lang.Integer.valueOf(b.SizeY)), series);
            omexml.setPixelsSizeZ(new PositiveInteger(java.lang.Integer.valueOf(b.SizeZ)), series);
            int samples = 1;
            if (b.isRGB)
                samples = 3;
            omexml.setPixelsSizeC(new PositiveInteger(java.lang.Integer.valueOf(b.SizeC * samples)), series);
            omexml.setPixelsSizeT(new PositiveInteger(java.lang.Integer.valueOf(b.SizeT)), series);


            ome.units.quantity.Length px = new ome.units.quantity.Length(java.lang.Double.valueOf(b.physicalSizeX), ome.units.UNITS.MICROMETER);
            omexml.setPixelsPhysicalSizeX(px, series);
            ome.units.quantity.Length py = new ome.units.quantity.Length(java.lang.Double.valueOf(b.physicalSizeY), ome.units.UNITS.MICROMETER);
            omexml.setPixelsPhysicalSizeY(py, series);
            ome.units.quantity.Length pz = new ome.units.quantity.Length(java.lang.Double.valueOf(b.physicalSizeZ), ome.units.UNITS.MICROMETER);
            omexml.setPixelsPhysicalSizeZ(pz, series);

            ome.units.quantity.Length sx = new ome.units.quantity.Length(java.lang.Double.valueOf(b.stageSizeX), ome.units.UNITS.MICROMETER);
            omexml.setStageLabelX(sx, series);
            ome.units.quantity.Length sy = new ome.units.quantity.Length(java.lang.Double.valueOf(b.stageSizeY), ome.units.UNITS.MICROMETER);
            omexml.setStageLabelY(sy, series);
            ome.units.quantity.Length sz = new ome.units.quantity.Length(java.lang.Double.valueOf(b.stageSizeZ), ome.units.UNITS.MICROMETER);
            omexml.setStageLabelZ(sz, series);
            omexml.setStageLabelName("StageLabel:0", series);

            for (int channel = 0; channel < b.Channels.Count; channel++)
            {
                Channel c = b.Channels[channel];
                omexml.setChannelID("Channel:" + channel + ":" + series, series, channel);
                omexml.setChannelSamplesPerPixel(new PositiveInteger(java.lang.Integer.valueOf(1)), series, channel);
                if (c.Name != "")
                    omexml.setChannelName(c.Name, series, channel);
                if (c.Color != null)
                {
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(c.Color.Value.R, c.Color.Value.G, c.Color.Value.B, c.Color.Value.A);
                    omexml.setChannelColor(col, series, channel);
                }
                if (c.Emission != -1)
                {
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(c.Emission), ome.units.UNITS.NANOMETER);
                    omexml.setChannelEmissionWavelength(fl, series, channel);
                }
                if (c.Excitation != -1)
                {
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(c.Excitation), ome.units.UNITS.NANOMETER);
                    omexml.setChannelEmissionWavelength(fl, series, channel);
                }
                if (c.Exposure != -1)
                {
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(c.Exposure), ome.units.UNITS.MILLISECOND);
                    omexml.setChannelEmissionWavelength(fl, series, channel);
                }
                if (c.ContrastMethod != null)
                {
                    omexml.setChannelContrastMethod(c.ContrastMethod, series, channel);
                }
                if (c.Fluor != "")
                {
                    omexml.setChannelFluor(c.Fluor, series, channel);
                }
                if (c.IlluminationType != null)
                {
                    omexml.setChannelIlluminationType(c.IlluminationType, series, channel);
                }
                if (c.LightSourceIntensity != -1)
                {
                    ome.units.quantity.Power fl = new ome.units.quantity.Power(java.lang.Double.valueOf(c.LightSourceIntensity), ome.units.UNITS.VOLT);
                    omexml.setLightEmittingDiodePower(fl, series, channel);
                }
            }

            int i = 0;
            foreach (ROI an in b.Annotations)
            {
                if (an.roiID == "")
                    omexml.setROIID("ROI:" + i.ToString() + ":" + series, i);
                else
                    omexml.setROIID(an.roiID, i);
                omexml.setROIName(an.roiName, i);
                if (an.type == ROI.Type.Point)
                {
                    if (an.id == "")
                        omexml.setPointID(an.id, i, series);
                    else
                        omexml.setPointID("Shape:" + i + ":" + series, i, series);
                    omexml.setPointX(java.lang.Double.valueOf(an.X), i, series);
                    omexml.setPointY(java.lang.Double.valueOf(an.Y), i, series);
                    omexml.setPointTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setPointTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setPointTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    if (an.Text != "")
                        omexml.setPointText(an.Text, i, series);
                    else
                        omexml.setPointText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    b.meta.setPointFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setPointStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setPointStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setPointFillColor(colf, i, series);
                }
                else
                if (an.type == ROI.Type.Polygon || an.type == ROI.Type.Freeform)
                {
                    if (an.id == "")
                        omexml.setPolygonID(an.id, i, series);
                    else
                        omexml.setPolygonID("Shape:" + i + ":" + series, i, series);
                    omexml.setPolygonPoints(an.PointsToString(), i, series);
                    omexml.setPolygonTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setPolygonTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setPolygonTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    if (an.Text != "")
                        omexml.setPolygonText(an.Text, i, series);
                    else
                        omexml.setPolygonText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    omexml.setPolygonFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setPolygonStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setPolygonStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setPolygonFillColor(colf, i, series);
                }
                else
                if (an.type == ROI.Type.Rectangle)
                {
                    if (an.id != "")
                        omexml.setRectangleID(an.id, i, series);
                    else
                        omexml.setRectangleID("Shape:" + i + ":" + series, i, series);
                    omexml.setRectangleWidth(java.lang.Double.valueOf(an.W), i, series);
                    omexml.setRectangleHeight(java.lang.Double.valueOf(an.H), i, series);
                    omexml.setRectangleX(java.lang.Double.valueOf(an.Rect.X), i, series);
                    omexml.setRectangleY(java.lang.Double.valueOf(an.Rect.Y), i, series);
                    omexml.setRectangleTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setRectangleTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setRectangleTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    omexml.setRectangleText(i.ToString(), i, series);
                    if (an.Text != "")
                        omexml.setRectangleText(an.Text, i, series);
                    else
                        omexml.setRectangleText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    omexml.setRectangleFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setRectangleStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setRectangleStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setRectangleFillColor(colf, i, series);
                }
                else
                if (an.type == ROI.Type.Line)
                {
                    if (an.id == "")
                        omexml.setLineID(an.id, i, series);
                    else
                        omexml.setLineID("Shape:" + i + ":" + series, i, series);
                    omexml.setLineX1(java.lang.Double.valueOf(an.GetPoint(0).X), i, series);
                    omexml.setLineY1(java.lang.Double.valueOf(an.GetPoint(0).Y), i, series);
                    omexml.setLineX2(java.lang.Double.valueOf(an.GetPoint(1).X), i, series);
                    omexml.setLineY2(java.lang.Double.valueOf(an.GetPoint(1).Y), i, series);
                    omexml.setLineTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setLineTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setLineTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    if (an.Text != "")
                        omexml.setLineText(an.Text, i, series);
                    else
                        omexml.setLineText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    omexml.setLineFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setLineStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setLineStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setLineFillColor(colf, i, series);
                }
                else
                if (an.type == ROI.Type.Ellipse)
                {

                    if (an.id == "")
                        omexml.setEllipseID(an.id, i, series);
                    else
                        omexml.setEllipseID("Shape:" + i + ":" + series, i, series);
                    //We need to change System.Drawing.Rectangle to ellipse radius;
                    double w = (double)an.W / 2;
                    double h = (double)an.H / 2;
                    omexml.setEllipseRadiusX(java.lang.Double.valueOf(w), i, series);
                    omexml.setEllipseRadiusY(java.lang.Double.valueOf(h), i, series);

                    double x = an.Point.X + w;
                    double y = an.Point.Y + h;
                    omexml.setEllipseX(java.lang.Double.valueOf(x), i, series);
                    omexml.setEllipseY(java.lang.Double.valueOf(y), i, series);
                    omexml.setEllipseTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setEllipseTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setEllipseTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    if (an.Text != "")
                        omexml.setEllipseText(an.Text, i, series);
                    else
                        omexml.setEllipseText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    omexml.setEllipseFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setEllipseStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setEllipseStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setEllipseFillColor(colf, i, series);
                }
                else
                if (an.type == ROI.Type.Label)
                {
                    if (an.id != "")
                        omexml.setLabelID(an.id, i, series);
                    else
                        omexml.setLabelID("Shape:" + i + ":" + series, i, series);
                    omexml.setLabelX(java.lang.Double.valueOf(an.Rect.X), i, series);
                    omexml.setLabelY(java.lang.Double.valueOf(an.Rect.Y), i, series);
                    omexml.setLabelTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setLabelTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setLabelTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    omexml.setLabelText(i.ToString(), i, series);
                    if (an.Text != "")
                        omexml.setLabelText(an.Text, i, series);
                    else
                        omexml.setLabelText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    omexml.setLabelFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setLabelStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setLabelStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setLabelFillColor(colf, i, series);
                }
                i++;
            }

            writer.setMetadataRetrieve(omexml);
            //We delete the file so we don't just add more images to an existing file;
            if (File.Exists(file))
                File.Delete(file);
            writer.setSeries(serie);
            writer.setId(file);
            for (int bu = 0; bu < b.Buffers.Count; bu++)
            {
                writer.saveBytes(bu, b.Buffers[bu].GetSaveBytes(true));
                pr.UpdateProgressF((float)bu / b.Buffers.Count);
            }

            pr.Close();
            pr.Dispose();
            writer.close();
            Recorder.AddLine("Bio.BioImage.SaveOME(" + '"' + file + '"' + ");");
            return b;
        }
        public static void SaveOMESeries(string[] files, string f)
        {
            loci.formats.meta.IMetadata omexml = service.createOMEXMLMetadata();
            for (int fi = 0; fi < files.Length; fi++)
            {
                int serie = fi;
                string file = files[fi];
                BioImage b = Images.GetImage(file);
                // create OME-XML metadata store

                omexml.setImageID("Image:" + serie, serie);
                omexml.setPixelsID("Pixels:" + serie, serie);
                omexml.setPixelsDimensionOrder(ome.xml.model.enums.DimensionOrder.XYCZT, serie);
                if (b.bitsPerPixel > 8)
                    omexml.setPixelsType(ome.xml.model.enums.PixelType.UINT16, serie);
                else
                    omexml.setPixelsType(ome.xml.model.enums.PixelType.UINT8, serie);
                omexml.setPixelsSizeX(new PositiveInteger(java.lang.Integer.valueOf(b.SizeX)), serie);
                omexml.setPixelsSizeY(new PositiveInteger(java.lang.Integer.valueOf(b.SizeY)), serie);
                omexml.setPixelsSizeZ(new PositiveInteger(java.lang.Integer.valueOf(b.SizeZ)), serie);
                int samples = 1;
                if (b.isRGB)
                    samples = 3;
                omexml.setPixelsSizeC(new PositiveInteger(java.lang.Integer.valueOf(b.SizeC * samples)), serie);
                omexml.setPixelsSizeT(new PositiveInteger(java.lang.Integer.valueOf(b.SizeT)), serie);
                if (!BitConverter.IsLittleEndian)
                    omexml.setPixelsBinDataBigEndian(java.lang.Boolean.TRUE, serie, 0);
                else
                    omexml.setPixelsBinDataBigEndian(java.lang.Boolean.FALSE, serie, 0);
                ome.units.quantity.Length p1 = new ome.units.quantity.Length(java.lang.Double.valueOf(b.physicalSizeX), ome.units.UNITS.MICROMETER);
                omexml.setPixelsPhysicalSizeX(p1, serie);
                ome.units.quantity.Length p2 = new ome.units.quantity.Length(java.lang.Double.valueOf(b.physicalSizeY), ome.units.UNITS.MICROMETER);
                omexml.setPixelsPhysicalSizeY(p2, serie);
                ome.units.quantity.Length p3 = new ome.units.quantity.Length(java.lang.Double.valueOf(b.physicalSizeZ), ome.units.UNITS.MICROMETER);
                omexml.setPixelsPhysicalSizeZ(p3, serie);
                ome.units.quantity.Length s1 = new ome.units.quantity.Length(java.lang.Double.valueOf(b.Volume.Location.X), ome.units.UNITS.MICROMETER);
                omexml.setStageLabelX(s1, serie);
                ome.units.quantity.Length s2 = new ome.units.quantity.Length(java.lang.Double.valueOf(b.Volume.Location.Y), ome.units.UNITS.MICROMETER);
                omexml.setStageLabelY(s2, serie);
                ome.units.quantity.Length s3 = new ome.units.quantity.Length(java.lang.Double.valueOf(b.Volume.Location.Z), ome.units.UNITS.MICROMETER);
                omexml.setStageLabelZ(s3, serie);
                omexml.setStageLabelName("StageLabel:0", serie);

                for (int channel = 0; channel < b.Channels.Count; channel++)
                {
                    Channel c = b.Channels[channel];
                    omexml.setChannelID("Channel:" + channel + ":" + serie, serie, channel);
                    omexml.setChannelSamplesPerPixel(new PositiveInteger(java.lang.Integer.valueOf(1)), serie, channel);
                    if (c.Name != "")
                        omexml.setChannelName(c.Name, serie, channel);
                    if (c.Color != null)
                    {
                        ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(c.Color.Value.R, c.Color.Value.G, c.Color.Value.B, c.Color.Value.A);
                        omexml.setChannelColor(col, serie, channel);
                    }
                    if (c.Emission != -1)
                    {
                        ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(c.Emission), ome.units.UNITS.NANOMETER);
                        omexml.setChannelEmissionWavelength(fl, serie, channel);
                    }
                    if (c.Excitation != -1)
                    {
                        ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(c.Excitation), ome.units.UNITS.NANOMETER);
                        omexml.setChannelEmissionWavelength(fl, serie, channel);
                    }
                    if (c.Exposure != -1)
                    {
                        ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(c.Exposure), ome.units.UNITS.MILLISECOND);
                        omexml.setChannelEmissionWavelength(fl, serie, channel);
                    }
                    if (c.ContrastMethod != null)
                    {
                        omexml.setChannelContrastMethod(c.ContrastMethod, serie, channel);
                    }
                    if (c.Fluor != "")
                    {
                        omexml.setChannelFluor(c.Fluor, serie, channel);
                    }
                    if (c.IlluminationType != null)
                    {
                        omexml.setChannelIlluminationType(c.IlluminationType, serie, channel);
                    }
                    if (c.LightSourceIntensity != -1)
                    {
                        ome.units.quantity.Power fl = new ome.units.quantity.Power(java.lang.Double.valueOf(c.LightSourceIntensity), ome.units.UNITS.VOLT);
                        omexml.setLightEmittingDiodePower(fl, serie, channel);
                    }
                }

                int i = 0;
                foreach (ROI an in b.Annotations)
                {
                    if (an.roiID == "")
                        omexml.setROIID("ROI:" + i.ToString() + ":" + serie, i);
                    else
                        omexml.setROIID(an.roiID, i);
                    omexml.setROIName(an.roiName, i);
                    if (an.type == ROI.Type.Point)
                    {
                        if (an.id == "")
                            omexml.setPointID(an.id, i, serie);
                        else
                            omexml.setPointID("Shape:" + i + ":" + serie, i, serie);
                        omexml.setPointX(java.lang.Double.valueOf(an.X), i, serie);
                        omexml.setPointY(java.lang.Double.valueOf(an.Y), i, serie);
                        omexml.setPointTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, serie);
                        omexml.setPointTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, serie);
                        omexml.setPointTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, serie);
                        if (an.Text != "")
                            omexml.setPointText(an.Text, i, serie);
                        else
                            omexml.setPointText(i.ToString(), i, serie);
                        ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                        b.meta.setPointFontSize(fl, i, serie);
                        ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                        omexml.setPointStrokeColor(col, i, serie);
                        ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                        omexml.setPointStrokeWidth(sw, i, serie);
                        ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                        omexml.setPointFillColor(colf, i, serie);
                    }
                    else
                    if (an.type == ROI.Type.Polygon || an.type == ROI.Type.Freeform)
                    {
                        if (an.id == "")
                            omexml.setPolygonID(an.id, i, serie);
                        else
                            omexml.setPolygonID("Shape:" + i + ":" + serie, i, serie);
                        omexml.setPolygonPoints(an.PointsToString(), i, serie);
                        omexml.setPolygonTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, serie);
                        omexml.setPolygonTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, serie);
                        omexml.setPolygonTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, serie);
                        if (an.Text != "")
                            omexml.setPolygonText(an.Text, i, serie);
                        else
                            omexml.setPolygonText(i.ToString(), i, serie);
                        ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                        omexml.setPolygonFontSize(fl, i, serie);
                        ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                        omexml.setPolygonStrokeColor(col, i, serie);
                        ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                        omexml.setPolygonStrokeWidth(sw, i, serie);
                        ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                        omexml.setPolygonFillColor(colf, i, serie);
                    }
                    else
                    if (an.type == ROI.Type.Rectangle)
                    {
                        if (an.id != "")
                            omexml.setRectangleID(an.id, i, serie);
                        else
                            omexml.setRectangleID("Shape:" + i + ":" + serie, i, serie);
                        omexml.setRectangleWidth(java.lang.Double.valueOf(an.W), i, serie);
                        omexml.setRectangleHeight(java.lang.Double.valueOf(an.H), i, serie);
                        omexml.setRectangleX(java.lang.Double.valueOf(an.Rect.X), i, serie);
                        omexml.setRectangleY(java.lang.Double.valueOf(an.Rect.Y), i, serie);
                        omexml.setRectangleTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, serie);
                        omexml.setRectangleTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, serie);
                        omexml.setRectangleTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, serie);
                        omexml.setRectangleText(i.ToString(), i, serie);
                        if (an.Text != "")
                            omexml.setRectangleText(an.Text, i, serie);
                        else
                            omexml.setRectangleText(i.ToString(), i, serie);
                        ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                        omexml.setRectangleFontSize(fl, i, serie);
                        ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                        omexml.setRectangleStrokeColor(col, i, serie);
                        ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                        omexml.setRectangleStrokeWidth(sw, i, serie);
                        ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                        omexml.setRectangleFillColor(colf, i, serie);
                    }
                    else
                    if (an.type == ROI.Type.Line)
                    {
                        if (an.id == "")
                            omexml.setLineID(an.id, i, serie);
                        else
                            omexml.setLineID("Shape:" + i + ":" + serie, i, serie);
                        omexml.setLineX1(java.lang.Double.valueOf(an.GetPoint(0).X), i, serie);
                        omexml.setLineY1(java.lang.Double.valueOf(an.GetPoint(0).Y), i, serie);
                        omexml.setLineX2(java.lang.Double.valueOf(an.GetPoint(1).X), i, serie);
                        omexml.setLineY2(java.lang.Double.valueOf(an.GetPoint(1).Y), i, serie);
                        omexml.setLineTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, serie);
                        omexml.setLineTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, serie);
                        omexml.setLineTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, serie);
                        if (an.Text != "")
                            omexml.setLineText(an.Text, i, serie);
                        else
                            omexml.setLineText(i.ToString(), i, serie);
                        ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                        omexml.setLineFontSize(fl, i, serie);
                        ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                        omexml.setLineStrokeColor(col, i, serie);
                        ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                        omexml.setLineStrokeWidth(sw, i, serie);
                        ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                        omexml.setLineFillColor(colf, i, serie);
                    }
                    else
                    if (an.type == ROI.Type.Ellipse)
                    {

                        if (an.id == "")
                            omexml.setEllipseID(an.id, i, serie);
                        else
                            omexml.setEllipseID("Shape:" + i + ":" + serie, i, serie);
                        //We need to change System.Drawing.Rectangle to ellipse radius;
                        double w = (double)an.W / 2;
                        double h = (double)an.H / 2;
                        omexml.setEllipseRadiusX(java.lang.Double.valueOf(w), i, serie);
                        omexml.setEllipseRadiusY(java.lang.Double.valueOf(h), i, serie);

                        double x = an.Point.X + w;
                        double y = an.Point.Y + h;
                        omexml.setEllipseX(java.lang.Double.valueOf(x), i, serie);
                        omexml.setEllipseY(java.lang.Double.valueOf(y), i, serie);
                        omexml.setEllipseTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, serie);
                        omexml.setEllipseTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, serie);
                        omexml.setEllipseTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, serie);
                        if (an.Text != "")
                            omexml.setEllipseText(an.Text, i, serie);
                        else
                            omexml.setEllipseText(i.ToString(), i, serie);
                        ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                        omexml.setEllipseFontSize(fl, i, serie);
                        ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                        omexml.setEllipseStrokeColor(col, i, serie);
                        ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                        omexml.setEllipseStrokeWidth(sw, i, serie);
                        ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                        omexml.setEllipseFillColor(colf, i, serie);
                    }
                    else
                    if (an.type == ROI.Type.Label)
                    {
                        if (an.id != "")
                            omexml.setLabelID(an.id, i, serie);
                        else
                            omexml.setLabelID("Shape:" + i + ":" + serie, i, serie);
                        omexml.setLabelX(java.lang.Double.valueOf(an.Rect.X), i, serie);
                        omexml.setLabelY(java.lang.Double.valueOf(an.Rect.Y), i, serie);
                        omexml.setLabelTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, serie);
                        omexml.setLabelTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, serie);
                        omexml.setLabelTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, serie);
                        omexml.setLabelText(i.ToString(), i, serie);
                        if (an.Text != "")
                            omexml.setLabelText(an.Text, i, serie);
                        else
                            omexml.setLabelText(i.ToString(), i, serie);
                        ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                        omexml.setLabelFontSize(fl, i, serie);
                        ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                        omexml.setLabelStrokeColor(col, i, serie);
                        ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                        omexml.setLabelStrokeWidth(sw, i, serie);
                        ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                        omexml.setLabelFillColor(colf, i, serie);
                    }
                    i++;
                }

            }

            writer.setMetadataRetrieve(omexml);
            writer.setId(f);
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                Progress pr = new Progress(file, "Saving");
                pr.Show();
                BioImage b = Images.GetImage(files[i]);
                writer.setSeries(i);
                for (int bu = 0; bu < b.Buffers.Count; bu++)
                {
                    writer.saveBytes(bu, b.Buffers[bu].GetSaveBytes(true));
                    pr.UpdateProgressF((float)bu / b.Buffers.Count);
                }
                pr.Close();
                pr.Dispose();
            }
            writer.close();
        }
        public static BioImage OpenOME(string file)
        {
            return OpenOME(file, 0);
        }
        public static BioImage OpenOME(string file, int serie)
        {
            do
            {
                Thread.Sleep(250);
            } while (!Initialized);
            if (file == null || file == "")
                throw new InvalidDataException();
            Progress pr = new Progress(file, "Opening OME");
            pr.Show();
            Application.DoEvents();
            st.Start();
            BioImage b = new BioImage(file);
            b.Loading = true;

            b.meta = (IMetadata)((OMEXMLService)new ServiceFactory().getInstance(typeof(OMEXMLService))).createOMEXMLMetadata();
            reader = new ImageReader();
            reader.setMetadataStore((MetadataStore)b.meta);
            reader.setId(file);
            reader.setSeries(serie);
            int RGBChannelCount = reader.getRGBChannelCount();
            b.bitsPerPixel = reader.getBitsPerPixel();
            b.id = file;
            int SizeX = reader.getSizeX();
            int SizeY = reader.getSizeY();
            b.sizeC = reader.getSizeC();
            b.sizeZ = reader.getSizeZ();
            b.sizeT = reader.getSizeT();
            b.littleEndian = reader.isLittleEndian();
            b.seriesCount = reader.getSeriesCount();
            b.imagesPerSeries = reader.getImageCount() / b.seriesCount;

            b.series = serie;
            string order = reader.getDimensionOrder();
            PixelFormat PixelFormat;
            bool bit48 = false;
            int stride = 0;
            if (RGBChannelCount == 1)
            {
                if (b.bitsPerPixel > 8)
                {
                    PixelFormat = PixelFormat.Format16bppGrayScale;
                    stride = SizeX * 2;
                }
                else
                {
                    PixelFormat = PixelFormat.Format8bppIndexed;
                    stride = SizeY;
                }
            }
            else
            if (RGBChannelCount == 3)
            {
                b.sizeC = 1;
                if (b.bitsPerPixel > 8)
                {
                    PixelFormat = PixelFormat.Format48bppRgb;
                    stride = SizeX * 2 * 3;
                    bit48 = true;
                }
                else
                {
                    PixelFormat = PixelFormat.Format24bppRgb;
                    stride = SizeX * 3;
                }
            }
            else
            {
                PixelFormat = PixelFormat.Format32bppRgb;
                stride = SizeX * 4;
            }

            if (bit48)
                b.sizeC = 3;
            b.Coords = new int[b.SizeZ, b.SizeC, b.SizeT];
            //Lets get the channels amd initialize them.
            for (int i = 0; i < b.SizeC; i++)
            {
                Channel ch = new Channel(i, b.bitsPerPixel);
                try
                {
                    if (b.meta.getChannelName(0, i) != null)
                        ch.Name = b.meta.getChannelName(0, i);
                    if (b.meta.getChannelSamplesPerPixel(0, i) != null)
                    {
                        int s = b.meta.getChannelSamplesPerPixel(0, i).getNumberValue().intValue();
                        ch.SamplesPerPixel = s;
                    }
                    if (b.meta.getChannelID(0, i) != null)
                        ch.info.ID = b.meta.getChannelID(0, i);
                    if (b.meta.getChannelFluor(0, i) != null)
                        ch.Fluor = b.meta.getChannelFluor(0, i);
                    if (b.meta.getChannelColor(0, i) != null)
                    {
                        ome.xml.model.primitives.Color cc = b.meta.getChannelColor(0, i);
                        ch.Color = System.Drawing.Color.FromArgb(cc.getRed(), cc.getGreen(), cc.getBlue());
                    }
                    if (b.meta.getChannelIlluminationType(0, i) != null)
                        ch.IlluminationType = b.meta.getChannelIlluminationType(0, i);
                    if (b.meta.getChannelContrastMethod(0, i) != null)
                        ch.ContrastMethod = b.meta.getChannelContrastMethod(0, i);
                    if (b.meta.getPlaneExposureTime(0, i) != null)
                        ch.Exposure = b.meta.getPlaneExposureTime(0, i).value().intValue();
                    if (b.meta.getChannelEmissionWavelength(0, i) != null)
                        ch.Emission = b.meta.getChannelEmissionWavelength(0, i).value().intValue();
                    if (b.meta.getChannelExcitationWavelength(0, i) != null)
                        ch.Excitation = b.meta.getChannelExcitationWavelength(0, i).value().intValue();
                    //if (b.meta.getChannelLightSourceSettingsAttenuation(0, i) != null)
                    //    ch. = b.meta.getChannelLightSourceSettingsAttenuation(0, i).getNumberValue().doubleValue();
                    if (b.meta.getLightEmittingDiodePower(0, i) != null)
                        ch.LightSourceIntensity = b.meta.getLightEmittingDiodePower(0, i).value().doubleValue();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                if (i == 0)
                {
                    b.rgbChannels[0] = 0;
                }
                else
                if (i == 1)
                {
                    b.rgbChannels[1] = 1;
                }
                else
                if (i == 2)
                {
                    b.rgbChannels[2] = 2;
                }
                b.Channels.Add(ch);
            }

            int rc = b.meta.getROICount();
            for (int i = 0; i < rc; i++)
            {
                string roiID = b.meta.getROIID(i);
                string roiName = b.meta.getROIName(i);
                ZCT co = new ZCT(0, 0, 0);
                int scount = 1;
                try
                {
                    scount = b.meta.getShapeCount(i);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message.ToString());
                }


                for (int sc = 0; sc < scount; sc++)
                {
                    string type = b.meta.getShapeType(i, sc);
                    ROI an = new ROI();
                    an.roiID = roiID;
                    an.roiName = roiName;
                    an.shapeIndex = sc;
                    if (type == "Point")
                    {
                        an.type = ROI.Type.Point;
                        an.id = b.meta.getPointID(i, sc);
                        double dx = b.meta.getPointX(i, sc).doubleValue();
                        double dy = b.meta.getPointY(i, sc).doubleValue();
                        an.AddPoint(new PointD(dx, dy));
                        an.coord = new ZCT();
                        ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getPointTheZ(i, sc);
                        if (nz != null)
                            an.coord.Z = nz.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getPointTheC(i, sc);
                        if (nc != null)
                            an.coord.C = nc.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getPointTheT(i, sc);
                        if (nt != null)
                            an.coord.T = nt.getNumberValue().intValue();
                        an.Text = b.meta.getPointText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getPointFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getPointStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getPointStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getPointStrokeColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Line")
                    {
                        an.type = ROI.Type.Line;
                        an.id = b.meta.getLineID(i, sc);
                        double px1 = b.meta.getLineX1(i, sc).doubleValue();
                        double py1 = b.meta.getLineY1(i, sc).doubleValue();
                        double px2 = b.meta.getLineX2(i, sc).doubleValue();
                        double py2 = b.meta.getLineY2(i, sc).doubleValue();
                        an.AddPoint(new PointD(px1, py1));
                        an.AddPoint(new PointD(px2, py2));
                        ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getLineTheZ(i, sc);
                        if (nz != null)
                            co.Z = nz.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getLineTheC(i, sc);
                        if (nc != null)
                            co.C = nc.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getLineTheT(i, sc);
                        if (nt != null)
                            co.T = nt.getNumberValue().intValue();
                        an.coord = co;
                        an.Text = b.meta.getLineText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getLineFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getLineStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getLineStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getLineFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Rectangle")
                    {
                        an.type = ROI.Type.Rectangle;
                        an.id = b.meta.getRectangleID(i, sc);
                        double px = b.meta.getRectangleX(i, sc).doubleValue();
                        double py = b.meta.getRectangleY(i, sc).doubleValue();
                        double pw = b.meta.getRectangleWidth(i, sc).doubleValue();
                        double ph = b.meta.getRectangleHeight(i, sc).doubleValue();
                        an.Rect = new RectangleD(px, py, pw, ph);
                        ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getRectangleTheZ(i, sc);
                        if (nz != null)
                            co.Z = nz.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getRectangleTheC(i, sc);
                        if (nc != null)
                            co.C = nc.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getRectangleTheT(i, sc);
                        if (nt != null)
                            co.T = nt.getNumberValue().intValue();
                        an.coord = co;

                        an.Text = b.meta.getRectangleText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getRectangleFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getRectangleStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getRectangleStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getRectangleFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                        ome.xml.model.enums.FillRule fr = b.meta.getRectangleFillRule(i, sc);
                    }
                    else
                    if (type == "Ellipse")
                    {
                        an.type = ROI.Type.Ellipse;
                        an.id = b.meta.getEllipseID(i, sc);
                        double px = b.meta.getEllipseX(i, sc).doubleValue();
                        double py = b.meta.getEllipseY(i, sc).doubleValue();
                        double ew = b.meta.getEllipseRadiusX(i, sc).doubleValue();
                        double eh = b.meta.getEllipseRadiusY(i, sc).doubleValue();
                        //We convert the ellipse radius to System.Drawing.Rectangle
                        double w = ew * 2;
                        double h = eh * 2;
                        double x = px - ew;
                        double y = py - eh;
                        an.Rect = new RectangleD(x, y, w, h);
                        ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getEllipseTheZ(i, sc);
                        if (nz != null)
                            co.Z = nz.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getEllipseTheC(i, sc);
                        if (nc != null)
                            co.C = nc.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getEllipseTheT(i, sc);
                        if (nt != null)
                            co.T = nt.getNumberValue().intValue();
                        an.coord = co;
                        an.Text = b.meta.getEllipseText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getEllipseFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getEllipseStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getEllipseStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getEllipseFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Polygon")
                    {
                        an.type = ROI.Type.Polygon;
                        an.id = b.meta.getPolygonID(i, sc);
                        an.closed = true;
                        string pxs = b.meta.getPolygonPoints(i, sc);
                        PointD[] pts = an.stringToPoints(pxs);
                        if (pts.Length > 100)
                        {
                            an.type = ROI.Type.Freeform;
                        }
                        an.AddPoints(pts);
                        ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getPolygonTheZ(i, sc);
                        if (nz != null)
                            co.Z = nz.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getPolygonTheC(i, sc);
                        if (nc != null)
                            co.C = nc.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getPolygonTheT(i, sc);
                        if (nt != null)
                            co.T = nt.getNumberValue().intValue();
                        an.coord = co;
                        an.Text = b.meta.getPolygonText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getPolygonFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getPolygonStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getPolygonStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getPolygonFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Polyline")
                    {
                        an.type = ROI.Type.Polyline;
                        an.id = b.meta.getPolylineID(i, sc);
                        string pxs = b.meta.getPolylinePoints(i, sc);
                        an.AddPoints(an.stringToPoints(pxs));
                        ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getPolylineTheZ(i, sc);
                        if (nz != null)
                            co.Z = nz.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getPolylineTheC(i, sc);
                        if (nc != null)
                            co.C = nc.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getPolylineTheT(i, sc);
                        if (nt != null)
                            co.T = nt.getNumberValue().intValue();
                        an.coord = co;
                        an.Text = b.meta.getPolylineText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getPolylineFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getPolylineStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getPolylineStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getPolylineFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Label")
                    {
                        an.type = ROI.Type.Label;
                        an.id = b.meta.getLabelID(i, sc);

                        ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getLabelTheZ(i, sc);
                        if (nz != null)
                            co.Z = nz.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getLabelTheC(i, sc);
                        if (nc != null)
                            co.C = nc.getNumberValue().intValue();
                        ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getLabelTheT(i, sc);
                        if (nt != null)
                            co.T = nt.getNumberValue().intValue();
                        an.coord = co;

                        ome.units.quantity.Length fl = b.meta.getLabelFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getLabelStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getLabelStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getLabelFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                        //We set this last so the text is measured correctly.
                        an.AddPoint(new PointD(b.meta.getLabelX(i, sc).doubleValue(), b.meta.getLabelY(i, sc).doubleValue()));
                        an.Text = b.meta.getLabelText(i, sc);
                    }
                    b.Annotations.Add(an);
                }
            }

            List<string> serFiles = new List<string>();
            serFiles.AddRange(reader.getSeriesUsedFiles());

            //List<BufferInfo> BufferInfos = new List<BufferInfo>(); 
            //List<string> Files = new List<string>();
            b.Buffers = new List<BufferInfo>();
            // read the image data bytes
            int pages = reader.getImageCount();
            int z = 0;
            int c = 0;
            int t = 0;
            for (int p = 0; p < pages; p++)
            {
                byte[] bytes = reader.openBytes(p);
                if (!b.littleEndian)
                    Array.Reverse(bytes);
                if (bit48)
                {
                    Array.Reverse(bytes);
                    //We convert 48bpp plane to 3 16bpp channels
                    BufferInfo[] bfs = BufferInfo.RGB48To16(file, SizeX, SizeY, stride, bytes, new ZCT(z, c, t), p * 3);
                    bfs[0].RotateFlip(RotateFlipType.Rotate180FlipNone);
                    bfs[1].RotateFlip(RotateFlipType.Rotate180FlipNone);
                    bfs[2].RotateFlip(RotateFlipType.Rotate180FlipNone);
                    b.Buffers.Add(bfs[0]);
                    b.Buffers.Add(bfs[1]);
                    b.Buffers.Add(bfs[2]);
                    //We add the buffers to thresholding image statistics calculation threads.
                    Statistics.CalcStatistics(bfs[0]);
                    Statistics.CalcStatistics(bfs[1]);
                    Statistics.CalcStatistics(bfs[2]);
                }
                else
                {
                    BufferInfo bf = new BufferInfo(file, SizeX, SizeY, PixelFormat, bytes, new ZCT(z, c, t), p);
                    if (b.littleEndian)
                        bf.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    b.Buffers.Add(bf);
                    bf.UpdateStatistics();
                    //We add the buffers to thresholding image statistics calculation threads.
                    Statistics.CalcStatistics(bf);
                }
                pr.UpdateProgressF(((float)p / (float)pages));
                Application.DoEvents();
                b.Coords[z, c, t] = p;
                if (c < b.SizeC - 1)
                    c++;
                else
                {
                    c = 0;
                    if (z < b.SizeZ - 1)
                        z++;
                    else
                    {
                        z = 0;
                        if (t < b.SizeT - 1)
                            t++;
                        else
                            t = 0;
                    }
                }
            }

            if (bit48)
            {
                for (int im = 0; im < b.Buffers.Count; im += 3)
                {
                    b.Coords[z, 0, t] = im;
                    b.Coords[z, 1, t] = im + 1;
                    b.Coords[z, 2, t] = im + 2;
                    b.Buffers[im].Coordinate = new ZCT(z, 0, t);
                    b.Buffers[im + 1].Coordinate = new ZCT(z, 1, t);
                    b.Buffers[im + 2].Coordinate = new ZCT(z, 2, t);
                    if (z < b.SizeZ - 1)
                        z++;
                    else
                    {
                        z = 0;
                        if (t < b.SizeT - 1)
                            t++;
                        else
                            t = 0;
                    }
                }
            }

            try
            {
                bool hasPhysical = false;
                if (b.meta.getPixelsPhysicalSizeX(b.series) != null)
                {
                    b.physicalSizeX = b.meta.getPixelsPhysicalSizeX(b.series).value().doubleValue();
                    hasPhysical = true;
                }
                if (b.meta.getPixelsPhysicalSizeY(b.series) != null)
                {
                    b.physicalSizeY = b.meta.getPixelsPhysicalSizeY(b.series).value().doubleValue();
                }
                if (b.meta.getPixelsPhysicalSizeZ(b.series) != null)
                {
                    b.physicalSizeZ = b.meta.getPixelsPhysicalSizeZ(b.series).value().doubleValue();
                }
                else
                {
                    b.physicalSizeZ = 1;
                }
                if (b.meta.getStageLabelX(b.series) != null)
                    b.stageSizeX = b.meta.getStageLabelX(b.series).value().doubleValue();
                if (b.meta.getStageLabelY(b.series) != null)
                    b.stageSizeY = b.meta.getStageLabelY(b.series).value().doubleValue();
                if (b.meta.getStageLabelZ(b.series) != null)
                    b.stageSizeZ = b.meta.getStageLabelZ(b.series).value().doubleValue();
                else
                    b.stageSizeZ = 1;
                if (!hasPhysical)
                {
                    b.physicalSizeX = b.stageSizeX / b.SizeX;
                    b.physicalSizeY = b.stageSizeX / b.SizeX;
                    b.physicalSizeZ = b.stageSizeX / b.SizeX;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                b.Volume = new VolumeD(new Point3D(b.stageSizeX, b.stageSizeY, b.stageSizeZ), new Point3D(b.physicalSizeX * b.SizeX, b.physicalSizeY * b.SizeY, b.physicalSizeZ * b.SizeZ));
            }
            catch (Exception)
            {
                //Volume is used only for stage coordinates if error is thrown it is because this image doens't have any size information or it is incomplete as read by Bioformats.
            }
            reader.close();
            //We wait for threshold image statistics calculation
            do
            {
                Thread.Sleep(100);
            } while (b.Buffers[b.Buffers.Count - 1].Statistics == null);
            Statistics.ClearCalcBuffer();
            AutoThreshold(b, false);
            for (int ch = 0; ch < b.Channels.Count; ch++)
            {
                b.Channels[ch].Min = (int)b.Channels[ch].statistics.StackMin;
                b.Channels[ch].Max = (int)b.Channels[ch].statistics.StackMax;
            }
            Images.AddImage(b);
            Recorder.AddLine("Bio.BioImage.OpenOME(" + '"' + file + '"' + ");");
            b.Loading = false;
            pr.Close();
            pr.Dispose();
            return b;
        }
        public static BioImage[] OpenOMESeries(string file)
        {
            reader = new ImageReader();
            var meta = (IMetadata)((OMEXMLService)new ServiceFactory().getInstance(typeof(OMEXMLService))).createOMEXMLMetadata();
            reader.setMetadataStore((MetadataStore)meta);
            reader.setId(file);
            int count = reader.getSeriesCount();
            BioImage[] bs = new BioImage[count];
            reader.close();
            for (int i = 0; i < count; i++)
            {
                bs[i] = OpenOME(file, i);
            }
            return bs;
        }
        public static void OpenAsync(string file)
        {
            openfile.Add(file);
            Thread t = new Thread(OpenThread);
            t.Start();
        }
        public static void OpenAsync(string[] files)
        {
            foreach (string file in files)
            {
                OpenAsync(file);
            }
        }
        public static void Open(string file)
        {
            OpenFile(file);
        }
        public static void Open(string[] files)
        {
            foreach (string file in files)
            {
                Open(file);
            }
        }

        private static List<string> openfile = new List<string>();
        private static void OpenThread()
        {
            List<string> sts = new List<string>();
            for (int i = 0; i < openfile.Count; i++)
            {
                OpenFile(openfile[i]);
                sts.Add(openfile[i]);
            }
            for (int i = 0; i < sts.Count; i++)
            {
                openfile.Remove(sts[i]);
            }
        }
        public static void SaveAsync(string file, string ID)
        {
            saveid.Add(file);
            savefile.Add(ID);
            Thread t = new Thread(Save);
            t.Start();
        }
        public static void Save(string file, string ID)
        {
            SaveFile(file, ID);
        }
        private static List<string> savefile = new List<string>();
        private static List<string> saveid = new List<string>();
        private static void Save()
        {
            List<string> sts = new List<string>();
            for (int i = 0; i < savefile.Count; i++)
            {
                SaveAsync(savefile[i], saveid[i]);
                sts.Add(savefile[i]);
            }
            for (int i = 0; i < sts.Count; i++)
            {
                savefile.Remove(sts[i]);
                saveid.Remove(sts[i]);
            }
        }

        private static List<string> openOMEfile = new List<string>();
        public static void OpenOMEThread(string[] file)
        {
            openOMEfile.AddRange(file);
            Thread t = new Thread(OpenOME);
            t.Start();
        }
        private static void OpenOME()
        {
            foreach (string f in openOMEfile)
            {
                OpenOME(f);
            }
            openOMEfile.Clear();
        }
        public static void SaveOMEThread(string file, string ID)
        {
            saveOMEID = ID;
            saveOMEfile = file;
            Thread t = new Thread(SaveOME);
            t.Start();
        }
        private static string saveOMEfile;
        private static string saveOMEID;
        private static void SaveOME()
        {
            SaveOME(saveOMEfile, saveOMEID);
        }

        private static Stopwatch st = new Stopwatch();
        private static ServiceFactory factory;
        private static OMEXMLService service;
        private static ImageReader reader;
        private static ImageWriter writer;
        private loci.formats.meta.IMetadata meta;

        //We use UNIX type line endings since they are supported by ImageJ & BioImage.
        public const char NewLine = '\n';
        public const string columns = "ROIID,ROINAME,TYPE,ID,SHAPEINDEX,TEXT,S,C,Z,T,X,Y,W,H,POINTS,STROKECOLOR,STROKECOLORW,FILLCOLOR,FONTSIZE\n";
        public static List<ROI> OpenOMEROIs(string file)
        {
            List<ROI> Annotations = new List<ROI>();
            // create OME-XML metadata store
            ServiceFactory factory = new ServiceFactory();
            OMEXMLService service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            loci.formats.ome.OMEXMLMetadata meta = service.createOMEXMLMetadata();
            // create format reader
            ImageReader imageReader = new ImageReader();
            imageReader.setMetadataStore(meta);
            // initialize file
            imageReader.setId(file);
            int imageCount = imageReader.getImageCount();
            int seriesCount = imageReader.getSeriesCount();

            int rc = meta.getROICount();
            for (int i = 0; i < rc; i++)
            {
                string roiID = meta.getROIID(i);
                string roiName = meta.getROIName(i);
                ZCT co = new ZCT(0, 0, 0);
                int scount = 1;
                try
                {
                    scount = meta.getShapeCount(i);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message.ToString());
                }
                for (int sc = 0; sc < scount; sc++)
                {
                    string type = meta.getShapeType(i, sc);
                    ROI an = new ROI();
                    an.roiID = roiID;
                    an.roiName = roiName;
                    an.shapeIndex = sc;
                    if (type == "Point")
                    {
                        an.type = ROI.Type.Point;
                        an.id = meta.getPointID(i, sc);
                        double dx = meta.getPointX(i, sc).doubleValue();
                        double dy = meta.getPointY(i, sc).doubleValue();
                        an.AddPoint(new PointD(dx, dy));
                        if (imageCount > 1)
                        {
                            ome.xml.model.primitives.NonNegativeInteger nz = meta.getPointTheZ(i, sc);
                            if (nz != null)
                                co.Z = nz.getNumberValue().intValue();
                            ome.xml.model.primitives.NonNegativeInteger nc = meta.getPointTheC(i, sc);
                            if (nc != null)
                                co.C = nc.getNumberValue().intValue();
                            ome.xml.model.primitives.NonNegativeInteger nt = meta.getPointTheT(i, sc);
                            if (nt != null)
                                co.T = nt.getNumberValue().intValue();
                            an.coord = co;

                        }

                        an.Text = meta.getPointText(i, sc);
                        ome.units.quantity.Length fl = meta.getPointFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getPointStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getPointStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getPointStrokeColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Line")
                    {
                        an.type = ROI.Type.Line;
                        an.id = meta.getLineID(i, sc);
                        double px1 = meta.getLineX1(i, sc).doubleValue();
                        double py1 = meta.getLineY1(i, sc).doubleValue();
                        double px2 = meta.getLineX2(i, sc).doubleValue();
                        double py2 = meta.getLineY2(i, sc).doubleValue();
                        an.AddPoint(new PointD(px1, py1));
                        an.AddPoint(new PointD(px2, py2));
                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getLineTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getLineTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getLineTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = meta.getLineText(i, sc);
                        ome.units.quantity.Length fl = meta.getLineFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getLineStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getLineStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getLineFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Rectangle")
                    {
                        an.type = ROI.Type.Rectangle;
                        an.id = meta.getRectangleID(i, sc);
                        double px = meta.getRectangleX(i, sc).doubleValue();
                        double py = meta.getRectangleY(i, sc).doubleValue();
                        double pw = meta.getRectangleWidth(i, sc).doubleValue();
                        double ph = meta.getRectangleHeight(i, sc).doubleValue();
                        an.Rect = new RectangleD(px, py, pw, ph);
                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getRectangleTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getRectangleTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getRectangleTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = meta.getRectangleText(i, sc);
                        ome.units.quantity.Length fl = meta.getRectangleFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getRectangleStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getRectangleStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getRectangleFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                        ome.xml.model.enums.FillRule fr = meta.getRectangleFillRule(i, sc);
                    }
                    else
                    if (type == "Ellipse")
                    {
                        an.type = ROI.Type.Ellipse;
                        an.id = meta.getEllipseID(i, sc);
                        double px = meta.getEllipseX(i, sc).doubleValue();
                        double py = meta.getEllipseY(i, sc).doubleValue();
                        double ew = meta.getEllipseRadiusX(i, sc).doubleValue();
                        double eh = meta.getEllipseRadiusY(i, sc).doubleValue();
                        //We convert the ellipse radius to System.Drawing.Rectangle
                        double w = ew * 2;
                        double h = eh * 2;
                        double x = px - ew;
                        double y = py - eh;
                        an.Rect = new RectangleD(x, y, w, h);
                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getEllipseTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getEllipseTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getEllipseTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = meta.getEllipseText(i, sc);
                        ome.units.quantity.Length fl = meta.getEllipseFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getEllipseStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getEllipseStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getEllipseFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Polygon")
                    {
                        an.type = ROI.Type.Polygon;
                        an.id = meta.getPolygonID(i, sc);
                        an.closed = true;
                        string pxs = meta.getPolygonPoints(i, sc);
                        PointD[] pts = an.stringToPoints(pxs);
                        if (pts.Length > 100)
                        {
                            an.type = ROI.Type.Freeform;
                        }
                        an.AddPoints(pts);
                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getPolygonTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getPolygonTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getPolygonTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = meta.getPolygonText(i, sc);
                        ome.units.quantity.Length fl = meta.getPolygonFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getPolygonStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getPolygonStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getPolygonFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Polyline")
                    {
                        an.type = ROI.Type.Polyline;
                        an.id = meta.getPolylineID(i, sc);
                        string pxs = meta.getPolylinePoints(i, sc);
                        an.AddPoints(an.stringToPoints(pxs));
                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getPolylineTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getPolylineTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getPolylineTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = meta.getPolylineText(i, sc);
                        ome.units.quantity.Length fl = meta.getPolylineFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getPolylineStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getPolylineStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getPolylineFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Label")
                    {
                        an.type = ROI.Type.Label;
                        an.id = meta.getLabelID(i, sc);

                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getLabelTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getLabelTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getLabelTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }

                        ome.units.quantity.Length fl = meta.getLabelFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getLabelStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getLabelStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getLabelFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                        //We set this last so the text is measured correctly.
                        an.AddPoint(new PointD(meta.getLabelX(i, sc).doubleValue(), meta.getLabelY(i, sc).doubleValue()));
                        an.Text = meta.getLabelText(i, sc);
                    }
                    Annotations.Add(an);
                }
            }
            imageReader.close();
            return Annotations;
        }
        public static string ROIsToString(List<ROI> Annotations)
        {
            string s = "";
            for (int i = 0; i < Annotations.Count; i++)
            {
                s += ROIToString(Annotations[i]);
            }
            return s;
        }
        public static string ROIToString(ROI an)
        {
            PointD[] points = an.GetPoints();
            string pts = "";
            for (int j = 0; j < points.Length; j++)
            {
                if (j == points.Length - 1)
                    pts += points[j].X.ToString() + "," + points[j].Y.ToString();
                else
                    pts += points[j].X.ToString() + "," + points[j].Y.ToString() + " ";
            }

            char sep = (char)34;
            string sColor = sep.ToString() + an.strokeColor.A.ToString() + ',' + an.strokeColor.R.ToString() + ',' + an.strokeColor.G.ToString() + ',' + an.strokeColor.B.ToString() + sep.ToString();
            string bColor = sep.ToString() + an.fillColor.A.ToString() + ',' + an.fillColor.R.ToString() + ',' + an.fillColor.G.ToString() + ',' + an.fillColor.B.ToString() + sep.ToString();

            string line = an.roiID + ',' + an.roiName + ',' + an.type.ToString() + ',' + an.id + ',' + an.shapeIndex.ToString() + ',' +
                an.Text + ',' + an.coord.Z.ToString() + ',' + an.coord.C.ToString() + ',' + an.coord.T.ToString() + ',' + an.X.ToString() + ',' + an.Y.ToString() + ',' +
                an.W.ToString() + ',' + an.H.ToString() + ',' + sep.ToString() + pts + sep.ToString() + ',' + sColor + ',' + an.strokeWidth.ToString() + ',' + bColor + ',' + an.font.Size.ToString() + ',' + NewLine;
            return line;
        }
        public static ROI StringToROI(string sts)
        {
            if (sts.StartsWith("<?xml") || sts.StartsWith("{"))
                return null;
            ROI an = new ROI();
            string val = "";
            bool inSep = false;
            int col = 0;
            double x = 0;
            double y = 0;
            double w = 0;
            double h = 0;
            string line = sts;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == (char)34)
                {
                    if (!inSep)
                    {
                        inSep = true;
                    }
                    else
                        inSep = false;
                    continue;
                }

                if (c == ',' && !inSep)
                {
                    //ROIID,ROINAME,TYPE,ID,SHAPEINDEX,TEXT,S,C,Z,T,X,Y,W,H,POINTS,STROKECOLOR,STROKECOLORW,FILLCOLOR,FONTSIZE
                    if (col == 0)
                    {
                        //ROIID
                        an.roiID = val;
                    }
                    else
                    if (col == 1)
                    {
                        //ROINAME
                        an.roiName = val;
                    }
                    else
                    if (col == 2)
                    {
                        //TYPE
                        an.type = (ROI.Type)Enum.Parse(typeof(ROI.Type), val);
                    }
                    else
                    if (col == 3)
                    {
                        //ID
                        an.id = val;
                    }
                    else
                    if (col == 4)
                    {
                        //SHAPEINDEX/
                        an.shapeIndex = int.Parse(val);
                    }
                    else
                    if (col == 5)
                    {
                        //TEXT/
                        an.Text = val;
                    }
                    else
                    if (col == 6)
                    {
                        an.coord.Z = int.Parse(val);
                    }
                    else
                    if (col == 7)
                    {
                        an.coord.C = int.Parse(val);
                    }
                    else
                    if (col == 8)
                    {
                        an.coord.T = int.Parse(val);
                    }
                    else
                    if (col == 9)
                    {
                        x = double.Parse(val);
                    }
                    else
                    if (col == 10)
                    {
                        y = double.Parse(val);
                    }
                    else
                    if (col == 11)
                    {
                        w = double.Parse(val);
                    }
                    else
                    if (col == 12)
                    {
                        h = double.Parse(val);
                    }
                    else
                    if (col == 13)
                    {
                        //POINTS
                        an.AddPoints(an.stringToPoints(val));
                        an.Rect = new RectangleD(x, y, w, h);
                    }
                    else
                    if (col == 14)
                    {
                        //STROKECOLOR
                        string[] st = val.Split(',');
                        an.strokeColor = System.Drawing.Color.FromArgb(int.Parse(st[0]), int.Parse(st[1]), int.Parse(st[2]), int.Parse(st[3]));
                    }
                    else
                    if (col == 15)
                    {
                        //STROKECOLORW
                        an.strokeWidth = double.Parse(val);
                    }
                    else
                    if (col == 16)
                    {
                        //FILLCOLOR
                        string[] st = val.Split(',');
                        an.fillColor = System.Drawing.Color.FromArgb(int.Parse(st[0]), int.Parse(st[1]), int.Parse(st[2]), int.Parse(st[3]));
                    }
                    else
                    if (col == 17)
                    {
                        //FONTSIZE
                        double s = double.Parse(val);
                        an.font = new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont.FontFamily, (float)s, System.Drawing.FontStyle.Regular);
                    }
                    col++;
                    val = "";
                }
                else
                    val += c;
            }

            return an;
        }
        public static void ExportROIsCSV(string filename, List<ROI> Annotations)
        {
            string con = columns;
            con += ROIsToString(Annotations);
            File.WriteAllText(filename, con);
        }
        public static List<ROI> ImportROIsCSV(string filename)
        {
            List<ROI> list = new List<ROI>();
            if (!File.Exists(filename))
                return list;
            string[] sts = File.ReadAllLines(filename);
            //We start reading from line 1.
            for (int i = 1; i < sts.Length; i++)
            {
                list.Add(StringToROI(sts[i]));
            }
            return list;
        }
        public static void ExportROIFolder(string path, string filename)
        {
            string[] fs = Directory.GetFiles(path);
            int i = 0;
            foreach (string f in fs)
            {
                List<ROI> annotations = OpenOMEROIs(f);
                string ff = Path.GetFileNameWithoutExtension(f);
                ExportROIsCSV(path + "//" + ff + "-" + i.ToString() + ".csv", annotations);
                i++;
            }
        }

        private static BioImage bstats = null;
        private static bool update = false;
        public static void AutoThreshold(BioImage b, bool updateImageStats)
        {
            bstats = b;
            Statistics statistics = null;
            if (b.bitsPerPixel > 8)
                statistics = new Statistics(true);
            else
                statistics = new Statistics(false);
            for (int i = 0; i < b.Buffers.Count; i++)
            {
                if (b.Buffers[i].Statistics == null || updateImageStats)
                    b.Buffers[i].Statistics = Statistics.FromBytes(b.Buffers[i]);
                statistics.AddStatistics(b.Buffers[i].Statistics);
            }
            if (b.Buffers.Count > 0)
            {
                Statistics st;
                for (int c = 0; c < b.Channels.Count; c++)
                {
                    if (b.bitsPerPixel > 8)
                        st = new Statistics(true);
                    else
                        st = new Statistics(false);
                    for (int z = 0; z < b.SizeZ; z++)
                    {
                        for (int t = 0; t < b.SizeT; t++)
                        {
                            int ind;
                            if (b.isRGB)
                                ind = b.Coords[z, 0, t];
                            else
                                ind = b.Coords[z, c, t];
                            st.AddStatistics(b.Buffers[ind].Statistics);
                        }
                    }
                    st.MeanHistogram();
                    b.Channels[c].statistics = st;
                }
            }
            statistics.MeanHistogram();
            b.statistics = statistics;
            for (int c = 0; c < b.Channels.Count; c++)
            {
                b.Channels[c].Min = (int)b.Channels[c].statistics.StackMin;
                b.Channels[c].Max = (int)b.Channels[c].statistics.StackMax;
            }

            //We get rid of the histogram statistics data as they will otherwise consume too much memory.
            //Thread th = new Thread(DisposeHistogram);
            //th.Start();
        }
        public static void AutoThreshold()
        {
            AutoThreshold(bstats, update);
        }
        public static void DisposeHistogram()
        {
            for (int i = 0; i < bstats.Buffers.Count; i++)
            {
                //We get rid of the histogram statistics data as they will otherwise consume too much memory.
                bstats.Buffers[i].Statistics.DisposeHistogram();
            }
            GC.Collect();
        }
        public static void AutoThresholdThread(BioImage b)
        {
            bstats = b;
            Thread th = new Thread(AutoThreshold);
            th.Start();
        }
        public void Dispose()
        {
            for (int i = 0; i < Buffers.Count; i++)
            {
                Buffers[i].Dispose();
            }
            for (int i = 0; i < Channels.Count; i++)
            {
                Channels[i].Dispose();
            }
            if (rgbBitmap8 != null)
                rgbBitmap8 = null;
            if (rgbBitmap16 != null)
                rgbBitmap16 = null;
            Images.RemoveImage(this);
            GC.Collect();
        }
        public override string ToString()
        {
            return Filename.ToString();
        }
    }
}
