using System;
using System.Collections;
using System.Text;


public class WinningRecord {
	public int[] getBestAndWorst(string games) {
		bool[] wins = new bool[games.Length];
		for (int i=0; i<games.Length;i++) {
			if (games[i] == 'W')
				wins[i]=true;
		}
		double[] ratios = new double[games.Length];
		for (int i=0; i<games.Length;i++) {
			int winSum = 0;
			for (int j=0; j<=i; j++) {
				if (wins[j])
					winSum++;
			}
			ratios[i] = (double)winSum/(double)(i+1);
		}
		double bestRatio = -1.0;
		double worstRatio = 2.0;
		int bestIndex = -1;
		int worstIndex = -1;
		for (int i=2; i<ratios.Length;i++) {
			if (ratios[i] >= bestRatio) {
				bestRatio = ratios[i];
				bestIndex = i;
			}
			if (ratios[i] <= worstRatio) {
				worstRatio = ratios[i];
				worstIndex = i;
			}
		}
		return new int[] {bestIndex+1, worstIndex+1};
	}
}



	/// <summary>
	/// Summary description for Class1.
	/// </summary>
class Class1 {
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main(string[] args) {
		WinningRecord a = new WinningRecord();
		Console.Out.WriteLine("{0}", 
			a.getBestAndWorst("games")
			); 
		Console.In.ReadLine();
	}
}
