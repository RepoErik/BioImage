﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BioImage;
using System.IO;

namespace BioImage
{
    public partial class Tools : Form
    {
        public static bool applyToStack = false;
        public static ColorTool colorTool;
        public static bool rEnabled = true;
        public static bool gEnabled = true;
        public static bool bEnabled = true;
        public static PencilTool pencil;
        public static MoveTool move;
        public static BrushTool brush;
        public static BucketTool bucket;
        public static EraserTool eraser;
        public static PointTool point;
        public static LineTool line;
        public static RectTool rect;
        public static EllipseTool ellipse;
        public static PolygonTool polygon;
        public static TextTool text;
        public static DeleteTool delete;
        public static FreeformTool freeform;
        public static RectSelTool rectSel;
        public static PointSelTool pointSel;
        public static PanTool pan;
        public static Rectangle selectionRectangle;

        public class Tool : Control
        {
            public enum ToolType
            {
                color,
                annotation,
                select,
                function
            }
            public enum Type
            {
                pencil,
                brush,
                bucket,
                eraser,
                move,
                point,
                line,
                rect,
                ellipse,
                polyline,
                polygon,
                text,
                delete,
                freeform,
                rectSel,
                pointSel,
                pan
            }

            public BioImage.ColorS Color;
            public List<Point> Points;
            public ToolType toolType;
            public Type type;
        }
        public class PencilTool : Tool
        {
            public PencilTool(BioImage.ColorS col)
            {
                Color = col;
                toolType = ToolType.color;
                type = Type.pencil;
            }
        }
        public class BrushTool : Tool
        {
            public int BrushWidth;
            public BrushTool(BioImage.ColorS col,int BrushWidth)
            {
                this.BrushWidth = BrushWidth;
                this.Color = col;
                type = Type.brush;
            }
        }
        public class BucketTool : Tool
        {
            public BucketTool(BioImage.ColorS col)
            {
                this.Color = col;
                type = Type.bucket;
            }
        }
        public class EraserTool : Tool
        {
            public int EraserWidth;
            public EraserTool(BioImage.ColorS col, int EraserWidth)
            {
                this.EraserWidth = EraserWidth;
                this.Color = col;
                type = Type.brush;
            }
        }
        public class MoveTool : Tool
        {
            public MoveTool()
            {
                toolType = ToolType.annotation;
                type = Type.move;
            }
        }
        public class PointTool : Tool
        {
            public PointTool()
            {
                toolType = ToolType.annotation;
                type = Type.point;
            }
        }
        public class LineTool : Tool
        {
            public LineTool()
            {
                toolType = ToolType.annotation;
                type = Type.line;
            }
        }
        public class RectTool : Tool
        {
            public RectTool()
            {
                toolType = ToolType.annotation;
                type = Type.rect;
            }
        }
        public class EllipseTool : Tool
        {
            public EllipseTool()
            {
                toolType = ToolType.annotation;
                type = Type.ellipse;
            }
        }
        public class PolygonTool : Tool
        {
            public PolygonTool()
            {
                toolType = ToolType.annotation;
                type = Type.polygon;
            }
        }
        public class TextTool : Tool
        {
            public TextTool()
            {
                toolType = ToolType.annotation;
                type = Type.text;
            }
        }
        public class DeleteTool : Tool
        {
            public DeleteTool()
            {
                toolType = ToolType.annotation;
                type = Type.delete;
            }
        }
        public class FreeformTool : Tool
        {
            public FreeformTool()
            {
                toolType = ToolType.annotation;
                type = Type.freeform;
            }
        }
        public class PointSelTool : Tool
        {
            public PointSelTool()
            {
                toolType = ToolType.select;
                type = Type.pointSel;
            }
        }
        public class PanTool : Tool
        {
            public PanTool()
            {
                toolType = ToolType.select;
                type = Type.pan;
            }
        }
        public class RectSelTool : Tool
        {
            private BioImage.RectangleD selection;
            public BioImage.RectangleD Selection
            {
                get
                {
                    return selection;
                }
                set
                {
                    selectionRect = value;
                    selection = value;
                }
        
            }
            public RectSelTool()
            {
                toolType = ToolType.select;
                type = Type.rectSel;
                
            }
        }

