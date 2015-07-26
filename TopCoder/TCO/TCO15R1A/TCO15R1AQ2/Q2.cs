using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Numerics;

public class Autogame
{
    public int wayscnt(int[] a, int K)
    {
        bool[,] invalid = new bool[a.Length,a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            for (int j = i + 1; j < a.Length; j++)
            {
                int x = i;
                int y = j;
                bool failed = false;
                for (int k = 0; k < K && k < 6000; k++)
                {
                    x = a[x] - 1;
                    y = a[y] - 1;
                    if (x == y)
                    {
                        failed = true;
                        break;
                    }
                }
                if (failed)
                {
                    invalid[i, j] = true;
                    invalid[j, i] = true;
                }
            }
        }
        int[] groups = new int[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            groups[i] = i;
        }
        for (int i = 0; i < a.Length; i++)
        {
            for (int j = i+1; j < a.Length; j++)
            {
                if (invalid[i, j])
                {
                    groups[j] = groups[i];
                }
            }
            
        }
        int[] groupCounts = new int[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            groupCounts[groups[i]]++;
        }
        long total = 1;
        for (int i = 0; i < a.Length; i++)
        {
            total *= (long)(groupCounts[i] + 1);
        }

        return (int)(total % 1000000007);
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            Autogame c = new Autogame();
            object o = c.wayscnt(new int[] {2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50, 1}, 1000000000);
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