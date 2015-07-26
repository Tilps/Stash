using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class DancingFoxes
{
    public int minimalDays(string[] friendship)
    {
        int[] distances = new int[friendship.Length];
        for (int i = 0; i < distances.Length; i++)
            distances[i] = int.MaxValue;
        distances[0] = 0;
        Dictionary<int, bool> seen = new Dictionary<int, bool>();
        Queue<int> search = new Queue<int>();
        search.Enqueue(0);
        seen[0] = true;
        while (search.Count != 0)
        {
            int cur = search.Dequeue();
            for (int i = 0; i < friendship[cur].Length; i++)
            {
                if (friendship[cur][i] == 'Y' && !seen.ContainsKey(i))
                {
                    distances[i] = distances[cur] + 1;
                    search.Enqueue(i);
                    seen[i] = true;
                }

            }
        }
        if (distances[1] == int.MaxValue)
            return -1;
        int shortest = distances[1];
        List<int> path = new List<int>();
        for (int i = 0; i < shortest+1; i++)
            path.Add(i);
        int dances = 0;
        while (path.Count > 2)
        {
            dances++;
            List<int> newPath = new List<int>();
            int lastUsed = -1;
            for (int i = 2; i < path.Count; i+=3)
            {
                newPath.Add(path[i-2]);
                newPath.Add(path[i]);
                lastUsed = i;
            }
            lastUsed++;
            while (lastUsed < path.Count)
            {
                newPath.Add(path[lastUsed]);
                lastUsed++;
            }
            path = newPath;
        }
        return dances;
    }
}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            DancingFoxes c = new DancingFoxes();
            object o = c.minimalDays( new string[]{"NNY",
 "NNY",
 "YYN"});
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
