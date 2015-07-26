using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCJ08R1Q1
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
                int count = int.Parse(lines[index]);
                index++;
                string[] xStrings = lines[index].Split(' ');
                index++;
                string[] yStrings = lines[index].Split(' ');
                index++;
                int[] xs = new int[count];
                for (int j = 0; j < xs.Length; j++)
                {
                    xs[j] = int.Parse(xStrings[j]);
                }
                int[] ys = new int[count];
                for (int j = 0; j < ys.Length; j++)
                {
                    ys[j] = int.Parse(yStrings[j]);
                }
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(xs, ys)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static long Solve(int[] xs, int[] ys)
        {
            long total = 0;
            Array.Sort(xs);
            Array.Sort(ys);
            Array.Reverse(ys);
            for (int i = 0; i < xs.Length; i++)
            {
                total += (long)xs[i] * (long)ys[i];
            }
            return total;
        }
    }
}
