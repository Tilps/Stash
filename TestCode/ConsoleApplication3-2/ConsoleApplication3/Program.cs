using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i < 1000000000; i++)
            {
                double realRoot = Math.Pow(i, 1.0/3.0);
                int floor = (int) Math.Floor(realRoot);
                int firstDiff = i - floor*floor*floor;
                int secondDiff = (floor + 1)*(floor + 1)*(floor + 1) - i;
                if (firstDiff < secondDiff && realRoot-floor > 0.5 || secondDiff < firstDiff && realRoot-floor < 0.5)
                    Console.Out.WriteLine(i);
            }
        }
    }
}
