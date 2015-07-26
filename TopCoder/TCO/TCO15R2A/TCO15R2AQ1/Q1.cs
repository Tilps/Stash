using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class ModModMod
{
    public long findSum(int[] m, int R)
    {
        List<int> desc = new List<int>();
        for (int i = 0; i < m.Length; i++)
        {
            if (i == 0 || m[i] < desc[desc.Count - 1]) { desc.Add(m[i]); }
        }
        SortedDictionary<int, List<int>> resets  =new SortedDictionary<int, List<int>>();
        foreach (int i in desc)
        {
            var basis = new List<int>();
            basis.Add(i);
            resets.Add(i, basis);
        }
        int curMin = desc[desc.Count - 1];
        int val = 1;
        long total = 0;
        for (int i = 1; i <= R; i++)
        {
            if (i == curMin)
            {
                val = 0;
                List<int> basis = resets[curMin];
                resets.Remove(curMin);
                foreach (int b in basis)
                {
                    List<int> other;
                    if (!resets.TryGetValue(curMin + b, out other))
                    {
                        other = new List<int>();
                        resets[curMin + b] = other;
                    }
                    other.Add(b);
                }
                curMin = resets.First().Key;
            }
            total += val;
            val++;
        }
        return total;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            List<int> l = new List<int>();
            for (int i = 10000000; l.Count < 5000; i--)
            {
                l.Add(i);
            }
            File.WriteAllText("a.txt", string.Join(",", l));
            ModModMod c = new ModModMod();
            object o = c.findSum(new int[] {5,3,2}, 10);
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

