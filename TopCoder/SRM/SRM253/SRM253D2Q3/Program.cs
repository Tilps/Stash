using System;
using System.Collections;
using System.Text;

public class ABCPath
{
    public int length(string[] grid)
    {
        int[, ,] lengths = new int[28, grid.Length, grid[0].Length];
        for (int i = 0; i <= 'Z' - 'A'; i++)
        {
            int cur = 'Z' - i - 'A';
            for (int j = 0; j < grid.Length; j++)
            {
                for (int k = 0; k < grid[0].Length; k++)
                {
                    if (grid[j][k] == (char)('A' + cur))
                    {
                        int max = 0;
                        if (k > 0)
                        {
                            if (lengths[cur + 1, j, k - 1] > max)
                            {
                                max = lengths[cur + 1, j, k - 1];
                            }
                            if (j > 0)
                            {
                                if (lengths[cur + 1, j - 1, k - 1] > max)
                                {
                                    max = lengths[cur + 1, j - 1, k - 1];
                                }
                            }
                            if (j < grid.Length - 1)
                            {
                                if (lengths[cur + 1, j + 1, k - 1] > max)
                                {
                                    max = lengths[cur + 1, j + 1, k - 1];
                                }
                            }
                        }
                        if (j > 0)
                        {
                            if (lengths[cur + 1, j-1, k] > max)
                            {
                                max = lengths[cur + 1, j-1, k];
                            }
                            if (k < grid[0].Length -1)
                            {
                                if (lengths[cur + 1, j - 1, k + 1] > max)
                                {
                                    max = lengths[cur + 1, j - 1, k + 1];
                                }
                            }
                        }
                        if (k < grid[0].Length - 1)
                        {
                            if (lengths[cur + 1, j , k+1] > max)
                            {
                                max = lengths[cur + 1, j , k+1];
                            }
                            if (j < grid.Length - 1)
                            {
                                if (lengths[cur + 1, j + 1, k + 1] > max)
                                {
                                    max = lengths[cur + 1, j + 1, k + 1];
                                }
                            }
                        }
                        if (j < grid.Length - 1)
                        {
                            if (lengths[cur + 1, j + 1, k] > max)
                            {
                                max = lengths[cur + 1, j + 1, k];
                            }
                        }
                        lengths[cur, j, k] = max + 1;
                    }
                }
            }
        }
        int inmax = int.MinValue;
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[0].Length; j++)
            {
                if (lengths[0, i, j] > inmax)
                    inmax = lengths[0, i, j];
            }
        }
        return inmax;
    }
}

namespace SRM253D2Q3
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
