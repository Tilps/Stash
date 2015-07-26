using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;
// http://www.themissingdocs.net/downloads/TMD.Algo.0.0.4.0.zip
using TMD.Algo.Algorithms;
using TMD.Algo.Collections;

namespace GCJ10R1AQ3
{
    class Q3
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
                int a1 = int.Parse(bits[0]);
                int a2 = int.Parse(bits[1]);
                int b1 = int.Parse(bits[2]);
                int b2 = int.Parse(bits[3]);
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(a1, a2, b1, b2)));
                index++;
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int a1, int a2, int b1, int b2)
        {
            int counter = 0;
            for (int a = a1; a <= a2; a++)
            {
                for (int b = b1; b < b2; b++)
                {
                    if (Win(a, b))
                        counter++;
                }
            }
            return counter.ToString();
        }

        private static bool Win(int a, int b)
        {
            if (a > b)
                return Win(b, a);
            if (a == b)
                return false;
            if (b%a==0)
                return true;
            bool winPossible;
            if (winLookup.TryGetValue(a * 1000001 + b, out winPossible))
                return winPossible;
            int origA = a;
            int origB = b;
            long ignore1, ignore2;
            int shortcut = (int)ExtendedGcd(a, b, out ignore1, out ignore2);
            a /=shortcut;
            b /=shortcut;
            if (winLookup.TryGetValue(a * 1000001 + b, out winPossible))
                return winPossible;
            winPossible = false;
            int k = a;
            while (k < b)
            {
                bool loss = Win(a, b - k);
                if (!loss)
                    winPossible = true;
                k += a;
            }
            winLookup[a * 1000001 + b] = winPossible;
            winLookup[origA * 1000001 + origB] = winPossible;
            return winPossible;
        }
        private static Dictionary<long, bool> winLookup = new Dictionary<long, bool>();

        private static long ExtendedGcd(long first, long second, out long firstProduct, out long secondProduct)
        {
            if (second == 0)
            {
                firstProduct = 1;
                secondProduct = 0;
                return first;
            }
            long firstTemp;
            long secondTemp;
            long divisor = ExtendedGcd(second, first % second, out firstTemp, out secondTemp);
            firstProduct = secondTemp;
            secondProduct = firstTemp - (first / second) * secondTemp;
            return divisor;
        }
    }
}