        public static Tool currentTool;
        public static BioImage.RectangleD selectionRect;
        public Font font;
        public Tools()
        {
            InitializeComponent();
            BioImage.ColorS col = new BioImage.ColorS(ushort.MaxValue);
            pencil = new PencilTool(col);
            move = new MoveTool();
            currentTool = move;
            colorTool = new ColorTool();
            brush = new BrushTool(col,5);
            bucket = new BucketTool(col);
            eraser = new EraserTool(col, 5);
            move = new MoveTool();
            point = new PointTool();
            line = new LineTool();
            rect = new RectTool();
            ellipse = new EllipseTool();
            polygon = new PolygonTool();
            text = new TextTool();
            delete = new DeleteTool();
            freeform = new FreeformTool();
            rectSel = new RectSelTool();
            pointSel = new PointSelTool();
            pan = new PanTool();
        }

        public void UpdateOverlay()
        {
            ImageView.viewer.UpdateOverlay();
        }
        public void UpdateView()
        {
            ImageView.viewer.UpdateView();
        }
        public void UpdateSelected()
        {
            foreach (Control item in this.Controls)
            {
                item.BackColor = Color.White;
            }
        }

        BioImage.Annotation anno = new BioImage.Annotation();
        public void ToolDown(PointF e, MouseButtons buts)
        {
            if (ImageView.viewer == null)
                return;
            if (currentTool == null)
                return;
            if (currentTool.type == Tool.Type.line)
            {
                if (anno.GetPointCount() == 0)
                {
                    anno = new BioImage.Annotation();
                    anno.type = BioImage.Annotation.Type.Line;
                    anno.AddPoint(new BioImage.PointD(e.X, e.Y));
                    anno.AddPoint(new BioImage.PointD(e.X, e.Y));
                    anno.coord = ImageView.viewer.GetCoordinate();
                    ImageView.selectedImage.Annotations.Add(anno);
                    UpdateOverlay();
                }
            }
            else
            if (currentTool.type == Tool.Type.polygon)
            {
                if (anno.GetPointCount() == 0)
                {
                    anno = new BioImage.Annotation();
                    anno.type = BioImage.Annotation.Type.Polygon;
                    anno.AddPoint(new BioImage.PointD(e.X, e.Y));
                    anno.coord = ImageView.viewer.GetCoordinate();
                    ImageView.selectedImage.Annotations.Add(anno);
                }
                else
                {
                    //If we click on a point 1 we close this polygon
                    BioImage.RectangleD d = new BioImage.RectangleD(e.X, e.Y, anno.selectBoxSize, anno.selectBoxSize);
                    if (d.IntersectsWith(anno.Point))
                    {
                        anno.closed = true;
                        anno = new BioImage.Annotation();
                    }
                    else
                    {
                        anno.AddPoint(new BioImage.PointD(e.X, e.Y));
                    }
                }
                UpdateOverlay();
            }
            else
            if (currentTool.type == Tool.Type.freeform)
            {
                if (anno.GetPointCount() == 0)
                {
                    anno = new BioImage.Annotation();
                    anno.type = BioImage.Annotation.Type.Freeform;
                    anno.AddPoint(new BioImage.PointD(e.X, e.Y));
                    anno.coord = ImageView.viewer.GetCoordinate();
                    anno.closed = true;
                    ImageView.selectedImage.Annotations.Add(anno);
                }
                else
                {
                    anno.AddPoint(new BioImage.PointD(e.X, e.Y));
                }
            }
            else
            if (currentTool.type == Tool.Type.rect)
            {
                anno.type = BioImage.Annotation.Type.Rectangle;
                anno.Rect = new BioImage.RectangleD(e.X, e.Y, 1, 1);
                anno.coord = ImageView.viewer.GetCoordinate();
                ImageView.selectedImage.Annotations.Add(anno);
            }
            else
            if (currentTool.type == Tool.Type.ellipse)
            {
                anno.type = BioImage.Annotation.Type.Ellipse;
                anno.Rect = new BioImage.RectangleD(e.X, e.Y, 1, 1);
                anno.coord = ImageView.viewer.GetCoordinate();
                ImageView.selectedImage.Annotations.Add(anno);
            }
            else
            if(currentTool.type == Tool.Type.delete || Win32.GetKeyState(Keys.Delete))
            {
                foreach (BioImage.Annotation an in ImageView.selectedAnnotations)
                {
                    if(an != null)
                    {
                        if (an.selectedPoints.Count == 0)
                        {
                            ImageView.selectedImage.Annotations.Remove(an);
                        }
                        else
                        {
                            if (an.type == BioImage.Annotation.Type.Polygon ||
                                an.type == BioImage.Annotation.Type.Polyline ||
                                an.type == BioImage.Annotation.Type.Freeform)
                            {
                                an.closed = false;
                                an.RemovePoints(an.selectedPoints.ToArray());
                            }
                        }
                    }
                }
            }
            else
            if (currentTool.type == Tool.Type.text)
            {
                BioImage.Annotation an = new BioImage.Annotation();
                an.type = BioImage.Annotation.Type.Label;
                an.AddPoint(new BioImage.PointD(e.X, e.Y));
                an.coord = ImageView.viewer.GetCoordinate();
                TextInput ti = new TextInput("");
                if (ti.ShowDialog() != DialogResult.OK)
                    return;
                an.font = ti.font;
                an.strokeColor = ti.color;
                an.Text = ti.textInput;
                ImageView.selectedImage.Annotations.Add(an);
            }
            if (buts == MouseButtons.Middle || currentTool.type == Tool.Type.pan)
            {
                currentTool = pan;
                UpdateSelected();
                panPanel.BackColor = Color.LightGray;
                Cursor.Current = Cursors.Hand;
            }
            UpdateOverlay();
        }
        
