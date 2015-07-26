using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;
// http://www.themissingdocs.net/downloads/TMD.Algo.0.0.4.0.zip
using TMD.Algo.Algorithms;
using TMD.Algo.Collections;

namespace GCJ10R1AQ2
{
    class Q2
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(args[0]);
            List<string> output = new List<string>();
            int cases = int.Parse(lines[0]);
            int index = 1;
            for (int i = 0; i < cases; i++)
            {
                string[] bits = lines[index].Split(' ');
                int D = int.Parse(bits[0]);
                int I = int.Parse(bits[1]);
                int M = int.Parse(bits[2]);
                int N = int.Parse(bits[3]);
                index++;
                string[] bits2 = lines[index].Split(' ');
                int[] nums = bits2.Select(delegate(string a) { return int.Parse(a); }).ToArray();
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(D, I, M, N, nums)));
                index++;
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int D, int I, int M, int N, int[] nums)
        {
            int max = (N + 1) * 300;
            int[,] table = new int[N + 1, 257];
            for (int i = 0; i < N + 1; i++)
            {
                for (int j = 0; j < 257; j++)
                {
                    table[i, j] = max;
                }
            }
            table[0, 256] = 0;
            // cost to Smooth first n elements, ending with value k.
            for (int i = 1; i < N + 1; i++)
            {
                for (int j = 0; j < 256; j++)
                {
                    int localBest = max;
                    int prevSame = table[i - 1, j];
                    if (prevSame + D < localBest)
                        localBest = prevSame + D;
                    int diff = Math.Abs(nums[i - 1] - j);
                    int changeCost = ChangeCost(nums[i - 1], j, I, M);
                    for (int k = Math.Max(0, j - M); k < Math.Min(256, j + M+1); k++)
                    {
                        int prev = table[i - 1, k];
                        if (prev >= max)
                            continue;
                        if (prev + diff < localBest)
                            localBest = prev + diff;
                    }
                    for (int k = Math.Max(0, nums[i-1] - M); k < Math.Min(256, nums[i-1] + M+1); k++)
                    {
                        int prev = table[i - 1, k];
                        if (prev >= max)
                            continue;
                        if (prev + changeCost < localBest)
                            localBest = prev + changeCost;
                    }
                    // Handle all deleted, change value.
                    int prevAllDell = table[i - 1, 256];
                    if (prevAllDell + diff < localBest)
                        localBest = prevAllDell + diff;
                    // Handle all deleted, insert after to get target value.
                    if (prevAllDell + changeCost < localBest)
                        localBest = prevAllDell + changeCost;
                    table[i, j] = localBest;
                }
                table[i, 256] = table[i - 1, 256] + D;
            }
            int best = max;
            for (int i = 0; i < 257; i++)
            {
                if (table[N, i] < best)
                    best = table[N, i];
            }
            return best.ToString();
        }

        private static int ChangeCost(int p, int j, int I, int M)
        {
            int diff = Math.Abs(p - j);
            if (diff == 0)
                return 0;
            if (M == 0)
                return 300000;
            int res = I;
            res += I * ((diff-1) / M);
            return res;
        }
    }
}
