using System;
using System.Collections;
using System.Text;

public class StockQuotes
{
	private Exchange[] exchanges = new Exchange[11];

	private class Exchange 
	{
		public int name;
		public int bid;
		public int ask;
		public int changeCount;
		public int cumulativeSpread;
	}

	public string[] report(string[] quotes)
	{

		for (int i=0; i < quotes.Length; i++) 
		{
			string[] spl = quotes[i].Split(' ');
			int name = int.Parse(spl[0]);
			int bid = int.Parse(spl[1]);
			int ask = int.Parse(spl[2]);
			if (exchanges[name] == null) 
			{
				exchanges[name] = new Exchange();
				exchanges[name].name = name;
				exchanges[name].bid = bid;
				exchanges[name].ask = ask;
				exchanges[name].changeCount = 1;
				exchanges[name].cumulativeSpread = ask-bid;
			}
			else 
			{
				exchanges[name].bid = bid;
				exchanges[name].ask = ask;
				exchanges[name].changeCount += 1;
				exchanges[name].cumulativeSpread += ask-bid;
			}
			if (exchanges[10] == null) 
			{
				exchanges[10] = new Exchange();
				exchanges[10].name = 10;
				exchanges[10].bid = bid;
				exchanges[10].ask = ask;
				exchanges[10].changeCount = 1;
				exchanges[10].cumulativeSpread = ask-bid;
			}
			else 
			{
				int maxBid = int.MinValue;
				int minAsk = int.MaxValue;
				for (int j=0; j<10; j++) 
				{
					if (exchanges[j] != null) 
					{
						if (maxBid < exchanges[j].bid)
							maxBid = exchanges[j].bid;
						if (minAsk > exchanges[j].ask)
							minAsk = exchanges[j].ask;
					}
				}
				if (maxBid != exchanges[10].bid || minAsk != exchanges[10].ask) 
				{
					exchanges[10].bid = maxBid;
					exchanges[10].ask = minAsk;
					exchanges[10].changeCount +=1;
					exchanges[10].cumulativeSpread += minAsk-maxBid;
				}
			}

		}

		ArrayList list = new ArrayList();
		for (int i=0; i < 11; i++) 
		{
			if (exchanges[i] != null) 
			{
				list.Add(i.ToString() + " " + exchanges[i].changeCount + " " + ((double)exchanges[i].cumulativeSpread/(double)exchanges[i].changeCount).ToString("0.00"));
			}
		}
		return (string[])list.ToArray(typeof(string));
	}
}

namespace SRM257Q2
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
			StockQuotes a = new StockQuotes();
			System.Console.Out.WriteLine(a.report(new string[]{}));
			System.Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}