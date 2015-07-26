using System;
using System.Collections;
using System.Text;

public class SMS
{
	private bool isvowel(char a) 
	{
		if ("aeiou".IndexOf(a) != -1)
			return true;
		return false;
	}
	public string compress(string orig)
	{
		StringBuilder newStr = new StringBuilder();
		for (int i=0; i < orig.Length; i++) 
		{
			if (orig[i] == ' ' || !isvowel(orig.ToLower()[i])) 
			{
				newStr.Append(orig[i]);
				continue;
			}
			bool onlyvowelsA=true;
			for (int j=i-1;j>=0; j--) 
			{
				if (orig[j] == ' ')
					break;
				if (!isvowel(orig.ToLower()[j])) 
				{
					onlyvowelsA=false;
				}
			}
			bool onlyvowelsB=true;
			for (int j=i+1;j<orig.Length; j++) 
			{
				if (orig[j] == ' ')
					break;
				if (!isvowel(orig.ToLower()[j])) 
				{
					onlyvowelsB=false;
				}
			}
			if (onlyvowelsA || onlyvowelsB)
				newStr.Append(orig[i]);
		}
		return newStr.ToString();
	}
}

namespace SRM251Q1
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

			SMS a = new SMS();
			System.Console.Out.WriteLine(a.compress(" I  like your   style "));
			System.Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}
