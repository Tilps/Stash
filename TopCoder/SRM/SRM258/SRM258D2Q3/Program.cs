using System;
using System.Collections;
using System.Text;

public class MissileTarget
{
    public int[] bestFit(int[] x, int[] y)
    {
        int xtot = 0;
        for (int i = 0; i < x.Length; i++)
        {
            xtot += x[i];
        }
        int ytot = 0;
        for (int i = 0; i < y.Length; i++)
        {
            ytot += y[i];
        }
        int xav = (int)Math.Floor((double)xtot / x.Length);
        int yav = (int)Math.Floor((double)ytot / y.Length);
        double bestFit = double.MaxValue;
        int[] res = new int[2];
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                double fit = 0;
                for (int k = 0; k < x.Length; k++)
                {
                    fit += Math.Pow(x[k] - xav - i, 2.0);
                    fit += Math.Pow(y[k] - yav - j, 2.0);
                }
                if (fit < bestFit)
                {
                    res[0] = xav + i;
                    res[1] = yav + j;
                    bestFit = fit;
                }
            }
        }
        return res;
    }
}

namespace SRM258D2Q3
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
