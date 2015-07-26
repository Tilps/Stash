using System;
using System.Collections;
using System.Text;

public class PluCodeGenerator
{
    private int bad(int n)
    {
        if (n % 1000 == 0)
            return 1;
        int p = -1, pp = -2;
        while (n > 0)
        {
            if (p == n % 10 && pp == p)
                return 1;
            pp = p;
            p = n % 10;
            n /= 10;
        }
        return 0;
    }

    public int countInvalidCodes(int N)
    {
        int cnt = 0;
        if (N < 1001)
        {
            for (int i = 1; i < N; i++)
            {
                cnt += bad(i);
            }
            return cnt;
        }
        for (int i = 1; i < 1000; i++)
        {
            cnt += bad(i);
        }
        for (int i = 1000; i + 1000 <= N; i += 1000)
        {
            if (bad(i / 1000) == 1) cnt += 1000;
            else if (i / 1000 % 10 == i / 10000 % 10) cnt += 109;
            else cnt += 19;
        }
        for (int i = N / 1000 * 1000; i < N; i++) cnt += bad(i);
        return cnt;

    }
}

namespace SRM255Q2
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
