using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;
// http://www.themissingdocs.net/downloads/TMD.Algo.0.0.4.0.zip
using TMD.Algo.Algorithms;
using TMD.Algo.Collections;

namespace GCJ10R1AQ4
{
    class Q4
    {
        static void Main(string[] args)
        {
            double fibRatio = 1.0;
            double fibRatioPrev = 1.0;
            int lastFib = 1;
            int curFib = 1;
            int counter = 0;
            while (true)
            {
                int nextFib = lastFib + curFib;
                double nextFibRatio = (double)nextFib / (double)curFib;
                Console.Out.WriteLine(Math.Abs((nextFibRatio-fibRatio)*2/(nextFibRatio+fibRatio)));
                if (Math.Abs(nextFibRatio - fibRatio) < 0.0000005)
                {
                    Console.Out.WriteLine(counter);
                    break;
                }
                fibRatioPrev = fibRatio;
                fibRatio = nextFibRatio;
                lastFib = curFib;
                curFib = nextFib;
                counter++;
            }
            Console.ReadKey();
        }

    }
}
