using System;
using System.Collections;
using System.Text;

public class RectangleError
{

    private double length(double left, double top, double right) {
        double res = Math.Sqrt(top * top + (left - right) * (left - right));
        return res;
    }

    public double bottomRange(double topMin, double topMax, double leftMin, double leftMax, double rightMin, double rightMax)
    {
        double max = double.MinValue;
        double min = double.MaxValue;
        double test = length(leftMax, topMax, rightMax);
        if (test > max)
            max = test;
        if (test < min)
            min = test;
        test = length(leftMax, topMax, rightMin);
        if (test > max)
            max = test;
        if (test < min)
            min = test;
        test = length(leftMax, topMin, rightMax);
        if (test > max)
            max = test;
        if (test < min)
            min = test;
        test = length(leftMax, topMin, rightMin);
        if (test > max)
            max = test;
        if (test < min)
            min = test;
        test = length(leftMin, topMax, rightMax);
        if (test > max)
            max = test;
        if (test < min)
            min = test;
        test = length(leftMin, topMax, rightMin);
        if (test > max)
            max = test;
        if (test < min)
            min = test;
        test = length(leftMin, topMin, rightMax);
        if (test > max)
            max = test;
        if (test < min)
            min = test;
        test = length(leftMin, topMin, rightMin);
        if (test > max)
            max = test;
        if (test < min)
            min = test;
        if (leftMin <= rightMax && rightMin <= leftMax)
        {
            test = topMin;
            if (test < min)
                min = test;
        }
        return max-min;
    }
}

namespace TCO05R1Q1
{
    class Program
    {
        static void Main(string[] args)
        {
            RectangleError c = new RectangleError();
            object o = c.bottomRange(10,20,30,40,35,45);

            if (o is IEnumerable)
            {
                foreach (object oi in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(oi);

                }
            }
            else
                System.Console.Out.WriteLine(o);
            System.Console.In.ReadLine();
        }
    }
}