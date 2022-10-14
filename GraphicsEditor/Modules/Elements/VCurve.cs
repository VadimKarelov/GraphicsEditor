using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEditor.Modules.Elements
{
    internal class VCurve : IElement
    {
        public Color Color { get; set; }

        public List<TDPoint> Points { get; set; }

        public int Size { get; set; }

        private Camera _camera;

        public VCurve(Camera camera, int size, Color color)
        {
            Points = new();
            _camera = camera;
            Size = size;
            Color = color;
        }

        public void ChangeProjection()
        {
            //_camera.ChangeProjection(this);

            lock (this.Points)
            {
                foreach (TDPoint pt in Points)
                {
                    pt.ChangeProjection();
                }
            }
        }

        public override string ToString()
        {
            if (Points.Count > 0)
                return $"Curve line ({Points[0].X};{Points[0].Y};{Points[0].Z}) ({Points.Count}) [{Size}]";
            else
                return $"Curve line - empty";
        }

        public override bool Equals(object? obj)
        {
            return obj is VCurve cl && this.Points.Equals(cl.Points) && this.Size.Equals(cl.Size)
                && this.Color.Equals(cl.Color);
        }
    }
}
