using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class FoxAndKgram
{
    public int maxK(int[] len)
    {
        Array.Sort(len);
        for (int i = len.Length; i > 0; i--)
        {
            if (Try(i, len))
                return i;
        }
        return 0;
    }

    private bool Try(int i, int[] len)
    {
        int count = 0;
        int top = len.Length - 1;
        while (top >= 0 && len[top] > i)
            top--;
        while (top >= 0 && len[top] == i)
        {
            count++;
            top--;
        }
        int bottom = 0;
        while (bottom < top)
        {
            if (len[bottom] + len[top] == i - 1)
            {
                bottom++;
                top--;
                count++;
            }
            else if (len[bottom] + len[top] > i - 1)
                top--;
            else
                bottom++;
        }
        return (count >= i);
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            FoxAndKgram c = new FoxAndKgram();
            object o = c.maxK(new int[] {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1});
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
