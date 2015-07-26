using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class SubstringReversal
{
    public int[] solve(string s)
    {
        int bestA = 0;
        int bestB = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (bestA != bestB) break;
            for (int j = i + 1; j < s.Length; j++)
            {
                if (s[i] <= s[j]) continue;
                if (bestA == bestB)
                {
                    bestA = i;
                    bestB = j;
                }
                else
                {
                    if (s[bestB] < s[j])
                    {
                        continue;
                    } else if (s[bestB] > s[j])
                    {
                        bestB = j;
                    }
                    else
                    {
                        int check = j;
                        int check2 = bestB;
                        while (s[check] == s[check2] && check2 > bestA)
                        {
                            check--;
                            check2--;
                        }
                        if (s[check] < s[check2])
                        {
                            bestB = j;
                        }
                        else if (s[check2] < s[check])
                        {
                            continue;
                        }
                        else
                        {
                            check--;
                            check2 = bestB + 1;
                            while (s[check] == s[check2] && check > bestA)
                            {
                                check--;
                                check2++;
                            }
                            if (s[check] < s[check2])
                            {
                                bestB = j;
                            }
                            else if (s[check2] < s[check])
                            {
                                continue;
                            }
                            int potentialNewA = bestA;
                            int potentialNewB = j;
                            int potentialOldA = bestA;
                            int potentialOldB = bestB;
                            Expand(s, ref potentialNewA, ref potentialNewB);
                            Expand(s, ref potentialOldA, ref potentialOldB);
                            if (potentialNewA < potentialOldA)
                            {
                                bestB = j;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }
        }
        Expand(s, ref bestA, ref bestB);
        return new int[] { bestA, bestB };
    }

    private static void Expand(string s, ref int bestA, ref int bestB)
    {
        while (bestA > 0 && bestB < s.Length - 1 && s[bestA - 1] == s[bestB + 1])
        {
            bestA--;
            bestB++;
        }
    }
}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            SubstringReversal c = new SubstringReversal();
            Random rnd = new Random();
            char[] letters = new char[2500];
            for (int i = 0; i < 2500; i++)
            {
                letters[i] = (char)('a' + rnd.Next(26));
            }
            Array.Sort(letters);
            Array.Reverse(letters);
            object o = c.solve("aaaaaaccbbbcbbbcbbc");
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