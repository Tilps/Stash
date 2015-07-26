#region Using directives

using System;
using System.Collections;
using System.Text;

#endregion

public class Rationalization {

    private int[] totalScores(int[] weights, int[][] sc) {
        int[] result = new int[sc.Length];
        for (int i = 0; i < sc.Length; i++) {
            int score = 0;
            for (int j = 0; j < weights.Length; j++) {
                score += sc[i][j] * weights[j];
            }
            result[i] = score;
        }
        return result;
    }

    public int minTweaks(int[] weights, string[] scores, int desired) {

        int[][] sc = new int[scores.Length][];
        for (int i = 0; i < sc.Length; i++) {
            sc[i] = new int[weights.Length];
            for (int j = 0; j < weights.Length; j++) {
                sc[i][j] = int.Parse("" + scores[i][j]);
            }
        }

        int index = 0;
        while (true) {
            int bestScore = -1;
            ArrayList currentBest = new ArrayList();
            int[] ts = totalScores(weights, sc);
            for (int i = 0; i < ts.Length; i++) {
                if (ts[i] > bestScore) {
                    bestScore = ts[i];
                    currentBest = new ArrayList();
                    currentBest.Add(i);
                }
                else if (ts[i] == bestScore) {
                    if ((int)currentBest[0] == desired) {
                        currentBest = new ArrayList();
                        currentBest.Add(i);
                    }
                    else if (i != desired) {
                        currentBest.Add(i);
                    }
                }
            }
            if ((int)currentBest[0] == desired)
                return index;


            int bestDelta = 0;
            int bestDeltaIndex = -1;
            for (int i = 0; i < weights.Length; i++) {
                int worstDelta = 1000000;
                for (int j=0; j < sc.Length; j++) {
                    if (j == desired)
                        continue;
                    int delta = sc[desired][i] - sc[j][i];
                    if (delta != 0)
                        if (Math.Abs(delta) < Math.Abs(worstDelta))
                            worstDelta = delta;
                }

                if (worstDelta > 0) {
                    if (weights[i] < 9) {
                        if (Math.Abs(worstDelta) >Math.Abs(bestDelta)) {
                            bestDelta = worstDelta;
                            bestDeltaIndex = i;
                        }
                    }
                }
                else if (worstDelta < 0) {
                    if (weights[i] > 1) {
                        if (Math.Abs(worstDelta) >Math.Abs(bestDelta)) {
                            bestDelta = worstDelta;
                            bestDeltaIndex = i;
                        }
                    }
                }
            }
            if (bestDeltaIndex == -1)
                return -1;
            else if (bestDelta == 0)
                return -1;
           else if (bestDelta > 0)
                weights[bestDeltaIndex]++;
            else
                weights[bestDeltaIndex]--;
            index++;
        }
    }

}



namespace SRM224d1q2 {
    class Program {
        static void Main(string[] args) {

            Rationalization c = new Rationalization();
            Console.Out.WriteLine(c.minTweaks(
                new int[]{3,2,1,1}, 
                new string[]{ 
                    "6354",
                    "5532",
                    "4626" }, 
                    2));
            Console.In.ReadLine();

        }
    }
}
