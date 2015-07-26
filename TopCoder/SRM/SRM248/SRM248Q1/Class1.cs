using System;

public class WordPattern 
{

	public long numberOfWays(string letters) 
	{
		if (letters.Length == 1)
			return 1;
		long result = numberOfWays(letters.Substring(1));
		result += ((long)1) << (letters.Length-1);
		return result;
	}

	public long countWords(string letters)
	{
		if (letters.Length == 1)
			return 1;
		long result = 4* numberOfWays(letters.Substring(1));
		return result;
	}
}

namespace SRM248Q1
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
			WordPattern a = new WordPattern();
			Console.Out.WriteLine(a.countWords("ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEFGHIJ"));
			Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}
