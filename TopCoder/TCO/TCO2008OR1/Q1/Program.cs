using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class DiscountCombination
{
    public double minimumPrice(string[] discounts, int price)
    {
        int[,] discs = new int[discounts.Length, 2];
        int j=0;
        foreach (string discount in discounts)
        {
            string[] bits = discount.Split(' ');
            discs[j, 0] = int.Parse(bits[0]);
            discs[j, 1] = int.Parse(bits[1]);
            j++;
        }
        // remove n% + cost
        bool[] used = new bool[discounts.Length];
        for (int i = 0; i < used.Length; i++)
            used[i] = true;
        bool improved = false;
        do
        {
            int worst = -1;
            double biggestImprovement = 0.0;
            double cur = CalcSituation(discs, used, price);
            for (int i = 0; i < discounts.Length; i++)
            {
                if (!used[i])
                    continue;
                used[i] = false;
                double next = CalcSituation(discs, used, price);
                double improv = cur - next;
                if (improv > biggestImprovement)
                {
                    biggestImprovement = improv;
                    worst = i;
                }
                used[i] = true;
            }
            if (worst != -1)
            {
                used[worst] = false;
                improved = true;
            }
            else
            {
                improved = false;
            }
        } while (improved);
        double p = CalcSituation(discs, used, price);
        return p;
    }

    private double CalcSituation(int[,] discs, bool[] used, int price)
    {
        double p = price;
        int addCosts = 0;
        for (int i = 0; i < used.Length; i++)
        {
            if (used[i])
            {
                addCosts += discs[i, 0];
                p *= (double)(100 - discs[i, 1]) / 100.0;
            }
        }
        return p + addCosts;
    }
}

namespace Q1
{
    class Program
    {
        static void Main(string[] args)
        {
            DiscountCombination c = new DiscountCombination();
            object o = c.minimumPrice(new string[] { "1 1", "1 2", "1 3" }, 100);
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
