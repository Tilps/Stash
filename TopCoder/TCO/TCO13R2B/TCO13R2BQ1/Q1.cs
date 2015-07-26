using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class FruitTrees
{
    public int maxDist(int apple, int kiwi, int grape)
    {   
        int akgcd = 1;
        int kggcd = 1;
        int aggcd = 1;
        for (int i = 2; i <= 2000; i++)
        {
            if ((apple%i) == 0 && (kiwi%i) == 0 )
                akgcd = i;
            if ((apple%i) == 0 && (grape%i) == 0 )
                aggcd = i;
            if ((grape%i) == 0 && (kiwi%i) == 0 )
                kggcd = i;
        }
        
        int best = 0;
        for (int i = 0; i < akgcd; i++)
        {
            int closest = Math.Min(i, akgcd - i);
            if (closest <= best)
                continue;
            for (int j = 0; j < grape; j++)
            {
                int diff1 = j;
                diff1 = diff1 % aggcd;
                diff1 += aggcd;
                diff1 = diff1 % aggcd;
                int closest2 = Math.Min(diff1, aggcd - diff1);
                if (closest2 <= best)
                    continue;
                int diff = i - j;
                diff = diff%kggcd;
                diff += kggcd;
                diff = diff%kggcd;
                int closest3 = Math.Min(diff, kggcd - diff);
                if (closest3 <= best)
                    continue;
                best = Math.Min(closest3, Math.Min(closest, closest2));
                if (closest <= best)
                    break;
            }
        }
        return best;
    }
}

namespace Q3
{
        //    int gcd = 1;
        //for (int i = 2; i <= 2000; i++)
        //{
        //    if ((apple%i) == 0 && (kiwi%i) == 0 && (grape%i) == 0)
        //        gcd = i;
        //}
        //return gcd/3;

    class Q3
    {
        static void Main(string[] args)
        {
            FruitTrees c = new FruitTrees();
            object o = c.maxDist(30,40,50);
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
