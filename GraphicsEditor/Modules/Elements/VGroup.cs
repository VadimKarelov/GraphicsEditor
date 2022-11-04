using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEditor.Modules.Elements
{
    internal class VGroup : IElement
    {
        public Color Color { get; set; }
        public List<IElement> Elements => _elements;

        private List<IElement> _elements;
        private Camera _camera;

        public VGroup(Camera camera)
        {
            _camera = camera;
            Color = Color.White;
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
            return $"VGroup ({_elements.Count})";
        }

        public override bool Equals(object? obj)
        {
            return obj is VGroup group && this._elements.Equals(group._elements);
        }

        public VGroup Clone()
        {
            return new VGroup(this._camera);
        }
    }
}
