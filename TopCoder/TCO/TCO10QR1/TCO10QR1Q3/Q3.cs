using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class SequenceMerger
{
    public int[] getVal(string[] seqs, int[] positions)
    {
        int max = 1000000001;
        // G's can be considered as E's or A's as maximum count is <30 or simply the same value repeated forever.
        // A's are the problem... have to determine the number of A's between or on two E values, then once proven it is in that range, determine what a given index hits. Binary search?
        List<int> distinct = new List<int>();
        List<int> starts = new List<int>();
        List<int> steps = new List<int>();
        List<int> lengths = new List<int>();
        foreach (string seq in seqs)
        {
            string[] bits = seq.Split(' ');
            switch (bits[0])
            {
                case "E":
                    for (int i = 1; i < bits.Length; i++)
                    {
                        int value;
                        if (int.TryParse(bits[i], out value))
                        {
                            if (value < max)
                                distinct.Add(value);
                        }
                    }
                    break;
                case "G":
                    if (bits[2] == "1")
                    {
                        int repeat;
                        if (int.TryParse(bits[1], out repeat))
                        {
                            if (repeat < max)
                            {
                                int value;
                                if (int.TryParse(bits[3], out value))
                                {
                                    starts.Add(repeat);
                                    steps.Add(0);
                                    lengths.Add(value);
                                }
                                else
                                {
                                    starts.Add(repeat);
                                    steps.Add(0);
                                    lengths.Add(max);
                                }
                            }
                        }
                    }
                    else
                    {
                        int start;
                        if (int.TryParse(bits[1], out start))
                        {
                            int mult;
                            if (int.TryParse(bits[2], out mult))
                            {
                                int count;
                                if (!int.TryParse(bits[3], out count))
                                    count = max;
                                long cur = start;
                                do
                                {
                                    distinct.Add((int)cur);
                                    cur *= mult;
                                    count--;
                                } while (cur < (long)max && count > 0);
                            }
                            else
                            {
                                distinct.Add(start);
                            }
                        }
                    }
                    break;
                case "A":
                    int start2;
                    if (int.TryParse(bits[1], out start2))
                    {
                        if (start2 < max)
                        {
                            int step;
                            if (!int.TryParse(bits[2], out step))
                                step = max;
                            if (step > max)
                                step = max;
                            int count;
                            if (!int.TryParse(bits[3], out count))
                                count = max;
                            if (count > max)
                                count = max;
                            starts.Add(start2);
                            steps.Add(step);
                            lengths.Add(count);
                        }
                    }
                    break;

            }
        }
        distinct.Sort();
        int[] results = new int[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            results[i] = -1;
            int target = positions[i];
            int high = max;
            int low = 0;
            do
            {
                int mid = low + (high - low) / 2;
                int count1 = CountLess(mid, distinct, starts, steps, lengths);
                int count2 = CountLess(mid + 1, distinct, starts, steps, lengths);
                if (count1 < target && count2 >= target) {
                    results[i] = mid;
                    break;
                }
                else if (target <= count1)
                {
                    high = mid - 1;
                }
                else
                {
                    low = mid + 1;
                }
            } while (high > low);
        }
        return results;
    }

    private int CountLess(int mid, List<int> distinct, List<int> starts, List<int> steps, List<int> lengths)
    {
        int counter = 0;
        for (int i = 0; i < distinct.Count; i++)
        {
            if (distinct[i] < mid)
                counter++;
            else
                break;
        }
        for (int i = 0; i < starts.Count; i++)
        {
            int start = starts[i];
            int step = steps[i];
            int length = lengths[i];
            if (start < mid)
            {
                //if (step
            }

        }
        return 0;
    }
}

namespace Q3
{
    class Q3
    {
        static void Main(string[] args)
        {
            SequenceMerger c = new SequenceMerger();
            object o = c.getVal(new string[] { }, new int[] { });
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
