using System;
using System.Collections;
using System.Text;

public class BridgePts
{
    public int pointValue(int[] hand)
    {
        int[] suitCount = new int[4];
        int[] typeCount = new int[13];
        for (int i = 0; i < hand.Length; i++)
        {
            suitCount[(hand[i]-1) / 13]++;
            typeCount[(hand[i]-1) % 13]++;
        }
        int total = 0;
        for (int i = 0; i < suitCount.Length; i++)
        {
            if (suitCount[i] == 0)
            {
                total += 3;   
            }
            if (suitCount[i] == 1)
                total += 2;
            if (suitCount[i] == 2)
                total += 1;
        }
        total += typeCount[0]*4;
        total += typeCount[12]*3;
        total += typeCount[11]*2;
        total += typeCount[10];

        return total;
    }
}

namespace SRM257D2Q2
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
