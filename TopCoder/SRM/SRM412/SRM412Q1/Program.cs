using System;
using System.Collections.Generic;
using System.Text;

public class ForbiddenStrings
{
    public long countNotForbidden(int a)
    {
        long[,] calc = new long[a + 1, 9];
        for (int i = 0; i < 9; i++)
        {
            int b = i / 3;
            int c = i % 3;
            if (b == c)
                calc[1, i] = 1;
            if (a > 1)
                calc[2, i] = 1;
        }
        for (int i = 3; i <= a; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                int b = j / 3;
                int c = j % 3;
                for (int k = 0; k < 3; k++)
                {
                    int next = (c) * 3 + k;
                    if (b == c || k == c || k == b)
                        calc[i, next] += calc[i - 1, j];
                }
            }
        }
        long total = 0;
        for (int i = 0; i < 9; i++)
        {
            total += calc[a, i];
        }
        return total;
    }
}

namespace SRM412Q1
{
    class Program
    {
        static void Main(string[] args)
        {
            ForbiddenStrings instance = new ForbiddenStrings();
            Console.Out.WriteLine(instance.countNotForbidden(30));
            Console.ReadKey();
        }
    }
}
