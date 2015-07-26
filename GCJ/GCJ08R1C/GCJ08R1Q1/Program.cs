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
                string[] bits = lines[index].Split(' ');
                index++;
                int P = int.Parse(bits[0]);
                int K = int.Parse(bits[1]);
                int L = int.Parse(bits[2]);
                string[] bits2 = lines[index].Split(' ');
                index++;
                int[] freqs = new int[bits2.Length];
                for (int j = 0; j < bits2.Length; j++)
                {
                    freqs[j] = int.Parse(bits2[j]);
                }
                // Process
                if (P * K < L)
                    output.Add(string.Format("Case #{0}: {1}", i + 1, "Impossible"));
                else
                    output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(P, K, L, freqs)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static long Solve(int P, int K, int L, int[] freqs)
        {
            Array.Sort(freqs);
            long total = 0;
            for (int i = 0; i < freqs.Length; i++)
            {
                total += (long)freqs[freqs.Length - i - 1] * (long)(i / K + 1);
            }
            return total;
        }

    }
}
