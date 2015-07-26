using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class BlurredDartboard
{
    public int minThrows(int[] points, int P)
    {
        bool[] seen = new bool[points.Length + 1];

        int highestSeen = 0;
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] > 0)
            {
                seen[points[i]] = true;
                if (points[i] > highestSeen)
                    highestSeen = points[i];
            }
        }
        List<int> missing = new List<int>();
        for (int i = 1; i < seen.Length; i++)
        {
            if (!seen[i])
                missing.Add(i);
        }
        int bestSoFar = P;
        if (highestSeen > 0)
        {
            bestSoFar = Math.Min(bestSoFar, (P + (highestSeen - 1)) / highestSeen);
        }
        int sum = 0;
        for (int i = 0; i < missing.Count; i++)
        {
            sum += missing[i];
        }
        if (sum > 0)
        {
            int fullReps = P / sum;
            int leftOvers = P - sum * fullReps;
            int fullRepsSection = missing.Count* fullReps;
            int leftOversSection = leftOvers;
            if (highestSeen > 0)
                leftOversSection = (leftOvers + (highestSeen - 1)) / highestSeen;
            if (leftOvers > 0)
            {
                int usingMissing = 0;
                int missingSum = 0;
                for (int i = 0; i < missing.Count && missingSum < leftOvers; i++)
                {
                    missingSum += missing[i];
                    usingMissing++;
                }
                leftOversSection = Math.Min(leftOversSection, usingMissing);
            }
            bestSoFar = Math.Min(bestSoFar, fullRepsSection + leftOversSection);
        }
        return bestSoFar;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            BlurredDartboard c = new BlurredDartboard();
            object o = c.minThrows(new int[] 
                {}, 0);
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
