using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;
// http://www.themissingdocs.net/downloads/TMD.Algo.0.0.4.0.zip
using TMD.Algo.Algorithms;
using TMD.Algo.Collections;

namespace GCJ11R2Q4
{
    class Q4
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
                int p = int.Parse(bits[0]);
                int w = int.Parse(bits[1]);
                index++;
                bits = lines[index].Split(' ');
                int[] starts = new int[w];
                int[] ends = new int[w];
                for (int j = 0; j < w; j++)
                {
                    starts[j] = int.Parse(bits[2 * j]);
                    ends[j] = int.Parse(bits[2 * j + 1]);
                }
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(p, w, starts, ends)));
                index++;
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int p, int w, int[] starts, int[] ends)
        {
            int[,] best = new int[p, p+1];
            return "";
        }

    }
}
