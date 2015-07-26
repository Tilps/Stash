using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class PasswordXPalindrome
{
    public int minSwaps(string password)
    {
        int[] counts = new int[26];
        foreach (char c in password)
            counts[c - 'a']++;
        int singles = 0;
        for (int i = 0; i < counts.Length; i++)
            if (counts[i] == 1)
                singles++;
        if (singles > 1)
            return -1;

        char[] letters = password.ToCharArray();
        int result = 0;
        if (letters.Length % 2 == 1)
        {
            int mid = letters.Length / 2;
            if (counts[letters[mid] - 'a'] != 1)
            {
                for (int i = 0; i < letters.Length; i++)
                {
                    if (counts[letters[i] - 'a'] == 1)
                    {
                        Swap(letters, i, mid);
                        break;
                    }
                }
                result++;
            }
        }
        Dictionary<char, int> firstPos = new Dictionary<char, int>();
        for (int i = 0; i < letters.Length; i++)
        {
            int otherPos;
            if (firstPos.TryGetValue(letters[i], out otherPos))
            {
                if (i != letters.Length - otherPos - 1)
                {
                    Swap(letters, i, letters.Length - otherPos - 1);
                    result++;
                    // Now we've swapped we need to try this one again.
                    i--;
                }
            }
            else
                firstPos[letters[i]] = i;
        }

        return result;
    }

    private void Swap(char[] letters, int i, int mid)
    {
        char temp = letters[i];
        letters[i] = letters[mid];
        letters[mid] = temp;
    }

}

namespace Q3
{
    class Q3
    {
        static void Main(string[] args)
        {
            PasswordXPalindrome c = new PasswordXPalindrome();
            object o = c.minSwaps("levle");
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
