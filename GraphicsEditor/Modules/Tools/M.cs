using GraphicsEditor.Modules.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsEditor.Modules.Tools
{
    internal class M
    {
        private double[][] _mat;
        private IElement _element;

        public M(VLine ln)
        {
            double[] p1 = { ln.Point1.X, ln.Point1.Y, ln.Point1.Z, 1 };
            double[] p2 = { ln.Point2.X, ln.Point2.Y, ln.Point2.Z, 1 };
            _mat = new double[2][];
            _mat[0] = p1;
            _mat[1] = p2;
            _element = ln;
        }

        public VLine GetLine()
        {
            VLine ln = _element as VLine;
            if (ln != null)
            {
                ln.Point1.X = (int)_mat[0][0];
                ln.Point1.Y = (int)_mat[0][1];
                ln.Point1.Z = (int)_mat[0][2];

                ln.Point2.X = (int)_mat[1][0];
                ln.Point2.Y = (int)_mat[1][1];
                ln.Point2.Z = (int)_mat[1][2];

                return ln;
            }
            else
            {
                throw new Exception("Type of M is not VLine.");
            }
        }

        public void Transition(int dx, int dy, int dz)
        {
            _mat = Multiplication(_mat, GetTransitionMatrix(dx, dy, dz));
            Normalization();
        }

        /// <summary>
        /// All angles in degrees
        /// </summary>
        public void Rotation(double ax, double ay, double az)
        {
            _mat = Multiplication(_mat, GetRotationMatrix(ax, ay, az));
            Normalization();
        }

        /// <summary>
        /// More than 1 -> bigger, less than 1 -> smaller
        /// </summary>
        public void Scaling(double sx, double sy, double sz)
        {
            _mat = Multiplication(_mat, GetScalingMatrix(sx, sy, sz));
            Normalization();
        }

        private static double[][] Multiplication(double[][] a, double[][] b)
        {
            if (a[0].Length != b.Length) 
                throw new Exception("Number of columns in mat 1 must be equal to row number in mat2");

            double[][] r = new double[a.Length][];

            for (int i = 0; i < a.Length; i++)
            {
                r[i] = new double[b[0].Length];
                for (int j = 0; j < b[0].Length; j++)
                {
                    for (int k = 0; k < b.Length; k++)
                    {
                        r[i][j] += a[i][k] * b[k][j];
                    }
                }
            }

            return r;
        }

        private void Normalization()
        {
            for (int i = 0; i < _mat.Length; i++)
            {
                _mat[i][0] = _mat[i][0] / _mat[i][3];
                _mat[i][1] = _mat[i][1] / _mat[i][3];
                _mat[i][2] = _mat[i][2] / _mat[i][3];
            }
        }

        private static double[][] GetTransitionMatrix(int dx, int dy, int dz)
        {
            double[] r1 = { 1, 0, 0, 0 };
            double[] r2 = { 0, 1, 0, 0 };
            double[] r3 = { 0, 0, 1, 0 };
            double[] r4 = { dx, dy, dz, 1 };
            double[][] res = new double[4][];
            res[0] = r1;
            res[1] = r2;
            res[2] = r3;
            res[3] = r4;
            return res;
        }

        /// <summary>
        /// All angles in degrees
        /// </summary>
        private static double[][] GetRotationMatrix(double ax, double ay, double az)
        {
            return Multiplication(Multiplication(Rx(ax * Math.PI / 180), Ry(ay * Math.PI / 180)), Rz(az * Math.PI / 180));
        }

        private static double[][] Rx(double angle)
        {
            double[] r1 = { 1, 0, 0, 0 };
            double[] r2 = { 0, Math.Cos(angle), Math.Sin(angle), 0 };
            double[] r3 = { 0, -Math.Sin(angle), Math.Cos(angle), 0 };
            double[] r4 = { 0, 0, 0, 1 };
            double[][] res = new double[4][];
            res[0] = r1;
            res[1] = r2;
            res[2] = r3;
            res[3] = r4;
            return res;
        }
        private static double[][] Ry(double angle)
        {
            double[] r1 = { Math.Cos(angle), 0, -Math.Sin(angle), 0 };
            double[] r2 = { 0, 1, 0, 0 };
            double[] r3 = { 0, Math.Sin(angle), Math.Cos(angle), 0 };
            double[] r4 = { 0, 0, 0, 1 };
            double[][] res = new double[4][];
            res[0] = r1;
            res[1] = r2;
            res[2] = r3;
            res[3] = r4;
            return res;
        }
        private static double[][] Rz(double angle)
        {
            double[] r1 = { Math.Cos(angle), Math.Sin(angle), 0, 0 };
            double[] r2 = { -Math.Sin(angle), Math.Cos(angle), 0, 0 };
            double[] r3 = { 0, 0, 1, 0 };
            double[] r4 = { 0, 0, 0, 1 };
            double[][] res = new double[4][];
            res[0] = r1;
            res[1] = r2;
            res[2] = r3;
            res[3] = r4;
            return res;
        }

        private static double[][] GetScalingMatrix(double sx, double sy, double sz)
        {
            double[] r1 = { sx, 0, 0, 0 };
            double[] r2 = { 0, sy, 0, 0 };
            double[] r3 = { 0, 0, sz, 0 };
            double[] r4 = { 0, 0, 0, 1 };
            double[][] res = new double[4][];
            res[0] = r1;
            res[1] = r2;
            res[2] = r3;
            res[3] = r4;
            return res;
        }
    }
}
