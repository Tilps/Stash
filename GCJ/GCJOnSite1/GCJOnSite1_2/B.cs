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
                // Process
                output.Add(string.Format("Case #{0}: {1}", loopvariable + 1, Solve(L)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int L)
        {
            return string.Empty;
        }
    }
}
