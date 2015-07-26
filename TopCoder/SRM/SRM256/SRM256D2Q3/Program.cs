using System;
using System.Collections;
using System.Text;

public class GraphLabel
{
    private int go(int[] nums, string[] adj, int depth)
    {
        if (depth == nums.Length)
        {
            int worst = int.MinValue;
            for (int i = 0; i < nums.Length; i++)
            {
                for (int j = 0; j < nums.Length; j++)
                {
                    if (adj[i][j] == '1')
                    {
                        int dif = nums[i] - nums[j];
                        dif = Math.Abs(dif);
                        if (dif > worst)
                        {
                            worst = dif;
                        }
                    }
                }
            }
            return worst;
        }
        else
        {
            int best = int.MaxValue;
            for (int i = 0; i < nums.Length; i++)
            {
                bool usedBefore = false;
                for (int j = 0; j < depth; j++)
                {
                    if (nums[j] == i)
                        usedBefore = true;
                }
                if (usedBefore)
                    continue;
                nums[depth] = i;
                int res = go(nums, adj, depth + 1);
                if (res < best)
                    best = res;
            }
            return best;
        }
    }

    public int adjacentDifference(string[] graph)
    {
        int[] nums = new int[graph.Length];
        return go(nums, graph, 0);
    }
}

namespace SRM256D2Q3
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
