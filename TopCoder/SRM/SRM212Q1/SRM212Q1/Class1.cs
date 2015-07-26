using System;
using System.Collections;
using System.Text;


public class LargestCircle {

	public bool CanDraw(bool[,] taken, int x, int y, int r, int sizeX, int sizeY) {
		int rsq = r*r;
		for (double i=(double)-r+1.0/100.0;i<r; i+= 1.0/10.0) {
			// (i)^2 + (y-y1)^2 = r^2
			// i^2 + y^2-2y*y1+y1^2 = r^2
			// y = (y1+/- sqrt(r^2-i^2))
			double yDesc = Math.Sqrt(-i*i + rsq);
			double yI = y + yDesc;
			int yIi = (int)Math.Floor(yI);
			int xIi = (int)Math.Floor(x+i);
			if (taken[xIi, yIi])
				return false;
			yI = y - yDesc;
			yIi = (int)Math.Floor(yI);
			if (taken[xIi, yIi])
				return false;
		}
		return true;
	}

	public int radius(string[] grid) {
		bool[,] taken = new bool[grid.Length,grid[0].Length];
		for (int i=0; i<grid.Length;i++) {
			for (int j=0; j<grid[i].Length;j++) {
				if (grid[i][j] == '#')
					taken[i,j] = true;
			}
		}
		int best = 0;
		for (int i=1; i<grid.Length; i++) {
			for (int j=1; j<grid[0].Length; j++) {
				int maxRadius = Math.Min(i,j);
				maxRadius = Math.Min(maxRadius, grid.Length-i);
				maxRadius = Math.Min(maxRadius, grid[0].Length -j);
				if (maxRadius==4) {
					int a=0;
				}
				for (int k=1; k<=maxRadius; k++) {
					if (CanDraw(taken, i,j,k, grid.Length, grid[0].Length))
						if (k > best)
							best = k;
				}
			}
		}
		return best;
	}
}



	/// <summary>
	/// Summary description for Class1.
	/// </summary>
class Class1 {
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main(string[] args) {
		LargestCircle a = new LargestCircle();
		Console.Out.WriteLine("{0}", 
			a.radius(new string[] { "#####.......",
									  "#####.......",
									  "#####.......",
									  "............",
									  "............",
									  "............",
									  "............",
									  "............",
									  "............",
									  "............" })
			); 
		Console.In.ReadLine();
	}
}
