using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{
    public struct Line
    {
        public Point Start;

        public Vector Direct;

        public Point Project(double t)
        {
            Point p;
            p.X = Start.X + t * Direct.Dx;
            p.Y = Start.Y + t * Direct.Dy;
            p.Z = Start.Z + t * Direct.Dz;
            p.Tag = 0;
            return p;
        }

    }
}
