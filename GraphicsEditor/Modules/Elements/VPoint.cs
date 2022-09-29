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
        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }

        public int Size => _size;

        private int _size;

        public VPoint(int x, int y, int size, Color cl)
        {
            X = x - size / 2;
            Y = y - size / 2;
            _size = size / 2;
            Color = cl;
        }
    }
}
