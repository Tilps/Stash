using System;
using System.Collections;
using System.Text;

public class GridGenerator
{
    public int generate(int[] row, int[] col)
    {
        int[,] res = new int[row.Length, col.Length];
        for (int i = 0; i < row.Length; i++)
        {
            res[0, i] = col[i];
            res[i, 0] = row[i];
        }
        for (int i = 1; i < row.Length; i++)
        {
            for (int j = 1; j < row.Length; j++)
            {
                res[i, j] = res[i - 1, j] + res[i, j - 1] + res[i - 1, j - 1];
            }
        }
        return res[row.Length - 1, row.Length - 1];
    }
}

namespace SRM256D2Q1
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
