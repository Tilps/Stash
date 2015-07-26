using System;
using System.Collections;
using System.Text;


public class Diving {

	public double DELTA = 0.000001;

	public double calc (double[] scores, double difficulty) {
		double min = double.MaxValue;
		double max = double.MinValue;
		double sum = 0.0;
		for (int i=0; i<scores.Length;i++) {
			if (scores[i] < min)
				min = scores[i];
			if (scores[i] > max)
				max = scores[i];
			sum += scores[i];
		}
		sum = sum - min - max;
		return sum * difficulty;

	}

	public string needed(string difficulty, string need, string ratings) {
		double dif_num = double.Parse(difficulty);
		double need_num = double.Parse(need);

		string[] splits = ratings.Split(' ');

		double[] scores = new double[4];
		int index=0;
		for (int j=0; j<splits.Length; j++) {
			if (splits[j][0]=='?')
				continue;
			scores[index] = double.Parse(splits[j]);
			index++;
		}

		int i;
		for (i=100;i>=0;i-=5) {
			double[] fullscores = new double[5];
			for (int k=0;k<4;k++)
				fullscores[k] = scores[k];
			fullscores[4] = ((double)i)/10.0;
			double score = calc(fullscores, dif_num);
			if (score < need_num-DELTA)
				break;
		}
		if (i==100)
			return "-1.0";
		i+=5;
		int whole = i/10;
		int part = i % 10;
		return whole.ToString()+"."+part.ToString();
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
		Diving a = new Diving();
		Console.Out.WriteLine("{0}", 
			a.needed("3.2","50.32","5.5 5.5 10.0 ?.? 4.5")
			); 
		Console.In.ReadLine();
	}
}
