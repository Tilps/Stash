using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class MedievalCity
{
    public int getDangerousBlocks(string[] boundry, int dangerBound)
    {
        List<byte> dirs = new List<byte>();
        string full = string.Concat(boundry);
        int index = 0;
        int maxX = 0;
        int maxY = 0;
        int minX = 0;
        int minY = 0;
        int x = 0;
        int y = 0;
        while (index < full.Length)
        {
            int reps = 1;
            int digits=0;
            if (index + 1 < full.Length && char.IsDigit(full[index + 1]))
            {
                if (index + 2 < full.Length && char.IsDigit(full[index + 2]))
                {
                    reps = int.Parse(full.Substring(index + 1, 2));
                    digits = 2;
                }
                else
                {
                    reps = int.Parse(full.Substring(index + 1, 1)); 
                    digits = 1;
                }
            }
            byte dir = 0;
            if (full[index] == 'R')
            {
                dir = 0;
                x += reps;
                if (x > maxX)
                    maxX = x;
            }
            if (full[index] == 'D')
            {
                dir = 1;
                y += reps;
                if (y > maxY)
                    maxY = y;

            }
            if (full[index] == 'L')
            {
                dir = 2;
                x -= reps;
                if (x < minX)
                    minX = x;
            }
            if (full[index] == 'U')
            {
                dir = 3;
                y -= reps;
                if (y < minY)
                    minY = y;
            }
            for (int i = 0; i < reps; i++)
                dirs.Add(dir);
            index++;
            index += digits;
        }
        int startX = -minX + 2;
        int startY = -minY + 2;
        int width = maxX - minX + 4;
        int height = maxY - minY + 4;
        byte[,] data = new byte[width, height];
        int curX = startX;
        int curY = startY;
        int lastX = curX;
        int lastY = curY;
        int total=0;
        List<int> xs = new List<int>();
        List<int> ys = new List<int>();
        for (int i = 0; i < dirs.Count; i++)
        {
            switch (dirs[i])
            {
                case 0:
                    curX++;
                    if (i > 0)
                    {
                        data[curX - 1, curY] = 1;
                        data[curX - 1, curY - 1] = 1;
                    }
                    break;
                case 1:
                    curY++;
                    if (i > 0)
                    {
                        data[curX, curY - 1] = 1;
                        data[curX - 1, curY - 1] = 1;
                    }
                    break;
                case 2:
                    curX--;
                    if (i > 0)
                    {
                        data[curX + 1, curY] = 1;
                        data[curX + 1, curY - 1] = 1;
                    }
                    break;
                case 3:
                    curY--;
                    if (i > 0)
                    {
                        data[curX, curY + 1] = 1;
                        data[curX - 1, curY + 1] = 1;
                    }
                    break;
            }
        }
        return 0;
    }
}

namespace Q1
{
    class Program
    {
        static void Main(string[] args)
        {
            MedievalCity c = new MedievalCity();
            object o = c.getDangerousBlocks(new string[] {}, 0);
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
