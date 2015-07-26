using System;
using System.Collections;
using System.Text;

public class Snaky
{
    public int longest(string[] snake)
    {
        int width = snake[0].Length;
        int height = snake.Length;
        int l = -1;
        for (int i = 0; i < width; i++)
        {
            int cur = 0;
            for (int j = 0; j < height; j++)
            {
                if (snake[j][i] == 'x')
                {
                    cur++;
                }
                else
                {
                    if (cur > l)
                        l = cur;
                    cur = 0;
                }
            }
            if (cur > l)
                l = cur;
        }
        for (int i = 0; i < height; i++)
        {
            int cur = 0;
            for (int j = 0; j < width; j++)
            {
                if (snake[i][j] == 'x')
                {
                    cur++;
                }
                else
                {
                    if (cur > l)
                        l = cur;
                    cur = 0;
                }
            }
            if (cur > l)
                l = cur;
        }
        return l;
    }
}

namespace Q1
{
    class Program
    {
        static void Main(string[] args)
        {
            Snaky c = new Snaky();
            object o = c.longest(new string[] {});
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
