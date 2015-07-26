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
			times[i]=2<<31;
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
