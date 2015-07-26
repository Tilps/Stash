using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class SumAndProductPuzzle
{
    public long getSum(int a_in, int b_in)
    {
        long a = a_in;
        long b = b_in;
        bool[] comps = new bool[b+1];
        for (int i = 2; i*i <= b; i++)
        {
            if (comps[i]) continue;
            for (int j = i*i; j <= b; j += i)
            {
                comps[j] = true;
            }
        }
        int[] counts = new int[b + 1];
        for (int i = 2; i <= b; i++)
        {
            for (long j = 1; j <= i/2; j++)
            {
                if (counts[i] > 1) break;
                long k = i - j;

            }
        }
        bool[] noPrimeSum = new bool[b+1];
        for (long i = 2; i <= b; i++)
        {
            long matches = 0;
            for (long j = 1; j*j <= i; j++)
            {
                if (i%j != 0) continue;
                long k = i/j;
                long l = j + k;
                if (comps[l - 1]) matches++;
            }
        }
        // S is satisfactory if not prime + 1 and only one decomp product can be re decomped and summed to give not a prime + 1
        return 0;
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            SumAndProductPuzzle c = new SumAndProductPuzzle();
            object o = c.getSum();
            PrintObj(o);
            System.Console.In.ReadLine();
        }

        private static void PrintObj(object o)
        {
            if (o is IEnumerable)
            {
                foreach (object b in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(b);
                }
            }
            else
                System.Console.Out.WriteLine(o);
        }
    }
}