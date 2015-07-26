using System;
using System.Collections.Generic;

using System.Text;

public class StringsAndTabs
{
    public string[] transformTab(string[] tab, int[] stringsA, int[] stringsB, int d)
    {
        int[] bIndexes = new int[stringsB.Length];
        for (int i = 0; i < stringsB.Length; i++)
        {
            bIndexes[i] = i;
        }
        Array.Sort(stringsB, bIndexes);
        Array.Reverse(stringsB);
        Array.Reverse(bIndexes);
        List<int>[] notes = new List<int>[tab[0].Length];
        for (int j = 0; j < tab[0].Length; j++)
        {
            notes[j] = new List<int>();
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i][j] == '-')
                    continue;
                int fret = 0;
                if (tab[i][j] >= '0' && tab[i][j] <= '9')
                    fret = tab[i][j] - '0';
                else
                    fret = tab[i][j] - 'A' + 10;
                notes[j].Add(stringsA[i] + fret + d);
            }
        }
        int[][] output = new int[tab[0].Length][];
        for (int i = 0; i < tab[0].Length; i++)
        {
            output[i] = new int[stringsB.Length];
            for (int j = 0; j < output[i].Length; j++)
            {
                output[i][j] = -1;
            }
            List<int> curNotes = notes[i];
            curNotes.Sort();
            curNotes.Reverse();
            bool fail = false;
            for (int j = 0; j < curNotes.Count; j++)
            {
                int bestPitch = int.MinValue;
                int bestString = int.MinValue;
                int bestK = -1;
                for (int k = 0; k < stringsB.Length; k++)
                {
                    if (output[i][k] >= 0)
                        continue;
                    if (stringsB[k] <= curNotes[j] && stringsB[k] + 35 >= curNotes[j])
                    {
                        if (stringsB[k] >= bestPitch)
                        {
                            bestPitch = stringsB[k];
                        }
                        else
                        {
                            break;
                        }
                        if (bIndexes[k] > bestString)
                        {
                            bestString = bIndexes[k];
                            bestK = k;
                        }
                    }
                }
                if (bestString != int.MinValue)
                {
                    output[i][bestString] = curNotes[j] - stringsB[bestK];
                }
                else
                {
                    fail = true;
                    break;
                }
            }
            if (fail)
            {
                for (int j = 0; j < output[i].Length; j++)
                {
                    output[i][j] = -2;
                }
            }
        }
        char[][] realOutput = new char[stringsB.Length][];
        for (int i = 0; i < stringsB.Length; i++)
        {
            realOutput[i] = new char[tab[0].Length];
            for (int j = 0; j < tab[0].Length; j++)
            {
                realOutput[i][j] = Map(output[j][i]);
            }
        }
        string[] finalOutput = new string[stringsB.Length];
        for (int i = 0; i < stringsB.Length; i++)
        {
            finalOutput[i] = new string(realOutput[i]);
        }
        return finalOutput;
    }

    private char Map(int p)
    {
        if (p == -2)
            return 'x';
        if (p == -1)
            return '-';
        if (p < 10)
            return (char)('0' + p);
        if (p <= 35)
            return (char)('A' + (p - 10));
        throw new ArgumentException();
    }
}

namespace SRM412Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            StringsAndTabs instance = new StringsAndTabs();
            foreach (string s in instance.transformTab(new string[] { }, new int[] { }, new int[] { }, 1))
                Console.Out.WriteLine(s);
            Console.ReadKey();
        }
    }
}
