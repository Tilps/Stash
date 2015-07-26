using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;
// http://www.themissingdocs.net/downloads/TMD.Algo.0.0.4.0.zip
using TMD.Algo.Algorithms;
using TMD.Algo.Collections;
using TMD.Algo.Collections.Generic;

namespace GCJ10R2Q1
{
    class Q1
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
                int k = int.Parse(bits[0]);
                int[,] diamond = new int[2*k-1, k];
                for (int j = 0; j < 2 * k - 1; j++)
                {
                    index++;
                    string[] diabits = lines[index].Trim().Split(' ');
                    int l = 0;
                    foreach (string diabit in diabits)
                    {
                        diamond[j, l] = int.Parse(diabit);
                        l++;
                    }
                }
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(k, diamond)));
                index++;
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int k, int[,] diamond)
        {
            int largest = FindLargest(k, diamond);
            int bigger = 2*(k-largest) + largest;
            return ((bigger * bigger) - (k * k)).ToString();
        }

        private static int FindLargest(int k, int[,] diamond)
        {
            int[,] square = new int[k,k];
            int x = 0;
            int y = 0;
            int counter = 0;
            int length = 1;
            bool flag = false;
            for (int i = 0; i < 2 * k - 1; i++)
            {
                for (int j = 0; j <= i && j <= 2*k-i-2; j++)
                {
                    square[x, y] = diamond[i, j];
                    counter++;
                    if (counter == length)
                    {
                        counter = 0;
                        if (length == k)
                            flag = true;
                        if (flag)
                        {
                            y = k-1;
                            length--;
                            x = k - length;
                        }
                        else
                        {
                            y = length;
                            x = 0;
                            length++;
                        }
                    }
                    else
                    {
                        x++;
                        y--;
                    }
                }
            }
            int best = BestTopLeft(k, square);
            int[,] rotated = Rotate(k, square);
            best = Math.Max(best, BestTopLeft(k, rotated));
            rotated = Rotate(k, rotated);
            best = Math.Max(best, BestTopLeft(k, rotated));
            rotated = Rotate(k, rotated);
            best = Math.Max(best, BestTopLeft(k, rotated));
            return best;
        }

        private static int[,] Rotate(int k, int[,] square)
        {
            int[,] rotated = new int[k, k];
            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < k; j++)
                {
                    rotated[i, j] = square[j, k - i - 1];
                }
            }
            return rotated;
        }

        private static int BestTopLeft(int k, int[,] square)
        {
            int best = int.MinValue;
            for (int i = k; i >= 1; i--)
            {
                bool works = true;
                for (int j = 0; j < (i + 1) / 2 && works; j++)
                {
                    for (int l = 0; l < i - j && works; l++)
                    {
                        if (square[j, l] != square[l, j])
                            works = false;
                        else if (square[j, l] != square[i - j - 1, i - l - 1])
                            works = false;
                        else if (square[j, l] != square[i - l - 1, i - j - 1])
                            works = false;
                    }
                }
                if (works)
                {
                    best = i;
                    break;
                }
            }
            return best;
        }

    }
}
