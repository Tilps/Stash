using System;
using System.Collections;
using System.Text;


public class HeatDeath {
	public int maxTime(int[] energy) {
		Array.Sort(energy);
		int time=0;
		while (true) {
			bool changed=false;
			for (int diff=1; diff < energy.Length; diff++) {
				for (int i=energy.Length-1; i>=diff; i--) {
					if (energy[i] >= energy[i-diff]+2) {
						energy[i]--;
						energy[i-diff]++;
						changed=true;
						break;
					}
				}
				if (changed)
					break;
			}
			if (!changed)
				break;
			time++;
		}
		return time;
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
			//
			// TODO: Add code to start application here
			//
		}
	}
