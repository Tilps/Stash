using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class A
{
    public int method()
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
            A c = new A();
            object o = c.method();
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
