using GraphicsEditor.Modules.Elements;
using System;

namespace GraphicsEditor.Modules
{
    internal class Camera
    {
        public ProjectionPlane Plane { get; set; }

        public Camera()
        {
            Plane = ProjectionPlane.XY;
        }

        public void ChangeProjection(object element)
        {
            switch (this.Plane)
            {
                case ProjectionPlane.XY: ProjectionXY(element); break;
                case ProjectionPlane.XZ: ProjectionXZ(element); break;
                case ProjectionPlane.YZ: ProjectionYZ(element); break;
                default: throw new Exception("Something went wrong");
            }
        }

        private void ProjectionXY(object element)
        {
            if (element is TDPoint tpt)
            {
                tpt.RenderX = tpt.X;
                tpt.RenderY = tpt.Y;
            }
            else if (element is VPoint pt)
            {
                pt.Point.RenderX = pt.Point.X - pt.Size / 2;
                pt.Point.RenderY = pt.Point.Y - pt.Size / 2;
            }
        }

        private void ProjectionXZ(object element)
        {
            if (element is TDPoint tpt)
            {
                tpt.RenderX = tpt.X;
                tpt.RenderY = tpt.Z;
            }
            else if (element is VPoint pt)
            {
                pt.Point.RenderX = pt.Point.X - pt.Size / 2;
                pt.Point.RenderY = pt.Point.Z - pt.Size / 2;
            }
        }

        private void ProjectionYZ(object element)
        {
            if (element is TDPoint tpt)
            {
                tpt.RenderX = tpt.Y;
                tpt.RenderY = tpt.Z;
            }
            else if (element is VPoint pt)
            {
                pt.Point.RenderX = pt.Point.Y - pt.Size / 2;
                pt.Point.RenderY = pt.Point.Z - pt.Size / 2;
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
