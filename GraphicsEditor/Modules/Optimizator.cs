using GraphicsEditor.Modules.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace GraphicsEditor.Modules
{
    internal class Optimizator
    {
        public bool Optimization(Camera camera, List<IElement> elements)
        {
            List<IElement> cpEl;
            List<IElement> toRemove = new();
            List<IElement> toAppend = new();

            // copy elements
            lock (elements)
            {
                cpEl = new List<IElement>(elements);
            }

            bool isOptimized = CleanR(camera, cpEl, toRemove, toAppend);

            lock (elements)
            {
                foreach (var item in toRemove)
                {
                    elements.Remove(item);
                }
                foreach (var item in toAppend)
                {
                    elements.Add(item);
                }
            }

            return isOptimized;
        }

        private bool CleanR(Camera camera, List<IElement> cpEl, List<IElement> toRemove, List<IElement> toAppend)
        {
            bool isOptimized = false;

            for (int i = cpEl.Count - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (cpEl[i].Equals(cpEl[j]))
                        {
                            toRemove.Add(cpEl[j]);
                            isOptimized = true;
                        }
                    }
                }
                if (cpEl[i] is VCurve cl)
                {
                    if (!cl.Points.Any())
                    {
                        toRemove.Add(cpEl[i]);
                        isOptimized = true;
                    }
                    else if (cl.Points.Count == 1)
                    {
                        toAppend.Add(new VPoint(camera, cl.Points[0], cl.Size, cl.Color));
                        toRemove.Add(cpEl[i]);
                        isOptimized = true;
                    }
                }
                else if (cpEl[i] is VLine ln)
                {
                    if (ln.Point1.Equals(ln.Point2))
                    {
                        toRemove.Add(cpEl[i]);
                        isOptimized = true;
                    }
                }
                else if (cpEl[i] is VGroup grp)
                {
                    isOptimized = CleanR(camera, grp.Elements, toRemove, toAppend);
                }
            }

            return isOptimized;
        }
    }
}
