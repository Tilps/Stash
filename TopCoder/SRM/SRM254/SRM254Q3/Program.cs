using System;
using System.Collections;
using System.Text;

public class SelfCatalogue
{

    private int[] go(int[] counts, bool[] fix, int depth, int max)
    {
        if (max < 0)
            return null;
        if (depth == counts.Length)
        {
            int[] catacata = new int[counts.Length];
            for (int i = 0; i < counts.Length; i++)
            {
                if (counts[i] > 0)
                {
                    catacata[i]++;
                    if (counts[i] < 10)
                        catacata[counts[i]]++;
                    else
                    {
                        catacata[counts[i] % 10]++;
                        catacata[counts[i] / 10]++;
                    }
                }
            }
            for (int i = 0; i < counts.Length; i++)
            {
                if (counts[i] != catacata[i])
                    return null;
            }
            // verify
            return catacata;
        }
        else
        {
            if (fix[depth])
                return go(counts, fix, depth + 1, max-counts[depth]);
            else
            {
                for (int i = 0; i <= max; i++)
                {
                    counts[depth] = i;
                    int[] res = go(counts, fix, depth+1, max-i);
                    if (res != null)
                        return res;
                }
                counts[depth] = -1;
                return null;
            }
        }
    }

    public int[] construct(int[] counts)
    {
        bool[] fix = new bool[counts.Length];
        for (int i = 0; i < counts.Length; i++)
        {
            if (counts[i] > -1)
                fix[i] = true;
            if (counts[i] > 9 && counts[i] != 11)
                return new int[0];
        }
        int[] res = go(counts, fix, 0, 22);
        if (res == null)
            return new int[0];
        else
            return res;
    }
}

namespace SRM254Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            SelfCatalogue c = new SelfCatalogue();
            string res = string.Empty;
            int[] ans = c.construct(new int[] { 1, -1, -1, -1, -1, -1, -1, -1, -1, -1 });
            for (int i = 0; i < ans.Length; i++)
            {
                res += ans[i];
                res += ", ";
            }
            System.Console.Out.WriteLine(res);
            System.Console.In.ReadLine();
        }
    }
}
