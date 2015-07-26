using System;
using System.Collections;
using System.Text;

public class CheapestFlights
{
    int[] visitCount;
    string[] prices;
    int end;

    private double go(int curLoc, int num)
    {
        if (num == 1)
        {
            if (curLoc == end)
                return -1;
            int basePrice = int.Parse(""+prices[curLoc][end]);
            double real = (double)basePrice / (double)(1 << visitCount[end]);
            return real;
        }
        else
        {
            double best = double.MaxValue;
            for (int i = 0; i < prices.Length; i++)
            {
                if (i == curLoc)
                    continue;
                int basePrice = int.Parse(""+prices[curLoc][i]);
                double real = (double)basePrice / (double)(1 << visitCount[i]);
                visitCount[i]++;
                double val = go(i, num - 1);
                if (val >= 0)
                {
                    val = val + real;
                    if (val < best)
                        best = val;
                }
                visitCount[i]--;
            }
            return best;
        }
    }

    public double getLowest(string[] prices, int startLocation, int endLocation, int num)
    {
        end = endLocation;
        this.prices = prices;
        visitCount = new int[prices.Length];
        double ans = go(startLocation, num);

        return ans;
    }
}

namespace TCO05QRQ2
{
    class Program
    {
        static void Main(string[] args)
        {
            CheapestFlights c = new CheapestFlights();
            object o = c.getLowest(new string[] { "11111111", "11111111", "11111111", "11111111", "11111111", "11111111", "11111111", "11111111" }, 0, 1, 8);

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