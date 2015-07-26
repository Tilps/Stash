using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class GirlsAndBoys
{
    public int sortThem(string row)
    {
        int gfirst = 0;
        char[] sort1 = row.ToCharArray();
        while (true)
        {
            bool nothing = true;
            for (int i = 0; i < sort1.Length-1; i++)
            {
                if (sort1[i] == 'B' && sort1[i + 1] == 'G')
                {
                    nothing = false;
                    char temp = sort1[i];
                    sort1[i] = sort1[i + 1];
                    sort1[i + 1] = temp;
                    gfirst++;
                }
            }
            if (nothing)
                break;
        }
        int bfirst = 0;
        char[] sort2 = row.ToCharArray();
        while (true)
        {
            bool nothing = true;
            for (int i = 0; i < sort2.Length - 1; i++)
            {
                if (sort2[i] == 'G' && sort2[i + 1] == 'B')
                {
                    nothing = false;
                    char temp = sort2[i];
                    sort2[i] = sort2[i + 1];
                    sort2[i + 1] = temp;
                    bfirst++;
                }
            }
            if (nothing)
                break;
        }

        return Math.Min(gfirst, bfirst);
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            GirlsAndBoys c = new GirlsAndBoys();
            object o = c.sortThem("B");
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
