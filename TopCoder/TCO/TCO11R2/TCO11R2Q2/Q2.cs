using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class STable
{
    public string getString(string s, string t, long pos)
    {
        long[,] lengths = new long[s.Length+1, t.Length+1];
        for (int i = 0; i < s.Length; i++)
            lengths[i+1, 0] = 1;
        for (int i = 0; i < t.Length; i++)
            lengths[0, i+1] = 1;
        for (int i = 1; i <= s.Length; i++)
            for (int j = 1; j <= s.Length; j++)
                lengths[i, j] = lengths[i - 1, j] + lengths[i, j - 1];

        // <900 distinct lengths.
        int[,] smaller = new int[s.Length + 1, t.Length + 1];
        for (int i = 1; i <= s.Length; i++)
            for (int j = 1; j <= s.Length; j++)
                smaller[i, j] = 1; // lengths[i - 1, j] + lengths[i, j - 1];

        //   a b c
        // a aaaabaabc
        // baabbabababac
        // caabccabacabacabac

        return "";
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            STable c = new STable();
            object o = c.getString("a", "A", 1);
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
