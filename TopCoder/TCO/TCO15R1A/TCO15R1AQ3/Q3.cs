using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class Revmatching
{
    public int smallest(string[] A)
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
            Revmatching c = new Revmatching();
            object o = c.smallest(new string[] {});
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

