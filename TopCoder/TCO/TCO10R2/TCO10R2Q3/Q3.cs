using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class BreakingChocolate
{
    public int minSteps(int W, int H, int[] sx, int[] sy)
    {
        return 0;
    }

}

namespace Q3
{
    class Q3
    {
        static void Main(string[] args)
        {
            BreakingChocolate c = new BreakingChocolate();
            object o = c.minSteps(100000, 1000000, new int[] {0}, new int[] { 1 });
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
