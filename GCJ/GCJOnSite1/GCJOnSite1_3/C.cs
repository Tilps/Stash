using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
// If TMD.Algo used, it is available from www.themissingdocs.net/downloads/TMD.Algo.0.0.3.0.zip

namespace GCJOnSite1
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(args[0]);
            List<string> output = new List<string>();
            int cases = int.Parse(lines[0]);
            int index = 1;
            for (int loopvariable = 0; loopvariable < cases; loopvariable++)
            {
                // Parse
                string[] bits = lines[index].Split(' ');
                int M = int.Parse(bits[0]);
                double P = double.Parse(bits[1]);
                int X = int.Parse(bits[2]);
                index++;
                // Process
                for (int i=0; i < 16; i++)
                    for (int j=0; j < 1000001; j++)
                        memo[i,j] = double.MinValue;
               output.Add(string.Format("Case #{0}: {1}", loopvariable + 1, Solve(M, P, X)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static double[,] memo = new double[16, 1000001];

        private static double Solve(int M, double P, int X)
        {
            if (X >= 1000000)
                return 1.0;
            if (M == 0)
                return 0;
            if (memo[M, X] != double.MinValue)
                return memo[M, X];
            int lowBet = 0;
            int highBet = X;
            if (highBet + X > 1000000)
                highBet = 1000000 - X;
            double a = Trial(M, P, X, lowBet);
            double b = Trial(M, P, X, highBet);
            int midBet = lowBet + (highBet - lowBet) / 2;
            double c = Trial(M, P, X, midBet);
            while (c == a || c == b)
            {
                if (c == a)
                {
                    if (lowBet == midBet)
                    {
                        if (a == b)
                            lowBet = highBet;
                        break;
                    }
                    lowBet = midBet;
                }
                else
                    highBet = midBet;
                if (lowBet == highBet)
                    break;
                a = Trial(M, P, X, lowBet);
                b = Trial(M, P, X, highBet);
                midBet = lowBet + (highBet - lowBet) / 2;
                c = Trial(M, P, X, midBet);
            }
            if (c < a && c < b)
                if (b > a)
                    lowBet = highBet;
                else
                    highBet = lowBet;
            double best = Math.Max(a, b);
            while (lowBet != highBet)
            {
                int betSize = (highBet - lowBet) / 3 + lowBet;
                double solveRes1 = Trial(M, P, X, betSize);
                best = Math.Max(best, solveRes1);
                if (highBet - lowBet == 1)
                    betSize = highBet;
                else
                    betSize = (highBet - lowBet)*2 / 3 + lowBet;
                double solveRes2 = Trial(M, P, X, betSize);
                best = Math.Max(best, solveRes2);
                if (solveRes1 > solveRes2)
                {
                    int oldHighBet = highBet;
                    highBet = betSize;
                    if (oldHighBet == highBet)
                        highBet--;
                }
                else
                {
                    int oldLowBet = lowBet;
                    lowBet = (highBet - lowBet) / 3 + lowBet;
                    if (oldLowBet == lowBet)
                        lowBet++;
                }
            }
            double res = Trial(M, P, X, lowBet);
            if (res < best)
                res = best;
            memo[M, X] = res;
            return res;

        }

        private static double Trial(int M, double P, int X, int betSize)
        {
            return Solve(M - 1, P, X + betSize) * P + Solve(M - 1, P, X - betSize) * (1 - P);
        }

    }
}
