using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class RaceTrack
{
    public string judgePositions(int length, int judges, int[] pos)
    {
        int[,] maxSmallest = new int[pos.Length, judges+1];
        for (int i = 2; i <= judges; i++)
        {
            for (int j = 0; j < pos.Length; j++)
            {
                maxSmallest[j, i] = -1;
            }
        }
        for (int i = 2; i <= judges; i++)
        {
            for (int j = 0; j < pos.Length; j++)
            {
                int max = -1;
                for (int k = 0; k < j; k++)
                {
                    if (maxSmallest[k, i - 1] > 0)
                    {
                        int newScore = Math.Min(maxSmallest[k, i - 1], pos[j] - pos[k]);
                        if (newScore > max)
                            max = newScore;
                    }
                    else if (maxSmallest[k, i - 1] == 0)
                    {
                        int newScore = pos[j] - pos[k];
                        if (newScore > max)
                            max = newScore;
                    }
                }
                maxSmallest[j, i] = max;
            }
        }
        string res = string.Empty;

        int best = 0;
        int bestIndex = -1;
        for (int i = 0; i < pos.Length; i++)
        {
            if (maxSmallest[i, judges] > best)
            {
                best = maxSmallest[i, judges];
                bestIndex = i;
            }
        }
        List<int> indexes = new List<int>();
        indexes.Add(bestIndex);
        for (int i = judges-1; i > 0; i--)
        {
            for (int j = 0; j < bestIndex; j++)
            {
                if (maxSmallest[j, i] >= best || maxSmallest[j,i] == 0)
                {
                    indexes.Add(j);
                    bestIndex = j;
                    //best = maxSmallest[j, i];
                    break;
                }
 /*               if (pos[bestIndex] - pos[j] >= best )
                {
                    indexes.Add(j);
                    bestIndex = j;
                    best = maxSmallest[j, i];
                    break;
                }
   */         }
        }
        for (int i = 0; i < pos.Length; i++)
        {
            if (indexes.Contains(i))
                res += "1";
            else
                res += "0";
        }
        return res;
    }
}

namespace Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            RaceTrack c = new RaceTrack();
            object o = c.judgePositions(324, 6, new int[] {16, 28, 31, 61, 62, 70, 78, 149, 160, 171, 201, 228, 238, 241, 271, 287, 295, 310});
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
