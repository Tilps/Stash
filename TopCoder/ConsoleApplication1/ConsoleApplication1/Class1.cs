

using System;
using System.Collections;

public class LeaderProduct{

	public int[] pricesSplit(string priceList) {
		string[] prices = priceList.Split(' ');
		int[] priceVals = new int[prices.Length];
		for (int i=0; i<priceVals.Length;i++) {
			priceVals[i] = int.Parse(prices[i]);
		}
		return priceVals;
	}

	public int[] getLeader(string[] prices, int[] sales) {
		int prodCount = pricesSplit(prices[0]).Length;
		int[,] allPrices = new int[sales.Length, prodCount];
		for (int i=0; i<sales.Length;i++) {
			int[] priceSplits = pricesSplit(prices[i]);
			for (int j=0;j<priceSplits.Length;j++) {
				allPrices[i,j] = priceSplits[j];
			}
		}

		ArrayList results = new ArrayList();
		for (int i=0; i<prodCount;i++) {
			bool exitLoops=false;
			for (int j=0;!exitLoops&&j<sales.Length-1;j++) {
				for (int k=j+1;!exitLoops&&k<sales.Length;k++) {
					if (j==k)
						continue;
					if (allPrices[j,i]==allPrices[k,i])
						continue;
					if (sales[j]==sales[k])
						continue;
					bool cheaperBefore = allPrices[j,i]<allPrices[k,i];
					bool higherSalesBefore = sales[j]>sales[k];
					if (cheaperBefore != higherSalesBefore) {
						exitLoops=true;
						break;
					}
				}
			}
			if (!exitLoops)
				results.Add(i);
		}

		return (int[])results.ToArray(typeof(int));
	}
	
}



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
			LeaderProduct temp = new LeaderProduct();

			Console.Out.WriteLine("{0}", temp.getLeader(new string[] {"5 10 76 48","4 9 49 50","4 5 67 61"}, new int[]{184,305,1945}));
			Console.In.ReadLine();
		}
	}

