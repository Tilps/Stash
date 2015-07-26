using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SequenceSums
{
    public int[] sequence(int N, int L)
    {
        for (int i = L; i <= 100; i++)
        {
            // Median entry has to be N/L
            // N = n(n+1)/2 - k(k+1)/2 where n=i+k;
            // 2N = (i+k)(i+k+1) -k(k+1)
            // 2N = i^2 + ik + i + ik + k^2 + k -k^2-k
            // 2N = i^2 +2ik +i
            // k = (2N-i^2-i)/2i

            int lowest = 2 * N - i * i - i;
            if ((lowest % (2 * i)) == 0)
            {
                if (lowest / 2 / i < -1)
                    continue;
                int[] result = new int[i];
                for (int j = 0; j < i; j++)
                {
                    result[j] = lowest / 2 / i + j+1;
                }
                return result;
            }
        }
        return new int[] {};
    }
}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            SequenceSums c = new SequenceSums();
            object o = c.sequence(18, 5);
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
