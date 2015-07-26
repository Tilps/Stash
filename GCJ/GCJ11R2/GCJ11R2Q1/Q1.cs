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

namespace GCJ11R2Q1
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
                int x = int.Parse(bits[0]);
                int s = int.Parse(bits[1]);
                int r = int.Parse(bits[2]);
                int t = int.Parse(bits[3]);
                int n = int.Parse(bits[4]);
                int[] starts = new int[n];
                int[] ends = new int[n];
                int[] speeds = new int[n];
                for (int j = 0; j < n; j++)
                {
                    index++;
                    bits = lines[index].Split(' ');
                    starts[j] = int.Parse(bits[0]);
                    ends[j] = int.Parse(bits[1]);
                    speeds[j] = int.Parse(bits[2]);
                }
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(x, s, r, t, n, starts, ends, speeds)));
                index++;
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int x, int s, int r, int t, int n, int[] starts, int[] ends, int[] speeds)
        {
            int[] dists = new int[n];
            int tot = 0;
            for (int i = 0; i < n; i++)
            {
                dists[i] = ends[i] - starts[i];
                tot += dists[i];
            }
            int remains = x - tot;
            Array.Sort(speeds, dists);
            double tUsed = 0.0;
            bool tDone = false;

            double totalTime = 0.0;
            // first time for empty space.
            if (remains > 0)
            {
                int maxDist = r * t;
                if (remains >= maxDist)
                {
                    tUsed = t;
                    tDone = true;
                    totalTime += t;
                    int leftovers = remains - maxDist;
                    totalTime += (double)leftovers / (double)s;
                }
                else
                {
                    tUsed = (double)remains / (double)r;
                    totalTime += tUsed;
                }
            }
            for (int i = 0; i < n; i++)
            {
                if (tDone)
                {
                    totalTime += (double)dists[i] / (double)(s+speeds[i]);
                }
                else
                {
                    double maxDist = (double)(r + speeds[i]) * ((double)t - tUsed);
                    if ((double)dists[i] >= maxDist)
                    {
                        totalTime += (double)t-tUsed;
                        tUsed = t;
                        tDone = true;
                        double leftovers = (double)dists[i] - maxDist;
                        totalTime += (double)leftovers / (double)(s+speeds[i]);
                    }
                    else
                    {
                        double tNext = (double)dists[i] / (double)(r+speeds[i]);
                        tUsed += tNext;
                        totalTime += tNext;
                    }
                }
            }

            return totalTime.ToString();
        }


    }
}
