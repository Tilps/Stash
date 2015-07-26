using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCJ08R2Q3
{
    class ProgramQ3
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
                int N = int.Parse(bits[0]);
                int[] xs = new int[N];
                int[] ys = new int[N];
                int[] zs = new int[N];
                int[] ps = new int[N];
                for (int j = 0; j < N; j++)
                {
                    string[] bits2 = lines[index].Split(' ');
                    xs[j] = int.Parse(bits2[0]);
                    ys[j] = int.Parse(bits2[1]);
                    zs[j] = int.Parse(bits2[2]);
                    ps[j] = int.Parse(bits2[3]);
                    index++;
                }
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(xs, ys, zs, ps)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static double Solve(int[] xs, int[] ys, int[] zs, int[] ps)
        {
            // binary search P using a 4 vector extent check to see if all octahedrons overlap.
            return 0.0;
        }

    }
}
