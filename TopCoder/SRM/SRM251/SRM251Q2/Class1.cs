using System;
using System.Collections;
using System.Text;

public class DominoesGame
{


	public int largestTotal(string[] doms)
	{
		ArrayList domList = new ArrayList();
		for (int i=0; i < doms.Length; i++) 
		{
			string[] vals = doms[i].Split(':');
			int[] nums = new int[2];
			nums[0] = int.Parse(vals[0]);
			nums[1] = int.Parse(vals[1]);
			domList.Add(nums);
		}
		int[][] ds = (int[][])domList.ToArray(typeof(int[]));
		bool[] doubs = new bool[ds.Length];
		int[] scores = new int[ds.Length];
		for (int i=0; i < doubs.Length; i++) 
		{
			doubs[i] = (ds[i][0] == ds[i][1]);
		}

		int length = 1<<(doms.Length);
		int[,,,,] largest = new int[length, 7, 7, 2, 2];
		int verybest = 0;

		for (int i=1; i < length; i++) 
		{
			for (int j=0; j <7; j++) 
			{
				for (int k=0; k < 7; k++) 
				{
					for (int l=0; l < 2; l++) 
					{
						for (int m=0; m < 2; m++) 
						{
							int best = 0;
							int index=1;
							for (int n=0; n < doms.Length; n++) 
							{
								if ((i & index) != 0) 
								{
									if (m==1 && !doubs[n])
										continue;
									if (m==0 && doubs[n])
										continue;
									int prior = (i & ~index);
									if (prior == 0) 
									{
										if (l==1 && !doubs[n])
											continue;
										if (l==0 && doubs[n])
											continue;
										if (doubs[n]) 
										{
											if (j != k)
												continue;
											if (j != ds[n][0])
												continue;
										}
										else 
										{
											if (j == k)
												continue;
											if (j != ds[n][0] && j != ds[n][1])
												continue;
											if (k != ds[n][0] && k != ds[n][1])
												continue;
										}
										int sc = ds[n][0] + ds[n][1];
										if (sc %5 == 0)
											best = ds[n][0]+ds[n][1];								
									}
									else 
									{
										if (doubs[n]) 
										{
											if (ds[n][0] != k)
												continue;
										}
										else 
										{
											if (k != ds[n][0] && k != ds[n][1])
												continue;
										}
										for (int b=0; b <2; b++) 
										{
											int pr =0;
											if (k == ds[n][0]) 
											{
												pr = largest[prior, j, ds[n][1], l, b];
											}
											else 
											{
												pr = largest[prior, j, ds[n][0], l, b];
											}
											int newsc = ds[n][0] * (doubs[n] ? 2 : 1) + j *((l==1) ? 2 : 1);
											if (newsc %5 == 0)
												pr += newsc;
											if (pr > best)
												best = pr;
										}
									}
								}
								index <<=1;
							}
							if (best > 0) 
							{
								largest[i,j,k,l,m] = best;
								if (best > verybest)
									verybest = best;
							}
						}
					}
				}
			}
		}


		return verybest;
	}
}

namespace SRM251Q2
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
			DominoesGame a = new DominoesGame();
			System.Console.Out.WriteLine(a.largestTotal(new string[] {"0:0","0:5","5:5"}));
			System.Console.In.ReadLine();
			//
			// TODO: Add code to start application here
			//
		}
	}
}
