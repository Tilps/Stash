using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class BlackWhiteMagic
{
    public int count(string creatures)
    {
        bool[] array = new bool[creatures.Length];
        for (int i = 0; i < creatures.Length; i++)
        {
            array[i] = creatures[i] == 'B';
        }
        int first = 0;
        int last = array.Length - 1;
        int count = 0;
        while (first < last)
        {
            if (array[first] && !array[last])
            {
                count++;
                first++;
                last--;
            }
            else if (array[first])
            {
                last--;
            }
            else if (!array[last])
            {
                first++;
            }
            else
            {
                last--;
                first++;
            }
        }
        return count;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            BlackWhiteMagic c = new BlackWhiteMagic();
            object o = c.count("B");
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
