using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEditor.Modules.Elements
{
    internal class VPoint : IElement
    {
        public Color Color { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int RenderX { get; set; }
        public int RenderY { get; set; }

        public int Size { get; set; }

        public VPoint(int x, int y, int size, Color cl)
        {
            RenderX = x - size / 2;
            RenderY = y - size / 2;
            X = x;
            Y = y;
            Size = size;
            Color = cl;
        }
    }
}
