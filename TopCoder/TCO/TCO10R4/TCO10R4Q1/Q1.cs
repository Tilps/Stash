using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class BankLottery
{
    public double expectedAmount(int[] accountBalance, int weeklyJackpot, int weekCount)
    {
        int total = 0;
        for (int i = 0; i < accountBalance.Length; i++)
            total += accountBalance[i];
        // me - everyone else. - me defines the other.
        double[,] dp = new double[weekCount+1, weekCount+1];
        dp[0, 0] = 1.0;
        for (int i = 1; i <= weekCount; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                // Probability of j wins in i weeks. = probability of j wins in i-1 weeks * probability of losing that situation + probabilityof j-1 wins in i-1 weeks * probability of winning in that situation.
                int prevTotal = total + (i-1) * weeklyJackpot;

                double loss = (double)(prevTotal - accountBalance[0] - j * weeklyJackpot) / (double)prevTotal;
                dp[i, j] = dp[i - 1, j] * loss;
                if (j > 0)
                {
                    double win = (double)(accountBalance[0] + (j - 1) * weeklyJackpot) / (double)prevTotal;
                    dp[i, j] += dp[i - 1, j - 1] * win;
                }
            }
        }
        double expectation = 0.0;
        for (int j = 0; j <= weekCount; j++)
        {
            expectation += dp[weekCount, j] * (accountBalance[0] + j * weeklyJackpot);
        }
        return expectation;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            BankLottery c = new BankLottery();
            object o = c.expectedAmount(new int[] { 2, 2, 2 }, 1, 2);
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
