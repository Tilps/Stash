using System;
using System.Collections;
using System.Text;

public class T
{
	public int m()
	{
		return 0;
	}
}

namespace SRM251Q3
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
			T a = new T();
			System.Console.Out.WriteLine(a.m());
			System.Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}
