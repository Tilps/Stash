using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;


public class RepresentableNumbers
{
    public int getNext(int X)
    {
        List<int> totallyOdd = new List<int>();
        totallyOdd.Add(1);
        totallyOdd.Add(3);
        totallyOdd.Add(5);
        totallyOdd.Add(7);
        totallyOdd.Add(9);
        for (int i = 2; i <= 8; i++)
        {
            Populate(i, 0, totallyOdd);
        }
        int j = totallyOdd.Count - 1;
        int best = totallyOdd[0] + totallyOdd[j];
        for (int i = 0; i <= j; i++)
        {
            while (j > 0 && totallyOdd[i] + totallyOdd[j-1] >= X)
            {
                j--;
            }
            if (totallyOdd[i] + totallyOdd[j] >= X && totallyOdd[i] + totallyOdd[j] < best)
                best = totallyOdd[i] + totallyOdd[j];
        }
        return best;
    }

    private void Populate(int depth, int seed, List<int> totallyOdd)
    {
        if (depth == 0)
            totallyOdd.Add(seed);
        else
        {
            for (int i = 1; i <= 9; i += 2)
            {
                Populate(depth - 1, seed * 10 + i, totallyOdd);
            }
        }
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            RepresentableNumbers c = new RepresentableNumbers();
            object o = c.getNext(99999998);
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
