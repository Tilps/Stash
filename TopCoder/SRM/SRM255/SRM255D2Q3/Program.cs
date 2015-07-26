using System;
using System.Collections;
using System.Text;

public class KthElement
{
    int cB(int a)
    {
        int count = 0;
        for (int i = 0; i < 32; i++)
        {
            if ((a & (1 << i)) != 0)
                count++;
        }
        return count;
    }
    public int find(int A, int B, int k)
    {
        int[] ba = new int[33];
        ba[0] = 0;
        for (int i = 1; i < ba.Length; i++)
        {
            ba[i] = A * cB(ba[i - 1]) + B;
        }
        if (k < ba.Length) {
            return ba[k];
        }
        int a = 0;
        int b = 0;
        for (; a < ba.Length-1; a++)
        {
            bool rep = false;
            for (b = a+1; b < ba.Length; b++)
            {
                if (ba[a] == ba[b])
                {
                    rep = true;
                    break;
                }
            }
            if (rep)
                break;
        }

        int l = b - a;
        if (l == 0)
            return ba[a];
        int dist = k - a;
        int offset = dist % l;
        return ba[offset + a];
    }
}

namespace SRM255D2Q3
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
