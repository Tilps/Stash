using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class Bitwisdom
{
    public double expectedActions(int[] p)
    {
        double[,] dp = new double[p.Length+1,2];
        dp[0, 0] = 0;
        dp[0, 1] = 0;
        for (int i = 1; i < p.Length; i++)
        {
            dp[i, 1] = (dp[i - 1, 0] + 1) * (100-p[i - 1]) / 100.0 + dp[i - 1, 1] * (p[i - 1]) / 100.0;
            dp[i, 0] = (dp[i - 1, 1] + 1) * p[i - 1] / 100.0 + dp[i - 1, 0] * (100 - p[i - 1]) / 100.0;
        }
        double probAll1 = 1.0;
        for (int i = 0; i < p.Length; i++)
        {
            probAll1 *= p[i]/100.0;
        }
        double ans = probAll1 + dp[p.Length - 1, 0]*(100 - p[p.Length - 1])/100.0 +
                     dp[p.Length - 1, 1]*p[p.Length - 1]/100.0;

        return ans;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            Bitwisdom c = new Bitwisdom();
            object o = c.expectedActions(new int[] {50, 50});
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

