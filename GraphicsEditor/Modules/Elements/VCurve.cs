using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GraphicsEditor.Modules.Elements
{
    internal class VCurve : IElement
    {
        public Color Color { get; set; }

        public List<TDPoint> Points { get; set; }
        public Point[] RenderPoints { get; set; }

        public int Size { get; set; }

        private Camera _camera;

        public VCurve(Camera camera, int size, Color color)
        {
            Points = new();
            RenderPoints = new Point[0];
            _camera = camera;
            Size = size;
            Color = color;

            this.ChangeProjection();
        }

        public void AddPoint(TDPoint pt)
        {
            lock (this.Points)
            {
                Points.Add(pt);
            }
            lock (this.RenderPoints)
            {
                RenderPoints = Points.Select(x => new Point(x.RenderX, x.RenderY)).ToArray();
            }
        }

        public void ChangeProjection()
        {
            lock (this.Points)
            {
                foreach (TDPoint pt in Points)
                {
                    pt.ChangeProjection();
                }
            }
            lock (this.RenderPoints)
            {
                RenderPoints = Points.Select(x => new Point(x.RenderX, x.RenderY)).ToArray();
            }
        }

        public override string ToString()
        {
            if (Points.Count > 0)
                return $"Кривая ({Points[0].X};{Points[0].Y};{Points[0].Z}) ({Points.Count}) [{Size}]";
            else
                return $"Кривая - пустая";
        }

        public override bool Equals(object? obj)
        {
            return obj is VCurve cl && this.Points.Equals(cl.Points) && this.Size.Equals(cl.Size)
                && this.Color.Equals(cl.Color);
        }
    }
}
