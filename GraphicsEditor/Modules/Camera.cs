using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEditor.Modules
{
    internal class Camera
    {
        public ProjectionPlane Plane { get; set; }

        public void ChangeProjection(int[,] matrix)
        {

        }
    }

    enum ProjectionPlane
    {
        XY,
        XZ,
        YZ,
        Other
    }
}
