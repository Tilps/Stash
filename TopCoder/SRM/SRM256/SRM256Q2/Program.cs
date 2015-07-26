using System;
using System.Collections;
using System.Text;

public class CliqueCount
{
    public int countCliques(string[] graph)
    {
        bool[] cliques = new bool[1<<graph.Length];
        int total = 0;
        for (int i = cliques.Length-1; i > 0; i--)
        {
            bool clique = true;
            bool maximal = true;
            for (int j = 0; j < graph.Length; j++)
            {
                if ((i & (1 << j)) == 0)
                {
                    if (cliques[i | (1 << j)])
                    {
                        maximal = false;
                        break;
                    }
                }
            }
            for (int j = 0; j < graph.Length-1; j++)
            {
                if ((i & (1 << j)) == (1 << j))
                {
                    for (int k = j + 1; k < graph.Length; k++)
                    {
                        if ((i & (1 << k)) == (1 << k))
                        {
                            if (graph[j][k] == '0')
                            {
                                clique = false;
                                break;
                            }
                        }
                    }
                }
                if (!clique)
                    break;
            }
            if (clique)
            {
                cliques[i] = true;
                if (maximal)
                    total++;
            }
        }
        return total;
    }
}

namespace SRM256Q2
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
