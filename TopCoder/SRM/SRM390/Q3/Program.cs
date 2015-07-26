using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class BuildCircuit
{
    public int minimalCount(int a, int b)
    {
        Frac toFind = new Frac(a, b);
        Simplify(toFind);
        List<Frac>[] lists = new List<Frac>[16];
        Dictionary<Frac, bool> found = new Dictionary<Frac, bool>();
        for (int i = 0; i < 16; i++)
        {
            lists[i] = new List<Frac>();
        }
        lists[0].Add(new Frac(1,1));
        lists[0].Add(new Frac(2,1));
        found[new Frac(1, 1)] = true;
        found[new Frac(2, 1)] = true;
        if (found.ContainsKey(toFind))
            return 1;
        for (int i = 1; i < 16; i++)
        {
            for (int j = 0; j < (i+1)/2; j++)
            {
                int other = i - j;
                for (int k = 0; k < lists[j].Count; k++)
                {
                    Frac f1 = lists[j][k];
                    for (int l = 0; l < lists[other].Count; l++)
                    {
                        Frac f2 = lists[other][l];
                        Frac newFrac = Add(f1, f2);
                        if (newFrac.Equals(toFind))
                            return i+1;
                        if (!found.ContainsKey(newFrac))
                        {
                            lists[i].Add(newFrac);
                            found[newFrac] = true;
                        }
                        newFrac = RecipAdd(f1, f2);
                        if (newFrac.Equals(toFind))
                            return i + 1;
                        if (!found.ContainsKey(newFrac))
                        {
                            lists[i].Add(newFrac);
                            found[newFrac] = true;
                        }
                    }
                }
            }
        }
        
        return -1;
    }

    private Frac RecipAdd(Frac f1, Frac f2)
    {
        Frac newFrac = new Frac( f1.n * f2.n, f1.n * f2.d + f1.d * f2.n);
        Simplify(newFrac);
        return newFrac;
    }

    private Frac Add(Frac f1, Frac f2)
    {
        Frac newFrac = new Frac(f1.n * f2.d + f1.d * f2.n, f1.d * f2.d);
        Simplify(newFrac);
        return newFrac;
    }

    class Frac
    {
        public Frac(long a, long b)
        {
            n = a;
            d = b;
        }
        public long n;
        public long d;
        public override int GetHashCode()
        {
            return (int)(n * d);
        }

        public override bool Equals(object obj)
        {
            Frac other = (Frac)obj;
            long a = n * other.d;
            long b = d * other.n;
            return a == b;
        }
    }

    private void Simplify(Frac f)
    {
    }
}

namespace Q3
{
    class Program
    {
        static void Main(string[] args)
        {

            BuildCircuit c = new BuildCircuit();
            object o = c.minimalCount(1,2);
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
