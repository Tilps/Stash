﻿using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class ScotlandYard
{
    public int maxMoves(string[] taxi, string[] bus, string[] metro)
    {
        cities = taxi.Length;
        taxiTravel = new long[cities];
        busTravel = new long[cities];
        metroTravel = new long[cities];
        for (int i = 0; i < cities; i++)
        {
            for (int j = 0; j < taxi[i].Length; j++)
                if (taxi[i][j] == 'Y')
                    taxiTravel[i] |= (1L << j);
            for (int j = 0; j < bus[i].Length; j++)
                if (bus[i][j] == 'Y')
                    busTravel[i] |= (1L << j);
            for (int j = 0; j < metro[i].Length; j++)
                if (metro[i][j] == 'Y')
                    metroTravel[i] |= (1L << j);
        }
        for (int i = 0; i < cities; i++)
            dists[(1L << i)] = 0;
        long loc = (1L << cities) - 1L;
        return Search(loc);
    }

    private int cities;
    long[] taxiTravel;
    long[] busTravel;
    long[] metroTravel;
    Dictionary<long, int> dists = new Dictionary<long, int>();

    private int Search(long loc)
    {
        int result;
        if (dists.TryGetValue(loc, out result))
            return result;
        dists[loc] = -1;
        int best = 0;
        long newState = Map(loc, taxiTravel);
        if (newState != 0)
        {
            int next = Search(newState) + 1;
            if (next == 0)
                return -1;
            if (next > best)
                best = next;
        }
        newState = Map(loc, busTravel);
        if (newState != 0)
        {
            int next = Search(newState) + 1;
            if (next == 0)
                return -1;
            if (next > best)
                best = next;
        }
        newState = Map(loc, metroTravel);
        if (newState != 0)
        {
            int next = Search(newState) + 1;
            if (next == 0)
                return -1;
            if (next > best)
                best = next;
        }
        dists[loc] = best;
        return best;
    }

    private long Map(long loc, long[] travel)
    {
        long accumulate = 0;
        for (int i = 0; i < cities; i++)
        {
            if ((loc & (1L << i)) != 0)
                accumulate |= travel[i];
        }
        return accumulate;
    }
}

