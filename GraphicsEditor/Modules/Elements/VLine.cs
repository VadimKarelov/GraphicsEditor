using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GraphicsEditor.Modules.Elements
{
    internal class VLine : IElement
    {
        public Color Color { get; set; }

        public TDPoint Point1 { get; set; }
        public TDPoint Point2 { get; set; }

        public int Size { get; set; }

        public VLine(Camera camera, int x1, int y1, int z1, int x2, int y2, int z2, int size, Color color)
        {
            Point1 = new TDPoint(camera, x1, y1, z1);
            Point2 = new TDPoint(camera, x2, y2, z2);

            Size = size;
            Color = color;

            this.ChangeProjection(camera);
        }

        public void ChangeProjection(Camera camera)
        {
            camera.ChangeProjection(this);
        }

        public override string ToString()
        {
            return $"VLine ({Point1.X};{Point1.Y};{Point1.Z})-" +
                $"({Point2.X};{Point2.Y};{Point2.Z}) [{Size}]";
        }

        public override bool Equals(object? obj)
        {
            return obj is VLine ln && Point1.Equals(ln.Point1) && Point2.Equals(ln.Point2)
                && this.Color == ln.Color && this.Size == ln.Size;
        }
    }
}
