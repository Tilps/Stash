using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Alarmed
{
    public double noise(int[] x, int[] y, int[] threshold)
    {
        int largestThreshold = 0;
        for (int i = 0; i < threshold.Length; i++)
            if (threshold[i] > largestThreshold)
                largestThreshold = threshold[i];
        double maxA = 10000.0 * largestThreshold;
        double minA = 0.0;
        for (int cuts = 0; cuts < 35; cuts++)
        {
            double midA = (maxA + minA) / 2.0;
            if (CanBeDone(midA, x, y, threshold))
            {
                minA = midA;
            }
            else
            {
                maxA = midA;
            }
        }
        return (maxA+minA)/2.0;
    }

    private bool CanBeDone(double midA, int[] x, int[] y, int[] threshold)
    {
        //List<Line> lines = new List<Line>();
        for (int i = 0; i < x.Length; i++)
        {

        }
        return false;
    }
}

namespace Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            Alarmed c = new Alarmed();
            object o = c.noise(new int[] {}, new int[] {}, new int[] {});
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
