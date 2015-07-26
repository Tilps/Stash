using System;
using System.Collections;
using System.Text;

public class Pitches {
    public double strikeOut(double[] stats, int balls, int strikes) {
        double P_strike = (strikes < 2) ? strikeOut(stats, balls,
strikes+1) : 1;
        double P_ball   = (balls < 3)   ? strikeOut(stats, balls+1,
strikes) : 0;
        double A = P_ball * stats[0] + P_strike*stats[1];
        double B = P_ball * stats[2] + P_strike*stats[3];
        double C = P_ball * stats[4] + P_strike*stats[5];
        double D = P_ball * stats[6] + P_strike*stats[7];
        if (A >= B && C >= D) return Math.Max(B, D);
        if (A <= B && C <= D) return Math.Max(A, C);
        if (A >= C && B >= D) return Math.Min(A, B);
        if (A <= C && B <= D) return Math.Min(C, D);
        return (A*D - B*C) / (A+D-B-C);
   }
}


namespace SRM253Q3
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
