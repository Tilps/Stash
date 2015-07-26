using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;
// http://www.themissingdocs.net/downloads/TMD.Algo.0.0.4.0.zip
using TMD.Algo.Algorithms;
using TMD.Algo.Collections.Generic;

namespace GCJ11R2Q2
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
                int r = int.Parse(bits[0]);
                int c = int.Parse(bits[1]);
                int d = int.Parse(bits[2]);
                int[,] delta = new int[r, c];
                for (int j = 0; j < r; j++)
                {
                    index++;
                    for (int k = 0; k < c; k++)
                    {
                        delta[j, k] = int.Parse("" + lines[index][k]);
                    }
                }
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(r, c, d, delta)));
                index++;
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int r, int c, int d, int[,] delta)
        {
            int largest = 0;
            for (int i = 1; i < r - 1; i++)
            {
                for (int j = 1; j < c - 1; j++)
                {
                    for (int k = 1; k <= i && k <= j; k++)
                    {
                        if (i + k >= r)
                            break;
                        if (j + k >= c)
                            break;
                        long vsum = 0;
                        long hsum = 0;
                        for (int x = j - k; x <= j + k; x++)
                        {
                            for (int y = i - k; y <= i + k; y++)
                            {
                                // Skip the corners.
                                if (y == i - k || y == i + k)
                                    if (x == j - k || x == j + k)
                                        continue;
                                hsum += (x - j) * (d + delta[y, x]);
                                vsum += (y - i) * (d + delta[y, x]);
                            }
                        }
                        if (hsum == 0 && vsum == 0 && (2 * k + 1) > largest)
                            largest = 2 * k + 1;

                    }
                }
            }
            for (int i = 2; i < r - 2; i++)
            {
                for (int j = 2; j < c - 2; j++)
                {
                    for (int k = 2; k <= i && k <= j; k++)
                    {
                        if (i + k-1 >= r)
                            break;
                        if (j + k-1 >= c)
                            break;
                        long vsum = 0;
                        long hsum = 0;
                        for (int x = j - k; x < j + k; x++)
                        {
                            for (int y = i - k; y < i + k; y++)
                            {
                                // Skip the corners.
                                if (y == i - k || y == i + k-1)
                                    if (x == j - k || x == j + k-1)
                                        continue;
                                hsum += (2*x - 2*j+1) * (d + delta[y, x]);
                                vsum += (2*y - 2*i+1) * (d + delta[y, x]);
                            }
                        }
                        if (hsum == 0 && vsum == 0 && (2 * k) > largest)
                            largest = 2 * k;

                    }
                }
            }
            if (largest == 0)
                return "IMPOSSIBLE";
            else
                return largest.ToString();
        }


    }
}