        public void ToolUp(PointF e, MouseButtons buts)
        {
            if (ImageView.viewer == null)
                return;
            if (currentTool == null)
                return;
            if (anno == null)
                return;
            if (currentTool.type == Tool.Type.point)    
            {
                BioImage.Annotation an = new BioImage.Annotation();
                an.AddPoint(new BioImage.PointD(e.X, e.Y));
                an.type = BioImage.Annotation.Type.Point;
                an.coord = ImageView.viewer.GetCoordinate();
                ImageView.selectedImage.Annotations.Add(an);
                UpdateOverlay();
            }
            else
            if (currentTool.type == Tool.Type.line && anno.type == BioImage.Annotation.Type.Line)
            {
                if (anno.GetPointCount() > 0)
                {
                    anno.UpdatePoint(new BioImage.PointD(e.X, e.Y), 1);
                    anno = new BioImage.Annotation();
                    UpdateOverlay();
                }
            }
            else
            if (currentTool.type == Tool.Type.rect && anno.type == BioImage.Annotation.Type.Rectangle)
            {
                if (anno.GetPointCount() == 4)
                {
                    anno = new BioImage.Annotation();
                }
            }
            else
            if (currentTool.type == Tool.Type.ellipse && anno.type == BioImage.Annotation.Type.Ellipse)
            {
                if (anno.GetPointCount() == 4)
                {
                    anno = new BioImage.Annotation();
                }
            }
            else
            if (currentTool.type == Tool.Type.freeform && anno.type == BioImage.Annotation.Type.Freeform)
            {
                anno = new BioImage.Annotation();
            }
            else
            if (currentTool.type == Tool.Type.rectSel)
            {
                ImageView.selectedAnnotations.Clear();
                RectangleF r = rectSel.Selection.ToRectangleF();
                foreach (BioImage.Annotation an in ImageView.viewer.AnnotationsRGB)
                {
                    if (an.GetSelectBound().ToRectangleF().IntersectsWith(r))
                    {
                        an.selectedPoints.Clear();
                        ImageView.selectedAnnotations.Add(an);
                        an.selected = true;
                        for (int i = 0; i < an.selectBoxs.Count; i++)
                        {
                            if (an.selectBoxs[i].IntersectsWith(r))
                            {
                                an.selectedPoints.Add(i);
                            }
                        }
                    }
                    else
                        an.selected = false;
                }
                rectSel.Selection = new BioImage.RectangleD(0, 0, 0, 0);
                UpdateOverlay();
            }

            if (Tools.currentTool.type == Tools.Tool.Type.pan)
            {
                PointF pf = new PointF(ImageView.mouseUp.X - ImageView.mouseDown.X, ImageView.mouseUp.Y - ImageView.mouseDown.Y);
                ImageView.viewer.Origin = new PointF(ImageView.viewer.Origin.X + pf.X, ImageView.viewer.Origin.Y + pf.Y);
                UpdateView();
            }
        }
        public void ToolMove(PointF e, MouseButtons buts)
        {
            if (ImageView.viewer == null)
                return;
            if (currentTool.type == Tool.Type.line && ImageView.down)
            {
                anno.UpdatePoint(new BioImage.PointD(e.X, e.Y), 1);
                UpdateOverlay();
            }
            else
            if (currentTool.type == Tool.Type.freeform && buts == MouseButtons.Left && ImageView.down)
            {
                if (anno.GetPointCount() == 0)
                {
                    anno.type = BioImage.Annotation.Type.Freeform;
                    anno.AddPoint(new BioImage.PointD(e.X, e.Y));
                    anno.coord = ImageView.viewer.GetCoordinate();
                    anno.closed = true;
                    ImageView.selectedImage.Annotations.Add(anno);
                }
                else
                {
                    anno.AddPoint(new BioImage.PointD(e.X, e.Y));
                }
                UpdateOverlay();
            }
            else
            if (currentTool.type == Tool.Type.rect && anno.type == BioImage.Annotation.Type.Rectangle)
            {
                if (anno.GetPointCount() == 4)
                {
                    anno.Rect = new BioImage.RectangleD(anno.X, anno.Y, e.X - anno.X, e.Y - anno.Y);
                    UpdateOverlay();
                }
            }
            else
            if (currentTool.type == Tool.Type.ellipse && anno.type == BioImage.Annotation.Type.Ellipse)
            {
                if (anno.GetPointCount() == 4)
                {
                    anno.Rect = new BioImage.RectangleD(anno.X, anno.Y, e.X - anno.X, e.Y - anno.Y);
                    UpdateOverlay();
                }
            }
            else
            if (currentTool.type == Tool.Type.rectSel && buts == MouseButtons.Left && ImageView.down)
            {
                BioImage.PointD d = new BioImage.PointD(e.X - ImageView.mouseDown.X, e.Y - ImageView.mouseDown.Y);
                rectSel.Selection = new BioImage.RectangleD(ImageView.mouseDown.X, ImageView.mouseDown.Y, d.X, d.Y);
                RectangleF r = rectSel.Selection.ToRectangleF();
                foreach (BioImage.Annotation an in ImageView.viewer.AnnotationsRGB)
                {
                    if (an.GetSelectBound().ToRectangleF().IntersectsWith(r))
                    {
                        an.selectedPoints.Clear();
                        ImageView.selectedAnnotations.Add(an);
                        an.selected = true;
                        for (int i = 0; i < an.selectBoxs.Count; i++)
                        {
                            if (an.selectBoxs[i].IntersectsWith(r))
                            {
                                an.selectedPoints.Add(i);
                            }
                        }
                    }
                    else
                        an.selected = false;
                }
                UpdateOverlay();
            }
            else
            if (currentTool.type == Tool.Type.rectSel && ImageView.up)
            {
                rectSel.Selection = new BioImage.RectangleD(0, 0, 0, 0);
            }
            else
            if (Win32.GetKeyState(Keys.Delete))
            {
                foreach (BioImage.Annotation an in ImageView.selectedAnnotations)
                {
                    if (an != null)
                    {
                        if (an.selectedPoints.Count == 0)
                        {
                            ImageView.selectedImage.Annotations.Remove(an);
                            UpdateOverlay();
                        }
                        else
                        {
                            if (an.type == BioImage.Annotation.Type.Polygon ||
                                an.type == BioImage.Annotation.Type.Polyline ||
                                an.type == BioImage.Annotation.Type.Freeform)
                            {
                                an.closed = false;
                                an.RemovePoints(an.selectedPoints.ToArray());
                                UpdateOverlay();
                            }
                        }
                    }
                }
            }
            if (Tools.currentTool.type == Tools.Tool.Type.pan && (buts == MouseButtons.Middle || buts == MouseButtons.Left))
            {
                PointF pf = new PointF(e.X - ImageView.mouseDown.X, e.Y - ImageView.mouseDown.Y);
                ImageView.viewer.Origin = new PointF(ImageView.viewer.Origin.X + pf.X, ImageView.viewer.Origin.Y + pf.Y);
                UpdateView();
            }

        }

