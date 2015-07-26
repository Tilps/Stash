using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCJ08R3Q3
{
    class ProgramQ3
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
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, "Solve()"));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }
    }
}
