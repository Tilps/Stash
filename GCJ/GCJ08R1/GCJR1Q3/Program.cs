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
                int n = int.Parse(lines[index]);
                index++;
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(n)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int n)
        {

            // The equation can be integerized by adding (3-sqrt(5))^n, which is always less than 1.
            // Therefore we only need the first 3 digits of both the rational and irrational components of the expansion
            // The irrational components will cancel out and we double the rational component indicating the sum of the two.
            // we then subtract 1 as the integerized version is always above the pre-integerized version and the difference is always less than 1.
            long[] a = new long[32];
            long[] b = new long[32];
            a[0] = 3;
            b[0] = 1;
            for (int i = 0; i < 31; i++)
            {
                a[i + 1] = (a[i] * a[i] + 5 * b[i] * b[i]) % 1000;
                b[i + 1] = (2 * a[i] * b[i]) % 1000;
            }
            long p1 = 1;
            long p2 = 0;
            for (int i = 0; i < 31; i++)
            {
                if ((n & (1 << i)) != 0)
                {
                    long oldp = p1;
                    p1 = (p1 * a[i] + 5 * p2 * b[i]) % 1000;
                    p2 = (p2 * a[i] + oldp * b[i]) % 1000;
                }
            }
            long ans = (2 * p1 + 999) % 1000;
            return ans.ToString("000");
        }
    }
}
