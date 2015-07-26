using System;
using System.Collections;
using System.Text;

public class BalancedGame
{
    public int result(string[] conf, int p, int q)
    {
        int res = -1;
        for (int i = 0; i < conf.Length; i++)
        {
            int wCount = 0;
            int lCount = 0;
            for (int j = 0; j < conf[i].Length; j++)
            {
                if (i == j)
                    continue;
                if (conf[i][j] == 'W')
                    wCount++;
                if (conf[i][j] == 'L')
                    lCount++;
            }
            if (wCount * 100 < (conf.Length - 1) * p)
                return i;
            if (lCount * 100 < (conf.Length - 1) * q)
                return i;
        }
        return res;
    }
}

namespace SRM254D2Q1
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
