using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class SixteenBricks
{
    public int maximumSurface(int[] heights)
    {
        Array.Sort(heights);
        var bestArea = Squarify(heights, false);
        bestArea = Math.Max(bestArea, Squarify(heights, true));
        int firstArea = bestArea;
        Array.Reverse(heights);
        bestArea = Math.Max(bestArea, Squarify(heights, false));
        bestArea = Math.Max(bestArea, Squarify(heights, true));
        Random rnd = new Random();
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 16; j++)
            {
                int pos = rnd.Next(16 - j) + j;
                int tmp = heights[pos];
                heights[pos] = heights[j];
                heights[j] = tmp;
                bestArea = Math.Max(bestArea, Squarify(heights, false));
                bestArea = Math.Max(bestArea, Squarify(heights, true));
            }
        }
        if (bestArea > firstArea)
        {
            Console.Out.WriteLine("BadOmen");
        }
        return bestArea;
    }

    private int Squarify(int[] heights, bool interleave)
    {
        int[,] testHeights = new int[4, 4];
        int counter = 0;
        bool reverse = false;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (!interleave)
                {
                    testHeights[i, j] = heights[4*i + j];
                }
                else
                {
                    if (j%2 == 0)
                    {
                        int index = !reverse ? counter : 15 - counter;
                        testHeights[i, j] = heights[index];
                    }
                    else
                    {
                        int index = reverse ? counter : 15 - counter;
                        testHeights[i, j] = heights[index];
                        counter++;
                    }
                }                
            }
            reverse = !reverse;
        }
        var bestArea = OptimizeBestArea(testHeights);
        return bestArea;
    }

    private int OptimizeBestArea(int[,] testHeights)
    {
        int bestArea = surfaceArea(testHeights);
        nextLoop:
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    for (int l = 0; l < 4; l++)
                    {
                        if (i == k && j == l) continue;
                        int temp = testHeights[i, j];
                        testHeights[i, j] = testHeights[k, l];
                        testHeights[k, l] = temp;
                        int checkArea = surfaceArea(testHeights);
                        if (checkArea > bestArea)
                        {
                            bestArea = checkArea;
                            goto nextLoop;
                        }
                        temp = testHeights[i, j];
                        testHeights[i, j] = testHeights[k, l];
                        testHeights[k, l] = temp;
                    }
                }
            }
        }
        return bestArea;
    }

    private int surfaceArea(int[,] heights)
    {
        int total = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (i == 0 || i == 3)
                {
                    total += heights[i, j];
                }
                if (j == 0 || j == 3)
                {
                    total += heights[i, j];
                }
                if (i > 0 && heights[i - 1, j] < heights[i, j])
                {
                    total += heights[i, j] - heights[i - 1, j];
                }
                if (j > 0 && heights[i, j - 1] < heights[i, j])
                {
                    total += heights[i, j] - heights[i, j - 1];
                }
                if (i < 3 && heights[i + 1, j] < heights[i, j])
                {
                    total += heights[i, j] - heights[i + 1, j];
                }
                if (j < 3 && heights[i, j + 1] < heights[i, j])
                {
                    total += heights[i, j] - heights[i, j + 1];
                }
            }
        }
        return total + 16;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            SixteenBricks c = new SixteenBricks();
            Random rnd =new Random();
            int[] data = new int[16];
            for (int i = 0; i < 10000; i++)
            {
                //data[0] = 1;
                for (int j = 0; j < 16; j++)
                {
                  //  data[j] = data[j - 1] + j;
                    data[j] = rnd.Next(100);
                }
                object o = c.maximumSurface(data);
//                PrintObj(o);
            }
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