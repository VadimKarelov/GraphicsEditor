using GraphicsEditor.Modules.Elements;
using System.Collections.Generic;
using System.Drawing;

namespace GraphicsEditor.Modules
{
    internal static class DrawingEngineModule
    {
        public static Bitmap Draw(Bitmap btmp, List<IElement> elements, List<IElement>? editingElements)
        {
            List<IElement> cpEl = new();
            lock (elements)
            {
                cpEl = new(elements);
            }

            Graphics gr = Graphics.FromImage(btmp);

            gr.Clear(Color.White);

            DrawR(gr, cpEl);

            if (editingElements is not null)
            {
                DrawSelection(gr, editingElements, Color.Red);
            }

            return btmp;
        }

        private static void DrawR(Graphics gr, List<IElement> cpEl)
        {
            foreach (IElement el in cpEl)
            {
                if (el is VPoint pt)
                {
                    gr.FillEllipse(new SolidBrush(pt.Color), pt.Point.RenderX, pt.Point.RenderY, pt.Size, pt.Size);
                }
                else if (el is VLine ln)
                {
                    gr.DrawLine(new Pen(ln.Color, ln.Size), ln.Point1.RenderX, ln.Point1.RenderY, ln.Point2.RenderX, ln.Point2.RenderY);
                }
                else if (el is VCurve cl)
                {
                    DrawCurveLine(gr, cl);
                }
                else if (el is VGroup group)
                {
                    DrawR(gr, group.Elements);
                }
            }
        }

        private static void DrawCurveLine(Graphics gr, VCurve cl)
        {
            Point[] pts = cl.RenderPoints;
            if (pts.Length == 1)
            {
                gr.FillEllipse(new SolidBrush(cl.Color), pts[0].X, pts[0].Y, cl.Size, cl.Size);
            }
            else if (pts.Length > 1)
            {
                Pen pen = new Pen(cl.Color, cl.Size);
                //pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                //pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                gr.DrawCurve(pen, pts);
            }
        }

        private static void DrawSelection(Graphics gr, List<IElement> editingElements, Color color)
        {
            foreach (var elem in editingElements)
            {
                if (elem is VLine ln)
                {
                    DrawEllipseFromCenterPoint(gr, ln.Point1.RenderX, ln.Point1.RenderY, color);
                    DrawEllipseFromCenterPoint(gr, ln.Point2.RenderX, ln.Point2.RenderY, color);
                }
                else if (elem is VGroup grp)
                {
                    DrawSelection(gr, grp.Elements, grp.Color);
                }
            }
        }

        private static void DrawEllipseFromCenterPoint(Graphics gr, int x, int y, Color color)
        {
            int s = 10;
            gr.FillEllipse(new SolidBrush(color), x - s / 2, y - s / 2, s, s);
            gr.DrawEllipse(new Pen(Color.Purple, 1), x - s / 2, y - s / 2, s, s);
        }
    }
}
