using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class EllysJuice
{
    public string[] getWinners(string[] players)
    {
        List<string> distinctPlayers = new List<string>();
        List<int> distinctMap = new List<int>();
        for (int i = 0; i < players.Length; i++)
        {
            if (!distinctPlayers.Contains(players[i]))
            {
                distinctPlayers.Add(players[i]);
                distinctMap.Add(distinctPlayers.Count - 1);
            }
            else
            {
                distinctMap.Add(distinctPlayers.IndexOf(players[i]));
            }
        }
        List<string> winnersFound = new List<string>();
        int[] scores = new int[distinctPlayers.Count];
        bool[] seen = new bool[players.Length];
        Recurse(0, distinctPlayers, scores, winnersFound, seen, 1024, players, distinctMap);
        winnersFound.Sort();
        return winnersFound.ToArray();
    }

    private void Recurse(int depth, List<string> distinctPlayers, int[] scores, List<string> winnersFound, bool[] seen, int score, string[] players, List<int> distinctMap)
    {
        if (depth == players.Length)
        {
            int max = 0;
            int pos = -1;
            for (int i = 0; i < scores.Length; i++)
            {
                if (scores[i] > max)
                {
                    max = scores[i];
                    pos = i;
                }
                else if (scores[i] == max && pos != i)
                    return;
            }
            if (winnersFound.Contains(distinctPlayers[pos]))
                return;
            winnersFound.Add(distinctPlayers[pos]);
            return;
        }
        for (int i = 0; i < players.Length; i++)
        {
            if (seen[i])
                continue;
            scores[distinctMap[i]] += score;
            seen[i] = true;
            int nextScore = score;
            if (depth % 2 == 1)
                nextScore = score / 2;
            Recurse(depth + 1, distinctPlayers, scores, winnersFound, seen, nextScore, players, distinctMap);
            seen[i] = false;
            scores[distinctMap[i]] -= score;
        }
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            EllysJuice c = new EllysJuice();
            object o = c.getWinners(new string[] { "elly", "kriss", "stancho", "elly", "stancho" });
            PrintObj(o);
            System.Console.In.ReadLine();
        }

        private static void PrintObj(object o)
        {
            if (o is IEnumerable)
            {
                foreach (object b in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(b);
                }
            }
            else
                System.Console.Out.WriteLine(o);
        }
    }
}
