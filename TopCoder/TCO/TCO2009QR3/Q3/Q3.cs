using System;
using System.Collections;
using System.Text;

public class MismatchedStrings
{
    public string getString(int N, long K)
    {
        long[,] missMatchCounts = new long[N+1, N];
        missMatchCounts[0, 0] = 1;
        missMatchCounts[1, 0] = 1;
        for (int i = 2; i <= N; i++)
        {
            for (int j = 0; j < i; j++)
            {
                long total = 0;
                if (j > 0)
                    total += missMatchCounts[i - 1, j - 1];
                else
                    total += missMatchCounts[i - 1, 0];

                missMatchCounts[i, j] = total;
            }
        }
        // getString(N, K, 0) = K > missmatches of length N starting with ( ? ")"+getString(N-1, K-mi
        return "";
    }
}

namespace Q3
{
    class Q3
    {
        static void Main(string[] args)
        {
            MismatchedStrings c = new MismatchedStrings();
            object o = c.getString(1,1);
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
