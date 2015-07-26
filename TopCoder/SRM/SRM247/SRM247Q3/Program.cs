
using System;
using System.Collections;
using System.Text;

public class Necklace
{
    int[,,] scores;

    int size;

    private int Best(int n, ArrayList cuts, int[] gems)
    {
        if (cuts.Count >= 2)
        {
            int val = scores[n, (int)cuts[0], (int)cuts[cuts.Count-1]];
            if (val != int.MaxValue)
                return val;
        }
        if (n == 0)
        {
        }
        return 0;
    }

    public int inequity(int n, int[] gems)
    {
        scores = new int[n, n, n];
        size = n;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    scores[i, j, k] = int.MaxValue;
                }
            }
        }
        ArrayList cuts = new ArrayList();
        return Best(n, cuts, gems);
        return 0;
    }
}

namespace SRM247Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            Necklace blah = new Necklace();
            Console.Out.WriteLine(blah.inequity(1, new int[] {}));
            Console.In.ReadLine();
        }
    }
}
