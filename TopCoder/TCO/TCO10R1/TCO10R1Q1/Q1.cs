using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class EqualizeStrings
{
    public string getEq(string s, string t)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < s.Length; i++)
        {
            char a = s[i];
            char b = t[i];
            int x = a - 'a';
            int y = b - 'a';
            if (x < y)
            {
                if (y - x < 13)
                    result.Append(a);
                else
                    result.Append('a');
            }
            else
            {
                if (x - y < 13)
                    result.Append(b);
                else
                    result.Append('a');
            }
        }
        return result.ToString();
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            EqualizeStrings c = new EqualizeStrings();
            object o = c.getEq("dog", "cat");
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
