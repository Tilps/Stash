using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Defects
{
    public double maxSum(int w, int h, double[] defectws, double[] defecths)
    {
        double fullLength = (double)(2 * w + 2 * h);
        double[] defectPos = new double[defectws.Length];
        for (int i = 0; i < defectPos.Length; i++)
        {
            if (defecths[i] <= 0.0)
                defectPos[i] = defectws[i];
            else if (defecths[i] >= h)
                defectPos[i] = 2 * w + h - defectws[i];
            else if (defectws[i] >= w)
                defectPos[i] = w + defecths[i];
            else
                defectPos[i] = 2 * w + 2 * h - defecths[i];
        }
        List<double> testPoints = new List<double>();
        for (int i = 0; i < defectPos.Length; i++)
        {
            testPoints.Add(defectPos[i]);
            testPoints.Add(Wrap(fullLength / 2.0 + defectPos[i], fullLength));
        }
        testPoints.Sort();
        double max = 0.0;
        for (int i = 0; i < testPoints.Count; i++)
        {
            double localMax = Test(testPoints[i], defectPos, fullLength);
            if (localMax > max)
                max = localMax;
        }
        return max;
    }

    private double Test(double a, double[] defectPos, double fullLength)
    {
        double sum = 0.0;
        for (int i = 0; i < defectPos.Length; i++)
        {
            double diff = Math.Abs(a - defectPos[i]);
            if (diff < fullLength / 2.0)
                sum += diff;
            else
                sum += fullLength - diff;
        }
        return sum;
    }

    private double Wrap(double p, double fullLength)
    {
        if (p < fullLength)
            return p;
        else
            return p - fullLength;
    }
}

namespace Q1
{
    class Program
    {
        static void Main(string[] args)
        {
            Defects c = new Defects();
            object o = c.maxSum(75, 20, new double[] { 0, 0, 35, 49, 75 }, new double[] { 15, 20, 0, 20, 6.2934 });
            PrintObj(o, 0);
            System.Console.In.ReadLine();
        }

        private static void PrintObj(object o, int depth)
        {
            System.Console.Out.Write(new string(' ', depth));
            if (o is string)
            {
                System.Console.Out.WriteLine(o);
            }
            else if (o is IEnumerable)
            {
                System.Console.Out.WriteLine("List:");
                foreach (object b in (IEnumerable)o)
                {
                    PrintObj(b, depth + 1);
                }
            }
            else
                System.Console.Out.WriteLine(o);
        }
    }
}
