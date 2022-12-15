using GraphicsEditor.Modules.Elements;
using System;

namespace GraphicsEditor.Modules.Tools
{
    internal class MGroup : M
    {
        private M[] _ms;
        private VGroup _element;

        public MGroup(VGroup grp)
        {
            _ms = new M[grp.Elements.Count];
            for (int i = 0; i < grp.Elements.Count; i++)
            {
                if (grp.Elements[i] is VLine ln)
                {
                    _ms[i] = new M(ln);
                }
                else if (grp.Elements[i] is VGroup group)
                {
                    _ms[i] = new MGroup(group);
                }
                else
                {
                    throw new Exception("Unsupported type");
                }
            }
            _element = grp;
        }

        public VGroup GetGroup()
        {
            _element.Elements.Clear();

            foreach (M m in _ms)
            {
                if (m is MGroup mg)
                {
                    VGroup gr = mg.GetGroup();
                    if (gr != null)
                    {
                        _element.Elements.Add(gr);
                    }
                    else
                    {
                        throw new Exception("Unsupported type.");
                    }
                }
                else
                {
                    VLine ln = m.GetLine();
                    if (ln != null)
                    {
                        _element.Elements.Add(ln);
                    }
                    else
                    {
                        throw new Exception("Unsupported type.");
                    }
                }
            }

            return _element;
        }

        public override void Transition(int dx, int dy, int dz)
        {
            foreach (M m in _ms)
            {
                m.Transition(dx, dy, dz);
            }
        }

        /// <summary>
        /// All angles in degrees
        /// </summary>
        public override void Rotation(double ax, double ay, double az)
        {
            foreach (M m in _ms)
            {
                m.Rotation(ax, ay, az);
            }
        }

        /// <summary>
        /// More than 1 -> bigger, less than 1 -> smaller
        /// </summary>
        public override void Scaling(double sx, double sy, double sz)
        {
            foreach (M m in _ms)
            {
                m.Scaling(sx, sy, sz);
            }
        }

        /// <param name="x">Whether it is necessary to reflect on x</param>
        /// <param name="y">Whether it is necessary to reflect on y</param>
        /// <param name="z">Whether it is necessary to reflect on z</param>
        public override void Reflection(bool x, bool y, bool z)
        {
            foreach (M m in _ms)
            {
                m.Reflection(x, y, z);
            }
        }

        /// <summary>
        /// All angles (fi & teta) in degrees
        /// </summary>
        public override void TrimetricProjection(double fi, double teta, double z)
        {
            foreach (M m in _ms)
            {
                m.TrimetricProjection(fi, teta, z);
            }
        }
    }
}
