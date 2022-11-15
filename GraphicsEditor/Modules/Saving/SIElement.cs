using GraphicsEditor.Modules.Elements;
using System;
using System.Xml.Serialization;

namespace GraphicsEditor.Modules.Saving
{
    [XmlInclude(typeof(SVCurve))]
    [XmlInclude(typeof(SVGroup))]
    [XmlInclude(typeof(SVLine))]
    [XmlInclude(typeof(SVPoint))]
    public abstract class SIElement
    {
        public SColor Color { get; set; }

        public abstract IElement GetRealElement(Camera camera);

        public static SIElement GetSElement(IElement el)
        {
            if (el is VCurve curve)
            {
                return new SVCurve(curve);
            }
            else if (el is VGroup g)
            {
                return new SVGroup(g);
            }
            else if (el is VLine ln)
            {
                return new SVLine(ln);
            }
            else if (el is VPoint pt)
            {
                return new SVPoint(pt);
            }
            else
            {
                throw new Exception("Unsupported type");
            }
        }
    }
}
