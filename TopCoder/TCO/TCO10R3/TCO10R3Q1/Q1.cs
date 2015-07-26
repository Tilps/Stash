using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class SieveOfEratosthenes
{
    public int lastScratch(int input)
    {
        bool[] comps = new bool[(int)Math.Sqrt(input) + 4];
        for (int i = 2; i * i <= input; i++)
        {
            if (!comps[i])
            {
                for (int j = i * 2; j < comps.Length; j+=i)
                {
                    comps[j] = true;
                }
            }
        }
        List<int> primes = new List<int>();
        int limit = (int)Math.Sqrt(input);
        for (int i = 2; i <= limit; i++)
        {
            if (!comps[i])
            {
                primes.Add(i);
            }
        }
        int mult = input / primes[primes.Count-1];
        while (mult >= 2)
        {
            bool bad = false;
            for (int i = 0; i < primes.Count - 1; i++)
            {
                if (mult % primes[i] == 0)
                {
                    bad = true;
                    break;
                }
            }
            if (bad)
                mult--
                   ;
            else break;
        }
        return primes[primes.Count - 1] * mult;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            SieveOfEratosthenes c = new SieveOfEratosthenes();
            object o = c.lastScratch(8);
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
