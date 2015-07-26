using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;


public class EllysLamps
{
    public int getMin(string lamps)
    {
        int[] best = new int[lamps.Length+1];
        for (int i = 1; i < lamps.Length; i++)
        {
            int curBest = best[i-1];
            if (lamps[i] == 'Y' && lamps[i - 1] == 'N'
                || lamps[i] == 'N' && lamps[i - 1] == 'Y')
            {
                if (i > 1)
                {
                    curBest = Math.Max(curBest, best[i - 2] + 1);
                }
                else
                {
                    curBest = Math.Max(curBest, 1);
                }
            }
            if (lamps[i] == 'Y' && lamps[i - 1] == 'Y' && i > 1 && lamps[i - 2] == 'Y')
            {
                if (i > 2)
                {
                    curBest = Math.Max(curBest, best[i - 3] + 1);
                }
                else
                {
                    curBest = Math.Max(curBest, 1);
                }
            }
            best[i] = curBest;
        }
        return best[lamps.Length - 1];
    }

}

namespace Q3
{
    class Q3
    {
        static void Main(string[] args)
        {
            EllysLamps c = new EllysLamps();
            object o = c.getMin("YNYYYYNYNNYYNNNNNNYNYNYNYNNYNYYYNY");
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

