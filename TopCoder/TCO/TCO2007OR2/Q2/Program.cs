using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class WordSplit
{
    int bestLength = int.MaxValue;
    string[] bestResult = null;
    public string[] pieces(string theString)
    {
        List<string> parts = new List<string>();
        int max = speedup(theString);
        for (int i = max; i >= 0; i--)
        {
            Search(parts, theString, 0, 0, i);
        }
        return bestResult;
    }

    private void Search(List<string> parts, string theString, int depth, int start, int end)
    {
        if (depth+1 > bestLength)
            return;
        string newPart = theString.Substring(start, end - start + 1);
        if (ok(newPart))
        {
            parts.Add(newPart);
            if (end == theString.Length - 1)
            {
                CheckBest(parts);
            }
            else
            {
                int max = speedup(theString.Substring(end + 1));
                for (int i = max+end+1; i >= end+1; i--)
                {
                    Search(parts, theString, depth + 1, end + 1, i);
                }
            }
            parts.RemoveAt(parts.Count - 1);
        }
    }

    private int speedup(string newPart)
    {
        Array.Clear(counts, 0, counts.Length);
        for (int i = 0; i < newPart.Length; i++)
        {
            int index = newPart[i] - 'a';
            if (counts[index] > 0)
                return i-1;
            counts[index]++;
        }
        return newPart.Length-1;
    }

    private void CheckBest(List<string> parts)
    {
        string[] bits = parts.ToArray();
        Array.Sort(bits);
        if (bits.Length < bestLength)
        {
            bestLength = bits.Length;
            bestResult = bits;
        }
        else if (bits.Length == bestLength)
        {
            for (int i = 0; i < bestLength; i++)
            {
                if (bits[i].CompareTo(bestResult[i]) > 0)
                    break;
                if (bits[i].CompareTo(bestResult[i]) < 0)
                {
                    bestResult = bits;
                    return;
                }
            }
        }
        else
            throw new Exception("umm?");

    }

    int[] counts = new int[27];
    private bool ok(string newPart)
    {
        Array.Clear(counts, 0, counts.Length);
        for (int i = 0; i < newPart.Length; i++)
        {
            int index =newPart[i]-'a';
            if (counts[index] > 0)
                return false;
            counts[index]++;
        }
        return true;
    }
}

namespace Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            WordSplit c = new WordSplit();
            string word = "cabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabca";
            object o = c.pieces(word);
            PrintObj(o, 0);
            System.Console.In.ReadLine();
        }

        private static void PrintObj(object o, int depth)
        {
            System.Console.Out.Write(new string(' ', depth));
            if (o is string)
            {
                System.Console.Out.WriteLine(o);
            }
            else if (o is IEnumerable)
            {
                System.Console.Out.WriteLine("List:");
                foreach (object b in (IEnumerable)o)
                {
                    PrintObj(b, depth + 1);
                }
            }
            else
                System.Console.Out.WriteLine(o);
        }
    }
}
