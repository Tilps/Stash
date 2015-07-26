using System;
using System.Collections;
using System.Text;

public class SuperString
{
    public string goodnessSubstring(string[] superstring)
    {
        string full = string.Concat(superstring);
        string best = string.Empty;
        int bestGood = 0;
        for (int i = 0; i < full.Length; i++)
        {
            int[] scores = new int[26];
            for (int j = i; j < full.Length; j++)
            {
                scores[full[j]-'A']++;
                int total = 0;
                for (int k = 0; k < scores.Length; k++)
                {
                    if (scores[k] == 1)
                        total++;
                }
                if (total > bestGood)
                {
                    bestGood = total;
                    best = full.Substring(i, j - i + 1);
                }
                else if (total == bestGood)
                {
                    string next = full.Substring(i, j - i + 1);
                    if (string.Compare(next, best) < 0)
                        best = next;
                }
            }
        }
        return best;
    }
}

namespace SRM259Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            SuperString c = new SuperString();
            object o = c.goodnessSubstring(new string[] { });
            if (o is IEnumerable)
            {
                foreach (object b in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(b);
                }
            }
            else
                System.Console.Out.WriteLine(o);
            System.Console.In.ReadLine();
        }
    }
}
