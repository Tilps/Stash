using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class T
{
    public int m()
    {
        return 0;
    }
}

namespace Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            T c = new T();
            object o = c.m();
            PrintObj(o, 0);
            System.Console.In.ReadLine();
        }

        private static void PrintObj(object o, int depth)
        {
            System.Console.Out.Write(new string(' ', depth));
            if (o is string)
            {
                System.Console.Out.WriteLine(o);
            }
            else if (o is IEnumerable)
            {
                System.Console.Out.WriteLine("List:");
                foreach (object b in (IEnumerable)o)
                {
                    PrintObj(b, depth + 1);
                }
            }
            else
                System.Console.Out.WriteLine(o);
        }
    }
}
