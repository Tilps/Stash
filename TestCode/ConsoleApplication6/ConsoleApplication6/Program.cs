using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication6
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = i; j < 50; j++)
                {
                    double c = Math.Sqrt(i * i + j * j);
                    if (c - Math.Round(c) < 0 && c - Math.Round(c) > -0.01)
                        Console.Out.WriteLine("{0}, {1}, {2}", i, j, c);
                }
            }
            Console.ReadKey();
        }
    }
}
