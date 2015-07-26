using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCJ09R2Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] file = File.ReadAllLines(args[0]);
            List<string> output = new List<string>();
            int count = int.Parse(file[0]);
            int offset = 1;
            for (int counter = 0; counter < count; counter++)
            {                
                string[] bits = file[offset].Split(' ');
                int N = int.Parse(bits[0]);
                int K = int.Parse(bits[1]);
                int[,] stocks = new int[N, K];
                offset++;
                for (int i=0; i < N; i++) {
                    string[] bits2 = file[offset].Split(' ');
                    for (int j = 0; j < K; j++)
                    {
                        stocks[i, j] = int.Parse(bits2[j]);
                    }
                    offset++;
                }
                output.Add(string.Format("Case #{0}: {1}", counter + 1, Solve(stocks, N, K)));
                Console.Out.WriteLine(counter + 1);
            }
            File.WriteAllLines("output.txt", output.ToArray());

        }

        private static int Solve(int[,] stocks, int N, int K)
        {
            bool[,] fail = new bool[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i==j)
                        continue;
                    for (int k = 0; k < K-1; k++)
                    {
                        if (stocks[i, k] <= stocks[j, k])
                        {
                            if (stocks[i, k + 1] >= stocks[j, k + 1])
                                fail[i, j] = true;
                        }
                        else
                        {
                            if (stocks[i, k + 1] <= stocks[j, k + 1])
                                fail[i, j] = true;
                        }
                    }
                }
            }
            int[] best = new int[1 << N];
            int complete = 0;

            return Recurse(N, fail, complete, best);
        }

        private static int Recurse(int N, bool[,] fail, int complete, int[] best)
        {
            if (complete == (1 << N) - 1)
                return 0;
            if (best[complete] == 0)
            {
                int best2 = int.MaxValue;
                for (int i = NextBitPattern(0, (1 << (N)) - 1 - complete, 0); i < 1 << N; i = NextBitPattern(i, (1 << (N + 1)) - 1 - complete, 0))
                {
                    if (i==0)
                        continue;
                    if ((complete & i) != 0)
                        continue;
                    bool failed = false;
                    for (int j = 0; j < N - 1 && !failed; j++)
                    {
                        if ((i & (1 << j)) != 0)
                        {
                            for (int k = j + 1; k < N; k++)
                            {
                                if ((i & (1 << k)) != 0)
                                {
                                    if (fail[j, k])
                                    {
                                        failed = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    int found = Recurse(N, fail, complete | i, best)
                        + 1;
                    if (found < best2)
                        best2 = found;
                }
                best[complete] = best2;
            }
            return best[complete];
        }
        public static int NextBitPattern(int i, int changingMask, int constantMask)
        {
            return ((i - (changingMask + constantMask)) & changingMask) + constantMask;
        }

    }
}
