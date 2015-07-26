using System;
using System.Collections;
using System.Text;

public class SubstitutionCode
{
    public int getValue(string key, string code)
    {
        char[] ans = code.ToCharArray();
        int res = 0;
        for (int i = 0; i < ans.Length; i++)
        {
            int a = key.IndexOf(ans[i]);
            if (a < 0)
                continue;
            res *= 10;
            if (a == 9)
            {
                a = -1;
            }
            res += a+1;
        }
        return res;
    }
}

namespace SRM257D2Q1
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
