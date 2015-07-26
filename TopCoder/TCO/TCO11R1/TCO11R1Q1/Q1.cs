using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class TripleStrings
{
    public int getMinimumOperations(string init, string goal)
    {
        for (int i = 0; i < init.Length; i++)
        {
            if (init.Substring(i) == goal.Substring(0, goal.Length - i))
                return 2 * i;
        }
        return 2*init.Length;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            TripleStrings c = new TripleStrings();
            object o = c.getMinimumOperations("","");
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
