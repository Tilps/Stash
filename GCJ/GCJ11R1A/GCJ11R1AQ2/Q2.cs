using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Numerics;
// http://www.themissingdocs.net/downloads/TMD.Algo.0.0.4.0.zip
using TMD.Algo.Algorithms;
using TMD.Algo.Collections.Generic;

namespace GCJ11R1AQ2
{
    class Q2
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(args[0]);
            List<string> output = new List<string>();
            int cases = int.Parse(lines[0]);
            int index = 1;
            for (int i = 0; i < cases; i++)
            {
                string[] bits = lines[index].Split(' ');
                int n = int.Parse(bits[0]);
                int m = int.Parse(bits[1]);
                List<string> words = new List<string>();
                List<string> orders = new List<string>();
                for (int j = 0; j < n; j++)
                {
                    index++;
                    words.Add(lines[index]);
                }
                for (int j = 0; j < m; j++)
                {
                    index++;
                    orders.Add(lines[index]);
                }
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(words, orders)));
                index++;
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private static string Solve(List<string> words, List<string> orders)
        {
            // For determining winner in case of equality.
            Dictionary<string, int> wordOrder = new Dictionary<string,int>();
            for (int i=0; i < words.Count; i++)
                wordOrder[words[i]] = i;
            // lookup pattern of word by letter.
            Dictionary<string, int>[] lookup = new Dictionary<string, int>[26];
            for (int j = 0; j < 26; j++)
            {
                lookup[j] = new Dictionary<string, int>();
                for (int i = 0; i < words.Count; i++)
                {
                    string word = words[i];
                    int pattern = 0;
                    for (int k = 0; k < word.Length; k++)
                    {
                        if (word[k] == (char)('a' + j))
                            pattern += 1 << k;
                    }
                    lookup[j][word] = pattern;
                }
            }
            Dictionary<int, List<string>> bySize = new Dictionary<int, List<string>>();
            for (int i = 0; i < words.Count; i++)
            {
                if (!bySize.ContainsKey(words[i].Length))
                    bySize[words[i].Length] = new List<string>();
                bySize[words[i].Length].Add(words[i]);
            }

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < orders.Count; i++)
            {
                string bestWord = words[0];
                int bestScore = 0;
                string order = orders[i];

                // For each possible length.
                for (int j = 1; j <= 10; j++)
                {
                    if (!bySize.ContainsKey(j))
                        continue;
                    // Score will be 0 if this is true, and if everything gives score 0 then first word is best.
                    if (bySize[j].Count == 1)
                        continue;
                    List<List<string>> cur = new List<List<string>>();
                    cur.Add(bySize[j]);
                    List<int> curScores = new List<int>();
                    curScores.Add(0);
                    for (int k = 0; k < 26; k++)
                    {
                        int charIndex = (int)(order[k] - 'a');
                        Dictionary<string, int> charLookup = lookup[charIndex];
                        List<List<string>> next = new List<List<string>>();
                        List<int> nextScores = new List<int>();
                        for (int l = 0; l < cur.Count; l++)
                        {
                            Dictionary<int, List<string>> nextParts = new Dictionary<int, List<string>>();
                            foreach (string w in cur[l])
                            {
                                int pattern = charLookup[w];
                                List<string> toAddTo;
                                if (!nextParts.TryGetValue(pattern, out toAddTo))
                                {
                                    toAddTo = new List<string>();
                                    nextParts[pattern] = toAddTo;
                                }
                                toAddTo.Add(w);
                            }
                            int curScore = curScores[l];
                            foreach (KeyValuePair<int, List<string>> kvp in nextParts)
                            {
                                int score = curScore;
                                if (kvp.Key == 0 && nextParts.Count > 1)
                                    score += 1;
                                if (kvp.Value.Count == 1)
                                {
                                    if (score > bestScore)
                                    {
                                        bestScore = score;
                                        bestWord = kvp.Value[0];
                                    }
                                    else if (score == bestScore && wordOrder[bestWord] > wordOrder[kvp.Value[0]])
                                    {
                                        bestWord = kvp.Value[0];
                                    }
                                }
                                else
                                {
                                    nextScores.Add(score);
                                    next.Add(kvp.Value);
                                }
                            }
                        }
                        cur = next;
                        curScores = nextScores;
                    }
                    // best score after n letters where bitmask has been revealed and there are still more solutions
                    // Trying to maximize misses - words that don't have the 
                }

                if (i != 0)
                    result.Append(' ');
                result.Append(bestWord);
            }
            return result.ToString();
        }

    }
}
