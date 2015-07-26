using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class HeavyBooks
{
    public int[] findWeight(int[] books, int[] moves)
    {
        int count = moves[0];

        List<int> indexesW = new List<int>();
        List<int> indexesT = new List<int>();
        for (int i = 0; i < count; i++)
        {
            indexesW.Add(i);
        }
        for (int i = 1; i < moves.Length; i++)
        {
            if (i % 2 == 1)
            {
                int toMove = Math.Min(indexesW.Count, moves[i]);
                if (toMove > 0)
                {
                    for (int j = indexesW.Count - toMove; j < indexesW.Count; j++)
                    {
                        indexesT.Add(indexesW[j]);
                    }
                    indexesW.RemoveRange(indexesW.Count - toMove, toMove);
                }
            }
            else
            {
                int toMove = Math.Min(indexesT.Count, moves[i]);
                if (toMove > 0)
                {
                    for (int j = indexesT.Count - toMove; j < indexesT.Count; j++)
                    {
                        indexesW.Add(indexesT[j]);
                    }
                    indexesT.RemoveRange(indexesT.Count - toMove, toMove);
                }
            }
        }
        Array.Sort(books);
        bool[] wSide = new bool[count+1];
        for (int i=0; i < indexesW.Count; i++) {
            wSide[indexesW[i]+1] = true;
        }
        Pair[,] memo = new Pair[books.Length + 1, count + 1];
        for (int i = 1; i <= count; i++)
            memo[0, i] = new Pair() { A = 0, B = 500000000 };
        for (int i = 1; i <= books.Length; i++)
        {
            for (int j = 1; j <= count; j++)
            {
                int value = books[i - 1];
                if (wSide[j])
                {
                    memo[i, j] = Pair.Max(memo[i - 1, j-1].Add(value, false), memo[i-1, j]);
                }
                else
                {
                    memo[i, j] = Pair.Max(memo[i - 1, j - 1].Add(value, true), memo[i - 1, j]);
                }
            }
        }



        return new int[] {memo[books.Length, count].A,memo[books.Length, count].B};
    }

    private struct Pair
    {
        public int A;
        public int B;

        public Pair Add(int value, bool forB)
        {
            Pair result =  new Pair() { A = this.A, B = this.B };
            if (forB)
                result.B += value;
            else
                result.A += value;
            return result;
        }

        public static Pair Max(Pair a, Pair b)
        {
            if (a.A - a.B > b.A - b.B)
                return a;
            if (a.A - a.B < b.A - b.B)
                return b;
            if (a.A + a.B > b.A + b.B)
                return a;
            return b;
        }
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            HeavyBooks c = new HeavyBooks();
            object o = c.findWeight(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }, new int[] { 20, 19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 });
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
