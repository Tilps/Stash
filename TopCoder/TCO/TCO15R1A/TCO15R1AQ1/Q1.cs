using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Xml.Schema;

public class Similars
{
    public int maxsim(int L, int R)
    {
        if (R - L > 200)
        {
            int best = 0;
            for (int x = L; x <= R; x++)
            {
                int score = Value(x, L, R);
                if (score > best) best = score;
            }
            return best;
        }
        int best2 = 0;
        for (int x = L; x <= R; x++)
        {
            for (int y = x + 1; y <= R; y++)
            {
                int score = Value(x, y);
                if (score > best2) best2 = score;
            }
        }
        return best2;
    }

    private int Value(int x, int y)
    {
        int[] parts = Split(x);
        int[] parts2 = Split(y);
        var counts = Counts(parts);
        var counts2 = Counts(parts2);
        int digitCount = 0;
        for (int i = 0; i < 10; i++)
        {
            if (counts[i] > 0 && counts2[i] > 0) digitCount++;
        }
        return digitCount;
    }

    private int Value(int x, int L, int R)
    {
        int[] parts = Split(x);
        var counts = Counts(parts);
        int digitCount = 0;
        for (int i = 0; i < 10; i++)
        {
            if (counts[i] > 0) digitCount++;
        }
        for (int i = 0; i < parts.Length; i++)
        {
            for (int j = i + 1; j < parts.Length; j++)
            {
                if (j == parts.Length - 1 && parts[i] == 0) continue;
                if (parts[i] == parts[j]) continue;
                int temp = parts[i];
                parts[i] = parts[j];
                parts[j] = temp;
                int next = Eval(parts);
                if (next >= L && next <= R)
                {
                    return digitCount;
                }
                temp = parts[i];
                parts[i] = parts[j];
                parts[j] = temp;
            }
        }
        return digitCount - 1;
    }

    private int Eval(int[] parts)
    {
        int total = 0;
        for (int i = parts.Length - 1; i >= 0; i--)
        {
            total *= 10;
            total += parts[i];
        }
        return total;
    }

    private static int[] Counts(int[] parts)
    {
        int[] counts = new int[10];
        for (int i = 0; i < parts.Length; i++)
        {
            counts[parts[i]]++;
        }
        return counts;
    }

    private int[] Split(int x)
    {
        List<int> parts = new List<int>();
        while (x != 0)
        {
            parts.Add(x % 10);
            x /= 10;
        }
        return parts.ToArray();
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            Similars c = new Similars();
            object o = c.maxsim(9999,10300);
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