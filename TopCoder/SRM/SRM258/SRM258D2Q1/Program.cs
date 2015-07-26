using System;
using System.Collections;
using System.Text;

public class ClassScores
{
    public int[] findMode(int[] scores)
    {
        int[] freqs = new int[101];
        for (int i = 0; i < scores.Length; i++)
        {
            freqs[scores[i]]++;
        }
        int max=0;
        for (int i = 0; i < 101; i++)
        {
            if (freqs[i] > max)
                max = freqs[i];
        }
        ArrayList a = new ArrayList();
        for (int i = 0; i < 101; i++)
        {
            if (freqs[i] == max)
            {
                a.Add(i);
            }
        }
        return (int[])a.ToArray(typeof(int));
    }

}

namespace SRM258D2Q1
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
