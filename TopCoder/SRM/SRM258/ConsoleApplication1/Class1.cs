using System;
using System.Collections;
using System.Text;

public class ChutesAndLadders
{
	int[] fromTo;

	public double[] winningChance(int[] startLadder, int[] endLadder, int[] players)
	{
		fromTo = new int[112];
		for (int i=0; i < fromTo.Length; i++) 
		{
			fromTo[i] = i;
		}
		for (int i=100; i < fromTo.Length; i++) 
		{
			fromTo[i] = 99;
		}
		for (int j=0; j < startLadder.Length; j++) 
		{
			fromTo[startLadder[j]] = endLadder[j];
		}

		double[,] probs = new double[100,players.Length];
		double[,] nextProbs = new double[100,players.Length];
		double[] res = new double[players.Length];
		for (int i=0; i < players.Length; i++) 
		{
			probs[players[i],i] = 1.0;
		}
		double cumProb = 1;
		for (int t=0; t < 3000; t++) 
		{
			for (int i=0; i < players.Length; i++) 
			{
				for (int j=0; j < 100; j++)
					nextProbs[j,i] = 0.0;
			}
			for (int i=0; i < players.Length; i++) 
			{
				for (int j=0; j < 100; j++) 
				{
					for (int l=1; l <= 6; l++) 
					{
						for (int k=1; k <= 6; k++) 
						{
							nextProbs[fromTo[j+k+l],i] += probs[j,i]/6.0/6.0;
						}
					}
				}
				if (nextProbs[99,i] > 0) 
				{
					double oldCumProb = cumProb;
					cumProb *= (1-nextProbs[99,i]);
					res[i] += oldCumProb-cumProb;
					for (int j=0; j < 99; j++) 
					{
						nextProbs[j,i] /= (1-nextProbs[99,i]);
					}
					nextProbs[99,i] = 0.0;
				}
			}
			double[,] temp = probs;
			probs = nextProbs;
			nextProbs = temp;
		}


		return res;
	}
}

namespace SRM258Q3
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			ChutesAndLadders a = new ChutesAndLadders();
			object o = a.winningChance(new int[] {}, new int[]{}, new int[]{0,0});
			if (o is IEnumerable) 
			{
				foreach (object inner in (IEnumerable)o) 
				{
					System.Console.Write(inner);
					System.Console.Write(" ");
				}
				System.Console.WriteLine();
			}
			else
				System.Console.WriteLine(o);
			System.Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}