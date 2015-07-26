using System;
using System.Collections;
using System.Text;

public class FibonacciSum
{
    public int howMany(int n)
    {
        ArrayList fibs = new ArrayList();
        int[] dist = new int[1000001];
        int fib1 = 1;
        int fib2 = 1;
        fibs.Add(1);
        dist[0] = 0;
        while (fib2 <= n)
        {
            int next = fib1 + fib2;
            if (next > 1000000)
                break;
            fibs.Add(next);
            fib1 = fib2;
            fib2 = next;
        }
        for (int i = 1; i <= n; i++)
        {
            int best = int.MaxValue;
            foreach (int j in fibs)
            {
                if (j > i)
                    break;
                int ind = i - j;
                int next = dist[ind] + 1;
                if (next < best)
                    best = next;
            }
            dist[i] = best;
        }


        return dist[n];
    }
}

namespace TCO05R1Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            FibonacciSum c = new FibonacciSum();
            object o = c.howMany(1);

            if (o is IEnumerable)
            {
                foreach (object oi in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(oi);

                }
            }
            else
                System.Console.Out.WriteLine(o);
            System.Console.In.ReadLine();
        }
    }
}