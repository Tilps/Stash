using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TestA
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> lines = new List<string>();
            lines.Add("1");
            int min = 0;
            int max = 25;
            int minY = 2;
            int maxY = 45;
            Random rnd = new Random();
            int L = 20;
            int M = 30;
            lines.Add(L.ToString());
            for (int i = 0; i < L; i++)
            {
                int x = rnd.Next(0, 100);
                int y = rnd.Next(0, 100);
                if (x <= max && x >= min && y <= maxY && y >= minY)
                {
                    lines.Add(string.Format("{0} {1} {2}", x, y, "BIRD"));
                }
                else
                    lines.Add(string.Format("{0} {1} {2}", x, y, " NOT BIRD"));
            }
            lines.Add(M.ToString());
            for (int i = 0; i < M; i++)
            {
                int x = rnd.Next(0, 100);
                int y = rnd.Next(0, 100);
                if (x <= max && x >= min && y <= maxY && y >= minY)
                {
                    lines.Add(string.Format("{0} {1} {2}", x, y, "BIRD"));
                }
                else
                    lines.Add(string.Format("{0} {1} {2}", x, y, " NOT BIRD"));
            }
            File.WriteAllLines("sample.in", lines.ToArray());
        }
    }
}
