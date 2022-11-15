using System.Drawing;

namespace GraphicsEditor.Modules.Elements
{
    public class VPoint : IElement
    {
        public Color Color { get; set; }

        public TDPoint Point { get; set; }

        public int Size { get; set; }

        private Camera _camera;

        public VPoint(Camera camera, int x, int y, int z, int size, Color cl)
        {
            Point = new TDPoint(camera, x, y, z);
            Size = size;
            Color = cl;
            _camera = camera;

            this.ChangeProjection();
        }

        public VPoint(Camera camera, TDPoint pt, int size, Color cl)
        {
            Point = new TDPoint(camera, pt);
            Size = size;
            Color = cl;
            _camera = camera;

            this.ChangeProjection();
        }

        public void ChangeProjection()
        {
            _camera.ChangeProjection(this);
        }

        public override string ToString()
        {
            return $"Точка ({Point.X};{Point.Y};{Point.Z}) [{Size}]";
        }

        public override bool Equals(object? obj)
        {
            return obj is VPoint pt && this.Point.Equals(pt.Point)
                && this.Color == pt.Color && this.Size == pt.Size;
        }

        public object Clone()
        {
            return new VPoint(_camera, new TDPoint(_camera, Point), Size, Color);
        }
    }
}
