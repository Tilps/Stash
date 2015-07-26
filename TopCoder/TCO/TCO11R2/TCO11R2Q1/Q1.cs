using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class GuessTheNumberGame
{
    public int possibleClues(int N)
    {
        bool[] comps = new bool[N + 1];
        for (int i = 2; i * i <= N; i++)
        {
            if (!comps[i])
            {
                for (int j = i * i; j <= N; j += i)
                    comps[j] = true;
            }
        }
        long mod = 1000000007;
        long result = 1; 
        for (int i = 2; i <= N; i++)
        {
            if (!comps[i])
            {
                int powCount = 0;
                long cur = i;
                while (cur <= (long)N)
                {
                    cur *= i;
                    powCount++;
                }
                result *= powCount + 1;
                result %= mod;
                // YYY, YYN, YNN, NNN
            }
        }

        return (int)result;
    }
    public int possibleCluesWrong(int N)
    {
        bool[] comps = new bool[N + 1];
        for (int i = 2; i * i <= N; i++)
        {
            if (!comps[i])
            {
                for (int j = i * i; j <= N; j += i)
                    comps[j] = true;
            }
        }
        int mod = 1000000007;
        int result = 1;
        for (int i = 2; i <= N; i++)
        {
            if (!comps[i])
            {
                int powCount = 0;
                long cur = i;
                while (cur <= (long)N)
                {
                    cur *= i;
                    powCount++;
                }
                result = result * (powCount + 1);
                result %= mod;
                // YYY, YYN, YNN, NNN
            }
        }

        return (int)result;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            GuessTheNumberGame c = new GuessTheNumberGame();
            for (int i = 1000000; i >= 1; i--)
            {
                if (c.possibleClues(i) != c.possibleCluesWrong(i))
                    Console.Out.WriteLine(i);
            }
            object o = c.possibleClues(44);
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
