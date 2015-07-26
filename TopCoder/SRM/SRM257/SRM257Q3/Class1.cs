using System;
using System.Collections;
using System.Text;

public class Computers
{

	public long[,] memo = new long[1001,1001];

	private long go(int amount, int minDiff, int n) 
	{
		if (amount < 0)
			return 0;
		if (memo[n,amount] == -1) 
		{
			long total =0;
			if (n==0) 
			{
                if (amount == 0)
    				total = 1;
			}
			else 
			{
				for (int i=minDiff; i <= amount; i++) 
				{
					total += go(amount-i*n, minDiff, n-1);
				}
				total += go(amount, minDiff, n-1);
			}
			memo[n,amount] = total;
		}
		return memo[n,amount];

	}


	public long choices(int amount, int minDif, int minInComp, int n)
	{
        for (int i = 0; i < 1001; i++)
        {
            for (int j = 0; j < 1001; j++)
            {
                memo[i, j] = -1;
            }
        }
		long total= 0;
		for (int i=minInComp; i <= amount;i++) 
		{
			total += go(amount-n*i, minDif, n-1);
		}
		return total;
	}
}

namespace SRM257Q3
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
			Computers a = new Computers();
			System.Console.Out.WriteLine(a.choices(1000,5,5,10));
			System.Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}