using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class ConcatenateNumber
{
    public int getSmallest(int number, int k)
    {
        // number/k = blah remainder n
        // number number / k = blah << + 

        long shift = CalcShift(number);
        long cur = number;
        long rem;
        int loops=0;
        do
        {
            Math.DivRem(cur, k, out rem);
            cur = rem * shift + number;
            loops++;
        } while (loops <= k && rem != 0);

        if (rem == 0)
            return loops;
        else
            return -1;
    }

    private long CalcShift(int number)
    {
        long shift = 1;
        while (number > 0)
        {
            shift *= 10;
            number /= 10;
        }
        return shift;
    }
}

namespace Q1
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcatenateNumber c = new ConcatenateNumber();
            object o = c.getSmallest(2,9);
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
