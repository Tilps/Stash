using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace GCJR1Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(args[0]);
            List<string> output = new List<string>();
            int cases = int.Parse(lines[0]);
            int index = 1;
            for (int i = 0; i < cases; i++)
            {
                // Parse
                string[] bits = lines[index].Split(' ');
                index++;
                long A = long.Parse(bits[0]);
                long B = long.Parse(bits[1]);
                long P = long.Parse(bits[2]);
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(A, B, P)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static int Solve(long A, long B, long P)
        {
            Dictionary<int, List<long>> sets = new Dictionary<int, List<long>>();
            bool[] primes = new bool[B-A+3];
            for (int i = 0; i < primes.Length; i++)
                primes[i] = true;
            Sieve(primes);
            // now we have primes we can start.
            int[] setRecs = new int[B - A + 1];
            for (int i = 0; i < setRecs.Length; i++)
            {
                setRecs[i] = i;
            }
            for (long i = P; i < primes.Length; i++)
            {
                if (!primes[i])
                    continue;
                if (i >= B - A + 1)
                    continue;
                long start;
                if (A % i == 0)
                    start = A;
                else
                    start = ((A / i) + 1) * i;
                int set = GetParent(setRecs,start - A);
                for (long j = start + i; j <= B; j+= i)
                {
                    int set2 = GetParent(setRecs, j - A);
                    if (set2 != set)
                    {
                        setRecs[set2] = set;
                    }
                }
            }
            int count = 0;
            for (int i = 0; i < setRecs.Length; i++)
            {
                if (setRecs[i] == i)
                    count++;
            }
            return count;
        }

        private static int GetParent(int[] setRecs, long p)
        {
            if (setRecs[p] != p)
                setRecs[p] = GetParent(setRecs, setRecs[p]);
            return setRecs[p];
        }

        private static void Sieve(bool[] primes)
        {
            primes[0] = false;
            primes[1] = false;
            primes[2] = true;
            for (int i = 2; i * i < primes.Length; i++)
            {
                if (!primes[i])
                    continue;
                for (int j = i * i; j < primes.Length; j += i)
                {
                    primes[j] = false;
                }
            }
        }

    }
}
