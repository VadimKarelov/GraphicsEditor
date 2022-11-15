using GraphicsEditor.Modules.Elements;
using System.Collections.Generic;

namespace GraphicsEditor.Modules.Saving
{
    public class SVGroup : SIElement
    {
        public List<SIElement> Elements { get; set; }

        public SVGroup() { }

        public SVGroup(VGroup grp)
        {
            Color = new SColor(grp.Color);
            Elements = new();
            foreach (var element in grp.Elements)
            {
                if (element is VCurve curve)
                {
                    Elements.Add(new SVCurve(curve));
                }
                else if (element is VGroup g)
                {
                    Elements.Add(new SVGroup(g));
                }
                else if (element is VLine ln)
                {
                    Elements.Add(new SVLine(ln));
                }
                else if (element is VPoint pt)
                {
                    Elements.Add(new SVPoint(pt));
                }
            }
        }

        public override IElement GetRealElement(Camera camera)
        {
            var res = new VGroup(camera);
            foreach (var element in Elements)
            {
                res.AddElement(element.GetRealElement(camera));
            }
            return res;
        }
    }
}
