using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class KindAndCruel
{
    public int crossTheField(string field, int K, int C)
    {
        int[,,] best = new int[field.Length, K, C];
        for (int i = 0; i < field.Length; i++)
        {
            for (int j = 0; j < K; j++)
                for (int k = 0; k < C; k++)
                    best[i, j, k] = int.MaxValue;
        }
        best[0, 0, 0] = 1;
        Queue<pos> queue = new Queue<pos>();
        queue.Enqueue(new pos(0, 0));
        while (queue.Count > 0)
        {
            pos cur = queue.Dequeue();
            int nextTmodK = (cur.T + 1) % K;
            int nextTmodC = (cur.T + 1) % C;
            int nextT = cur.T + 1;
            // 3 options for nextX
            int checkSpot = cur.X;
            TryGo(field, best, queue, nextTmodK, nextTmodC, nextT, checkSpot);
            if (cur.X > 0)
            {
                TryGo(field, best, queue, nextTmodK, nextTmodC, nextT, cur.X - 1);
            }
            if (cur.X < field.Length - 1)
            {
                TryGo(field, best, queue, nextTmodK, nextTmodC, nextT, cur.X + 1);
            }
        }
        int result = int.MaxValue;
        for (int j = 0; j < K; j++)
            for (int k = 0; k < C; k++)
            {
                int check = best[field.Length - 1, j, k];
                if (check < result)
                    result = check;
            }
        return result == int.MaxValue ? -1 : (result);
    }


    private static void TryGo(string field, int[, ,] best, Queue<pos> queue, int nextTmodK, int nextTmodC, int nextT, int checkSpot)
    {
        if (field[checkSpot] == '.' || (field[checkSpot] == 'K' && nextTmodK != 0) || (field[checkSpot] == 'C' && nextTmodC == 0))
        {
            int toCheck = best[checkSpot, nextTmodK, nextTmodC];
            if (toCheck > nextT)
            {
                best[checkSpot, nextTmodK, nextTmodC] = nextT;
                queue.Enqueue(new pos(checkSpot, nextT));
            }
        }
    }

    class pos
    {
        public pos(int x, int t)
        {
            X = x;
            T = t;
        }
        public int X;
        public int T;
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            KindAndCruel c = new KindAndCruel();
            object o = c.crossTheField(".C.C.C.C.C.C.CKC.C.C.C.C.C.C.C.C.C.C.C.C.C.C.C.C.C", 49,47);
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
