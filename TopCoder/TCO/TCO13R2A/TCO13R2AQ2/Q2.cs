using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class TheMagicMatrix
{
    public int find(int n, int[] rows, int[] columns, int[] values)
    {
        if (n > rows.Length)
            return Simple(n, rows.Length);
        int total = 0;
        for (int sum = 0; sum <= 9; sum++)
        {
            int[,] board = new int[n,n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    board[i, j] = -1;
            bool exit = false;
            for (int i = 0; i < rows.Length; i++)
            {
                if (!AddCell(rows[i], columns[i], values[i], board, sum, n))
                {
                    exit = true;
                    break;
                }
            }
            if (exit)
                continue;

            int localResult = 1;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (board[i, j] >= 0)
                        continue;
                    localResult = Mul(localResult, 10);
                    if (!AddCell(i, j, 0, board, sum, n))
                    {
                        exit = true;
                        break;
                    }
                }
                if (exit)
                    break;
            }
            if (exit)
                continue;
            total = Add(total, localResult);
        }
        return total;
    }

    private class Triple
    {
        public Triple(int x, int y, int v)
        {
            X = x;
            Y = y;
            V = v;
        }

        public int X;
        public int Y;
        public int V;
    }

    private bool AddCell(int p1, int p2, int p3, int[,] board, int total, int n)
    {
        Queue<Triple> toAdd = new Queue<Triple>();
        toAdd.Enqueue(new Triple(p1, p2, p3));
        while (toAdd.Count > 0)
        {
            Triple cur = toAdd.Dequeue();
            int prev = board[cur.X, cur.Y];
            if (prev != -1 && prev != cur.V)
                return false;
            board[cur.X, cur.Y] = cur.V;

            int cnt = 0;
            int sum = 0;
            int missing = -1;
            for (int j = 0; j < n; j++)
            {
                if (board[cur.X, j] >= 0)
                {
                    cnt++;
                    sum += board[cur.X, j];
                }
                else
                {
                    missing = j;
                }
            }
            if (cnt == n && sum % 10 != total)
                return false;
            if (cnt == n - 1)
            {
                toAdd.Enqueue(new Triple(cur.X, missing, ((total - sum) %10 + 10) %10));
            }

            cnt = 0;
            sum = 0;
            missing = -1;
            for (int j = 0; j < n; j++)
            {
                if (board[j, cur.Y] >= 0)
                {
                    cnt++;
                    sum += board[j, cur.Y];
                }
                else
                {
                    missing = j;
                }
            }
            if (cnt == n && sum % 10 != total)
                return false;
            if (cnt == n - 1)
            {
                toAdd.Enqueue(new Triple(missing, cur.Y, ((total - sum) % 10 + 10) % 10));
            }
        }
        return true;
    }

    private const int MOD = 1234567891;

    private int Add(int a, int b)
    {
        long res = (long) a + b;
        return (int) (res%MOD);
    }

    private int Simple(int n, int p)
    {
        return Mul(10, Pow(10, (n - 1)*(n - 1) - p));
    }

    private int Mul(int i, int j)
    {
        long res = (long) i*j;
        return (int)(res%MOD);
    }

    private int Pow(int b, int pow)
    {
        // TODO: do fast pow using bits if needed.
        int start = 1;
        for (int i = 0; i < pow; i++)
            start = Mul(start, b);
        return start;
    }
}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            TheMagicMatrix c = new TheMagicMatrix();
            object o = c.find(0, new int[0], new int[0], new int[0]);
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
