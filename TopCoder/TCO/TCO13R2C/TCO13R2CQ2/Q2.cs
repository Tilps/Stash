using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class TheMountain
{
    public int minSum(int n, int m, int[] rowIndex, int[] columnIndex, int[] element)
    {
        int[][] array = new int[n][];
        for (int i = 0; i < n; i++)
        {
            array[i] = new int[m];
            for (int j = 0; j < m; j++)
                array[i][j] = int.MinValue;
        }
        for (int i = 0; i < rowIndex.Length; i++)
        {
            if (element[i] < rowIndex[i] + columnIndex[i] + 1)
                return -1;
            if (element[i] < (n-1-rowIndex[i]) + columnIndex[i] + 1)
                return -1;
            if (element[i] < rowIndex[i] + (m-1-columnIndex[i]) + 1)
                return -1;
            if (element[i] < (n-1-rowIndex[i]) + (m-1-columnIndex[i]) + 1)
                return -1;
            array[rowIndex[i]][columnIndex[i]] = element[i];
        }
        int[] rowPosStarts = new int[n];
        int[] rowPosEnds = new int[n];
        for (int i = 0; i < n; i++)
        {
            int max = 0;
            int last = 0;
            int index = -1;
            int maxIndex = -1;
            bool dec = false;
            for (int j = 0; j < m; j++)
            {
                if (array[i][j] > int.MinValue)
                {
                    if (!dec)
                    {
                        if (array[i][j] > max)
                        {
                            max = array[i][j];
                            maxIndex = j;
                        }
                        else
                        {
                            dec = true;
                        }
                    }
                    //if ()
                    last = array[i][j];
                    index = j;

                }
            }
        }
        int[] colPosStarts = new int[m];
        int[] colPosEnds = new int[m];

        return 0;
    }
}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            TheMountain c = new TheMountain();
            object o = c.minSum(10,10, new int[] {},new int[] {}, new int[] {});
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
