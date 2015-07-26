using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class GreedyTravelingSalesman
{
    public int worstDistance(string[] thousands, string[] hundreds, string[] tens, string[] ones)
    {
        int[,] distances = new int[thousands.Length, thousands.Length];
        for (int i = 0; i < thousands.Length; i++)
        {
            for (int j = 0; j < thousands.Length; j++)
            {
                distances[i, j] = (int)(thousands[i][j] - '0') * 1000 +
                    (int)(hundreds[i][j] - '0') * 100 +
                    (int)(tens[i][j] - '0') * 10 +
                    (int)(ones[i][j] - '0');
            }
        }
        bool[] visited = new bool[thousands.Length];
        int worst = 0;
        for (int i = 8; i < thousands.Length; i++)
        {
            for (int j = 9; j < thousands.Length; j++)
            {
                if (i == j)
                    continue;
                for (int k = 1; k < 2; k++)
                {
                    // either make it the choosen option by reduction or increase to maximum.
                    Array.Clear(visited, 0, visited.Length);
                    visited[0] = true;
                    int current = 0;
                    int total=0;
                    for (int l = 0; l < thousands.Length-1; l++)
                    {
                        int bestOption = -1;
                        int bestLength = int.MaxValue;
                        int optionsCount = 0;
                        for (int m = 0; m < thousands.Length; m++)
                        {
                            if (m==current)
                                continue;
                            if (visited[m])
                                continue;
                            optionsCount++;
                            if (distances[current,m] < bestLength)
                            {
                                bestOption = m;
                                bestLength = distances[current,m];
                            }
                        }
                        if (current == i)
                        {
                            if (k == 0 && !visited[j])
                            {
                                if (j == bestOption)
                                {
                                    if (optionsCount == 1)
                                        bestLength = 9999;
                                    else
                                    {
                                        int bestOption2 = -1;
                                        int bestLength2 = int.MaxValue;
                                        for (int m = 0; m < thousands.Length; m++)
                                        {
                                            if (m == current)
                                                continue;
                                            if (m == bestOption)
                                                continue;
                                            if (visited[m])
                                                continue;
                                            if (distances[current, m] < bestLength2)
                                            {
                                                bestOption2 = m;
                                                bestLength2 = distances[current, m];
                                            }
                                        }
                                        if (j > bestOption2 && bestLength2 > 1)
                                        {
                                            bestLength = bestLength2 - 1;
                                        }
                                        else
                                            bestLength = bestLength2;
                                    }
                                }
                                else if (j > bestOption)
                                {
                                    if (bestLength > 1)
                                    {
                                        bestLength--;
                                        bestOption = j;
                                    }
                                }
                                else
                                    bestOption = j;

                            }
                            if (k == 1 && !visited[j] && bestOption == j)
                            {
                                if (optionsCount == 1)
                                {
                                    bestLength = 9999;
                                }
                                else if (bestLength != 9999)
                                {
                                    int bestOption2 = -1;
                                    int bestLength2 = int.MaxValue;
                                    for (int m = 0; m < thousands.Length; m++)
                                    {
                                        if (m == current)
                                            continue;
                                        if (m == bestOption)
                                            continue;
                                        if (visited[m])
                                            continue;
                                        if (distances[current, m] < bestLength2)
                                        {
                                            bestOption2 = m;
                                            bestLength2 = distances[current, m];
                                        }
                                    }
                                    // bug, should be wrapped with if (bestLength2 < 9999 || bestOption2 < bestOption) - otherwise bestLength should just be set to 9999, bestOption can't change.
                                    bestOption = bestOption2;
                                    bestLength = bestLength2;
                                }
                            }
                        }
                        current = bestOption;
                        total += bestLength;
                        visited[current] = true;
                    }
                    if (total > worst)
                        worst = total;
                }
            }
        }


        return worst;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            GreedyTravelingSalesman c = new GreedyTravelingSalesman();
            object o = c.worstDistance(new string[] {"003002080800", "909905560957", "580086426836", "008069599679", "962000499056", "961090009747", "000003049009", "800409008830", "790029970090", "300091080004", "009890050109", "040009910000"}, new string[] {"037001090509", "909903700916", "130082701110", "008069499841", "906000639004", "543010001822", "200000019009", "508609003850", "890019990090", "200090030005", "009991080109", "060009990800"}, new string[] {"032002050006", "909909740928", "170034820461", "004079899383", "931000139011", "035030004575", "200008029009", "703109008730", "790049990090", "200093030000", "009292040709", "070009980900"}, new string[] {"072119111715", "909914771937", "720178488077", "119049699123", "925101299184", "953140112390", "311118099119", "014619103011", "191159900191", "911193101017", "119190161309", "191119941710"});
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
