using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class HouseBuilding
{
    public int getMinimum(string[] area)
    {
        int lowest = int.MaxValue;
        for (int i = 0; i <= 9; i++)
        {
            int current = 0;
            for (int j = 0; j < area.Length; j++)
            {
                for (int k = 0; k < area[j].Length; k++)
                {
                    int height = int.Parse("" + area[j][k]);
                    if (height < i)
                        current += i - height;
                    else if (height > i + 1)
                        current += height - (i + 1);
                }
            }
            if (current < lowest)
                lowest = current;
        }
        return lowest;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            HouseBuilding c = new HouseBuilding();
            object o = c.getMinimum(new string[] {});
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
