using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class LongSeat
{
    public string[] canSit(int L, int[] width)
    {
        bool[] maybeDown = new bool[width.Length];
        bool[] maybeUp = new bool[width.Length];
        long maybeDownSum = 0;
        long maybeDownCount = 0;
        long maybeUpSum = 0;
        long maybeUpCount = 0;
        long total = 0;
        for (int i = 0; i < width.Length; i++)
        {
            long minimumSpace = L - maybeDownSum;
            long maximumSpace = L - (total - maybeUpSum);
            long startDownCount = maybeDownCount;
            long startUpCount = maybeUpCount;
            if (minimumSpace >= width[i])
            {
                maybeDown[i] = true;
                maybeDownSum += width[i];
                maybeDownCount ++;
            }
            if (minimumSpace < (long)width[i] * (long)(startDownCount + 1))
            {
                maybeUp[i] = true;
                maybeUpSum += width[i];
                maybeUpCount++;
            }
            if (maximumSpace < width[i])
            {
                if (!maybeUp[i])
                {
                    maybeUp[i] = true;
                    maybeUpSum += width[i];
                    maybeUpCount++;
                }
            }
            if (maximumSpace >= (long)width[i]*(long)(i - startUpCount + 1))
            {
                if (!maybeDown[i])
                {
                    maybeDown[i] = true;
                    maybeDownSum += width[i];
                    maybeDownCount++;
                }
            }
            total += width[i];
        }
        string[] result = new string[width.Length];
        for (int i = 0; i < width.Length; i++)
        {
            if (maybeDown[i] && maybeUp[i])
                result[i] = "Unsure";
            else if (maybeDown[i])
                result[i] = "Sit";
            else
                result[i] = "Stand";
        }
        return result;
    }

}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            LongSeat c = new LongSeat();
            object o = c.canSit(400, new int[] { 92, 65, 99, 46, 24, 85, 95, 84 });
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

