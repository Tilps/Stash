using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class PasswordXGrid
{
    public int minSum(string[] horizontal, string[] vertical)
    {
        int N = horizontal.Length - 1;
        int M = vertical[0].Length - 1;
        int[,] dist = new int[horizontal.Length, vertical[0].Length];
        for (int i = 0; i <= N; i++)
        {
            for (int j = 0; j <= M; j++)
            {
                dist[i, j] = 0;
                if (i > 0)
                {
                    dist[i,j] = dist[i - 1, j] + (vertical[i - 1][j] - '0');
                }
                if (j > 0)
                {
                    dist[i, j] = Math.Max(dist[i, j], dist[i, j - 1] + (horizontal[i][j - 1] - '0'));
                }
            }
        }
        return dist[N, M];
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            PasswordXGrid c = new PasswordXGrid();
            object o = c.minSum(new string[] { }, new string[] { });
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
