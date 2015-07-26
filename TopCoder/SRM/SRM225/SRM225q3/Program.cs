#region Using directives

using System;
using System.Collections;
using System.Text;

#endregion

public class Triangulation {

    public class StringPairComparer : IComparer {

        #region IComparer Members

        public int Compare(object x, object y) {
            string x1 = (string)x;
            string y1 = (string)y;
            string[] x1s = x1.Split(' ');
            string[] y1s = y1.Split(' ');
            int xv = int.Parse(x1s[0]);
            int yv = int.Parse(y1s[0]);
            if (xv > yv)
                return 1;
            if (yv > xv)
                return -1;
            xv = int.Parse(x1s[1]);
            yv = int.Parse(y1s[1]);
            if (xv > yv)
                return 1;
            if (yv > xv)
                return -1;
            return 0;
        }

        #endregion
    }

    private StringPairComparer stringPairComparer = new StringPairComparer();

    private int cross(int x1, int y1, int x2, int y2) {
        return x1 * y2 - x2 * y2;
    }

    private bool CanSplit(int[] x, int[] y, int i, int j) {
        ArrayList x1 = new ArrayList();
        ArrayList y1 = new ArrayList();
        ArrayList x2 = new ArrayList();
        ArrayList y2 = new ArrayList();
        for (int k = i+1; k < j; k++) {
            x1.Add(x[k]);
            y1.Add(y[k]);
        }
        int next = j;
        while (true) {
            next++;
            if (next >= x.Length)
                next = 0;
            if (next == i)
                break;
            x2.Add(x[next]);
            y2.Add(y[next]);
        }
        int left = 0;
        for (int k = 0; k < x1.Count; k++) {
            int thisLeft = cross((int)x1[k], (int)y1[k], x[i], y[i]);
            if (left == 0) {
                left = thisLeft;
            }
            else if (thisLeft > 0) {
                if (left < 0)
                    return false;
            }
            else if (thisLeft < 0) {
                if (left > 0)
                    return false;
            }
        }
        for (int k = 0; k < x2.Count; k++) {
            int thisLeft = cross((int)x2[k], (int)y2[k], x[i], y[i]);
            if (left == 0) {
                left = thisLeft;
            }
            else if (thisLeft > 0) {
                if (left > 0)
                    return false;
            }
            else if (thisLeft < 0) {
                if (left < 0)
                    return false;
            }
        }
        return true;

    }

    public string[] triangulate(int[] x, int[] y) {

        for (int i = 0; i < x.Length-1; i++) {
            for (int j = i; j < x.Length; j++) {
                if (CanSplit(x, y, i, j)) {
                    ArrayList x1 = new ArrayList();
                    ArrayList y1 = new ArrayList();
                    ArrayList x2 = new ArrayList();
                    ArrayList y2 = new ArrayList();
                    for (int k = i; k <= j; k++) {
                        x1.Add(x[k]);
                        y1.Add(y[k]);
                    }
                    int next = j;
                    x2.Add(x[next]);
                    y2.Add(y[next]);
                    while (true) {
                        next++;
                        if (next >= x.Length)
                            next = 0;
                        x2.Add(x[next]);
                        y2.Add(y[next]);
                        if (next == i)
                            break;
                    }
                    string[] a1 = triangulate((int[])x1.ToArray(typeof(int)), (int[])y1.ToArray(typeof(int)));
                    string[] a2 = triangulate((int[])x2.ToArray(typeof(int)), (int[])y2.ToArray(typeof(int)));
                    string[] a = new string[a1.Length + a2.Length+1];
                    Array.Copy(a1, a, a1.Length);
                    Array.Copy(a2, 0, a, a1.Length, a2.Length);
                    a[a.Length-1] = i.ToString() + " " + j.ToString();
                    Array.Sort(a, stringPairComparer);
                    return a;
                }
            }
        }

        return new string[0];
    }

}


namespace SRM225q3 {
    class Program {
        static void Main(string[] args) {
//#define REAL_DEBUG
#if REAL_DEBUG
            a c = new a();
            Console.Out.WriteLine(c.b());
            Console.In.ReadLine();
#endif

        }
    }
}
