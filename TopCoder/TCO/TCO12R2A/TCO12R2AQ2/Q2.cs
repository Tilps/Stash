using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class CucumberWatering
{
    public long theMin(int[] x, int K)
    {
        if (K >= x.Length)
            return 0;
        if (K < 2)
        {
            long total = 0;
            for (int i = 1; i < x.Length; i++)
            {
                total += Math.Abs((long)x[i] - (long)x[i - 1]);
            }
            return total;
        }
        bool[] teleports = new bool[x.Length];
        List<int> teleportSpots = new List<int>();
        int bestI = -1;
        int bestJ = -1;
        long bestLength = long.MaxValue;
        for (int i = 0; i < x.Length; i++)
        {
            for (int j = i + 1; j < x.Length; j++)
            {
                teleportSpots.Add(x[i]);
                teleportSpots.Add(x[j]);
                long dist = Simulate(x, teleportSpots);
                if (dist < bestLength)
                {
                    bestI = i;
                    bestJ = j;
                    bestLength = dist;
                }
                teleportSpots.Clear();
            }
        }
        teleports[bestI] = true;
        teleports[bestJ] = true;
        teleportSpots.Add(x[bestI]);
        teleportSpots.Add(x[bestJ]);
        K -= 2;
        while (K > 0)
        {
            bestI = -1;
            bestLength = long.MaxValue;
            for (int i = 0; i < x.Length; i++)
            {
                if (teleports[i])
                    continue;
                teleportSpots.Add(x[i]);
                long dist = Simulate(x, teleportSpots);
                if (dist < bestLength)
                {
                    bestLength = dist;
                    bestI = i;
                }
                teleportSpots.RemoveAt(teleportSpots.Count - 1);
            }
            teleports[bestI] = true;
            K--;
            teleportSpots.Add(x[bestI]);
        }
        return Simulate(x, teleportSpots);
    }

    private long Simulate(int[] x, List<int> teleportSpots)
    {
        long total = 0;
        for (int i = 1; i < x.Length; i++)
        {
            long straightDist = Math.Abs((long)x[i] - (long)x[i - 1]);
            long teleportDist = long.MaxValue;
            if (teleportSpots.Count > 0)
            {
                long bestDist1 = long.MaxValue;
                long bestDist2 = long.MaxValue;
                foreach (int spot in teleportSpots)
                {
                    long dist1 = Math.Abs((long)x[i] - (long)spot);
                    if (dist1 < bestDist1)
                        bestDist1 = dist1;
                    long dist2 = Math.Abs((long)x[i-1] - (long)spot);
                    if (dist2 < bestDist2)
                        bestDist2 = dist2;
                }
                teleportDist = bestDist1 + bestDist2;
            }
            total += Math.Min(straightDist, teleportDist);
        }
        return total;
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            CucumberWatering c = new CucumberWatering();
            object o = c.theMin(new int[] { 0, 6, 8, 2 }, 2);
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
