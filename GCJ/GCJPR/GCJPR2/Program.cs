using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TMD.Algo.Collections.Generic;

namespace GCJPR2
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
                string[] bits = lines[index].Split(' ');
                index++;
                int k = int.Parse(bits[1]);
                int n = int.Parse(bits[0]);
                int[] xs = new int[n];
                int[] ys = new int[n];
                for (int j = 0; j < n; j++)
                {
                    string[] bits2 = lines[index].Split(' ');
                    xs[j] = int.Parse(bits2[0]);
                    ys[j] = int.Parse(bits2[1]);
                    index++;
                }
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(n, k, xs, ys)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static int Solve(int n, int k, int[] xs, int[] ys)
        {
            if (n <= k)
                return 0;

            int[,] delta = new int[n, n];

            // Any two points can be covered by a square if their maximum of dx and dy is the size of the square.
            // Any n points can be covered by a square, if the maximum of the maximum of dx and dy for each pair is the size of the square.

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    delta[i, j] = Math.Max(Math.Abs(xs[i] - xs[j]), Math.Abs(ys[i] - ys[j]));
                }
            }
            for (int i = 0; i <= k; i++)
            {
                for (int j = 0; j < (1 << (n + 1)); j++)
                {
                    memo[i, j] = -1;
                }
            }

            int mask = 0;
            return RecurseSolve(n, k, mask, xs, ys, delta);
        }
        private static int[,] memo = new int[16, 1 << 17];

        private static int RecurseSolve(int n, int k, int mask, int[] xs, int[] ys, int[,] delta)
        {
            if (n <= k)
                return 0;
            if (k == 0)
                return int.MaxValue;
            if (memo[k, mask] >= 0)
                return memo[k, mask];
            int best = int.MaxValue;
            for (int i = 0; i < xs.Length; i++)
            {
                if ((mask & (1<<i)) != 0)
                    continue;
                for (int j = 0; j < xs.Length; j++)
                {
                    if (i == j)
                        continue;
                    if ((mask & (1 << j)) != 0)
                        continue;
                    int square = delta[i, j];
                    for (int l = 0; l < xs.Length; l++)
                    {
                        if ((mask & (1 << l)) != 0)
                            continue;
                        for (int m = 0; m < xs.Length; m++)
                        {
                            if (l == m)
                                continue;
                            if ((mask & (1 << m)) != 0)
                                continue;
                            if (delta[l, m] > square)
                                continue;
                            int minX = Math.Min(xs[l], xs[m]);
                            int minY = Math.Min(ys[l], ys[m]);
                            int maxX = minX + square;
                            int maxY = minY + square;
                            int covered = 2;
                            int newMask = mask | (1 << l) | (1 << m);
                            for (int o = 0; o < xs.Length; o++)
                            {
                                if (o == l)
                                    continue;
                                if (o == m)
                                    continue;
                                if ((mask & (1 << o)) != 0)
                                    continue;
                                if (xs[o] >= minX && xs[o] <= maxX && ys[o] >= minY && ys[i] <= maxY)
                                {
                                    covered++;
                                    newMask |= (1 << o);
                                }
                            }
                            int recurse = RecurseSolve(n - covered, k - 1, newMask, xs, ys, delta);
                            int size = Math.Max(square, recurse);
                            if (size < best)
                                best = size;
                        }
                    }
                }
            }
            memo[k, mask] = best;
            return best;
        }
    }
}
