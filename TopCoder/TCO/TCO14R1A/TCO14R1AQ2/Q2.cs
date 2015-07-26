using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class EllysScrabble
{
    public string getMin(string letters, int maxDistance)
    {
        char[] output = new char[letters.Length];
        bool[] used = new bool[letters.Length];
        for (int i = 0; i < letters.Length; i++)
        {
            int best = -1;
            for (int j = i - maxDistance; j <= i + maxDistance; j++)
            {
                if (j < 0 || j >= letters.Length) continue;
                if (used[j]) continue;
                if (j == i - maxDistance)
                {
                    best = j;
                    break;
                }
                if (best == -1 || letters[j] < letters[best])
                {
                    best = j;
                }
            }
            output[i] = letters[best];
            used[best] = true;
        }
        return new string(output);
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            EllysScrabble c = new EllysScrabble();
            object o = c.getMin("HELLO", 3);
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