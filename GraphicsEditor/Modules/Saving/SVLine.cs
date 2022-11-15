using GraphicsEditor.Modules.Elements;

namespace GraphicsEditor.Modules.Saving
{
    public class SVLine : SIElement
    {
        public STDPoint Point1 { get; set; }
        public STDPoint Point2 { get; set; }

        public int Size { get; set; }

        public SVLine() { }

        public SVLine(VLine ln)
        {
            Color = new SColor(ln.Color);
            Point1 = new STDPoint(ln.Point1);
            Point2 = new STDPoint(ln.Point2);
            Size = ln.Size;
        }

        public override IElement GetRealElement(Camera camera)
        {
            return new VLine(camera, Point1.GetRealElement(camera), Point2.GetRealElement(camera), Size, Color.GetRealElement());
        }
    }
}
