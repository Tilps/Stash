using System;
using System.Collections;
using System.Text;

public class NumericalSequence
{
    public int makePalindrome(int[] seq)
    {
        ArrayList pal = new ArrayList();
        for (int i = 0; i < seq.Length; i++)
        {
            pal.Add(seq[i]);
        }
        int left = 0;
        int right = pal.Count - 1;
        int moves = 0;
        while (left < right)
        {
            if ((int)pal[left] < (int)pal[right])
            {
                pal[left] = (int)pal[left] + (int)pal[left + 1];
                pal.RemoveAt(left + 1);
                right--;
                moves++;
            }
            else if ((int)pal[left] > (int)pal[right])
            {
                pal[right] = (int)pal[right] + (int)pal[right - 1];
                pal.RemoveAt(right - 1);
                right--;
                moves++;
            }
            else
            {
                left++;
                right--;
            }
        }
        return moves;
    }
}

namespace SRM259Q1
{
    class Program
    {
        static void Main(string[] args)
        {
            NumericalSequence c = new NumericalSequence();
            object o = c.makePalindrome(new int[] { 3, 23, 21, 23, 42, 39, 63, 76, 13, 13, 13, 32, 12, 42, 26 });
            if (o is IEnumerable)
            {
                foreach (object b in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(b);
                }
            }
            else
                System.Console.Out.WriteLine(o);
            System.Console.In.ReadLine();
        }
    }
}
