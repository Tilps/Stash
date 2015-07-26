using System;
using System.Collections;
using System.Text;

public class WordCompositionGame
{
    public string score(string[] a, string[] b, string[] c)
    {
        Hashtable freq = new Hashtable();
        for (int i = 0; i < a.Length; i++)
        {
            if (!freq.Contains(a[i]))
            {
                freq[a[i]] = 1;
            }
            else
            {
                int prev = (int)freq[a[i]];
                freq[a[i]] = prev + 1;
            }
        }
        for (int i = 0; i < b.Length; i++)
        {
            if (!freq.Contains(b[i]))
            {
                freq[b[i]] = 1;
            }
            else
            {
                int prev = (int)freq[b[i]];
                freq[b[i]] = prev + 1;
            }
        }
        for (int i = 0; i < c.Length; i++)
        {
            if (!freq.Contains(c[i]))
            {
                freq[c[i]] = 1;
            }
            else
            {
                int prev = (int)freq[c[i]];
                freq[c[i]] = prev + 1;
            }
        }
        string res = "";
        int score = 0;
        for (int i = 0; i < a.Length; i++)
        {
            score += 4 - (int)freq[a[i]];
        }
        res += score.ToString();
        res += "/";
        score = 0;
        for (int i = 0; i < b.Length; i++)
        {
            score += 4 - (int)freq[b[i]];
        }
        res += score.ToString();
        res += "/";
        score = 0;
        for (int i = 0; i < c.Length; i++)
        {
            score += 4 - (int)freq[c[i]];
        }
        res += score.ToString();
        return res;
    }
}


namespace SRM255Q1
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
