using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ProductsOfDigits
{
    public long firstOccurrence(int[] prod)
    {
        for (int i = 1; i < 3000; i++)
        {
            bool ok = Check(prod, i);
            if (ok)
                return i;
        }
        int first = -1;
        for (int i = 0; i < prod.Length; i++)
        {
            if (prod[i] != 0)
            {
                first = i;
                break;
            }
        }
        long best = long.MaxValue;
        int target = prod[first];
        for (int i = 1; i <= 9; i++)
            for (int j = 1; j <= 9; j++)
                for (int k = 1; k <= 9; k++)
                {
                    int endProd = i * j * k;
                    if (target % endProd != 0)
                        continue;
                    int rest = target / endProd;
                    long smallest = MakeSmall(rest);
                    long trial = smallest * 1000 + i * 100 + j * 10 + k - first;
                    if (Check(prod, trial))
                        if (trial < best)
                            best = trial;
                }
        return best;
    }

    private static bool Check(int[] prod, long i)
    {
        bool ok = true;
        for (int j = 0; j < prod.Length; j++)
        {
            if (Reduce(i + j) != prod[j])
            {
                ok = false;
                break;
            }
        }
        return ok;
    }

    private long MakeSmall(int rest)
    {
        if (rest == 1)
            return 1;
        long mult = 1;
        long result = 0;
        for (int i = 9; i >= 2; i--)
        {
            while (rest % i == 0)
            {
                result += mult * i;
                mult *= 10;
                rest /= i;
            }
        }
        return result;
    }

    private static long Reduce(long l)
    {
        if (l == 0)
            return 0;
        long result = 1;
        while (l > 0)
        {
            result *= l % 10;
            l /= 10;
        }
        return result;
    }
}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            ProductsOfDigits c = new ProductsOfDigits();
            object o = c.firstOccurrence(new int[] { });
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
