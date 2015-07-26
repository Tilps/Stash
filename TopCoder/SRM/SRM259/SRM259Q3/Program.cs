using System;
using System.Collections;
using System.Text;

public class CardRemover
{
    private int comp(string a, string b)
    {
        int total = 0;
        for (int i = 0; i < a.Length; i++)
        {
            for (int j = 0; j < b.Length; j++)
            {
                if (a[i] == b[j])
                    total++;
            }
        }
        return total;
    }
    public int calculate(string[] cards)
    {
        ArrayList c = new ArrayList(cards);
        int removed = 0;
        bool moved = true;
        while (moved)
        {
            moved = false;
            int bestRem = -1;
            int bestRemScore = 0;
            for (int i = 1; i < c.Count-1; i++)
            {
                int common = comp((string)c[i - 1], (string)c[i + 1]);
                if (common >= 2)
                {
                    int score = 3;
                    if (i > 1)
                    {
                        int comB = comp((string)c[i - 2], (string)c[i]);
                        int comC = comp((string)c[i - 2], (string)c[i + 1]);
                        if (comB >= 2 && comC < 2)
                        {
                            score--;
                        }
                        else if (comB < 2 && comC >= 2)
                        {
                            score++;
                        }
                    }
                    if (i < c.Count - 2)
                    {
                        int comB = comp((string)c[i + 2], (string)c[i]);
                        int comC = comp((string)c[i - 1], (string)c[i + 2]);
                        if (comB >= 2 && comC < 2)
                        {
                            score--;
                        }
                        else if (comB < 2 && comC >= 2)
                        {
                            score++;
                        }
                    }
                    if (score > bestRemScore)
                    {
                        bestRemScore = score;
                        bestRem = i;
                    }
                }
            }
            if (bestRem != -1)
            {
                c.RemoveAt(bestRem);
                moved = true;
                removed++;
            }
        }
        return removed;
    }
}

namespace SRM259Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            CardRemover c = new CardRemover();
            object o = c.calculate(new string[] { });
            if (o is IEnumerable)
            {
                foreach (object b in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(b);
                }
            }
            else
                System.Console.Out.WriteLine(o);
            System.Console.In.ReadLine();
        }
    }
}
