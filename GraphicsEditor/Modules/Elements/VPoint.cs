using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEditor.Modules.Elements
{
    internal class VPoint : IElement
    {
        public Color Color { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int RenderX { get; set; }
        public int RenderY { get; set; }

        public int Size { get; set; }

        public VPoint(int x, int y, int z, int size, Color cl)
        {
            RenderX = x - size / 2;
            RenderY = y - size / 2;
            X = x;
            Y = y;
            Z = z;
            Size = size;
            Color = cl;
        }

        public void ChangeProjection(Camera camera)
        {
            camera.ChangeProjection(this);
        }

        public override string ToString()
        {
            return $"VPoint ({X};{Y};{Z}) [{Size}]";
        }

        public override bool Equals(object? obj)
        {
            return obj is VPoint pt && this.X == pt.X && this.Y == pt.Y && this.Z == pt.Z
                && this.Color == pt.Color;
        }
    }
}
