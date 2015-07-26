using System;
using System.Collections;
using System.Text;

public class Decipher
{
    public string[] decipher(string[] encoded, string freqOrder)
    {
        string[] res = new string[encoded.Length];
        for (int i = 0; i < encoded.Length; i++)
        {
            res[i] = encoded[i];
        }
        int[] counts = new int[27];
        for (int i = 0; i < encoded.Length; i++)
        {
            for (int j = 0; j < encoded[i].Length; j++)
            {
                if (encoded[i][j] != ' ')
                    counts[encoded[i][j] - 'A']++;
            }
        }
        int maxCount = int.MinValue;
        for (int i = 0; i < counts.Length; i++)
        {
            if (counts[i] > maxCount)
            {
                maxCount = counts[i];
            }
        }
        int index = 0;
        while (maxCount > 0)
        {
            for (int i = 0; i < counts.Length; i++)
            {
                if (counts[i] == maxCount)
                {
                    for (int j = 0; j < encoded.Length; j++)
                    {
                        for (int k = 0; k < encoded[j].Length; k++)
                        {
                            if (encoded[j][k] == (char)('A' + i))
                            {
                                char[] lets = res[j].ToCharArray();
                                lets[k] = freqOrder[index];
                                res[j] = new string(lets);
                            }
                        }
                    }
                    index++;
                }
            }
            maxCount--;
        }
        return res;
    }
}

namespace SRM253Q1
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
