using System;
using System.Collections;
using System.Text;

public class Runway
{
    public int inspect(int[] x0, int[] x1)
    {
        int count = 0;
        int[] covered = new int[5281];
        for (int i = 0; i < x0.Length; i++)
        {
            int curStart = Math.Min(x0[i], x1[i]);
            int curEnd = Math.Max(x0[i], x1[i]);
            if (curStart == curEnd)
                continue;
            for (int j = curStart; j <= curEnd; j++)
            {
                covered[j] = i + 1;
            }
        }
        for (int i = 0; i < x0.Length; i++)
        {
            int curStart = Math.Min(x0[i], x1[i]);
            int curEnd = Math.Max(x0[i], x1[i]);
            if (curStart == curEnd)
                continue;
            bool fullycovered = true;
            for (int j = curStart; j <= curEnd; j++)
            {
                if (covered[j] == i + 1)
                {
                    fullycovered = false;
                    break;
                }
            }
            if (!fullycovered)
                count++;
        }
        return count;
    }
}

namespace TCO06QRQ1
{
    class Program
    {
        static void Main(string[] args)
        {
            Runway c = new Runway();
            object o = c.inspect(new int[] { }, new int[] { });

            if (o is IEnumerable)
            {
                foreach (object oi in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(oi);

                }
            }
            else
                System.Console.Out.WriteLine(o);
            System.Console.In.ReadLine();
        }
    }
}