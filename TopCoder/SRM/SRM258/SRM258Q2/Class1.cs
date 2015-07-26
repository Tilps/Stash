using System;
using System.Collections;
using System.Text;

public class CompressionText
{

	private int apply(string k, string k2, string orig) 
	{
        int res = orig.Length;
        for (int i = 0; i < orig.Length-2; i++)
        {
            if (orig.Substring(i, 3) == k)
            {
                i += 2;
                res -= 1;
            }
            else if (orig.Substring(i, 3) == k2)
            {
                i += 2;
                res -= 1;
            }
        }
		return res;
	}

	public int shortestLength(string orig)
	{
		Hashtable seq = new Hashtable();
		for (int i=0; i < orig.Length -2; i++) 
		{
			seq[orig.Substring(i, 3)] = true;
		}
		int bestLength = orig.Length;
		foreach (string key in seq.Keys) 
		{
			foreach (string key2 in seq.Keys) 
			{
				int length = apply(key, key2, orig);
				if (bestLength > length)
					bestLength = length;
			}
		}
		return bestLength;
	}
}

namespace SRM258Q2
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
			CompressionText a = new CompressionText();
			System.Console.Out.WriteLine(a.shortestLength("AAA"));
			System.Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}