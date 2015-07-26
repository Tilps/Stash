using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCJ08R3Q4
{
    class ProgramQ4
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
                int H = int.Parse(bits[0]);
                int W = int.Parse(bits[1]);
                int R = int.Parse(bits[2]);
                int[] xs = new int[R];
                int[] ys = new int[R];
                for (int j = 0; j < R; j++)
                {
                    string[] bits2 = lines[index].Split(' ');
                    xs[j] = int.Parse(bits2[0]);
                    ys[j] = int.Parse(bits2[1]);
                    index++;
                }
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(W, H, xs, ys)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static int Solve(int W, int H, int[] xs, int[] ys)
        {
            int[,] ways = new int[W, H];
            bool[,] invalid = new bool[W, H];
            for (int i = 0; i < xs.Length; i++)
            {
                invalid[xs[i] - 1, ys[i] - 1] = true;
            }
            ways[0, 0] = 1;
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    if (i == 0 && j == 0) continue;
                    if (invalid[i, j])
                        continue;
                    int total=0;
                    if (j > 1 && i > 0)
                        total += ways[i - 1, j - 2];
                    if (j > 0 && i > 1)
                        total += ways[i - 2, j - 1];
                    ways[i, j] = total % 10007;
                }
            }
            return ways[W - 1, H - 1];
        }
    }
}
