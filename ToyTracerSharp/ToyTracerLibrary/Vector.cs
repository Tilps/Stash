using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{
    public struct Vector
    {
        public double Dx, Dy, Dz;

        public Vector(double dx, double dy, double dz)
        {
            Dx = dx;
            Dy = dy;
            Dz = dz;
        }

        public double Dot(Vector other)
        {
            return Dx * other.Dx + Dy * other.Dy + Dz * other.Dz;
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(Dx * Dx + Dy * Dy + Dz * Dz);
            }
        }

        public bool IsZero
        {
            get
            {
                return Dx == 0 && Dy == 0 && Dz == 0;
            }
        }

        public Vector Cross(Vector other)
        {
            return new Vector(Dy * other.Dz - Dz * other.Dy, Dz * other.Dx - Dx * other.Dz, Dx * other.Dy - Dy * other.Dx);
        }

        public Vector Scale(double factor)
        {
            return new Vector(Dx * factor, Dy * factor, Dz * factor);
        }

        public void ScaleSelf(double factor)
        {
            Dx *= factor;
            Dy *= factor;
            Dz *= factor;
        }

        public void Add(Vector other)
        {
            Dx += other.Dx;
            Dy += other.Dy;
            Dz += other.Dz;
        }

    }

    public struct VectorF
    {
        public float Dx, Dy, Dz;
    }
}
