using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
// If TMD.Algo used, it is available from www.themissingdocs.net/downloads/TMD.Algo.0.0.3.0.zip

namespace GCJOnSite1
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(args[0]);
            List<string> output = new List<string>();
            int cases = int.Parse(lines[0]);
            int index = 1;
            for (int loopvariable = 0; loopvariable < cases; loopvariable++)
            {
                // Parse
                int L = int.Parse(lines[index]);
                index++;
                int[] Hs = new int[L];
                int[] Ws = new int[L];
                bool[] bird = new bool[L];
                for (int i = 0; i < L; i++)
                {
                    string[] bits = lines[index].Split(' ');
                    index++;
                    Hs[i] = int.Parse(bits[0]);
                    Ws[i] = int.Parse(bits[1]);
                    bird[i] = bits.Length == 3;
                }
                int M = int.Parse(lines[index]);
                index++;
                int[] unsureHs = new int[M];
                int[] unsureWs = new int[M];
                for (int i = 0; i < M; i++)
                {
                    string[] bits = lines[index].Split(' ');
                    index++;
                    unsureHs[i] = int.Parse(bits[0]);
                    unsureWs[i] = int.Parse(bits[1]);
                }
                // Process
                output.Add(string.Format("Case #{0}:", loopvariable + 1));
                string[] results = Solve(Hs, Ws, bird, unsureHs, unsureWs);
                output.AddRange(results);
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string[] Solve(int[] Hs, int[] Ws, bool[] bird, int[] unsureHs, int[] unsureWs)
        {
            int minTrueH = int.MaxValue;
            int maxTrueH = int.MinValue;
            int minTrueW = int.MaxValue;
            int maxTrueW = int.MinValue;
            for (int i = 0; i < Hs.Length; i++)
            {
                if (bird[i])
                {
                    if (Hs[i] < minTrueH)
                        minTrueH = Hs[i];
                    if (Hs[i] > maxTrueH)
                        maxTrueH = Hs[i];
                    if (Ws[i] < minTrueW)
                        minTrueW = Ws[i];
                    if (Ws[i] > maxTrueW)
                        maxTrueW = Ws[i];
                }
            }
            int upperBoundH = int.MaxValue;
            int lowerBoundH = int.MinValue;
            int upperBoundW = int.MaxValue;
            int lowerBoundW = int.MinValue;
            for (int i = 0; i < Hs.Length; i++)
            {
                if (!bird[i])
                {
                    if (Ws[i] <= maxTrueW && Ws[i] >= minTrueW)
                    {
                        if (maxTrueH > int.MinValue && Hs[i] > maxTrueH && Hs[i] < upperBoundH)
                            upperBoundH = Hs[i];
                        if (minTrueH < int.MaxValue && Hs[i] < minTrueH && Hs[i] > lowerBoundH)
                            lowerBoundH = Hs[i];
                    }
                    if (Hs[i] <= maxTrueH && Hs[i] >= minTrueH)
                    {
                        if (maxTrueW > int.MinValue && Ws[i] > maxTrueW && Ws[i] < upperBoundW)
                            upperBoundW = Ws[i];
                        if (minTrueW < int.MaxValue && Ws[i] < minTrueW && Ws[i] > lowerBoundW)
                            lowerBoundW = Ws[i];
                    }
                }
            }
            string[] results = new string[unsureWs.Length];
            for (int i = 0; i < unsureHs.Length; i++)
            {
                if (unsureHs[i] <= maxTrueH && unsureHs[i] >= minTrueH && unsureWs[i] <= maxTrueW && unsureWs[i] >= minTrueW)
                    results[i] = "BIRD";
                else if (unsureHs[i] >= upperBoundH)
                    results[i] = "NOT BIRD";
                else if (unsureHs[i] <= lowerBoundH)
                    results[i] = "NOT BIRD";
                else if (unsureWs[i] >= upperBoundW)
                    results[i] = "NOT BIRD";
                else if (unsureWs[i] <= lowerBoundW)
                    results[i] = "NOT BIRD";
                else
                {
                    bool found = false;
                    for (int j = 0; j < Hs.Length; j++)
                    {
                        if (!bird[j])
                        {
                            if (unsureHs[i] == Hs[j] && unsureWs[i] == Ws[j])
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                        results[i] = "UNKNOWN";
                    else
                        results[i] = "NOT BIRD";
                }
            }
            return results;
        }
    }
}
