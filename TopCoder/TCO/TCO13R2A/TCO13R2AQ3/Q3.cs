using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class ThePowers
{
    public long find(int A, int B)
    {
        long answer = (long)A*(long)B;
        // Duplicates of unity.
        answer -= B - 1;
        for (int i = 2; i*i <= A; i++)
        {

        }
        return answer;
    }

}

namespace Q3
{
    class Q3
    {
        static void Main(string[] args)
        {
            ThePowers c = new ThePowers();
            object o = c.find(4, 7);
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
