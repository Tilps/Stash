using System;
using System.Collections;
using System.Text;

public class Piglets
{
    int[][] memo;
    int length;

    private int[] go(int t)
    {
        if (memo[t] != null)
        {
            return memo[t];
        }
        int[] res = new int[0];
        if (t == (1 << length) - 1)
        {
            res = new int[0];
        }
        else
        {
            int[] best = null;
            int bestTime = int.MinValue;
            int bestIndex = -1;
            for (int i = 1; i < length - 1; i++)
            {
                if ((t & (1 << i)) == 0)
                {
                    int[] temp = go(t | (1 << i));
                    int imin1 = int.MaxValue;
                    int iplus1 = int.MaxValue;
                    if ((t & (1 << (i - 1))) != 0)
                    {
                        imin1 = 0;
                    }
                    if ((t & (1 << (i + 1))) != 0)
                    {
                        iplus1 = 0;
                    }
                    int time =0;
                    for (int j = temp.Length - 1; j >= 0; j--)
                    {
                        time++;
                        if (temp[j] == i - 1)
                        {
                            if (imin1 > time)
                            {
                                imin1 = time;
                            }
                        }
                        if (temp[j] == i + 1)
                        {
                            if (iplus1 > time)
                            {
                                iplus1 = time;
                            }
                        }
                    }
                    int longTime = imin1 < iplus1 ? iplus1 : imin1;
                    if (longTime > bestTime)
                    {
                        best = temp;
                        bestTime = longTime;
                        bestIndex = i;
                    }
                }
            }
            res = new int[best.Length + 1];
            for (int i = 0; i < res.Length - 1; i++)
            {
                res[i] = best[i];
            }
            res[best.Length] = bestIndex;
        }

        memo[t] = res;
        return res;
    }

    public int choose(string trough)
    {
        if (trough[0] == '-')
            return 0;
        if (trough[trough.Length - 1] == '-')
            return trough.Length - 1;
        length = trough.Length;
        memo = new int[1<<trough.Length][];
        for (int i = 0; i < memo.Length; i++)
        {
            memo[i] = null;
        }
        int t = 0;
        for (int i = 0; i < length; i++)
        {
            if (trough[i] == 'p')
                t += (1 << i);
        }

        int[] res =  go(t);
        if (res.Length > 0)
            return res[res.Length - 1];
        else
            return -1;
    }
}

namespace SRM254Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            Piglets c = new Piglets();
            System.Console.Out.WriteLine(c.choose("p-------------p"));
            System.Console.In.ReadLine();
        }
    }
}
