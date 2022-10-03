using GraphicsEditor.Modules.Elements;
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

        public Camera()
        {
            Plane = ProjectionPlane.XY;
        }

        public void ChangeProjection(IElement element)
        {
            switch (this.Plane)
            {
                case ProjectionPlane.XY: ProjectionXY(element); break;
                case ProjectionPlane.XZ: ProjectionXZ(element); break;
                case ProjectionPlane.YZ: ProjectionYZ(element); break;
                default: throw new Exception("Something went wrong");
            }
        }

        private void ProjectionXY(IElement element)
        {
            if (element is VPoint pt)
            {
                pt.RenderX = pt.X - pt.Size / 2;
                pt.RenderY = pt.Y - pt.Size / 2;
            }
            else if (element is VLine ln)
            {
                ln.RenderX1 = ln.X1;
                ln.RenderX2 = ln.X2;
                ln.RenderY1 = ln.Y1;
                ln.RenderY2 = ln.Y2;
            }
        }

        private void ProjectionXZ(IElement element)
        {
            if (element is VPoint pt)
            {
                pt.RenderX = pt.X - pt.Size / 2;
                pt.RenderY = pt.Z - pt.Size / 2;
            }
            else if (element is VLine ln)
            {
                ln.RenderX1 = ln.X1;
                ln.RenderX2 = ln.X2;
                ln.RenderY1 = ln.Z1;
                ln.RenderY2 = ln.Z2;
            }
        }

        private void ProjectionYZ(IElement element)
        {
            if (element is VPoint pt)
            {
                pt.RenderX = pt.Y - pt.Size / 2;
                pt.RenderY = pt.Z - pt.Size / 2;
            }
            else if (element is VLine ln)
            {
                ln.RenderX1 = ln.Y1;
                ln.RenderX2 = ln.Y2;
                ln.RenderY1 = ln.Z1;
                ln.RenderY2 = ln.Z2;
            }
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
