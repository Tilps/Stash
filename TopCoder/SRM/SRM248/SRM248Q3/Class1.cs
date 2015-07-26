using System;

public class T 
{
	public int meth(int a)
	{
		return 0;
	}
}

namespace SRM248Q3
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
			Console.Out.WriteLine(a.meth(2));
			Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}
