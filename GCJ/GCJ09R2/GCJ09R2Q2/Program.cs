using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCJ09R2Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] file = File.ReadAllLines(args[0]);
            List<string> output = new List<string>();
            int count = int.Parse(file[0]);
            int offset = 1;
            for (int counter = 0; counter < count; counter++)
            {
                string[] bits = file[offset].Split(' ');
                int[] bases = new int[bits.Length];
                for (int i = 0; i < bits.Length; i++)
                {
                    bases[i] = int.Parse(bits[i]);
                }
                offset++;
                output.Add(string.Format("Case #{0}: {1}", counter + 1, Solve()));
            }
            File.WriteAllLines("output.txt", output.ToArray());

        }

        private static object[] Solve()
        {
            throw new NotImplementedException();
        }
    }
}
