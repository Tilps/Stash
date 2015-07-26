using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class TwoRegisters
{

    public static double GoldenRatio = (1.0 + Math.Sqrt(5)) / 2.0;
    
    public string minProg(int r)
    {
        lookup = new string[250, 250];
        if (r == 1)
            return string.Empty;
        if (r == 2)
            return "X";
        string best = null;
        for (int i = r / 2; i >= 1; i--)
        {
            int y = r - i;
            string res = Make(i, y);
            if (res != null) {
                string next = res +"X";
                if (best == null)
                    best = next;
                else if (next.Length < best.Length)
                {
                    best = next;
                }
                else if (next.Length == best.Length && string.Compare(next, best) < 0)
                    best = next;

            }
        }
        return best;
    }
    string[,] lookup;

    private string Make(int x, int y)
    {
        if (x < 250 && y < 250 && lookup[x, y] != null)
            return lookup[x, y] == "NULL" ? null : lookup[x, y];
        if (x == y && x == 1)
        {
            return UpdateLookup(x, y, string.Empty);
        }
        if (x == y)
        {
            return UpdateLookup(x, y, null);
        }
        if (x < y)
        {
            string prev = Make(x, y - x);
            if (prev != null)
            {
                return UpdateLookup(x, y, prev + "Y");
            }
            else
            {
                return UpdateLookup(x, y, null);
            }
        }
        else
        {
            string prev = Make(x - y, y);
            if (prev != null)
            {
                return UpdateLookup(x, y, prev + "X");
            }
            else
            {
                return UpdateLookup(x, y, null);
            }
        }
    }

    private string UpdateLookup(int x, int y, string res)
    {
        if (res != null)
        {
            if (x < 250 && y < 250)
            {
                lookup[x, y] = res;
            }
        }
        else
        {
            if (x < 250 && y < 250)
            {
                lookup[x, y] = "NULL";
            }
        }
        return res;
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            TwoRegisters c = new TwoRegisters();
            object o = c.minProg(1000000);
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
