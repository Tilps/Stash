using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class SwitchingGame
{
    public int timeToWin(string[] states)
    {
        int n = states.Length;
        int m = states[0].Length;
        char[,] backFill = new char[n,m];
        for (int i = n-1; i >= 0; i--)
        {
            for (int j = 0; j < m; j++)
            {
                if (i == n - 1)
                {
                    backFill[i, j] = states[i][j];
                }
                else if (states[i][j] == '?')
                {
                    backFill[i, j] = backFill[i + 1, j];
                }
                else
                {
                    backFill[i, j] = states[i][j];                    
                }
            }
        }
        bool[] current = new bool[m];
        int steps = 0;
        for (int i = 0; i < n; i++)
        {
            bool needOn = false;
            bool needOff = false;
            for (int j = 0; j < m; j++)
            {
                if (states[i][j] == '+' && !current[j])
                {
                    needOn = true;
                }
                if (states[i][j] == '-' && current[j])
                {
                    needOff = true;
                }
            }
            if (needOn) steps++;
            if (needOff) steps++;
            for (int j = 0; j < m; j++)
            {
                if (needOn)
                {
                    if (states[i][j] == '+' && !current[j]) current[j] = true;
                    if (states[i][j] == '?' && backFill[i,j] == '+' && !current[j]) current[j] = true;
                }
                if (needOff)
                {
                    if (states[i][j] == '-' && current[j]) current[j] = false;
                    if (states[i][j] == '?' && backFill[i, j] == '-' && current[j]) current[j] = false;
                    
                }
            }
        }
        return steps+n;
    }

}

namespace Q1
{
    class Q1
    {
        static void Main(string[] args)
        {
            SwitchingGame c = new SwitchingGame();
            object o = c.timeToWin(new string[] {"+??+++",
 "++??+-",
 "?++??+",
 "?-+-??",
 "??+?++",
 "++-?+?",
 "?++?-+",
 "?--+++",
 "-??-?+"});
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