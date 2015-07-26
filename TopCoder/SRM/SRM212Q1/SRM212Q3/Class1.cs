using System;
using System.Collections;
using System.Text;


public class YahtzeeScore {
	public int maxPoints(int[] toss) {
		int best = 0;
		for (int i=1; i<=6;i++) {
			int total = 0;
			for (int j=0; j<toss.Length;j++) {
				if (toss[j] == i)
					total += i;
			}
			if (total > best)
				best = total;
		}
		return best;
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
		YahtzeeScore a = new YahtzeeScore();
		Console.Out.WriteLine("{0}", 
			a.maxPoints(new int[] {})
			); 
		Console.In.ReadLine();
	}
}
