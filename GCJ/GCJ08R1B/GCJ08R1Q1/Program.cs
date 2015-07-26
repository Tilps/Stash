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
                int n = int.Parse(bits[0]);
                int A = int.Parse(bits[1]);
                int B = int.Parse(bits[2]);
                int C = int.Parse(bits[3]);
                int D = int.Parse(bits[4]);
                int x0 = int.Parse(bits[5]);
                int y0 = int.Parse(bits[6]);
                int M = int.Parse(bits[7]);
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(n, A, B, C, D, x0, y0, M)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static long Solve(int n, int A, int B, int C, int D, int x0, int y0, int M)
        {
            int[] counts = new int[9];
            counts[GetIndex(x0, y0)]++;
            int X = x0;
            int Y = y0;
            for (int i = 0; i < n - 1; i++)
            {
                int nX = (int)(((long)A * (long)X + (long)B) % (long)M);
                int nY = (int)(((long)C * (long)Y + (long)D) % (long)M);
                counts[GetIndex(nX, nY)]++;
                X = nX;
                Y = nY;
            }
            long total = 0;
            for (int i = 0; i < 9; i++)
            {
                int xi;
                int yi;
                GetCoords(i, out xi, out yi);
                if (counts[i] == 0)
                    continue;
                for (int j = i; j < 9; j++)
                {
                    int xj;
                    int yj;
                    GetCoords(j, out xj, out yj);
                    if (counts[j] == 0)
                        continue;
                    int xk = (6 - xi - xj) % 3;
                    int yk = (6 - yi - yj) % 3;
                    int k = GetIndex(xk, yk);
                    if (k < j)
                        continue;
                    long ratio = 1;
                    int first = counts[i];
                    int second;
                    if (i == j)
                    {
                        second = Math.Max(counts[j] - 1, 0);
                        ratio++;
                    }
                    else
                        second = counts[j];
                    if (second == 0)
                        continue;
                    int third;
                    if (i == j && j == k)
                    {
                        third = Math.Max(counts[k] - 2, 0);
                        ratio += 4;
                    }
                    else if (j == k)
                    {
                        third = Math.Max(counts[k] - 1, 0);
                        ratio++;
                    }
                    else
                        third = counts[k];
                    if (third == 0)
                        continue;
                    total += ((long)first * (long)second * (long)third)/ratio;
                }
            }
            return total;
        }

        private static void GetCoords(int i, out int xi, out int yi)
        {
            yi = i % 3;
            xi = (i / 3) % 3;
        }

        private static int GetIndex(int x0, int y0)
        {
            return (x0 % 3) * 3 + (y0 % 3);
        }

    }
}
