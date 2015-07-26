using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {
            int divisor = 2543;
            Parallel.For(int.MinValue, int.MaxValue, i =>
            {
                if (i != int.MinValue && Math.Abs(i) % 10000000 == 0)
                    Console.Out.WriteLine("Trying:" + i);
                Check(divisor, i, 0);
                Check(divisor, i, i);
                if (i != int.MinValue)
                    Check(divisor, i, -i);
            });
            Check(divisor, int.MaxValue, 0);
            Check(divisor, int.MaxValue, int.MaxValue);
            Check(divisor, int.MaxValue, -int.MaxValue);
        }

        private static void Check(int divisor, int trial, int p)
        {
            for (int j = 0; j <= 32; j++)
            {
                int first = int.MaxValue;
                for (long i = int.MinValue; i <= int.MaxValue; i++)
                {
                    int expected = first / divisor;
                    int check = (int)(((long)first * (long)trial + (long)p) >> j);
                    if (expected != check)
                        goto a;
                    first++;
                }
                Console.Out.WriteLine("" + divisor + ":" + trial + ":" + p + ":" + j);
                a: first = 0;
            }
        }
    }
}
