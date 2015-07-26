using System;
using System.Collections;
using System.Text;

public class Predicting
{
	private double[] d;
	private double score(double[] weights) 
	{
		double score = 0.0;
		for (int i=5; i < d.Length; i++) 
		{
			double prediction = 0.0;
			for (int j=0; j < 5; j++) 
			{
				prediction += d[i-j-1]*weights[j];
			}
			score += Math.Abs(prediction-d[i]);
		}
		return score;
	}
	public double avgError(double[] data)
	{
		d = data;
		double best = double.MaxValue;
		double[] weights = new double[5];
		for (int i=-10; i <= 10; i++) 
		{
			for (int i2=-10; i2 <= 10; i2++) 
			{
				for (int i3=-10; i3 <= 10; i3++) 
				{
					for (int i4=-10; i4 <= 10; i4++) 
					{
						int i5 = 10-i-i2-i3-i4;
						if (i5 < -10 || i5 > 10)
							continue;
						weights[0] = (double)i/10.0;
						weights[1] = (double)i2/10.0;
						weights[2] = (double)i3/10.0;
						weights[3] = (double)i4/10.0;
						weights[4] = (double)i5/10.0;
						double sc = score(weights);
						if (sc < best)
							best = sc;
					}
				}
			}
		}
		return best/(d.Length-5);
	}
}

namespace SRM257Q1
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
			Predicting a = new Predicting();
			System.Console.Out.WriteLine(a.avgError(new double[] {10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,1010,10,10,10,10,10,10,10,10,10,10,10,10,10,10}));
			System.Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}