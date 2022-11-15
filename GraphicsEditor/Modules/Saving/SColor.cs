using System.Drawing;

namespace GraphicsEditor.Modules.Saving
{
    public class SColor
    {
        public int A { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        public SColor() { }

        public SColor(Color cl)
        {
            A = cl.A;
            R = cl.R;
            G = cl.G;
            B = cl.B;
        }

        public Color GetRealElement()
        {
            return Color.FromArgb(A, R, G, B);
        }
    }
}
