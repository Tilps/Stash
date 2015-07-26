using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace GCJR1Q2
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
                string input = lines[index];
                index++;
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(input)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static long Solve(string input)
        {
            long[,] table = new long[210, input.Length + 1];
            table[0, 0] = 1;
            for (int i = 1; i <= input.Length; i++)
            {
                for (int j = 1; j <= i; j++)
                {
                    int mod = Mod(input.Substring(i-j, j));
                    for (int k = 0; k < 210; k++)
                    {
                        int next = (k+mod) % 210;
                        table[next, i] += table[k, i - j];
                        if (j != i)
                        {
                            int other = (210 + k - mod) % 210;
                            table[other, i] += table[k, i - j];
                        }
                    }
                }
            }
            long total = 0;
            for (int i = 0; i < 210; i++)
            {
                if (i % 2 == 0 || i % 3 == 0 || i % 5 == 0 || i % 7 == 0)
                {
                    total += table[i, input.Length];
                }
            }
            return total;
        }

        private static int Mod(string p)
        {
            // calculate mod 210 of a potentially zero padded string.
            p = p.TrimStart('0');
            if (p.Length == 0)
                return 0;
            // lets do 6 digits at a time, we could easily do more, but 6 will do.
            int segCount = (p.Length+5) / 6;
            int lastRem = 0;
            for (int i = 0; i < segCount; i++)
            {
                int start = p.Length - (segCount - i) * 6;
                int length = 6;
                if (start < 0)
                    length = 6+start;
                start = Math.Max(start, 0);
                string part = p.Substring(start, length);
                if (lastRem != 0)
                    part = lastRem.ToString() + part;
                lastRem = (int)(long.Parse(part) % 210L);
            }
            return lastRem;
        }

    }
}
