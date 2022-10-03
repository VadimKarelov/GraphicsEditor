using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEditor.Modules.Elements
{
    internal interface IElement
    {
        public Color Color { get; set; }

        public void ChangeProjection(Camera camera);

        public string ToString();

        public bool Equals(object? obj);
    }
}
