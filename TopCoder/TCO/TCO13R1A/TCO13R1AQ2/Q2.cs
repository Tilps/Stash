using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class TheFrog
{
    public double getMinimum(int D, int[] L, int[] R)
    {
        Array.Sort(L, R);
        double min = double.MaxValue;
        for (int i = 0; i < L.Length; i++)
        {
            int offset = R[i]%(R[i] - L[i]);
            int maxJumps = (R[i] - offset)/(R[i] - L[i]);
//            int maxJumps = (int) Math.Floor((double) R[i]/(double) (R[i] - L[i]));
            for (int j = maxJumps; j > 0; j--)
            {
                if (Works(L, R, R[i], j))
                {
                    double jumpLength = (double) R[i]/(double) j;
                    if (jumpLength < min)
                        min = jumpLength;
                    break;
                }
            }
        }
        return min;
    }

    private bool Works(int[] L, int[] R, int touchPoint, int jumpsToTouchPoint)
    {
        int currentJump = 1;
        int currentDitch = 0;
        while (currentDitch < L.Length)
        {
            if (R[currentDitch]*jumpsToTouchPoint <= currentJump*touchPoint)
            {
                currentDitch++;
                continue;
            }
            if (L[currentDitch]*jumpsToTouchPoint >= currentJump*touchPoint)
            {
                // Skip current jump to one immediately before or at the current ditch.
                currentJump = L[currentDitch] * jumpsToTouchPoint/touchPoint;
                currentJump++;
                continue;
            }
            return false;
        }
        return true;
    }
}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            TheFrog c = new TheFrog();
            object o = c.getMinimum(1, new int[] {}, new int[] {} );
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
