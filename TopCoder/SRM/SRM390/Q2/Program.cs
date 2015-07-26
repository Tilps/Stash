using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class PaintingBoards
{
    public double minimalTime(int[] boardLength, int[] painterSpeed)
    {
        double[,,] lengthTotals = new double[boardLength.Length+1, boardLength.Length+1, painterSpeed.Length];
        for (int i=0; i <= boardLength.Length; i++) {
            for (int j=i; j <= boardLength.Length; j++) {
                int total=0;
                for (int loop=i; loop < j; loop++) {
                    total += boardLength[loop];
                }
                for (int k = 0; k < painterSpeed.Length; k++)
                {
                    lengthTotals[i, j,k] = (double)total/(double)painterSpeed[k];
                }
            }
        }
        int bitfieldLimit = 1 << (painterSpeed.Length);
        int[] bitcount = new int[bitfieldLimit];
        for (int i = 1; i < bitfieldLimit; i++)
        {
            for (int l = 0; l < painterSpeed.Length; l++)
            {
                if ((i & (1 << l)) != 0)
                {
                    int prev = i - (1 << l);
                    bitcount[i] = bitcount[prev] + 1;
                    break;
                }
            }
        }

        double[,] table = new double[boardLength.Length + 1, bitfieldLimit];
        for (int j = 1; j <= boardLength.Length; j++)
        {
            table[j, 0] = double.PositiveInfinity;
            for (int i = 1; i < bitfieldLimit; i++)
            {
                if (bitcount[i] > j)
                {
                    table[j, i] = double.PositiveInfinity;
                    continue;
                }
               
                // best of each shorter board count with one less person + time for new person to do the new boards.
                double best = double.MaxValue;
                for (int l = 0; l < painterSpeed.Length; l++)
                {
                    if ((i & (1 << l)) != 0)
                    {
                        for (int k = 0; k <= j; k++)
                        {
                            int prev = i - (1 << l);
                            double basis = table[k, prev];
                            double next = lengthTotals[k, j,l];
                            double slowest = basis < next ? next : basis;
                            if (slowest < best)
                                best = slowest;
                        }
                    }
                }
                table[j, i] = best;
            }
        }
        return table[boardLength.Length, bitfieldLimit - 1];
    }
}

namespace Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            PaintingBoards c = new PaintingBoards();
            object o = c.minimalTime(new int[]{90, 5, 3, 17, 2, 27, 84, 73, 67, 2, 33, 71, 31, 40, 99, 35, 81, 48, 57, 68, 46, 51, 92, 35, 24, 9, 87, 43, 41, 67, 85, 23, 40, 32, 22, 85, 6, 60, 75, 94, 32, 97, 54, 23, 1, 54, 58, 6, 95, 23}, new int[] {4, 4, 13, 10, 5, 17, 7, 9, 4, 12, 13, 4, 6, 12, 1});
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
