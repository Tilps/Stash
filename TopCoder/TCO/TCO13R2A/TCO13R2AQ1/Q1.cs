using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class TheLargestString
{
    public string find(string s, string t)
    {
        StringBuilder first = new StringBuilder();
        StringBuilder second = new StringBuilder();
        first.Append(s[0]);
        second.Append(t[0]);
        for (int i = 1; i < s.Length; i++)
        {
            int removeCount = 0;
            int j = first.Length - 1;
            while (j >= 0 && first[j] < s[i])
            {
                j--;
                removeCount++;
            }
            if (removeCount > 0)
            {
                first.Remove(first.Length - removeCount, removeCount);
                second.Remove(second.Length - removeCount, removeCount);
            }
            first.Append(s[i]);
            second.Append(t[i]);
        }
        string newS = first.ToString();
        string newT = second.ToString();
        string best = first.ToString() + second.ToString();

        string[,] dp = new string[newS.Length+1,newS.Length+1];
        dp[0, 0] = string.Empty;
        for (int i = 1; i <= newS.Length; i++)
        {
            dp[i, 0] = string.Empty;
            for (int j = 1; j <= newS.Length; j++)
            {
                if (dp[i - 1, j-1] == null)
                    continue;
                string prevS = dp[i - 1, j - 1].Substring(0, j - 1);
                string prevT = dp[i - 1, j - 1].Substring(j-1, j - 1);
                string nextS = newS[newS.Length - i] + prevS;
                string nextT = newT[newT.Length - i] + prevT;
                string nextTrial = nextS + nextT;
                if (dp[i - 1, j] == null || string.CompareOrdinal(nextTrial, dp[i - 1, j]) > 0)
                    dp[i, j] = nextTrial;
                else
                    dp[i, j] = dp[i - 1, j];
            }
        }
        for (int i = 1; i < newS.Length; i++)
        {
            if (string.CompareOrdinal(dp[newS.Length, i], best) > 0)
                best = dp[newS.Length, i];
        }

        //for (int i = 0; i < newS.Length; i++)
        //{
        //    char current = newS[i];
        //    for (char target = 'a'; target <= 'z'; target++)
        //    {
        //        StringBuilder editS = new StringBuilder(newS);
        //        StringBuilder editT = new StringBuilder(newT);
        //        int pos = i + 1;
        //        int index = IndexOf(editS, pos);
        //        while (index >= 0)
        //        {
        //            if (CharAt(editS, editT, pos) < target && editS[index] <= current)
        //            {
        //                editS.Remove(index, 1);
        //                editT.Remove(index, 1);
        //                string newTrial = editS.ToString() + editT.ToString();
        //                if (string.CompareOrdinal(newTrial, best) > 0)
        //                    best = newTrial;
        //                if (index < pos)
        //                    pos--;
        //            }
        //            else
        //            {
        //                pos++;
        //            }
        //            index = IndexOf(editS, pos);
        //        }
        //    }

        //}

        //bool improving = true;
        //while (improving)
        //{
        //    improving = false;
        //    for (int i = newS.Length-1; i >=0; i--)
        //    {
        //        for (int j = 1; i + j <= newS.Length; j++)
        //        {
        //            string testS = newS.Substring(0, i) + newS.Substring(i + j, newS.Length - i - j);
        //            string testT = newT.Substring(0, i) + newT.Substring(i + j, newT.Length - i - j);
        //            string newTrial = testS + testT;
        //            if (string.CompareOrdinal(newTrial, best) > 0)
        //            {
        //                best = newTrial;
        //                newS = testS;
        //                newT = testT;
        //                improving = true;
        //                break;
        //            }
        //        }
        //        if (improving)
        //            break;
        //    }
        //}

        return best;
    }

    private char CharAt(StringBuilder editS, StringBuilder editT, int pos)
    {
        if (pos < editS.Length)
            return editS[pos];
        else
        {
            return editT[pos - editS.Length];
        }
    }

    private int IndexOf(StringBuilder editS, int pos)
    {
        if (pos < editS.Length)
            return pos;
        pos -= editS.Length;
        if (pos < editS.Length)
            return pos;
        return -1;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            TheLargestString c = new TheLargestString();
            object o = c.find("hhhhhhhh", "aahahhhh");
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
