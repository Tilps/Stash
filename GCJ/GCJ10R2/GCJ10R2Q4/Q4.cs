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

namespace GCJ10R2Q4
{
    class Q4
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
                int n = int.Parse(bits[0]);
                int m = int.Parse(bits[1]);
                int [] px = new int[n];
                int[] py = new int[n];
                int[] qx = new int[m];
                int[] qy = new int[m];
                for (int j = 0; j < n; j++)
                {
                    index++;
                    string[] bits2 = lines[index].Split(' ');
                    px[j] = int.Parse(bits2[0]);
                    py[j] = int.Parse(bits2[1]);
                }
                for (int j = 0; j < m; j++)
                {
                    index++;
                    string[] bits2 = lines[index].Split(' ');
                    qx[j] = int.Parse(bits2[0]);
                    qy[j] = int.Parse(bits2[1]);
                }
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(n, m, px, py, qx, qy)));
                index++;
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int n, int m, int[] px, int[] py, int[] qx, int[] qy)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < m; i++)
            {
                if (i > 0)
                    result.Append(' ');
                double[] rads = new double[n];
                double[] rads2 = new double[n];
                for (int j = 0; j < n; j++)
                {
                    int dx = px[j]-qx[i];
                    int dy = py[j]-qy[i];
                    rads[j] = Math.Sqrt(dx * dx + dy * dy);
                    rads2[j] = (double)dx * dx + dy * dy;
                }
                int ddx = px[0]-px[1];
                int ddy = py[0]-py[1];
                double d = Math.Sqrt(ddx * ddx + ddy * ddy);
                double d2 = (double)ddx * ddx + ddy * ddy;
                if (d + rads[0] < rads[1])
                    result.Append(Math.PI * rads2[0]);
                else if (d + rads[1] < rads[0])
                    result.Append(Math.PI * rads2[1]);
                else
                {
                    double area = rads2[0] * Math.Acos((d * d + rads2[0] - rads2[1]) / 2.0 / d / rads[0]) +
                        rads2[1] * Math.Acos((d * d + rads2[1] - rads2[0]) / 2.0 / d / rads[1]) -
                        0.5 * Math.Sqrt((-d + rads[0] + rads[1]) * (d + rads[0] - rads[1]) * (d - rads[0] + rads[1]) * (d + rads[0] + rads[1]));
                    result.Append(area);
                }
            }
            return result.ToString();
        }


    }
}
