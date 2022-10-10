using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEditor.Modules.Elements
{
    /// <summary>
    /// Represents a 3D point
    /// </summary>
    internal class TDPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public int RenderX { get; set; }
        public int RenderY { get; set; }

        public TDPoint(Camera camera, int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
            this.ChangeProjection(camera);
        }

        public void ChangeProjection(Camera camera)
        {
            camera.ChangeProjection(this);
        }

        public override bool Equals(object? obj)
        {
            return obj is TDPoint pt && this.X == pt.X && this.Y == pt.Y && this.Z == pt.Z;
        }
    }
}
