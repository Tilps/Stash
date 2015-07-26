using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class CliqueGraph
{
    public long calcSum(int N, int[] v, int[] sizes)
    {
        HashSet<int>[] clumps = new HashSet<int>[N];
        for (int i = 0; i < clumps.Length; i++) { clumps[i] = new HashSet<int>(); }
        int part = 0;
        for (int i = 0; i < sizes.Length; i++)
        {
            for (int j = part; j < part + sizes[i]; j++)
            {
                clumps[v[j]].Add(i);
            }
            part += sizes[i];
        }
        List<int>[] edges = new List<int>[N];
        for (int i = 0; i < N; i++)
        {
            if (clumps[i].Count == 1) continue;
            edges[i] = new List<int>();
            for (int j = i + 1; j < N; j++)
            {
                if (clumps[j].Count == 1) continue;
                if (clumps[i].Overlaps(clumps[j]))
                {
                    edges[i].Add(j);
                }
            }
        }
        long total = 0;
        for (int i = 0; i < N; i++)
        {
            if (clumps[i].Count == 1) continue;
            
        }
        return 0;
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            CliqueGraph c = new CliqueGraph();
            object o = c.calcSum();
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