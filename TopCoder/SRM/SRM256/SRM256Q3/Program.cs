using System;
using System.Collections;
using System.Text;

public class ImageRepeat
{
    public int largestSize(string[] a, string[] b)
    {
        int[,,,] dp =new int[a.Length, a[0].Length, b.Length, b[0].Length];
        int max = 0;
        for (int i = 0; i < dp.GetLength(0); i++)
        {
            for (int j = 0; j < dp.GetLength(1); j++)
            {
                for (int k = 0; k < dp.GetLength(2); k++)
                {
                    for (int l = 0; l < dp.GetLength(3); l++)
                    {
                        if (a[i][j] == b[k][l])
                        {
                            int min = int.MaxValue;
                            if (i > 0 && k > 0 && j > 0 && l > 0)
                            {
                                int next = dp[i - 1, j - 1, k - 1, l - 1];
                                if (next < min)
                                    min = next;
                                next = dp[i - 1, j, k - 1, l];
                                if (next < min)
                                    min = next;
                                next = dp[i, j - 1, k, l - 1];
                                if (next < min)
                                    min = next;
                            }
                            else min = 0;

                            if (min+1 > max)
                                max = min+1;
                            dp[i, j, k, l] = min+1;
                        }
                    }
                }
            }
        }
        return max;

    }
}

namespace SRM256Q3
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
