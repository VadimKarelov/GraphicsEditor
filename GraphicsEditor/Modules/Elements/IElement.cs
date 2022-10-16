using System.Drawing;

namespace GraphicsEditor.Modules.Elements
{
    public interface IElement
    {
        public Color Color { get; set; }

        public void ChangeProjection();

        public string ToString();

        public bool Equals(object? obj);
    }
}
