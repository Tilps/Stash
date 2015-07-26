using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class BoxFilling
{
    public long getNumber(int sizex, int sizey, int sizez, int cubex, int cubey, int cubez)
    {
        long soFar = 0;
        long sx = sizex;
        long sy = sizey;
        long sz = sizez;
        while (true)
        {
            if (cubez == 1)
            {
                while (true)
                {
                    if (cubey == 1)
                    {
                        return soFar + cubex;
                    }
                    soFar += sx;
                    cubey--;
                    sy--;
                    if (cubex == 1)
                    {
                        return soFar + cubey;
                    }
                    soFar += sy;
                    cubex--;
                    sx--;
                }
            }
            soFar += sx * sy;
            cubez--;
            sz--;
            if (cubey == 1)
            {
                while (true)
                {
                    if (cubez == 1)
                    {
                        return soFar + cubex;
                    }
                    soFar += sx;
                    cubez--;
                    sz--;
                    if (cubex == 1)
                    {
                        return soFar + cubez;
                    }
                    soFar += sz;
                    cubex--;
                    sx--;
                }
            }
            soFar += sx * sz;
            cubey--;
            sy--;
            if (cubex == 1)
            {
                while (true)
                {
                    if (cubez == 1)
                    {
                        return soFar + cubey;
                    }
                    soFar += sy;
                    cubez--;
                    sz--;
                    if (cubey == 1)
                    {
                        return soFar + cubez;
                    }
                    soFar += sz;
                    cubey--;
                    sy--;
                }
            }
            soFar += sy * sz;
            cubex--;
            sx--;
        }
        return 0;
    }
}

namespace Q3
{
    class Program
    {
        static void Main(string[] args)
        {

            BoxFilling c = new BoxFilling();
            object o = c.getNumber(1,1,1,1,1,1);
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
