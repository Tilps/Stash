using System;
using System.Collections;
using System.Text;

public class PrettyPrintASpiral
{
    public string[] getWindow(int row1, int col1, int row2, int col2)
    {
        int[,] output = new int[col2 - col1 + 1, row2 - row1 + 1];
        int curX = 0;
        int curY = 0;
        int curV = 1;
        int nextSq = 1;
        for (int i = 0; i < 10001 * 10001 + 1; i++)
        {
            if (curX >= col1 && curX <= col2 && curY >= row1 && curY <= row2)
            {
                output[curX - col1, curY - row1] = curV;
            }
            if (curV == nextSq * nextSq)
            {
                curX++;
                nextSq += 2;
            }
            else if (curV < nextSq * nextSq - 3 * nextSq + 3)
            {
                curY--;
            }
            else if (curV < nextSq * nextSq - 2 * nextSq + 2)
            {
                curX--;
            }
            else if (curV < nextSq * nextSq - nextSq+1)
            {
                curY++;
            }
            else
            {
                curX++;
            }
            curV++;
        }
        int maxSize = 1;
        for (int x = col1; x <= col2; x++)
        {
            for (int y = row1; y <= row2; y++)
            {
                maxSize = Math.Max(maxSize, Size(output[x - col1, y - row1]));
            }
        }
        string[] result = new string[row2 - row1 + 1];
        for (int y = row1; y <= row2; y++)
        {
            StringBuilder res = new StringBuilder();
            for (int x = col1; x <= col2; x++)
            {
                if (x > col1)
                    res.Append(' ');
                res.Append(Pad(output[x - col1, y - row1], maxSize));
            }
            result[y - row1] = res.ToString();
        }

        return result;
    }

    private string Pad(int p, int maxSize)
    {
        string start = p.ToString();
        if (start.Length < maxSize)
        {
            start = new string(' ', maxSize - start.Length) + start;
        }
        return start;
    }

    private int Size(int p)
    {
        int size = 0;
        while (p > 0)
        {
            size++;
            p = p / 10;
        }
        return size;
    }
}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            PrettyPrintASpiral c = new PrettyPrintASpiral();
            object o = c.getWindow(5000,5000,5000,5000);
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
