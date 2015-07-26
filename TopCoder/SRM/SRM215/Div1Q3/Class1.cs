using System;
using System.Collections;
using System.Text;


public class ShortCut {

	public double calcShortest(double[,] w, int last) {
		// start is 0 // end is last
		double[] arrivals = new double[last+1];
		for (int i=0; i<arrivals.Length;i++) {
			arrivals[i]= 1000000;
		}
		int[] froms = new int[last+1];
		double[] weights = new double[last+1];
		arrivals[0] = 0.0;
		froms[0] = -1;
		bool changing = true;
		while (changing) {
			changing = false;
			for (int i=0; i<arrivals.Length;i++) {
				for (int j=0; j<arrivals.Length;j++) {
					if (i==j)
						continue;
					double newDist = arrivals[i]+w[i,j];
					if (newDist < arrivals[j]) {
						arrivals[j] = newDist;
						froms[j] = i;
						weights[j] = w[i,j];
						changing = true;
					}
				}
			}
		}
		for (int i=0; i<froms.Length;i++) {
			// Debugging.
			Console.Out.WriteLine("{0}\t{1}\t{2}", i, froms[i], weights[i]);
		}
		return arrivals[last];
	}


	public double square(double a) {
		return a*a;
	}


	public double dist(double xdiff, double ydiff) {
		return Math.Sqrt(square(xdiff)+square(ydiff));
	}

	public double suvTime(int[] roadX, int[] roadY) {
		double[,] weights = new double[roadX.Length, roadX.Length];
		for (int i=0; i<roadX.Length;i++) {
			for (int j=0; j<roadX.Length;j++) {
				if (i==j)
					continue;
				weights[i,j] = dist(roadX[i]-roadX[j], roadY[i]-roadY[j]);

				if (j-i != 1) { // Not a road.
					weights[i,j] *= 2.0;
					// Maybe we can do better then the direct travel.
					// Theory, minimal travel is between end points and points on a road.
					// Never between one middle point on one road and another middle point on another road.
					// Therefore we try shortest for each road in between.
					for (int k=1;k<roadX.Length;k++) {
						// Road a,b -> c,d
						int a = roadX[k-1];
						int b = roadY[k-1];
						int c = roadX[k];
						int d = roadY[k];

						// Starting point e1,f1
						int e1 = roadX[i];
						int f1 = roadY[i];
						// Ending point e2,f2
						int e2 = roadX[j];
						int f2 = roadY[j];

						// Road xdiff
						int ix=c-a;
						// Road ydiff
						int jy=d-b;
						// Convienience differences from the algebra.
						int k1=e1-a;
						int l1=f1-b;
						int k2=e2-a;
						int l2=f2-b;

						// First calculate the location on the infinite extension of the road which is closest to each point.
						// derivative of sqrt((it-k)^2+(jt+l)^2)) = 0 - solve for t.
						int num1 = (ix*k1+jy*l1);
						int num2 = (ix*k2+jy*l2);
						// Length of road squared.
						int den = ix*ix+jy*jy;

						double tperp1 = (double)num1/(double)den;
						double tperp2 = (double)num2/(double)den;
						// find closest point locations (perpendicular intersections).
						double xperp1 = ix*tperp1+a;
						double xperp2 = ix*tperp2+a;
						double yperp1 = jy*tperp1+b;
						double yperp2 = jy*tperp2+b;

						// Length of perpendicular lines.
						double dperp1 = dist(e1-xperp1, f1-yperp1);
						double dperp2 = dist(e2-xperp2, f2-yperp2);

						// Now - minimal distance is where b (aditional distance along the line from perp point) minimises 2*sqrt(dperp^2+b^2)-b
						// derivative is 0 at b = dperp/sqrt(3) - assuming b is short of the end point, but that case is taken care of further down.
						// This is an additional t of b/length of road - b/sqrt(den);
						double t1 = tperp1 + dperp1/Math.Sqrt(3)/Math.Sqrt(den);
						// because we are leaving the road to get to e2,f2 rather then arriving, subtraction is called for.
						double t2 = tperp2 - dperp2/Math.Sqrt(3)/Math.Sqrt(den);

						// Bring the points back to end points if they are exterior to the road.
						if (t1 < 0.0) t1 = 0.0;
						if (t2 < 0.0) t2 = 0.0;
						if (t1 > 1.0) t1 = 1.0;
						if (t2 > 1.0) t2 = 1.0;

						// Going backwards on the road is worse then just going straight from point i to point j.
						if (t1 > t2)
							continue;
						// Positions of road points.
						double x1 = ix*t1+a;
						double x2 = ix*t2+a;
						double y1 = jy*t1+b;
						double y2 = jy*t2+b;

						// Total distance.
						double minWeight = 2*dist(e1-x1, f1-y1) + dist(x1-x2,y1-y2) + 2*dist(e2-x2, f2-y2);

						// Check minimum.
						if (minWeight < weights[i,j])
							weights[i,j] = minWeight;
					}
					
				}
			}
		}

		// Calculate normal length.
		double normalLength = 0.0;
		for (int i=0; i<roadX.Length-1;i++) {
			normalLength += weights[i,i+1];
		}

		Console.Out.WriteLine(normalLength);

		// Run standard shortest path algorithm with the new special weights.
		double shortestLength = calcShortest(weights, roadX.Length-1);
		Console.Out.WriteLine(shortestLength);

		// return ratio.
		return shortestLength/normalLength;
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
		ShortCut a = new ShortCut();
		Console.Out.WriteLine("{0}", 
			a.suvTime(new int[]{0, 10, 10, -40, -40, 5, 5, 150, 150, 160, 160, 150, 150}
					, new int[] {0, 0, -10, -10, 50, 5, 50, 50, 60, 60, -10, -10, 0})
			); 
		Console.In.ReadLine();
	}
}
