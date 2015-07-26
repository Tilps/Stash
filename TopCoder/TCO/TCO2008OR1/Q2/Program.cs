using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class Chomp
{
    public int moves(int N)
    {
        int[, ,] movesToWin = new int[N+1, N+1, N+1];
        movesToWin[0, 0, 0] = 0;
        for (int i = 0; i <= N; i++)
        {
            for (int j = i; j <= N; j++)
            {
                int start = j;
                if (start == 0)
                    start = 1;
                for (int k = start; k <= N; k++)
                {
                    int bestWin = int.MaxValue;
                    int bestLoss = int.MinValue;
                    for (int pos = 0; pos < i; pos++)
                    {
                        int next = movesToWin[pos, j, k] + 1;
                        if ((next) % 2 == 0)
                        {
                            if (next < bestWin)
                                bestWin = next;
                        }
                        else
                        {
                            if (next > bestLoss)
                                bestLoss = next;
                        }
                    }
                    for (int pos = 0; pos < j; pos++)
                    {
                        int next = movesToWin[Math.Min(pos, i), pos, k] + 1;
                        if ((next) % 2 == 0)
                        {
                            if (next < bestWin)
                                bestWin = next;
                        }
                        else
                        {
                            if (next > bestLoss)
                                bestLoss = next;
                        }
                    }
                    for (int pos = 0; pos < k; pos++)
                    {
                        int next = movesToWin[Math.Min(pos, i), Math.Min(pos, j), pos] + 1;
                        if ((next) % 2 == 0)
                        {
                            if (next < bestWin)
                                bestWin = next;
                        }
                        else
                        {
                            if (next > bestLoss)
                                bestLoss = next;
                        }
                    }
                    if (bestWin != int.MaxValue)
                    {
                        movesToWin[i, j, k] = bestWin;
                    }
                    else
                    {
                        Console.Out.WriteLine("{0}, {1}, {2}= {3}", i, j, k, bestLoss);
                        movesToWin[i, j, k] = bestLoss;
                    }
                }
            }
        }
        return movesToWin[N, N, N] % 2 == 0 ? movesToWin[N,N,N] : -movesToWin[N,N,N];
    }
}

namespace Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            Chomp c = new Chomp();
                object o = c.moves(100);
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
