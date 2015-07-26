using System;
using System.Collections;
using System.Text;

public class TableSeating
{
	public int trial(Random rnd, bool[] tables, int numTables, int[] probs) 
	{
		int size = 0;
		int opt = rnd.Next(100);
		int i=-1;
		while (opt >= 0) 
		{
			i++;
			opt-=probs[i];
		}
		size = i+1;
		ArrayList options = new ArrayList();
		for (int j=size-1; j < tables.Length; j++) 
		{
			bool all_false = true;
			for (int k=j;k>j-size;k--) 
			{
				if (tables[k])
					all_false = false;
			}
			if (all_false)
				options.Add(j);
		}
		if (options.Count == 0)
			return 0;
		opt = rnd.Next(options.Count);
		for (int k=(int)options[opt]; k > ((int)options[opt])-size; k--) 
		{
			tables[k] = true;
		}
		return size+trial(rnd, tables, numTables, probs);
	}
	public double getExpected(int numTables, int[] probs) 
	{
		Random rnd = new Random();
		double total = 0.0;
		int trials = 0;
		double last_average = 0.0;
		double average = 0.0;
		int confidence = 0;
		do 
		{
			do 
			{
				bool[] tables = new bool[numTables];
				trials += 1;
				total += trial(rnd, tables, numTables, probs);
				last_average = average;
				average = total/trials;
			} while (Math.Abs(average - last_average) > 0.00000001);	
			confidence++;
		} while (confidence < 10);
		return average;
	}
}

namespace SRM249Q1
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
			TableSeating t = new TableSeating();
			Console.Out.WriteLine(t.getExpected(4, new int[] {0,100}));
			Console.In.ReadLine();
				//
				// TODO: Add code to start application here
				//
			}
	}
}
