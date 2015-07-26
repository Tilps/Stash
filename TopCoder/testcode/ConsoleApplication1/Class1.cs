using System;

namespace ConsoleApplication1
{
	using System;

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class MovingPath {
		public MovingPath() {
		}

		public int cur;

		public void InitWeights(int[][] weights, int nodes, int a, int b, int m) {
			for (int i = 0; i <nodes; i++) {
				for (int j=0; j<nodes; j++) {
					if(i != j) {
						cur = ((cur * a) + b) % m;
						weights[i][j] = (cur % 1000) + 1;
					}
					else
						weights[i][j] = 1;
				}
			}
		}

		public void StepWeights(int[][] weights, int nodes, int a, int b, int m) {
			for (int i = 0; i <nodes; i++) {
				for (int j=0; j<nodes; j++) {
					if(i != j) {
						cur = ((cur * a) + b) % m;
						weights[i][j] += (cur % 21) - 10;
						if (weights[i][j] <= 0)
							weights[i][j] = 1;
					}
				}
			}
		}

		public int shortest (int nodes, int u, int v, int a, int b, int m) {
			int[][] weights = new int[nodes][];

			cur=1;

			for (int i=0;i<nodes;i++) {
				weights[i]  = new int[nodes];
			}

			int time=0;
			int[] times = new int[nodes];
			for (int i=0; i<nodes; i++) {
				times[i]=(1<<29);
			}

			InitWeights(weights, nodes, a, b, m);
			times[u] = time;
			while (true) {
				for (int i=0;i<nodes;i++) {
					if (times[i] == time) {
						if (i==v)
							return time;
						for (int j=0; j<nodes;j++) {
							if (weights[i][j]+time < times[j])
								times[j] = weights[i][j]+time;
						}
						times[i]++;
					}
				}
				StepWeights(weights, nodes, a, b, m);
				time++;
			}


		}
	}


	public class CellTower {
		public int best(string[] towers, int x, int y) {
			double[] x2 = new double[towers.Length];
			double[] y2 = new double[towers.Length];

			for (int i=0; i< towers.Length; i++) {
				towers[i] = towers[i].Substring(1, towers[i].Length-2);
				string[] coords = towers[i].Split(',');
				x2[i] = double.Parse(coords[0]);
				y2[i] = double.Parse(coords[1]);
			}
			double[] dist = new double[towers.Length];
			for (int i=0; i<towers.Length; i++) {
				dist[i] = Math.Sqrt((x-x2[i])*(x-x2[i]) + (y-y2[i])*(y-y2[i]));
			}
			double mindist = -1;
			for (int i=0; i<towers.Length; i++) {
				if (mindist < 0 || mindist > dist[i])
					mindist = dist[i];
			}
			for (int i=0; i<towers.Length; i++) {
				if (dist[i] <= mindist+2.0)
					return i;
			}
			return -1;
		}
	}


	public class Resistors {
		private double switchType(ref int index, string[] resistors) {
			if (resistors[index] == "P")
				return doParallel(ref index, resistors);
			else if (resistors[index] == "S")
				return doSeries(ref index, resistors);
			else
				return doValue(ref index, resistors);
		}


		private double doParallel(ref int index, string[] resistors) {
			index++;
			double v1 = switchType(ref index, resistors);
			double v2 = switchType(ref index, resistors);
			if (v1 == 0.0 || v2 == 0.0)
				return 0.0;
			return 1.0/((1.0/v1)+(1.0/v2));
		}


		private double doSeries(ref int index, string[] resistors) {
			index++;
			double v1 = switchType(ref index, resistors);
			double v2 = switchType(ref index, resistors);
			return v1+v2;
		}


		private double doValue(ref int index, string[] resistors) {
			index++;
			return double.Parse(resistors[index]);
		}


		public double getResistance(string[] resistors) {
			int index = 0;
			return switchType(ref index, resistors);
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
			Resistors solver = new Resistors();
			Console.Out.WriteLine("{0}", solver.getResistance(new string[]{"S","5.3","P","40","60"}));
			Console.In.ReadLine();
		}
	}
}
