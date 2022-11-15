using GraphicsEditor.Modules.Elements;

namespace GraphicsEditor.Modules.Saving
{
    public class SVPoint : SIElement
    {
        public STDPoint Point { get; set; }

        public int Size { get; set; }

        public SVPoint() { }

        public SVPoint(VPoint point)
        {
            Color = new SColor(point.Color);
            Size = point.Size;
            Point = new STDPoint(point.Point);
        }

        public override IElement GetRealElement(Camera camera)
        {
            return new VPoint(camera, Point.GetRealElement(camera), Size, Color.GetRealElement());
        }
    }
}
