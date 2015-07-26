using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;


public class SnowPlow
{
    public int solve(string[] roads)
    {
        int[,] grid = new int[roads.Length, roads.Length];
        for (int i = 0; i < roads.Length; i++)
        {
            for (int j = 0; j < roads.Length; j++)
            {
                grid[i, j] = int.Parse("" + roads[i][j]);
            }
        }
        Dictionary<int, bool> reachable = new Dictionary<int, bool>();
        Queue<int> toCheck = new Queue<int>();
        toCheck.Enqueue(0);
        reachable[0] = true;
        while (toCheck.Count != 0)
        {
            int cur = toCheck.Dequeue();
            for (int i = 0; i < roads.Length; i++)
            {
                if (grid[cur, i] != 0)
                {
                    if (!reachable.ContainsKey(i))
                    {
                        toCheck.Enqueue(i);
                        reachable[i] = true;
                    }
                }
            }
        }
        for (int i = 0; i < roads.Length; i++)
        {
            for (int j = 0; j < roads.Length; j++)
            {
                if (grid[i, j] != 0 && (!reachable.ContainsKey(i) || !reachable.ContainsKey(j)))
                    return -1;
            }
        }
        int minTime = 0;
        for (int i = 0; i < roads.Length-1; i++)
        {
            for (int j = i+1; j < roads.Length; j++)
            {
                minTime += grid[i, j] * 2;
            }
        }

        return minTime;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            SnowPlow c = new SnowPlow();
            object o = c.solve(new string[] {"010000",
 "101000",
 "010100",
 "001010",
 "000101",
 "000010"});
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
