using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{
    public class Matrix
    {
        public double[,] Values
        {
            get
            {
                return values;
            }
            set
            {
                values = value;
            }
        }
        double[,] values = new double[4, 4];

        public Matrix Inverse()
        {
            return new Matrix();
        }
        public Vector Apply(Vector other)
        {
            Vector v;
            v.Dx = other.Dx * values[0, 0] + other.Dy * values[0, 1] + other.Dz * values[0, 2];
            v.Dy = other.Dx * values[1, 0] + other.Dy * values[1, 1] + other.Dz * values[1, 2];
            v.Dz = other.Dx * values[2, 0] + other.Dy * values[2, 1] + other.Dz * values[2, 2];
            return v;
        }

        public Point Apply(Point other)
        {
            Point p;
            p.Tag = 0;
            p.X = other.X * values[0, 0] + other.Y * values[0, 1] + other.Z * values[0, 2] + values[0, 3];
            p.Y = other.X * values[1, 0] + other.Y * values[1, 1] + other.Z * values[1, 2] + values[1, 3];
            p.Z = other.X * values[2, 0] + other.Y * values[2, 1] + other.Z * values[2, 2] + values[2, 3];
            return p;
        }

        public Line Apply(Line other)
        {
            Line res = new Line();
            res.Direct = this.Apply(other.Direct);
            res.Start = this.Apply(other.Start);
            return res;
        }

        public Matrix MulBy(Matrix other)
        {
            Matrix res = new Matrix();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    res.values[i, j] =
                        values[i, 0] * other.values[0, j] +
                        values[i, 1] * other.values[1, j] +
                        values[i, 2] * other.values[2, j] +
                        values[i, 3] * other.values[3, j];
                }
            }
            return res;

        }

        public void MakeRot(double xrotIn, double yrotIn, double zrotIn)
        {
            Matrix res = new Matrix();
            Matrix xrot = new Matrix();
            Matrix yrot = new Matrix();
            Matrix zrot = new Matrix();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    xrot.values[i, j] = 0.0;
                    yrot.values[i, j] = 0.0;
                    zrot.values[i, j] = 0.0;
                    res.values[i, j] = 0.0;
                }
            res.values[0, 0] = 1;
            res.values[1, 1] = 1;
            res.values[2, 2] = 1;
            res.values[3, 3] = 1;
            if (xrotIn != 0)
            {
                xrot.values[0, 0] = 1;
                xrot.values[3, 3] = 1;
                xrot.values[1, 1] = Math.Cos(xrotIn);
                xrot.values[2, 2] = Math.Cos(xrotIn);
                xrot.values[1, 2] = -Math.Sin(xrotIn);
                xrot.values[2, 1] = Math.Sin(xrotIn);
                res = xrot.MulBy(res);
            }
            if (yrotIn != 0)
            {
                yrot.values[1, 1] = 1;
                yrot.values[3, 3] = 1;
                yrot.values[0, 0] = Math.Cos(yrotIn);
                yrot.values[2, 2] = Math.Cos(yrotIn);
                yrot.values[2, 0] = -Math.Sin(yrotIn);
                yrot.values[0, 2] = Math.Sin(yrotIn);
                res = yrot.MulBy(res);
            }
            if (zrotIn != 0)
            {
                zrot.values[2, 2] = 1;
                zrot.values[3, 3] = 1;
                zrot.values[1, 1] = Math.Cos(zrotIn);
                zrot.values[0, 0] = Math.Cos(zrotIn);
                zrot.values[0, 1] = -Math.Sin(zrotIn);
                zrot.values[1, 0] = Math.Sin(zrotIn);
                res = zrot.MulBy(res);
            }
            this.values = res.values;
        }

        public void MakeScale(double xscale, double yscale, double zscale)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    values[i, j] = 0.0;
            values[0, 0] = xscale;
            values[1, 1] = yscale;
            values[2, 2] = zscale;
            values[3, 3] = 1;
        }

        public void MakeTrans(double xtrans, double ytrans, double ztrans)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    values[i,j] = 0.0;
            values[0,0] = 1;
            values[1,1] = 1;
            values[2,2] = 1;
            values[0,3] = xtrans;
            values[1,3] = ytrans;
            values[2,3] = ztrans;
            values[3,3] = 1;
        }
    }
}
