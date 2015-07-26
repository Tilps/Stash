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
                int flavCount = int.Parse(lines[index]);
                index++;
                int custCount = int.Parse(lines[index]);
                index++;
                bool[, ,] data = new bool[custCount, flavCount, 2];
                for (int j = 0; j < custCount; j++)
                {
                    string[] bits = lines[index].Split(' ');
                    int bitCount = int.Parse(bits[0]);
                    Debug.Assert(bitCount * 2 + 1 == bits.Length);
                    for (int k = 1; k < bits.Length; k+=2)
                    {
                        int flav = int.Parse(bits[k])-1;
                        int maltFlag = int.Parse(bits[k + 1]);
                        data[j, flav, maltFlag] = true;
                    }
                    index++;
                }
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(flavCount, custCount, data)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int flavCount, int custCount, bool[, ,] data)
        {
            bool[] flavsMalted = new bool[flavCount];
            int[] satisfactionCount = new int[custCount];
            for (int i = 0; i < flavCount; i++)
            {
                for (int j = 0; j < custCount; j++)
                {
                    if (data[j, i, 0])
                    {
                        satisfactionCount[j]++;
                    }
                }
            }
            int satisCount = 0;
            for (int i = 0; i < custCount; i++)
            {
                if (satisfactionCount[i] > 0)
                    satisCount++;
            }
            if (satisCount >= custCount)
            {
                return Format(flavsMalted);
            }
            int[] malts = new int[custCount];
            for (int i = 0; i < custCount; i++)
            {
                malts[i] = -1;
                for (int j = 0; j < flavCount; j++)
                {
                    if (data[i, j, 1])
                        malts[i] = j;
                }
            }
            bool notDone = true;
            while (notDone)
            {
                notDone = false;
                for (int i = 0; i < custCount; i++)
                {
                    if (satisfactionCount[i] > 0)
                        continue;
                    else if (malts[i] > -1 && !flavsMalted[malts[i]])
                    {
                        satisCount = Malt(custCount, data, satisfactionCount, satisCount, malts[i], flavsMalted);
                        notDone = true;
                    }
                    else
                        return "IMPOSSIBLE";
                }
            }
            return Format(flavsMalted);
        }

        private static int Malt(int custCount, bool[, ,] data, int[] satisfactionCount, int satisCount, int j, bool[] flavsMalted)
        {
            flavsMalted[j] = true;
            for (int k = 0; k < custCount; k++)
            {
                if (data[k, j, 0])
                {
                    satisfactionCount[k]--;
                    if (satisfactionCount[k] == 0)
                        satisCount--;
                }
                if (data[k, j, 1])
                {
                    if (satisfactionCount[k] == 0)
                        satisCount++;
                    satisfactionCount[k]++;
                }
            }
            return satisCount;
        }


        private static string Format(bool[] flavsMalted)
        {
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < flavsMalted.Length; i++)
            {
                if (i > 0)
                    res.Append(' ');
                res.Append(flavsMalted[i] ? 1 : 0);

            }
            return res.ToString();
        }
    }
}
