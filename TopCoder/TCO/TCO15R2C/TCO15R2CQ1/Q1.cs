using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class YetAnotherCardGame
{
    public int maxCards(int[] p, int[] s)
    {
        Array.Sort(p);
        Array.Sort(s);
        int[][] both = new int[2][];
        both[0] = p;
        both[1] = s;
        int worst = 0;
        for (int i = 0; i < p.Length; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int[,] dists = new int[p.Length, 2];
                dists[i, j] = 1;
                int idx1 = 0;
                int idx2 = 0;
                while (idx1 < p.Length && idx2 < s.Length)
                {
                    int a = p[idx1];
                    int b = s[idx2];
                    if (a < b)
                    {
                        if (dists[idx1, 0] > 0)
                        {
                            UpdateDists(dists, idx1, 0, both);
                        }
                        idx1++;
                    }
                    else
                    {
                        if (dists[idx2, 1] > 0)
                        {
                            UpdateDists(dists, idx2, 1, both);
                        }
                        idx2++;
                    }
                }
                while (idx1 < p.Length)
                {
                    if (dists[idx1, 0] > 0)
                    {
                        UpdateDists(dists, idx1, 0, both);
                    }
                    idx1++;

                }
                while (idx2 < s.Length)
                {
                    if (dists[idx2, 1] > 0)
                    {
                        UpdateDists(dists, idx2, 1, both);
                    }
                    idx2++;
                }
                for (int k = 0; k < p.Length; k++)
                {
                    for (int l = 0; l < 2; l++)
                    {
                        if (j == 1 && dists[k, l] == p.Length*2)
                        {
                            int possible = p.Length*2 - 1;
                            if (possible > worst) worst = possible;
                        }
                        else
                        {
                            if (dists[k, l] > worst) worst = dists[k, l];
                        }
                    }
                }
            }
        }

        return worst;
    }

    private void UpdateDists(int[,] dists, int idx2, int p2, int[][] both)
    {
        int v = both[p2][idx2];
        int other = (p2 + 1)%2;
        for (int i = 0; i < both[other].Length; i++)
        {
            if (both[other][i] > v)
            {
                if (dists[i, other] <= dists[idx2, p2]) dists[i, other] = dists[idx2, p2] + 1;
            }
        }
    }
}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            YetAnotherCardGame c = new YetAnotherCardGame();
            object o = c.maxCards(new int[] {4,5,6 },new int[] {1,2,3, });
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

