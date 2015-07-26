#region Using directives

using System;
using System.Collections;
using System.Text;

#endregion

public class ArithSeq {

    private class Range : IComparable {
        public Range(long start, long end) {
            this.start = start;
            this.end = end;
            valid = true;
        }

        public long start;

        public long end;

        public long length {
            get {
                return end - start + 1;
            }
        }

        public bool valid;

        #region IComparable Members

        public int CompareTo(object obj) {
            Range otherRange = (Range)obj;
            if (end < otherRange.start)
                return -1;
            if (start > otherRange.end)
                return 1;
            return 0;
        }

        #endregion
    }

    public long minStart(int n, int delta) {
        ArrayList parts = new ArrayList();
        long start = 1;
        for (int i = 1; i < 1000000; i++) {
            parts.Add(new Range(start, start + i - 1));
            start += 2 * i;
            if ((long)i >= (long)n*(long)delta)
                break;
        }
        n--;
        Range newRange = new Range(0,0);

        while (n > 0) {
            for (int i = 0; i < parts.Count; i++) {
                Range range = (Range)parts[i];
                newRange.start = range.start + delta;
                newRange.end = range.end + delta;
                int index = parts.BinarySearch(i, parts.Count - i, newRange, null);
                if (index < 0)
                    range.valid = false;
                else {
                    Range otherRange = (Range)parts[index];
                    if (range.end + delta <= otherRange.end) {
                        long diff = range.end + delta - otherRange.start + 1;
                        if (diff < range.length) {
                            range.start = range.end - diff + 1;
                        }
                    }
                    else if (range.start + delta >= otherRange.start) {
                        long diff = otherRange.end - range.start - delta + 1;
                        if (diff < range.length) {
                            range.end = range.start + diff - 1;
                        }
                    }
                }
            }
            ArrayList temp = parts;
            parts = new ArrayList();
            for (int i = 0; i < temp.Count; i++) {
                if (((Range)temp[i]).valid)
                    parts.Add(temp[i]);
            }
            n--;
        }
        for (int i = 0; i < parts.Count; i++) {
            Range range = (Range)parts[i];
            if (!range.valid)
                continue;
            return (range.start);
        }

        return -1;
    }

}




namespace SRM224d1q3 {
    class Program {
        static void Main(string[] args) {

            ArithSeq c = new ArithSeq();
            Console.Out.WriteLine(c.minStart(30, 1000000000));
            Console.In.ReadLine();

        }
    }
}
