using GraphicsEditor.Modules.Elements;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GraphicsEditor.Modules.Saving
{
    public class SVCurve : SIElement
    {
        public List<STDPoint> Points { get; set; }
        public Point[] RenderPoints { get; set; }

        public int Size { get; set; }

        public SVCurve() { }

        public SVCurve(VCurve curve)
        {
            Points = curve.Points.Select(x => new STDPoint(x)).ToList();
            RenderPoints = curve.RenderPoints;
            Size = curve.Size;
            Color = new SColor(curve.Color);
        }

        public override IElement GetRealElement(Camera camera)
        {
            var res = new VCurve(camera, Size, Color.GetRealElement());
            res.Points = Points.Select(x => x.GetRealElement(camera)).ToList();
            res.RenderPoints = RenderPoints;
            return res;
        }
    }
}
