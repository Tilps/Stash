using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class EllysFractions
{
    public long getCount(int N)
    {
        if (N == 1)
            return 0;

        bool[] primes = new bool[400];
        for (int i=0; i< primes.Length; i++)
            primes[i] = true;
        for (int i = 2; i < 20; i++)
        {
            if (primes[i])
            {
                for (int k = i * i; k < 400; k += i)
                    primes[k] = false;
            }
        }
        int[] primeCounts = new int[400];

        long total = 0;
        for (int k = 2; k <= N; k++)
            total += Solve(k, primes, primeCounts);
        return total;
    }

    private long Solve(int k, bool[] primes, int[] primeCounts)
    {
        for (int i = 2; i < primes.Length; i++)
        {
            if (primes[i])
            {
                while (k % i == 0 && k > 1)
                {
                    k /= i;
                    primeCounts[i]++;
                }
            }
        }
        List<int> distinctPrimes = new List<int>();
        for (int i = 0; i < primeCounts.Length; i++)
            if (primeCounts[i] > 0)
                distinctPrimes.Add(i);
        long total = 0;
        for (int i = 0; i <= distinctPrimes.Count; i++)
        {
            total += Choose(i, distinctPrimes.Count);
        }
        return total / 2;
    }

    private long Choose(int k, int n)
    {
        if (choose == null)
            BuildChoose();
        return choose[k, n];
    }

    private void BuildChoose()
    {
        choose = new long[100, 100];
        choose[0, 0] = 1;
        choose[0, 1] = 1;
        choose[1, 1] = 1;
        for (int i = 2; i < 100; i++)
        {
            choose[0, i] = 1;
            choose[i, i] = 1;
            for (int j = 1; j < i; j++)
            {
                choose[j, i] = choose[j, i - 1] + choose[j - 1, i - 1];
            }
        }
    }
    long[,] choose;

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            EllysFractions c = new EllysFractions();
            object o = c.getCount(100);
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
