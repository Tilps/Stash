using System;
using System.Collections;
using System.Text;

public class ListeningIn
{
    public string[,] memo;
    public string t;
    int tLength;
    public string p;
    int pLength;
    int counter = 0;

    public string go(int curT, int curP)
    {
        if (memo[curT, curP] == null)
        {
            counter++;
            if (curT == tLength && curP == pLength)
            {
                memo[curT, curP] = string.Empty;
            }
            else if (curT == tLength)
            {
                memo[curT, curP] = p.Substring(curP);
            }
            else if (curP == pLength)
            {
                memo[curT, curP] = "UNMATCHED";
            }
            else
            {
                memo[curT, curP] = "UNMATCHED";
                if (t[curT] == p[curP])
                {
                    memo[curT, curP] = go(curT + 1, curP + 1);
                }
                if (memo[curT, curP] == "UNMATCHED")
                {
                    string temp = go(curT, curP+1);
                    if (temp != "UNMATCHED")
                        memo[curT, curP] = "" + p[curP] + temp;
                }

            }
        }
        return memo[curT, curP];
    }

    public string probableMatch(string typed, string phrase)
    {
        memo = new string[typed.Length+1, phrase.Length+1];
        for (int i = 0; i < typed.Length + 1; i++)
        {
            for (int j = 0; j < phrase.Length + 1; j++)
            {
                memo[i, j] = null;
            }
        }
        t = typed;
        tLength = t.Length;
        p = phrase;
        pLength = p.Length;
        string res = go(0,0);

        if (res != null)
            return res;
        else
            return "UNMATCHED";
    }
}

namespace SRM254Q1
{
    class Program
    {
        static void Main(string[] args)
        {
            ListeningIn c = new ListeningIn();
            System.Console.Out.WriteLine(c.probableMatch("bbbbbbbbbbbbbbbbbbbbbbbbbb", "ababababababababababababababababababababababababab"));
            System.Console.In.ReadLine();
        }
    }
}
