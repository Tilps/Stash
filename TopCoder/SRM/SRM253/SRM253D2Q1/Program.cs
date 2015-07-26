using System;
using System.Collections;
using System.Text;

public class ObjectPacking
{
    public int smallBox(int objWidth, int objLength, int[] boxWidth, int[] boxLength)
    {
        int fit = -1;
        int smallest = int.MaxValue;
        for (int i = 0; i < boxWidth.Length; i++)
        {
            if (boxWidth[i] >= objWidth && boxLength[i] >= objLength || boxWidth[i] >= objLength && boxLength[i] >= objWidth)
            {
                int a = boxWidth[i] * boxLength[i];
                if (a < smallest)
                {
                    fit = i;
                    smallest = a;
                }
            }
        }
        if (fit != -1)
            return smallest;
        return fit;
    }
}

namespace SRM253D2Q1
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