        public void ToolDoubleClick(PointF e, MouseButtons buts)
        {
            if (currentTool == null)
                return;
            
        }
        
        public static void SetColor(BioImage.ColorS col)
        {
            currentTool.Color = col;
        }

        private void pencilPanel_DoubleClick(object sender, EventArgs e)
        {
            colorTool.Show();
            currentTool = pencil;
            UpdateSelected();
            pencilPanel.BackColor = Color.DarkGray;
            Cursor.Current = Cursors.Arrow;
        }

        private void pencilPanel_Click(object sender, EventArgs e)
        {
            currentTool = pencil;
            UpdateSelected();
            pencilPanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Arrow;
        }

        private void movePanel_Click(object sender, EventArgs e)
        {
            currentTool = move;
            UpdateSelected();
            movePanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Arrow;
        }

        private void textPanel_Click(object sender, EventArgs e)
        {
            currentTool = text;
            UpdateSelected();
            textPanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Arrow;
        }

        private void textPanel_DoubleClick(object sender, EventArgs e)
        {
            currentTool = text;
            UpdateSelected();
            textPanel.BackColor = Color.LightGray;

            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;
            font = fontDialog.Font;
            Cursor.Current = Cursors.Arrow;
        }
        private void pointPanel_Click(object sender, EventArgs e)
        {
            currentTool = point;
            UpdateSelected();
            pointPanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Arrow;
        }
        private void linePanel_Click(object sender, EventArgs e)
        {
            currentTool = line;
            UpdateSelected();
            linePanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Arrow;
        }
        private void rectPanel_Click(object sender, EventArgs e)
        {
            currentTool = rect;
            UpdateSelected();
            rectPanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Arrow;
        }
        private void ellipsePanel_Click(object sender, EventArgs e)
        {
            currentTool = ellipse;
            UpdateSelected();
            ellipsePanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Arrow;
        }

        private void polyPanel_Click(object sender, EventArgs e)
        {
            currentTool = polygon;
            UpdateSelected();
            polyPanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Arrow;
        }

        private void deletePanel_Click(object sender, EventArgs e)
        {
            currentTool = delete;
            UpdateSelected();
            deletePanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Arrow;
        }

        private void freeformPanel_Click(object sender, EventArgs e)
        {
            currentTool = freeform;
            UpdateSelected();
            freeformPanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Arrow;
        }

        private void rectSelPanel_Click(object sender, EventArgs e)
        {
            currentTool = rectSel;
            UpdateSelected();
            rectSelPanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Arrow;
        }

        private void panPanel_Click(object sender, EventArgs e)
        {
            currentTool = pan;
            UpdateSelected();
            panPanel.BackColor = Color.LightGray;
            Cursor.Current = Cursors.Hand;
        }
    }
}
