using System;
using System.Collections.Generic;
using System.Drawing;

namespace GraphicsEditor.Modules.Elements
{
    public class VGroup : IElement
    {
        public Color Color { get; set; }
        public List<IElement> Elements => _elements;

        private List<IElement> _elements;
        private Camera _camera;

        public VGroup(Camera camera)
        {
            _camera = camera;
            _elements = new();
            Color = RandomColor();
        }

        public void ChangeProjection()
        {
            foreach (var element in _elements)
            {
                element.ChangeProjection();
            }
        }

        public void AddElement(IElement element)
        {
            _elements.Add(element);
        }

        public override string ToString()
        {
            string res = $"Группа ({_elements.Count})";
            foreach (var element in _elements)
            {
                res += $"\n-{element}";
            }
            return res;
        }

        public override bool Equals(object? obj)
        {
            return obj is VGroup group && this._elements.Equals(group._elements);
        }

        public object Clone()
        {
            VGroup res = new VGroup(this._camera);
            foreach (IElement element in this._elements)
            {
                res.AddElement(element.Clone() as IElement);
            }
            return res;
        }

        private Color RandomColor()
        {
            Random random = new Random();
            byte a = (byte)random.Next(0, 256);
            byte r = (byte)random.Next(0, 256);
            byte g = (byte)random.Next(0, 256);
            byte b = (byte)random.Next(0, 256);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
