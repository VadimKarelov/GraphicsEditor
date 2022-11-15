using GraphicsEditor.Modules.Elements;

namespace GraphicsEditor.Modules.Saving
{
    public class STDPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public STDPoint() { }

        public STDPoint(TDPoint point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        public TDPoint GetRealElement(Camera camera)
        {
            return new TDPoint(camera, X, Y, Z);
        }
    }
}
