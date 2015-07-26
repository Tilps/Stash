using System;
using System.Collections;
using System.Text;

public class AutoLoan
{

	public double interestRate(double price, double monthlyPay, int loanTerm)
	{

		double maxRate = 100.0;
		double minRate = 0.0;
		double rate = 50.0;
		double diff = 100.0;
		while (diff > 1e-12) 
		{
			double remaining = price;
			for (int i=0; i < loanTerm ;i++) 
			{
				remaining *= (1.0+rate/100.0/12.0);
				remaining -= monthlyPay;
			}
			if (remaining < 0) 
			{
				minRate = rate;
				rate = (minRate+maxRate)/2;
			}
			else if (remaining > 0)
			{
				maxRate = rate;
				rate = (minRate+maxRate)/2;
			}
			else 
			{
				minRate  = maxRate = rate;
			}
			diff = Math.Abs(maxRate-minRate);
		}

		return rate;
	}
}

namespace SRM258Q1
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
			AutoLoan a = new AutoLoan();
			System.Console.Out.WriteLine(a.interestRate(6800,100,680));
			System.Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}