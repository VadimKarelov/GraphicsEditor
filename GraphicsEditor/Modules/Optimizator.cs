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
            //List<IElement> cpEl;
            
            //// copy elements
            //lock (elements)
            //{
            //    cpEl = new List<IElement>(elements);
            //}

            bool isOptimizing = CleanR(camera, elements);

            return isOptimizing;
        }

        private bool CleanR(Camera camera, List<IElement> cpEl)
        {
            bool isOptimizing = false;

            List<IElement> toRemove = new();
            List<IElement> toAppend = new();

            for (int i = cpEl.Count - 1; i >= 0; i--)
            {
                if (i > 0)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (cpEl[i].Equals(cpEl[j]))
                        {
                            toRemove.Add(cpEl[j]);
                        }
                    }
                }
                if (cpEl[i] is VCurve cl)
                {
                    if (!cl.Points.Any())
                    {
                        toRemove.Add(cpEl[i]);
                    }
                    else if (cl.Points.Count == 1)
                    {
                        toAppend.Add(new VPoint(camera, cl.Points[0], cl.Size, cl.Color));
                        toRemove.Add(cpEl[i]);
                    }
                }
                else if (cpEl[i] is VLine ln)
                {
                    if (ln.Point1.Equals(ln.Point2))
                    {
                        toRemove.Add(cpEl[i]);
                    }
                }
                else if (cpEl[i] is VGroup grp)
                {
                    if (!grp.Elements.Any())
                    {
                        toRemove.Add(cpEl[i]);
                    }
                    else
                    {
                        isOptimizing = isOptimizing || CleanR(camera, grp.Elements);
                    }
                }
            }

            isOptimizing = isOptimizing || toRemove.Any() || toAppend.Any();

            if (isOptimizing)
            {
                lock (cpEl)
                {
                    foreach (var item in toRemove)
                    {
                        cpEl.Remove(item);
                    }
                    foreach (var item in toAppend)
                    {
                        cpEl.Add(item);
                    }
                }
            }

            return isOptimizing;
        }
    }
}
