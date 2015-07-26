using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCJR1Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(args[0]);
            List<string> output = new List<string>();
            int cases = int.Parse(lines[0]);
            int index = 1;
            for (int i = 0; i < cases; i++)
            {
                // Parse
                string[] bits1 = lines[index].Split(' ');
                index++;
                int n = int.Parse(bits1[0]);
                int m = int.Parse(bits1[1]);
                int X = int.Parse(bits1[2]);
                int Y = int.Parse(bits1[3]);
                int Z = int.Parse(bits1[4]);
                int[] A = new int[m];
                for (int j = 0; j < m; j++)
                {
                    A[j] = int.Parse(lines[index]);
                    index++;
                }
                List<int> signPosts = new List<int>();
                for (int j = 0; j < n; j++)
                {
                    signPosts.Add(A[j % m]);
                    A[j % m] = (int)(((long)X * (long)A[j % m] + (long)Y * (long)(j + 1)) % (long)Z);
                }
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(signPosts.ToArray())));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static long Solve(int[] p)
        {
            int[] sortedP = (int[])p.Clone();
            Array.Sort(sortedP);
            long total = 0;
            List<int> maxes = new List<int>();
            maxes.Add(-1);
            List<long> totals = new List<long>();
            totals.Add(1);
            List<long> sumTotals = new List<long>();
            sumTotals.Add(1);
            int last = -1;
            for (int i = 0; i < sortedP.Length; i++)
            {
                if (sortedP[i] == last)
                    continue;
                last = sortedP[i];
                maxes.Add(last);
                totals.Add(0);
                sumTotals.Add(1);
            }
            int validTo = sumTotals.Count-1;
            for (int i = 0; i < p.Length; i++)
            {
                int place = maxes.BinarySearch(p[i]);
                if (place > validTo)
                {
                    for (int j = validTo+1; j <= place; j++)
                    {
                        sumTotals[j] = (sumTotals[j - 1] + totals[j]) % 1000000007;
                    }
                }
                totals[place] = (totals[place] + sumTotals[place-1]) % 1000000007L;
                sumTotals[place] = (sumTotals[place - 1] + totals[place]) % 1000000007L;
                validTo = place;
            }
            for (int i = 1; i < totals.Count; i++)
                total += totals[i];
            return total % 1000000007L;
        }

    }
}
