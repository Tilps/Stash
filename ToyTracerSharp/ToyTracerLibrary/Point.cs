using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{
    public struct Point
    {
        public double X, Y, Z;

        public int Tag;


        public Point(double xIn, double yIn, double zIn)
        {
            X = xIn;
            Y = yIn;
            Z = zIn;
            Tag = 0;
        }

        public Vector LineTo(Point place)
        {
            return new Vector(place.X - X, place.Y - Y, place.Z - Z);
        }


        public Point MoveBy(Vector shift)
        {
            return new Point(X + shift.Dx, Y + shift.Dy, Z + shift.Dz);
        }
    }
    public struct PointF
    {
        public float X, Y, Z;
    }
}
