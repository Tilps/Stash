using System;

public class ContractWork 
{
	public int minimumCost(string[] costs, int numTasks)
	{
		int[][] rc = new int[costs.Length][];
		for (int i=0; i < costs.Length; i++) 
		{
			string[] splits = costs[i].Split(' ');
			rc[i] = new int[splits.Length];
			for (int j=0; j < splits.Length; j++) 
			{
				rc[i][j] = int.Parse(splits[j]);
			}
		}

		int[,,] minCosts = new int[numTasks+1, costs.Length+1, costs.Length+1];
		
		for (int i=1; i <= numTasks; i++) 
		{
			for (int j=0; j <= costs.Length; j++) 
			{
				for (int k=0; k <= costs.Length; k++)
				{
					if (k ==0)
						minCosts[i,j,k] = int.MaxValue;
					if (j==0 && i > 1)
						minCosts[i,j,k] = int.MaxValue;
					if (i==1 && j != 0)
						minCosts[i,j,k] = int.MaxValue;
				}
			}
		}
		
		for (int i=1; i <= numTasks; i++) 
		{
			for (int j=0; j <= costs.Length; j++) 
			{
				if (j==0 && i > 1)
					continue;
				if (i==1 && j >0)
					continue;
				for (int k=1; k <= costs.Length; k++)
				{
					int minimum = int.MaxValue;
					for (int oj=0;oj <= costs.Length; oj++)
					{
						int ok=j;
						if (oj==ok&&ok==k)
							continue;
						int min = minCosts[i-1, oj, ok];
						if (min < minimum)
							minimum = min;
					}
					minCosts[i,j,k] = minimum+rc[k-1][i-1];

				}
			}
		}
		int best = int.MaxValue;
		int start = 1;
		for (int j=start; j <= costs.Length; j++)
		{
			for (int k=1; k <= costs.Length; k++)
			{
				int min = minCosts[numTasks, j, k];
				if (min < best)
					best = min;
			}
		}
		return best;
	}
}

namespace SRM248Q2
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
			ContractWork a = new ContractWork();
			Console.Out.WriteLine(a.minimumCost(new string[] {"1 2 3", "4 5 6"}, 3));
			Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}
