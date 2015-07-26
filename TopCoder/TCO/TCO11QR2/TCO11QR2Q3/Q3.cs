using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class FoxIntegerLevelThree
{
    public long count(long min, long max)
    {
        // Find count of all numbers between min and max inclusive where X = y * (y%9 == 0 ? 0 : y%9))
        long total = 0;
        total += CountBetween(min, max, 81, 0);
        // 9k*9 = 0 mod 9
        total += CountBetween(min, max, 9, 1);
        // 9k+1 = 1 mod 9
        total += CountBetween(min, max, 18, 4);
        // 18k+4 = 4 mod 9
        total += CountBetween(min, max, 27, 9);
        // 27k+9 = 0 mod 9
        total += CountBetween(min, max, 36, 16);
        // 36k+16 = 7 mod 9
        total += CountBetween(min, max, 45, 25);
        // 45k+25 = 7 mod 9
        total += CountBetween(min, max, 54, 36);
        // 54k+36 = 0 mod 9
        total += CountBetween(min, max, 63, 49);
        // 63k+49 = 4 mod 9
        total += CountBetween(min, max, 72, 64);
        // 72k+64 = 1 mod 9
        // only ever get overlaps in sets.
        // worst case is the 3 0's mod 9.
        // pairs on 1 mod 9
        total -= CountBetween(min, max, 72, 64);
        // pairs on 4 mod 9
        total -= CountBetween(min, max, 14*9, 49+63);
        // pairs on 7 mod 9
        // mod 180 pattern is 16, 52,88,124,160 vs 25, 70, 115, 160
        total -= CountBetween(min, max, 180, 160);
        // tripple interaction on 0 mod 9 - mod 162 patterns are
        // 36, 90, 144 vs 9, 36, 63, 90, 117, 144 vs 0,81
        // first is removed by the second entirely, otherwise no overlap
        total -= CountBetween(min, max, 54, 36);
       
        return total;
    }

    private long CountBetween(long min, long max, long mod, long remainder)
    {
        return CountLess(max + 1, mod, remainder) - CountLess(min, mod, remainder);
    }

    private long CountLess(long exclusiveCeiling, long mod, long remainder)
    {
        if (((exclusiveCeiling - 1) % mod) >= remainder)
            return (exclusiveCeiling - 1) / mod + 1;
        return (exclusiveCeiling - 1) / mod;
    }

}

namespace Q3
{
    class Q3
    {
        static void Main(string[] args)
        {
            FoxIntegerLevelThree c = new FoxIntegerLevelThree();
            object o = c.count(1L, 10000000000L);
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
