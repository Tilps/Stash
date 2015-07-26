using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class MuddyRoad
{
    public double getExpectedValue(int[] road)
    {
        double[] roadFracs = new double[road.Length+2];
        for (int i = 0; i < road.Length; i++)
        {
            roadFracs[i] = (double)road[i] / 100.0;
        }
        roadFracs[road.Length] = 0.0;
        roadFracs[road.Length+1] = 0.0;
        // current position, is dry, next is dry
        double[,,] dp = new double[road.Length + 1, 2, 2];
        for (int i = road.Length - 2; i >= 0; i--)
        {
            for (int j = 0; j < 2; j++)
            {
                double delta = j == 0 ? 1.0 : 0.0;
                for (int k = 0; k < 2; k++)
                {
                    if (k == 1)
                    {
                        dp[i, j, k] = dp[i + 1, k, 1] * (1.0 - roadFracs[i + 2]) + dp[i + 1, k, 0] * roadFracs[i + 2] + delta;
                    }
                    else
                    {
                        dp[i, j, k] = (dp[i + 2, 1, 0] * roadFracs[i + 3] + dp[i + 2, 1, 1] * (1.0 - roadFracs[i + 3]) + delta) * (1.0 - roadFracs[i + 2]);
                        dp[i, j, k] += Math.Min((dp[i + 2, 0, 0] * roadFracs[i + 3] + dp[i + 2, 0, 1] * (1.0 - roadFracs[i + 3]) + delta) * (roadFracs[i + 2]), 
                            (dp[i + 1, k, 0] + delta) * roadFracs[i + 2]);
                        // next is wet, so is one after - choose min of either.
                    }
                }
            }
        }
        return dp[0, 1, 1]*(1.0-roadFracs[1]) + dp[0, 1, 0]*roadFracs[1];
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            MuddyRoad c = new MuddyRoad();
            object o = c.getExpectedValue(new int[] {0,60, 60, 0});
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
