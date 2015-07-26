using System;
using System.Collections;
using System.Text;

public class PermutationSum
{
    Hashtable res;
    private void Permute(int[] dig, ArrayList chosen)
    {
        if (chosen.Count == dig.Length) {
            int num = 0;
            int baseNum = 1;
            for (int i=0; i < chosen.Count; i++) {
                num += dig[(int)chosen[i]]*baseNum;
                baseNum *= 10;
            }
            if (!res.ContainsKey(num))
                res.Add(num, true);
            return;
        }
        for (int i = 0; i < dig.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < chosen.Count; j++)
            {
                if ((int)chosen[j] == i)
                    found = true;
            }
            if (found)
                continue;
            chosen.Add(i);
            Permute(dig, chosen);
            chosen.RemoveAt(chosen.Count - 1);
        }
    }
    public int add(int n)
    {
        res = new Hashtable();
        ArrayList digits = new ArrayList();
        while (n > 0)
        {
            int a = n % 10;
            n = n / 10;
            digits.Add(a);
        }
        int[] dig = (int[])digits.ToArray(typeof(int));
        ArrayList chosen = new ArrayList();
        Permute(dig, chosen);
        int total = 0;
        foreach (DictionaryEntry de in res)
        {
            total += (int)de.Key;
        }
        return total;
    }
}

namespace SRM252Q1
{
    class Program
    {
        static void Main(string[] args)
        {
            PermutationSum c = new PermutationSum();
            System.Console.Out.WriteLine(c.add(99999));
            System.Console.In.ReadLine();
        }
    }
}
