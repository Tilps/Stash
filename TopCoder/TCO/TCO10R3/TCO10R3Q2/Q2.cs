using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class TheChroniclesOfAmber
{
    public double minimumTime(int[] princeX, int[] princeY, int[] destinationX, int[] destinationY)
    {
        double[,] distances = new double[princeX.Length, princeX.Length];
        for (int i = 0; i < princeX.Length; i++)
        {
            for (int j = 0; j < princeX.Length; j++)
            {
                // Time to get to destination for i, from location of j.
                distances[i, j] = Distance(princeX[j], princeY[j], destinationX[i], destinationY[i]);
            }
        }
        // Best Time to get to finish state for first N princes, where the jth prince cannot be teleported to.
        double[,] dp = new double[princeX.Length+1, princeX.Length];
        for (int i = 1; i <= princeX.Length; i++)
        {
            for (int j = 0; j < princeX.Length; j++)
            {
                double start = distances[i - 1, i - 1];
                double prev = dp[i - 1, j];
                for (int k = 0; k < princeX.Length; k++)
                {
                    if (k == j)
                        continue;
                    if (distances[i - 1, k] < start)
                        start = distances[i - 1, k];
                }
                dp[i, j] = Math.Max(prev, start);
            }
        }
        double best = double.MaxValue;
        for (int j = 0; j < princeX.Length; j++)
        {
            if (dp[princeX.Length, j] < best)
                best = dp[princeX.Length, j];
        }
        return best;
    }

    private double Distance(int p, int p_2, int p_3, int p_4)
    {
        int dx = p - p_3;
        int dy = p_2 - p_4;
        return Math.Sqrt(dx * dx + dy * dy);
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            TheChroniclesOfAmber c = new TheChroniclesOfAmber();
            object o = c.minimumTime(new int[] { 1, 5, 5 }, new int[] { 0, 0, 0 }, new int[] { 1, 1, 0 }, new int[] { 4, 2, 3 });
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
