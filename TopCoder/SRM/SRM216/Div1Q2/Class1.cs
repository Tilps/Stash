using System;
using System.Collections;

public class Refactoring {

	int[][] factorsStack;
	int[][] primesStack;

	private int countFactors(int depth) {
		int[] factors = factorsStack[depth];
		int[] primes = primesStack[depth];
		int fl= factors.Length;
		if (fl==2) {
			return 1;
		}
		/*for (int i=0; i<factors.Length;i++) {
		 Console.Out.Write(factors[i]);
		 Console.Out.Write(" ");
		}
		Console.Out.WriteLine();*/
		int total = 1;
		int largestComp = 0;
		int fi;
		for (int i=fl-1; i>=0;i--) {
			fi=factors[i];
			if (fi!=primes[i]) {
				largestComp = fi;
				break;
			}
		}
		int lfi=0;
		for (int i=0; i<fl-1;lfi=fi,i++) {
			fi=factors[i];
			if (fi==lfi)
				continue;
			int pi=primes[i];
			int lfj=0;
			int fj;
			for (int j=i+1;j<fl;lfj=fj,j++) {
				fj=factors[j];
				if (fj==lfj)
					continue;
				int pj=primes[j];
				if (pi==fi || pj==fj) {
					if (fj!=pj) {
						if (pj > fi) {
							continue;
						}
					}
					int prod = fi*fj;
					if (prod < largestComp)
						continue;
					int[] newFactors = factorsStack[depth+1];
					int[] newPrimes = primesStack[depth+1];
					int index=0;
					bool done=false;
					for (int k=0; k<fl;k++) {
						if (k==i||k==j)
							continue;
						int fk = factors[k];
						if (!done && fk>prod) {
							newFactors[index] = prod;
							newPrimes[index] = Math.Max(pi, pj);
							done = true;
							index++;
						}
						newFactors[index] = fk;
						newPrimes[index] = primes[k];
						index++;
					}
					if (!done) {
						newFactors[fl-2] = prod;
						newPrimes[fl-2] = Math.Max(pi, pj);
					}
					total += countFactors(depth+1);
				}
			}
		}
		return total;
	}

	private int doit(int n, int from) {
		int ret=0;
		for (int i=from;i*i<=n;i++) {
			if (n%i==0) ret+=doit(n/i,i);
		}
		return ret+1;
	}

	public int refactor(int n) {
		// stolen faster way. - ~2 times speed of mine - which is Just enough to get it past the systests.
		// There is a dynamic programing way too, which is n^2 in factor combinations - which I would of thought too slow.
		//return doit(n, 2)-1;
		ArrayList factors = new ArrayList();
		while (n%2==0) {
			n /= 2;
			factors.Add(2);
		}
		for (int i=3;i*i<=n;i+=2) {
			while (n%i==0) {
				n/=i;
				factors.Add(i);
			}
		}
		if (n!=1) {
			factors.Add(n);
		}
		if (factors.Count==1)
			return 0;

		primesStack = new int[factors.Count][];
		factorsStack = new int[factors.Count][];
		for (int i=0;i<factors.Count;i++) {
			primesStack[i] = new int[factors.Count-i];
			factorsStack[i] = new int[factors.Count-i];
		}

		int[] primes = primesStack[0];
		factorsStack[0] = (int[])factors.ToArray(typeof(int));
		for (int i=0; i<primes.Length;i++) {
			primes[i] = factorsStack[0][i];
		}
		return countFactors(0);
	}
}
namespace Div1Q2
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
			Console.Out.WriteLine(new Refactoring().refactor(1916006400));
			Console.In.ReadLine();
		}
	}
}
