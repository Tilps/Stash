using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TMD.Algo.Collections.Generic;

namespace GCJPR1
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
                int B = int.Parse(bits[1]);
                int W = int.Parse(bits[0]);
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(B, W)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int B, int W)
        {
            // 1) Remove a white ball.
            // 2) Remove a white ball.
            // 3) Exchange 2 black balls for a white.

            // If black is even final ball is white.
            // Otherwise the final ballk is black.
            if (B % 2 == 0)
                return "WHITE";
            else
                return "BLACK";
        }
    }
}
