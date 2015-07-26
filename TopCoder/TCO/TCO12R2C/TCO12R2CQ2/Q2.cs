using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class ThreePoints
{
    private class Point {
        public int X;
        public int Y;
    }
    private class PointComparer : IComparer<Point>
    {
        public int Compare(Point x, Point y)
        {
            return (x.X + x.Y).CompareTo(y.X + y.Y);
        }
    }

    public long countColoring(int N, int xzero, int xmul, int xadd, int xmod, int yzero, int ymul, int yadd, int ymod)
    {
        long[] x = new long[N];
        x[0] = (long)xzero;
        for (int i = 1; i < N; i++)
        {
            x[i] = (x[i - 1] * (long)xmul + (long)xadd) % (long)xmod;
        }
        long[] y = new long[N];
        y[0] = (long)yzero;
        for (int i = 1; i < N; i++)
        {
            y[i] = (y[i - 1] * (long)ymul + (long)yadd) % (long)ymod;
        }
        Point[] points = new Point[N];
        for (int i = 0; i < x.Length; i++)
        {
            points[i] = new Point() { X = (int)x[i], Y = (int)y[i] };
        }
        Array.Sort(points, new PointComparer());

        return 0;
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            ThreePoints c = new ThreePoints();
            object o = c.countColoring(9,3,8,6,11,5,7,8,11);
            PrintObj(o);
            System.Console.In.ReadLine();
        }

        private static void PrintObj(object o)
        {
            if (o is IEnumerable)
            {
                foreach (object b in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(b);
                }
            }
            else
                System.Console.Out.WriteLine(o);
        }
    }
}
