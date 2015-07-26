using System;
using System.Collections;
using System.Text;


public class AlphabetCount
{
    public int count(string[] grid, int length)
    {
        int[, ,] counts = new int[length, grid.Length, grid[0].Length];
        for (int i = 0; i < length; i++)
        {
            int cur = i;
            for (int j = 0; j < grid.Length; j++)
            {
                for (int k = 0; k < grid[0].Length; k++)
                {
                    if (grid[j][k] == (char)('A' + cur))
                    {
                        if (i == 0)
                        {
                            counts[0, j, k] = 1;
                            continue;
                        }
                        long total = 0;
                        if (k > 0)
                        {
                            total += counts[cur - 1, j, k - 1];
                            if (j > 0)
                            {
                                total += counts[cur - 1, j - 1, k - 1];
                            }
                            if (j < grid.Length - 1)
                            {
                                    total += counts[cur - 1, j + 1, k - 1];
                            }
                        }
                        if (j > 0)
                        {
                                total += counts[cur - 1, j - 1, k];
                            if (k < grid[0].Length - 1)
                            {
                                    total += counts[cur - 1, j - 1, k + 1];
                            }
                        }
                        if (k < grid[0].Length - 1)
                        {
                                total += counts[cur - 1, j, k + 1];
                            if (j < grid.Length - 1)
                            {
                                    total += counts[cur - 1, j + 1, k + 1];
                            }
                        }
                        if (j < grid.Length - 1)
                        {
                                total += counts[cur - 1, j + 1, k];
                        }
                        if (total < 1000000000)
                            counts[cur, j, k] = (int)total;
                        else
                            counts[cur, j, k] = 1000000000;
                    }
                }
            }
        }
        long intotal = 0;
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[0].Length; j++)
            {
                intotal += counts[length-1, i, j];
            }
        }
        if (intotal < 1000000000)
            return (int)intotal;
        else
            return 1000000000;
    }
}

namespace SRM253Q2
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
