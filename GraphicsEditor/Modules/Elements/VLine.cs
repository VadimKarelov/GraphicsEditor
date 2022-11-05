using System.Drawing;

namespace GraphicsEditor.Modules.Elements
{
    public class VLine : IElement
    {
        public Color Color { get; set; }

        public TDPoint Point1 { get; set; }
        public TDPoint Point2 { get; set; }

        public int Size { get; set; }

        private Camera _camera;

        public VLine(Camera camera, int x1, int y1, int z1, int x2, int y2, int z2, int size, Color color)
        {
            Point1 = new TDPoint(camera, x1, y1, z1);
            Point2 = new TDPoint(camera, x2, y2, z2);

            Size = size;
            Color = color;

            _camera = camera;

            this.ChangeProjection();
        }

        public VLine(Camera camera, TDPoint p1, TDPoint p2, int size, Color color)
        {
            Point1 = new TDPoint(camera, p1);
            Point2 = new TDPoint(camera, p2);

            Size = size;
            Color = color;

            _camera = camera;

            this.ChangeProjection();
        }

        public void ChangeProjection()
        {
            Point1.ChangeProjection();
            Point2.ChangeProjection();
        }

        public void SetNewPoint(TDPoint p1, TDPoint p2)
        {
            Point1 = p1;
            Point2 = p2;

            this.ChangeProjection();
        }

        public void ChangeLocationByRenderCoords(int dx, int dy)
        {
            switch (_camera.Plane)
            {
                case ProjectionPlane.XY: Point1.X += dx; Point2.X += dx; Point1.Y += dy; Point2.Y += dy; break;
                case ProjectionPlane.XZ: Point1.X += dx; Point2.X += dx; Point1.Z += dy; Point2.Z += dy; break;
                case ProjectionPlane.YZ: Point1.Y += dx; Point2.Y += dx; Point1.Z += dy; Point2.Z += dy; break;
            }
            this.ChangeProjection();
        }

        public override string ToString()
        {
            return $"Линия ({Point1.X};{Point1.Y};{Point1.Z})-" +
                $"({Point2.X};{Point2.Y};{Point2.Z}) [{Size}]";
        }

        public override bool Equals(object? obj)
        {
            return obj is VLine ln && Point1.Equals(ln.Point1) && Point2.Equals(ln.Point2)
                && this.Color == ln.Color && this.Size == ln.Size;
        }

        public VLine Clone()
        {
            return new VLine(this._camera, this.Point1, this.Point2, this.Size, this.Color);
        }
    }
}
