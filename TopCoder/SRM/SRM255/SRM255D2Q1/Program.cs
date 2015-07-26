using System;
using System.Collections;
using System.Text;

public class SequenceOfNumbers
{
    public string[] rearrange(string[] seq)
    {
        int[] a = new int[seq.Length];
        for (int i = 0; i < seq.Length; i++)
        {
            a[i] = int.Parse(seq[i]);
        }
        Array.Sort(a);
        string[] res = new string[seq.Length];
        for (int i = 0; i < seq.Length; i++)
        {
            res[i] = a[i].ToString();
        }
        return res;
    }
}

namespace SRM255D2Q1
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
