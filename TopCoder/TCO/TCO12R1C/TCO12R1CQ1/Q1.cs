using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class PasswordXGuessing
{
    public long howMany(string[] guesses)
    {
        int length = guesses[0].Length;
        Dictionary<long, long> lookup = new Dictionary<long, long>();
        foreach (string guess in guesses)
        {
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (guess[i]-'0' == j)
                        continue;
                    long value = Eval(guess, i, j);
                    if (lookup.ContainsKey(value))
                        lookup[value] = lookup[value]+1;
                    else
                        lookup[value] = 1;
                }
            }
        }
        long total = 0;
        foreach (KeyValuePair<long, long> entry in lookup)
        {
            if (entry.Value == guesses.Length)
                total++;
        }
        return total;
    }

    private long Eval(string guess, int replace, int with)
    {
        long value = 0;
        for (int i = 0; i < guess.Length; i++)
        {
            value *= 10;
            if (i == replace)
                value += with;
            else
                value += guess[i] - '0';
        }
        return value;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            PasswordXGuessing c = new PasswordXGuessing();
            object o = c.howMany(new string[] { "500000000000000000000000000000000000000000", 
           
            });
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
