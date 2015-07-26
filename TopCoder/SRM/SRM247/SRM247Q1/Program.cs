using System;
using System.Collections;
using System.Text;
public class Musical
{
    public string loser(int count, double time)
    {
        double[] positions = new double[count];
        for (int i = 0; i < count; i++)
        {
            positions[i] = time / 10.0 - (double)i / (double)count;
            while (positions[i] < 0)
            {
                positions[i] += 1;
            }
            while (positions[i] >= 1)
            {
                positions[i] -= 1;
            }
        }
        double furthest = 0;
        int index = -1;
        for (int i = 0; i < count; i++)
        {
            double closest = 1;
            for (int j = 0; j < count; j++)
            {
                double dist = Math.Abs((double)j / (double)(count - 1) - positions[i]);
                if (dist < closest)
                    closest = dist;
            }
            if (closest > furthest)
            {
                furthest = closest;
                index = i;
            }
        }
        return "" + (char)('A' + index);
    }
}

namespace SRM247Q1
{


    class Program
    {
        static void Main(string[] args)
        {
            Musical m = new Musical();
            Console.Out.WriteLine(m.loser(2, 15.0));
            Console.In.ReadLine();
        }
    }
}
