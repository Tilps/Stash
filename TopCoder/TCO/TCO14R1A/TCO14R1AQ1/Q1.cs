using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class EllysSortingTrimmer
{
    public string getMin(string S, int L)
    {
        char[] letters = S.ToCharArray();
        for (int i = letters.Length - L; i >= 0; i--)
        {
            Array.Sort(letters, i, L);
        }
        return new string(letters, 0, Math.Min(L, letters.Length));
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            EllysSortingTrimmer c = new EllysSortingTrimmer();
            object o = c.getMin("ABRACADABRA", 5);
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