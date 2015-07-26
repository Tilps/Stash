using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class BidirectionalQueue
{
    public int extractElements(int N, int[] indices)
    {
        List<int> values = new List<int>();
        for (int i = 1; i <= N; i++)
            values.Add(i);
        int total = 0;
        foreach (int index in indices)
        {
            int spot = values.IndexOf(index);
            int dist = Math.Min(spot, values.Count - spot);
            total += dist;
            RotateAndRemove(values, spot);
        }
        return total;
    }

    private void RotateAndRemove(List<int> values, int spot)
    {
        List<int> receive = new List<int>();
        for (int i = 0; i < spot; i++) {
            receive.Add(values[i]);
        }
        values.RemoveRange(0, spot + 1);
        values.AddRange(receive);
    }
}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            BidirectionalQueue c = new BidirectionalQueue();
            object o = c.extractElements(10, new int[] {1,2,3});
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
