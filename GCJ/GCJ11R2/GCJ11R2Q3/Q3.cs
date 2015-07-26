using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;
// http://www.themissingdocs.net/downloads/TMD.Algo.0.0.4.0.zip
using TMD.Algo.Algorithms;
using TMD.Algo.Collections;

namespace GCJ11R2Q3
{
    class Q3
    {
        static void Main(string[] args)
        {
            MakePrimes();
            string[] lines = File.ReadAllLines(args[0]);
            List<string> output = new List<string>();
            int cases = int.Parse(lines[0]);
            int index = 1;
            for (int i = 0; i < cases; i++)
            {
                string[] bits = lines[index].Split(' ');
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(long.Parse(bits[0]))));
                index++;
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static void MakePrimes()
        {
            for (int i = 2; i*i <= 1000000; i++)
            {
                if (!comps[i])
                {
                    int next = i * i;
                    while (next <= 1000000)
                    {
                        comps[next] = true;
                        next += i;
                    }
                }
            }
        }

        static bool[] comps = new bool[1000001];

        private static string Solve(long n)
        {
            // small only approach to start.
            /*
            int total = 1;
            BigInteger lcm = 1;
            for (int i = 2; i <= n; i++)
            {
                if (lcm % i != 0)
                {
                    total++;
                    lcm = lcm*i/BigInteger.GreatestCommonDivisor(lcm, i);
                }
            }
             */
            // worst case = this.
            int total = 1;
            for (int i = 2; i <= n; i++)
            {
                if (!comps[i])
                {
                    for (int j = 1; Pow(i, j) <= n; j++)
                    {
                        total++;
                    }
                }
            }
            int[] nums = new int[n];
            Pop(nums, n);
            int total2 = 1;
            BigInteger lcm = nums[n-1];
            for (int i = (int)n-2; i >=0; i--)
            {
                int next = nums[i];
                if (lcm % next != 0)
                {
                    total2++;
                    lcm = lcm * next / BigInteger.GreatestCommonDivisor(lcm, next);
                }
            }
             
            return (total-total2).ToString();
        }

        private static void Pop(int[] nums, long n)
        {
            for (int i = 0; i < n; i++)
                nums[i] = i + 1;

            int[] factCount = new int[n];
            for (int i = 0; i < n; i++)
                factCount[i] = CountFact(i + 1)*1001+(i);
            Array.Sort(factCount, nums);

        }

        private static int CountFact(int p)
        {
            int best = 0;
            int factTypeCount = 0;
            for (int i = 2; i <= p; i++)
            {
                if (!comps[i])
                {
                    int factCount = 0;
                    int test = p;
                    while (test > 0 && (test % i) == 0)
                    {
                        test /= i;
                        factCount++;
                    }
                    if (factCount > 0)
                        factTypeCount++;
                    if (factCount > best)
                        best = factCount;
                }
            }
            return factTypeCount==1? best : 0;
        }

        private static long Pow(int i, int j)
        {
            long i2 = i;
            long res = i2;
            for (int k = 1; k < j; k++)
            {
                res *= i2;
            }
            return res;
        }

    }
}
