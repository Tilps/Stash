using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCJ08Pre1
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(args[0]);
            int count = int.Parse(lines[0]);
            List<string> output = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                string[] parts = lines[i].Split(' ');
                int sourceBase = parts[1].Length;
                int destBase = parts[2].Length;
                Dictionary<char, int> sourceLookup = new Dictionary<char, int>();
                Dictionary<int, char> destLookup = new Dictionary<int, char>();
                for (int j = 0; j < parts[1].Length; j++)
                {
                    sourceLookup[parts[1][j]] = j;
                }
                for (int j = 0; j < parts[2].Length; j++)
                {
                    destLookup[j] = parts[2][j];
                }
                long res = Translate(sourceLookup, parts[0]);
                output.Add(string.Format("Case #{0}: {1}", i, RevTranslate(destLookup, res)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string RevTranslate(Dictionary<int, char> destLookup, long res)
        {
            int b = destLookup.Count;
            List<char> result = new List<char>();
            while (res != 0)
            {
                result.Add(destLookup[(int)(res % b)]);
                res /= b;
            }
            if (result.Count == 0)
                result.Add(destLookup[0]);
            result.Reverse();
            return new string(result.ToArray());
        }

        private static long Translate(Dictionary<char, int> sourceLookup, string p)
        {
            long a = 0;
            long m = 1;
            int b = sourceLookup.Count;
            for (int i = p.Length - 1; i >= 0; i--)
            {
                a += (long)sourceLookup[p[i]] * m;
                m *= b;
            }
            return a;
        }
    }
}
