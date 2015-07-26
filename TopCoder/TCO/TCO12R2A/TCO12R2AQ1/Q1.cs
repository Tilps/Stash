using System;
using System.Collections;
using System.Collections.Generic;

using System.Text;

public class SwitchesAndLamps
{
    public int theMin(string[] switches, string[] lamps)
    {
        int size = switches[0].Length;
        bool[,] isNot = new bool[switches[0].Length, switches[0].Length];
        for (int i = 0; i < switches.Length; i++)
        {
            for (int j = 0; j < switches[i].Length; j++)
            {
                for (int k = 0; k < lamps[i].Length; k++)
                {
                    if (switches[i][j] != lamps[i][k])
                        isNot[j, k] = true;
                }
            }
        }
        for (int i = 0; i < switches.Length; i++)
        {
            for (int j = i+1; j < switches.Length; j++)
            {
                List<int> sameSwitches = new List<int>();
                List<int> sameLamps = new List<int>();
                List<int> diffSwitches = new List<int>();
                List<int> diffLamps = new List<int>();
                for (int k = 0; k < size; k++)
                {
                    if (switches[i][k] == switches[j][k])
                        sameSwitches.Add(k);
                    else
                        diffSwitches.Add(k);
                    if (lamps[i][k] == lamps[j][k])
                        sameLamps.Add(k);
                    else
                        diffLamps.Add(k);
                }
                if (sameSwitches.Count != sameLamps.Count)
                    return -1;
                foreach (int k in sameSwitches)
                    foreach (int l in diffLamps)
                        isNot[k, l] = true;
                foreach (int k in diffSwitches)
                    foreach (int l in sameLamps)
                        isNot[k, l] = true;
            }
        }
        bool changing = true;
        while (changing)
        {
            changing = false;
            for (int i = 0; i < size; i++)
            {
                int total = 0;
                int which =-1;
                for (int j = 0; j < size; j++)
                {
                    if (!isNot[i, j]) {
                        total++;
                        which = j;
                    }
                }
                if (total == 0)
                    return -1;
                if (total == 1)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (j == i)
                            continue;
                        if (!isNot[j, which])
                        {
                            isNot[j, which] = true;
                            changing = true;
                        }
                    }
                }
            }
        }
        int[] possibilityCount = new int[size];
        for (int i = 0; i < size; i++)
        {
            int total = 0;
            for (int j = 0; j < size; j++)
            {
                if (!isNot[i, j])
                    total++;
            }
            if (total == 0)
                return -1;
            possibilityCount[i] = total;
        }
        int worst = 1;
        for (int i = 0; i < possibilityCount.Length; i++)
        {
            if (possibilityCount[i] > worst)
                worst = possibilityCount[i];
        }
        return CeilLog2(worst);
    }

    private static int CeilLog2(int worst)
    {
        if (worst <= 1)
            return 0;
        if (worst <= 2)
            return 1;
        if (worst <= 4)
            return 2;
        if (worst <= 8)
            return 3;
        if (worst <= 16)
            return 4;
        if (worst <= 32)
            return 5;
        if (worst <= 64)
            return 6;

        return 7;
    }

    public int theRealMin(string[] switches, string[] lamps)
    {
        Dictionary<long, int> sPat = new Dictionary<long, int>();
        Dictionary<long, int> lPat = new Dictionary<long, int>();
        for (int i = 0; i < switches[0].Length; i++)
        {
            long pat = 0;
            for (int j = 0; j < switches.Length; j++)
            {
                if (switches[j][i] == 'Y')
                    pat |= 1L << j;
            }
            if (!sPat.ContainsKey(pat)) sPat[pat] = 0;
            sPat[pat]++;
        }
        for (int i = 0; i < switches[0].Length; i++)
        {
            long pat = 0;
            for (int j = 0; j < switches.Length; j++)
            {
                if (lamps[j][i] == 'Y')
                    pat |= 1L << j;
            }
            if (!lPat.ContainsKey(pat)) lPat[pat] = 0;
            lPat[pat]++;
        }
        if (sPat.Count != lPat.Count)
            return -1;
        int maxCount=0;
        foreach (KeyValuePair<long, int> patCount in sPat)
        {
            int count;
            if (!lPat.TryGetValue(patCount.Key, out count))
                return -1;
            if (count != patCount.Value)
                return -1;
            if (count > maxCount)
                maxCount = count;
        }
        return CeilLog2(maxCount);
    }


}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            SwitchesAndLamps c = new SwitchesAndLamps();
            object o2 = c.theRealMin(new string[] { "YNYNNNYNNYYNY", "YNNNYYNYNYNNY", "NYYNYYYYNNYNN" }, new string[] { "NYYYNNYYNNYNN","NNYYYNNYYYNNN","YNNNYNYYYNYNY" });
            Random rnd = new Random();
            while (true)
            {
                int states = rnd.Next(1, 4);
                int lights = rnd.Next(1, 15);
                List<int> lightPats = new List<int>();
                for (int i = 0; i < lights; i++)
                {
                    lightPats.Add(rnd.Next(0, 1 << states));
                }
                List<int> switchPats = new List<int>(lightPats);
                for (int i = 0; i < switchPats.Count; i++)
                {
                    int other = rnd.Next(switchPats.Count);
                    if (other == i)
                        continue;
                    int temp = switchPats[other];
                    switchPats[other] = switchPats[i];
                    switchPats[i] = temp;
                }
                string[] switches = new string[states];
                string[] lamps = new string[states];
                for (int i = 0; i < states; i++)
                {
                    StringBuilder a = new StringBuilder();
                    StringBuilder b = new StringBuilder();
                    for (int j = 0; j < lights; j++)
                    {
                        if ((switchPats[j] & (1 << i)) != 0)
                            a.Append('Y');
                        else
                            a.Append('N');
                        if ((lightPats[j] & (1 << i)) != 0)
                            b.Append('Y');
                        else
                            b.Append('N');
                    }
                    switches[i] = a.ToString();
                    lamps[i] = b.ToString();
                }
                if (c.theMin(switches, lamps) != c.theRealMin(switches, lamps))
                {
                    Console.WriteLine("Failure shuffled!");
                    PrintObj(switches);
                    PrintObj(lamps);
                }
                for (int i = 0; i < states; i++)
                {
                    StringBuilder a = new StringBuilder();
                    StringBuilder b = new StringBuilder();
                    for (int j = 0; j < lights; j++)
                    {
                        a.Append(rnd.Next(2) == 1 ? 'Y' : 'N');
                    }
                    b.Append(a.ToString());
                    for (int j = 0; j < lights; j++)
                    {
                        int other = rnd.Next(lights);
                        if (other == j)
                            continue;
                        char temp = b[other];
                        b[other] = b[j];
                        b[j] = temp;
                    }
                    switches[i] = a.ToString();
                    lamps[i] = b.ToString();
                }
                if (c.theMin(switches, lamps) != c.theRealMin(switches, lamps))
                {
                    Console.WriteLine("Failure true random!");
                    PrintObj(switches);
                    PrintObj(lamps);
                }
            }
            object o = c.theMin(new string[] { "YNNYNNYNYY", "NYNNYNYNYN", "YNYNYYYYYN", "NNYYNYNYNN" }, new string[] { "NYYNYNNYNY", "NYYYNNYNNN", "YYYYNYNNYY", "YNNNNYNYYN" });
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
