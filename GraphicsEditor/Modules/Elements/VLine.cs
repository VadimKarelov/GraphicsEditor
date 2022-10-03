﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace GraphicsEditor.Modules.Elements
{
    internal class VLine : IElement
    {
        public Color Color { get; set; }

        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int Z1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int Z2 { get; set; }

        public int Size { get; set; }

        public int RenderX1 { get; set; }
        public int RenderY1 { get; set; }
        public int RenderX2 { get; set; }
        public int RenderY2 { get; set; }

        public VLine(Color color, int x1, int y1, int z1, int x2, int y2, int z2, int size)
        {
            Color = color;
            X1 = x1;
            Y1 = y1;
            Z1 = z1;
            X2 = x2;
            Y2 = y2;
            Z2 = z2;
            Size = size;

            RenderX1 = x1;
            RenderY1 = y1;
            RenderX2 = x2;
            RenderY2 = y2;
        }

        public void ChangeProjection(Camera camera)
        {
            camera.ChangeProjection(this);
        }
    }
}
