using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class FoxAndDoraemon
{
    public int minTime(int[] workCost, int splitCost)
    {
        List<int> costs = new List<int>(workCost);
        while (costs.Count > 1)
        {
            costs.Sort();
            costs.Reverse();
            costs[costs.Count - 2] += splitCost;
            costs.RemoveAt(costs.Count - 1);
        }
        return costs[0];
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            FoxAndDoraemon c = new FoxAndDoraemon();
            object o = c.minTime(new int[] { 1000, 100, 10, 1 }, 1);
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
