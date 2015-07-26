using System;
using System.Collections;
using System.Text;

public class ChatExit 
{

	private int[] find(int length, int[,] before, ArrayList sofar) 
	{
		if (sofar.Count == length)
			return (int[])sofar.ToArray(typeof(int));
		for (int i=0; i < length; i++) 
		{
			bool possible = true;
			bool all_afters = true;
			for (int j=0; j < sofar.Count; j++) 
			{
				if (i == (int)sofar[j]) 
				{
					possible = false;
					break;
				}
				if (before[i, (int)sofar[j]] >0) 
				{
					possible = false;
					break;
				}
			}
			if (!possible)
				continue;
			for (int j=0; j < length; j++) 
			{
				if (before[i, j] < 0)
				{
					bool contains = false;
					for (int k=0; k <sofar.Count; k++) 
					{
						if (j == (int)sofar[k]) 
						{
							contains = true;
							break;
						}
					}
					if (!contains) 
					{
						all_afters = false;
						break;
					}
				}
			}
			if (!all_afters)
				continue;
			sofar.Add(i);
			int[] res = find(length, before, sofar);
			if (res.Length != 0)
				return res;
			sofar.RemoveAt(sofar.Count-1);
		}
		return new int[0];
	}

	public int[] leaveOrder(string[] numSeen) 
	{
		int[,] seen = new int[numSeen.Length, numSeen.Length];
		for (int i=0; i < numSeen.Length;i++) 
		{
			string[] splits = numSeen[i].Split(' ');
			for (int j=0; j < splits.Length; j++) 
			{
				seen[i, j] = int.Parse(splits[j]);
			}
		}
		int[,] before = new int[numSeen.Length, numSeen.Length];
		for (int i=0; i < numSeen.Length; i++) 
		{
			for (int j=0; j < numSeen.Length; j++) 
			{
				if (i==j)
					continue;
				for (int k=j+1; k < numSeen.Length; k++)
				{
					if (i==k)
						continue;
					int diff = seen[j,i]-seen[k,i];
					if (diff > 0)
					{
						if (before[k,j] < 0)
							return new int[0];
						before[k,j] = 1;
						before[j,k] = -1;
					}
					else if (diff < 0)
					{
						if (before[j,k] > 0)
							return new int[0];
						before[j,k] = -1;
						before[k,j] = 1;
					}
				}
			}
		}
		ArrayList sofar = new ArrayList();
		int[] result = find(numSeen.Length, before, sofar);
		return result;
	}
}

namespace SRM249Q2
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
			StringBuilder builder = new StringBuilder();
			ChatExit t = new ChatExit();
			int[] res = t.leaveOrder(new string[] {
													  "0 1 1",
													  "4 0 0",
													  "3 1 0"
												  }
);
			for (int i=0; i < res.Length; i++) 
			{
				builder.Append(" ");
				builder.Append(res[i].ToString());
			}
			Console.Out.WriteLine(builder.ToString());
			Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}
