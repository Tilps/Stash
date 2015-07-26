using System;
using System.Collections;
using System.Text;

public class Bigital
{
    public double energy(string tStart, string tEnd)
    {
        byte[] curBits = Trans(tStart);
        byte[] endBits = Trans(tEnd);
        long total = 0;
        while (true)
        {
            total += CountBits(curBits);
            if (Equal(curBits, endBits))
                break;
            Incriment(curBits);
        }
        return ((double)total) / 3600.0 / 1000.0;
    }

    private byte[] Trans(string tEnd)
    {
        byte[] res = new byte[6];
        res[0] = (byte)(tEnd[0] - '0');
        res[1] = (byte)(tEnd[1] - '0');
        res[2] = (byte)(tEnd[3] - '0');
        res[3] = (byte)(tEnd[4] - '0');
        res[4] = (byte)(tEnd[6] - '0');
        res[5] = (byte)(tEnd[7] - '0');
        return res;
    }

    private long CountBits(byte[] a) {
        long bits = 0;
        for (int i = 0; i < 6; i++)
        {
            bits += CountBits(a[i]);
        }
        return bits;
    }

    private long CountBits(byte a)
    {
        long total=0;
        for (int i=0 ; i< 4; i++) {
            if ((a & (1 << i)) != 0)
                total++;
        }
        return total;
    }

    private void Incriment(byte[] a)
    {
        a[5]++;
        if (a[5] > 9)
        {
            a[5] = 0;
            a[4]++;
            if (a[4] > 5)
            {
                a[4] = 0;
                a[3]++;
                if (a[3] > 9)
                {
                    a[3] = 0;
                    a[2]++;
                    if (a[2] > 5)
                    {
                        a[2] = 0;
                        a[1]++;
                        if (a[1] > 9 && a[0] == 0)
                        {
                            a[1] = 0;
                            a[0]++;
                        }
                        else if ((a[1] > 2 && a[0] == 1))
                        {
                            a[0] = 0;
                            a[1] = 1;
                        }
                    }
                }
            }
        }
    }

    private bool Equal(byte[] a, byte[] b)
    {
        for (int i = 0; i < 6; i++)
            if (a[i] != b[i])
                return false;
        return true;
    }
}

namespace Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            Bigital c = new Bigital();
            object o = c.energy("12:01:00", "12:00:00");
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
