using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class KthProbableElement
{
    public double probability(int M, int lowerBound, int upperBound, int N, int K)
    {
        // Eliminate useless lowerBound.
        upperBound -= lowerBound;
        N -= lowerBound;
        // WRONG - this doesn't compensate for the overlap of the first two terms.
        // probability(M, range, N, K) = probability(M-1, range, N, K)*prob(range>=N)+probability(M-1, range, N, K-1)*prob(range <= N)+prob(range=N)*(prob(range != N)^(M-1)*prob(first K-1 ordered out of M-1 <= N)
        int range = upperBound + 1;
        double[,] prob = new double[M+1,M+1];
        for (int i = 1; i <= M; i++)
        {
            for (int j = 1; j <= i; j++)
            {
                double total = 0.0;
                total += prob[i - 1, j] * (range - N) / range;
                total += prob[i - 1, j - 1] * (N+1) / range;
                if (j == 1 && j==i)
                    total += 1.0 * 1.0 / range;
                else if (j == 1)
                    total += 1.0 * Math.Pow((double)(range - N - 1) / (double)range, i - 1 - (j - 1)) / range;
                else if (j == i)
                    total += Math.Pow((double)(N) / (double)range, j - 1) * 1.0 / range;
                else
                    total += Math.Pow((double)(N) / (double)range, j - 1) * Math.Pow((double)(range - N - 1) / (double)range, i - 1 - (j - 1)) / range;
                prob[i, j] = total;
            }
        }
        return prob[M,K];
    }
}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            KthProbableElement c = new KthProbableElement();
            object o = c.probability(3, 1, 2, 2, 2);
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
