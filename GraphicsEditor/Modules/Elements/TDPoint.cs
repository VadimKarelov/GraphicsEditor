namespace GraphicsEditor.Modules.Elements
{
    /// <summary>
    /// Represents a 3D point
    /// </summary>
    public class TDPoint
    {
        public int X { get { return _x; } set { _x = value; this.ChangeProjection(); } }
        public int Y { get { return _y; } set { _y = value; this.ChangeProjection(); } }
        public int Z { get { return _z; } set { _z = value; this.ChangeProjection(); } }

        public int RenderX { get; set; }
        public int RenderY { get; set; }

        private Camera _camera;

        private int _x;
        private int _y;
        private int _z;

        public TDPoint(Camera camera, int x, int y, int z)
        {
            _x = x;
            _y = y;
            _z = z;

            _camera = camera;

            this.ChangeProjection();
        }

        public TDPoint(Camera camera, TDPoint p)
        {
            _x = p.X;
            _y = p.Y;
            _z = p.Z;

            _camera = camera;

            this.ChangeProjection();
        }

        public void ChangeProjection()
        {
            _camera.ChangeProjection(this);
        }

        public override bool Equals(object? obj)
        {
            return obj is TDPoint pt && this.X == pt.X && this.Y == pt.Y && this.Z == pt.Z;
        }
    }
}
