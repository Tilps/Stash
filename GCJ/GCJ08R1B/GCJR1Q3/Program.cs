using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCJR1Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(args[0]);
            List<string> output = new List<string>();
            int cases = int.Parse(lines[0]);
            int index = 1;
            for (int i = 0; i < cases; i++)
            {
                // Parse
                int n = int.Parse(lines[index]);
                index++;
                string[] bits = lines[index].Split(' ');
                index++;
                int[] indices = new int[bits.Length - 1];
                for (int j = 1; j < bits.Length; j++)
                {
                    indices[j - 1] = int.Parse(bits[j]);
                }
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(n, indices)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(int n, int[] indicies)
        {
            List<int> record = new List<int>(n);
            int index = 1;
            int count = n;
            while (count > 0)
            {
                int shifts = index-1;
                shifts = shifts % count;
                record.Add(shifts);
                count--;
                index++;
            }
            // now we have the record, reconstruct the deck.
            Queue<int> deck = new Queue<int>(n);
            for (int i = n; i > 0; i--)
            {
                deck.Enqueue(i);
                for (int j = 0; j < record[i - 1]; j++)
                {
                    int move = deck.Dequeue();
                    deck.Enqueue(move);
                }
            }
            StringBuilder res = new StringBuilder();
            int[] deckArray = deck.ToArray();
            for (int i = 0; i < indicies.Length; i++)
            {
                if (i > 0)
                    res.Append(' ');
                res.Append(deckArray[deckArray.Length-indicies[i]].ToString());
            }
            return res.ToString();
        }
    }
}