namespace Q3
{
    class Q3
    {
        static void Main(string[] args)
        {
            ScotlandYard c = new ScotlandYard();
            object o = c.maxMoves(
                new string[]
                    {
                        "NYYNYNYYNYYYYYYNNNYYYYNYYYYYYNNNYYNYNYNYNYYYNYNNYN",
                    "NNYNNYNYNNNNYYNNYYNYYNYNYNNNNNYNYNYNYYYYYNNYNNYNNY",
                    "NNNYNYYNYYNNNNYNYYNNNYYNNYNYYYYYNNNYYYYNNYNNYYYNNN",
                    "NNYNNYNNYNYNYYYYYYYYYNNNYYNNYYYNYYNYNYNNYYYYNNNYNN",
                    "NNNYNYNYYYYYYNNYYYYNYNYYYNYYNYNNNYNYNNYYNNNYYYYYNN",
                    "NNYYYNYYNYNNNNYNYYNYNNYNYYYNNYYYYNNNNYNNYNNYYNNYNN",
                    "YYNYNYNYYNYYNNNYYNYYYNYNNNYYYNNNNYYNNYNNYNNYNNYNYN",
                    "YYYYNYNNYNYNYYNYYNNNYNYNYNNYNNNNYNYYNNNYYNYNNYYYNN",
                    "NNNNYYYNNYNNYYNNNNYNNNYNYYNNYNNYNYYNNYYNNYYNNNNYYY",
                    "YYYYYYYYNNYNYNNYNYYNYYYNNNNNNYNNNNNNNNYNYYNNYNYNYN",
                    "NNNNNNYNNYNNYYYYYYNYNNNNYYNNYNNNNNNYNNYNNNNYYNYYYY",
                    "YNNYYNYYNYYNNNYYNYYYNNYYYYNNYNNNYYNNYYYNYYNYNYYYYN",
                    "YYYNNNYNYNYNNNNNYNYYYYYYNYNYYYNYYNNYNNYYNNNYNYYNNY",
                    "YNNNYNNYNYNYYNYNNNYYNYYNNNYYNNYYYNNNNYYYNNYYNNNYNN",
                    "NNYYNNYNNNYNNYNNYNYYYNYYNYNNYNYYNNYYNNNYYNYNYYYYYY",
                    "YYNYYYNYNNNNYNYNNYNYYYNYNNNNYNNNNNYNNNNYYNNYYYNNYY",
                    "YYNNYYNNNYNYNYNNNNNYNNNNNNNYNYNNYYYNYYNYNYYYNNNYYN",
                    "NYYYNNNNYNNYNNNNNNYNNNNYNYNYYYNYNNYYNNNYYNNNNYNNNY",
                    "NYYNYYNYNNNYYYNYYYNNNYYNYNNNNNNYYNNYYNNYYYNYNNYYNN",
                    "YNNYNNNNNNNNYYYNNNNNYYYNNNYNYNNNYNNYNYYYYYYNNNNYNN",
                    "NNYYYNYNYNNYNYYNNYNNNNYNYYNYYNYNYNYNNYNNNYNYNYYNYY",
                    "NYYYNYNYNYYNYNYYYYNYNNNYYNNYNYYNYYNYNNNNNYYYNNNNNN",
                    "YNYNNYYNYYNNNNYNYYNYNNNNNYYYNYYYNYNYNNYYYYNYNNYNYN",
                    "YYNNNYYNYYNYYYYYNNNNNYNNNNNYYNNYYYNYNYNYYNYYYYNNNN",
                    "YNYYYNYYYYYYYNYNNNNYYNNNNNYNYNNYYNYNYYNYNYNNYNNNYN",
                    "NNNYYYYNNYYYYYYNYYNYYYYNNNNYYNYNYNYYYNYNNNYYNYNNNY",
                    "YNNYNNNYYNNYNYYYYNNYYYYNYYNNNYNYNNNYYYNNYNYYYNNYYN",
                    "NYNYYNNYNYNNNNNNNNNNNNNYYNYNNYNYYNYYYNNYNYNNNNYYNY",
                    "NNYNYNNYNYNYYNNYNNNNNYYNNNYNNNNYNNYYNYNNNNYNYNYNNN",
                    "NNYNNNYYNNNNYYYYNNNNNYYYNYYNNNYNYNYNNYYYNYNNYNNNYN",
                    "YYNYNNYNYYNYNYYYNNYNYYYYNNNNYNNYNYYYYYYYNNNYYYNNYN",
                    "NYYNYYNYYYNYNYYNYNYNNNYNYNYYYNNNYNNNNYNYYYNNYYYYNY",
                    "NYYYNNYYNNNNYYYYNYYYYYNYNYYYNYYNNNNNYYNYYNYNNYNYYY",
                    "YNNNNNNNNYYYYYYNNNNNYNNNNYYNYNYNNNYNYYNYYNNYYNYNNY",
                    "YYYYNYNYYNNNNNNNYYNYNYNYNNYNYNYNNNNYNNYNNYNYYNYYYY",
                    "NYYYNYNYNNYNYNYYYYNNYYYYNYYYNNNYYNYNNNYNYYYYYYYYNN",
                    "NNYYNYNNNYYYYYNYNYNNYNNNYNNNYNNYYNYYNYYNNYNYNYYYYY",
                    "NYNNNNYYNYYNNYYNYNYNNYNYNNNNNNYNNNNNNNNNYNYNNNNYYN",
                    "YNYNNYYYYYYYNNYNNYYNYYNNYYYYYYNYNYNNNNNYYYNNYNYYYY",
                    "YYNYYYNNNNNYNNNNYNYNNNNYNYNNNNNYNNYYNNNNNNYYYYYNNY",
                    "NNNYNYNYYNYNNYYYYYYNYNNYNYNYYNNYYNYYYYNNNNYYNNNNNY",
                    "YNYYYNYYYNYYYYYYNNYNYNYYNYYYNNNYYNYYNYYYNNNNNYNYYN",
                    "YYNNNYYNYNYYNYYNYYNNYNYYNNNNNYYYNNYNYYNYYYNNYYNNYN",
                    "YNNYYNNYNNNNNYNNNYNNNNYNYNYNNYNYNYNNNNNNNNNNNNNNNY",
                    "NNNNNNNYYYYYNNNNNYNYYYYNNYNYNNYYNNNNNYYYNNNYNYNYNY",
                    "YNNYNNNNNNYNYNYNNYNYYYYNYNYNYYNNNYNNYNYNNYNNNNYYNN",
                    "NNNNYYNNYYYYYNNYYNNYNNYYNYYNYYNYYNYNYYNYYNYNNNNNYY",
                    "YNYNNYNYNYNYNYNYYYNYYYNYNNNNYNNYNYYYNNNNNNYNYNNNYN",
                    "NYYYNNNNYYNYNNNYYNNYYYYYYYNNNYYYNNNNNYYYYYNNNYNYNN",
                    "YYNYYYNYYNYNNYNYNNYYYNNYNNYYNYNNNNYYNYYNNYNYNYYNYN",
                    },
                new string[]
                    {
                        "NYYNNYYYYNNNYNNNYYNNNYNNYYNNNYNNYYYYYNYYYYNNYYYNNN",
                    "YNYYYYNNYNYYNYNYNNNNNNYNNYYYNNNYYYNNNNYNYNYNNNYNNY",
                    "NYNYNYYNNNYNYYYNYYNNYYYNNYYYNYNNYNYYYNNNNNNNNNNNYN",
                    "NYYNNYNYYYNNYYNYNNYYYNYNNNYNNYYNNYYYYYNYNNNYNNYNYN",
                    "YNYYNYNNYNYYNNNYYYYYYNNNYNYNNNNYYYNYNYYYYNYYNNYYNN",
                    "YNNYYNYNNNNYNNYYYYYYYNYYNNNYYYNNYYYNNYNYYNYNYYYYNY",
                    "NYNYYNNYYYNYYYNYYNYNNNYNYYYYYNNNNNYYNNYNYNNNYNYYYY",
                    "YYYYYNNNYYNNNNNYYYNYYYYYNNNNNNYYNYYYYYYYNYNYYNNNYN",
                    "YYNNNNNNNYNYYNNYYNYNNNYNNNNNYYYYYYNYYNYYYYYYNNNNNN",
                    "YYNNYYNNNNNNYNYNNYYNNNNYYNYNYYYNYYYYYNNNYNNYYNYYYN",
                    "NYNYNNYNNYNNYNNYNYYNNNNYNYNNYNNYYNYNNYYYNNNNYNYNNY",
                    "NYNYYNNNNNYNYNYYNYYNNNYYYNYYYNNNNNYNNNYYNYYNYNNYNN",
                    "NNYYYYNNNYNYNNYYNYYYNYNYNNYYYNNNNYNNNNNNYYNYNNYNNY",
                    "YNYNYYNYYYNNNNNYNNNNNNYNYYNYYYYNYYNYNNNYYNYYNNYYNY",
                    "YNNYYYYYYYNNNYNNNYYNYNNYNYNYYNNNYYYNYYYNNNNYYNNNNY",
                    "YNYYYYNNNNNYYNYNYNNYNNNYNNNYYYNYNNYNNYYYYYYYNNYNYN",
                    "YNNNYNNYYNNYYYNYNNYNNYYYNNNNNNNYYNYNYYYYNNYYYYNYYN",
                    "NNNNYNYNNYYYYYNYNNYYNYNYNNYNYYNYNNNNYNNNNNNYYYNYNY",
                    "NNYYYYNYYYNYYNYYYYNYNNNYYYYYNYYYYNNNYNNYYYNYYYNNNY",
                    "YYNYYYYNNNYNNNNNNNNNYNNYYNYYYNNYNYNNNNYYYNYNYYYNNY",
                    "NYYYYNNYYNYYYYNNYNNYNNYYYNNNYNYNYYNNYYNYYNYNYNNYNY",
                    "NNNYNNNYYYYYYNNNNNYYYNYYNNYNNNYNNYNNYYYYNYNNYYNYYN",
                    "NNNNYYNYNYNYNNYYNNYYYNNNYYNYNYYYNYYYYYYYYNYYYYNNNY",
                    "NNNYNYYNNNNNYYYNYNNNYYNNNNNNYYNNYNNYNNYNNNYNNNNNYY",
                    "NNNYNNYNNNYNNYYYNNYNNYYNNNNYNYYYYYYYNNYYYNNYYNYNYY",
                    "NNNYYYNNYYNYNYYNNNYNYYNYYNNNYNNYNNYYYNNNYYNYNYYNYN",
                    "NYYYNNNNNNNYNNYYYYNYYNYNNNNYYNNNYYNNYYYYNNYYYYNNYN",
                    "YNYNYYNNNNNYYNNYYNYYNNNYYNYNYNNYNNYNYNNNNNYNNNNNYY",
                    "NNNYYNYYNYYYYYNYYNNNNNYNYYNNNYNYNNNYYNYNNYYNYYNNNY",
                    "NNYYYYYNNYYNYNNYNNNYYNYNNYYYYNYYNYYYYYNYYNNYNNNNYY",
                    "YYNNYNNYNYYNNNNNNYYYNYNNYNNYYNNNNNYYYNYNNYYYYYYYNY",
                    "YNNYYYYYNNYYNNYNNYYNNNNNYYNYNNNNNYYYYNYYNNYNNNNNNY",
                    "NYYNNYYYNYNYNNYNYNYYNYNNYNNNYNNNNYYNNNYNNYYNNYYNYN",
                    "NYYYYYNYYYYNYYYNYNNNNNYNYYYNYYNYNNNNYYYNNYNNNYNNYY",
                    "NYYNYYYNNNNNNYNYYNYYNNNNNYNYNNNYNYNYYYYYNNNYNYNNNY",
                    "NYNYYNYNYYNYYYNNNYNYNYYNNNNYNYNNYYYNYYNNYYNNNNNYYY",
                    "NNYNYYYNYNNNYYNYYNNYYYNYYYNYYNYNYNYNNNYNYNNYNYYYYY",
                    "NYNNNNNNYNYYNNYYNYYYYNYYNYYYNNNNYNNNNNNNYYNYNYYYYY",
                    "NYYNNYYYNNNNNNNNNYNYNYNYNYYNNNYYYYNYYYNYNYYYYNYYNY",
                    "NNYNNNNYNNYYNNNNNNNYYYNNNNNYNYNYYYYYNNNNNYYYNYNNNY",
                    "YYNNNNNNNNYNNYNYNYYNNYYYNNNYYYYNNYYYYYYYNYYNYYYYNN",
                    "NNNNNYNNNNYNNNYYYNYYYYYYNNNNNYNYYYNNNYYYYNYYYNYYNY",
                    "NNYYNYNNNYNNNNNYYYYNNNNYYYYNNYYNYYNYYYYYNYNNNNNNYY",
                    "YNYNNNNYNYNNNNYNYYNYYNNNNNNNYYYYNYNYNNYNNNNNYYNYNY",
                    "NNNYNNNNNNYYYNYYYNNYNNYNYYNYNYYYNNYYNNYNYNYYNNYNNY",
                    "YYYNYNYYYNNYYYNYYNYNYNYNYNYNNNYYYNYYNNNNNNYYNNNYNN",
                    "YNYYYYNYNYNYYYYNYYYYYYNYNYYYNNYYNYNNYNNNNNNYYNNNYY",
                    "NNNYYYYNNNNYYNNYNYYNNYYNYYYYNNYYNNYYNNNNNYYNYYYNYY",
                    "YYNYYNYYNNYNNYNYYNNNNYYNYYYYYYNNYYYNNYNNNNNYYNYYNY",
                    "YYNYNNNNNYNNNYYYYYNYNYNYNNNYYYNYYYNNNNNNNYYNYYNYYN",
                    },
                new string[]
                    {
                        "NYYNYYYYNNNNNYNNNYNYNNYNYNYNNNNNYNNNNNYNYNNYNYYYYY",
                    "YNNNNYNNNYNYYNYYYNYYNYNNNNNYNYNNNYNNNNNYYNNYYYNYYN",
                    "NNNYNYNYNNNNYYYYYYNNNYYYNYYYNNYNYYNYYYYYYNYYNNNNYY",
                    "YNNNYYNNNNNNYNYYNYNYYNNYNYNYNNYYNYNYNYNYYYYYNYNYYN",
                    "YNNYNNNNNNNNNNNNYYYYNYYYNYNNYNNYNYYYNYNYYYNYYNNYYN",
                    "NNYNYNYNNNNNNYNNYNNNYYYYNNNNNNNYYYNNNYYYYNNYNYNYYN",
                    "YNYNYNNNYNNNYYYYNYNYYYNNNNNNYNYYNNYYNNYNYYNNYYNYNY",
                    "YYNYNNYNNYYNNYNNNNNYNNNNYYYNYNYYYNNYYNNYYYYNYYYNYY",
                    "NNYNNNYNNNYNYNYNYYYYYNYYYNNYYYYNNYYNNNYNNYYNNNNYNY",
                    "NNYYNYNNYNYYYYNYNYYYNNYYNYYNNNYNYYNYYNYNYYYNNYNYYY",
                    "YYYNYYNNNNNYNYYNYYYNNYYYYYYNNYNNYNNNYYNNYYNNYNNYYN",
                    "YNYNYYNYNNYNNNNNNYNYYNYNYYNNNNYNNYNYNNNYYNYYYNYYNN",
                    "YNYNNYYYNNNYNNNNYYNYNNYNNNYYYYYNYNYYNNNYNNYYYYYNNY",
                    "NNNYNNNNNYNYNNNYYYYNNNNNNYNNYNYNYYNNYYNYNNNNYYYNYN",
                    "YYYNYYNYYNYYNYNYNYNYYYYNYYYNYYNNNNYYYYNYYYYNYYNYNY",
                    "NNYNNYYNYYNYYNNNYNNNNNNYYNNNNNYNNYNYNYYYNNNYYNNYYN",
                    "YYYYNYNNYYNNNYYNNNNYYNYNNYYYYNNYNNYNYNNNYYYNYNYNNN",
                    "NYYYYYYNYYYYNYNYYNNYYNNNYNYYNNYNYNYNYYYNYNNNYYYNNY",
                    "NNYYNNNYNNYNNNYNYNNNYYNNNNNYYNNNYYYNNYYYNNYYYNNNYN",
                    "NYYYYNNNNYYYNNNNYYYNNYNYYYYNNNYNYYNNNYYNNYYYYYYNNN",
                    "NYYNYYNNYYNNYYNNYNNNNNNNYYYYNNYYNYYNNYYYNYYNNYNNYY",
                    "YNNYYNNNYYNNNYYYYNYYNNNYYNNYYNNNYNNYNYNYYNNNYNNYYN",
                    "NYNYNNNYNNYNNNYNYNYNNYNNYYYNYNYYNYYNYYYYYYNYYYYYNY",
                    "NNYNYYNYNYYNYNYNYNNYNNYNNYNNNNYYNYYNYYYYYNNYNYNNNN",
                    "NYNNNYNNNNYYNNNNYNYYNNYYNYYYYNNYNYYYNNYYNNYYYNYNNY",
                    "YYNNYYYYNYYNYNNYYYNNNNYYNNNYYYNNNYNNYNNYYYNNNNNNNY",
                    "NNYNYYNNYNYYNNNNYNYNNYNNYYNNNNYNYNNYNNNYNYYNNNYNNN",
                    "NYYYNNNNNYNYNYNNYNYYYNNNYYNNYYYYYNYYYNYNNNNYNNNYNN",
                    "YYYYNNNNNNNNYYYNYNNYYYYYNNYYNYYNNNYYNNNYNNNYYNYNNN",
                    "YYNYYNYYNNNNYNYNNYYNYYYNYNNYNNYYYYYYNYNYYNYYNYNYYY",
                    "YNYNNYNYYNYYNYNNYYNYNNYNYNNNNYNNNYNNNNYNYYNNYNYNNY",
                    "YNNYNYYYNNNNNNNNNYYNNYYYYNYNNYYNYNNYYNYNYNNYYYYYYN",
                    "YYNNYNNNYYYYYYNYYNNNNYYYNNYYNNYNNNNYYNNYNYNYNYYNYN",
                    "YYNYNYNYNYNYNYYYYNYNNYNNNYNYYNNYNNYNNYYYYNNYNYNNYY",
                    "YNNYYNYYYNNYYNNYYNNNNYYNNNNYYNYNYYNNNNNNNNYNNNNYNY",
                    "YNYNNYNYNYNNYNYYYNNYNNYYYNNYYNYNNNYNNYYNNYYYYNNYNN",
                    "NYNNNNYNNYYNYYYYNYYNYYNNNNNYNNYYYNNYNNYNNNYNYYNNNY",
                    "YNNYNYYYYNYYYYYYNNYNNNYYNYYYYYYYYYNYYNYYYYYYNNYNYY",
                    "YYYYYNNNNYNYYNYNYYNYNYNYNYYNYYYNYYNNNNNYYNYYNYNNNN",
                    "NYYNNNNYNNNNYYYNNYNNNNYNNNYNNNYNYNNNNNYNYNNNNYYNYY",
                    "NNNNYYNNYYYNYYYNYYNYNNNYYNNYNNNYNNYNYNNNNNYYNNYYYY",
                    "NNYNYNNNYYNYYNYYNNNNNNYYNYYNYNNYYNYNNYYNYNYNYNYNYY",
                    "NYYYNYYYNYNYYNNNNYYYNNNNYNYYYYNNYNNYNYNNYYNYYYYNNN",
                    "YYYYYNYNYNYNYYYNNNNNNYYYYYYYYNYYYYYNYNYNYNYNNYNYYY",
                    "YYYYYYNYNYNYYYNYYYNYNYNYYNNYYNNYNNYNYNYNNYNNNNYNNN",
                    "NYYYNYNNNYYYYNNNYYNNYYNYNNYNYYYNNYNYNNYYYNNYYNNNNY",
                    "NYNYNYNNNNYYNYNNNNNYYNNYNNNNNYNNNYYYYYYNYNNYYYNYYY",
                    "YYYYNYNNNNYNYNYYNYNNNNYYNYNYYYYNNNYNNNNYNNYNNYYNNY",
                    "NNNNNNNYNYNNNNNYNNYYNNYNYYYYNYNYNYNNYNNYNYNNYNYNNY",
                    "NNYYNYYNNYNNYNNYNYYNNNYYNNYNYNYYYYNNYYNNNNYNYNNNNN",
                    });
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
