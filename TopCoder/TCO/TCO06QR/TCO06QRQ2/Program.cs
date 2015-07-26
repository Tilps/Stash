using System;
using System.Collections;
using System.Text;

public class NumJordanForms
{

    int[][] partCache;

    int modVal = 179424673;

    int partCount(int total, int maximumPart)
    {
        if (maximumPart >= total)
            maximumPart = total;
        if (partCache[total][maximumPart] == -1)
        {
            int sum = 0;
            if (total <= 1 || maximumPart <= 1)
                sum = 1;
            else
            {
                sum = partCount(total, maximumPart-1) + partCount(total - maximumPart, maximumPart);
                if (sum >= modVal)
                    sum -= modVal;
            }
            partCache[total][maximumPart] = sum;
        }
        return partCache[total][maximumPart];
    }

    public int howMany(int[] charPoly, int[] minPoly)
    {    
        partCache = new int[4001][];
        for (int i = 0; i < 4001; i++)
        {
            partCache[i] = new int[i+1];
            for (int j = 0; j <= i; j++)
            {
                partCache[i][j] = -1;
            }
        }
        long count = 1;
        for (int i = 0; i < charPoly.Length; i++)
        {
            count *= partCount(charPoly[i] - minPoly[i], minPoly[i]);
            count = count % modVal;
        }
        
        return (int)count;
    }
}

namespace TCO06QRQ2
{
    class Program
    {
        static void Main(string[] args)
        {
            NumJordanForms c = new NumJordanForms();
            object o = c.howMany(new int[] { 4000, 4000, 4000, 4000, 4000 }, new int[] { 500, 1000, 1500, 2000, 2500 });

            if (o is IEnumerable)
            {
                foreach (object oi in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(oi);

                }
            }
            else
                System.Console.Out.WriteLine(o);
            System.Console.In.ReadLine();
        }
    }
}