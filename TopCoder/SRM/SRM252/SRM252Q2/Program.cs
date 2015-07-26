using System;
using System.Collections;
using System.Text;

public class WatchtowerSpotlight
{
    const int mt = 1000;
    int[, ,] memo = new int[50, 50, mt];
    int[] fin = new int[2];
    int[] tower = new int[2];
    int s = 0;
    int d = 0;
    int bestEver = int.MaxValue;

    private bool isdead(int cx, int cy, int t)
    {
        if (cx == tower[0] && cy == tower[1])
            return true;
        int dx = cx - tower[0];
        int dy = cy - tower[1];
        if (dx * dx + dy * dy > d * d)
        {
            return false;
        }
        double a = Math.Atan2(dy, dx);
        if (a < 0)
            a += 2 * Math.PI;
        double deg = a/2/Math.PI*s;
        int min = (t % s);
        int max = ((t % s)+1);
        if (deg >= min && deg <= max)
            return true;
        if (max == s && a == 0)
            return true;
        return false;
    }

    private int go(int cx, int cy, int t)
    {
        if (t > bestEver)
            return -1;
        if (t >= mt)
            return -1;
        if (memo[cx, cy, t] != -2)
            return memo[cx, cy, t];
        int best = int.MaxValue;
        if (isdead(cx, cy, t))
            best = -1;
        else if (cx == fin[0] && cy == fin[1])
            best = 0;
        else
        {
            if (cx > 0)
            {
                int temp = go(cx - 1, cy, t + 1);
                if (temp != -1 && temp + 1 < best)
                {
                    best = temp + 1;
                }
            }
            if (cy > 0)
            {
                int temp = go(cx, cy -1, t + 1);
                if (temp != -1 && temp + 1 < best)
                {
                    best = temp + 1;
                }
            }
            if (cx < 49)
            {
                int temp = go(cx + 1, cy, t + 1);
                if (temp != -1 && temp + 1 < best)
                {
                    best = temp + 1;
                }
            }
            if (cy < 49)
            {
                int temp = go(cx, cy+1, t + 1);
                if (temp != -1 && temp + 1 < best)
                {
                    best = temp + 1;
                }
            }
        }
        if (best == int.MaxValue)
            best = -1;
        memo[cx,cy,t] = best;
       // if (best >=0 && best + t < bestEver)
       //     bestEver = best + t;
        return best;
    }

    public int quickestEscape(string start, string end, string watchtower)
    {
        for (int i=0; i < 50; i++) {
            for (int j=0; j < 50; j++) {
                for (int k=0; k < mt; k++) {
                    memo[i,j,k] = -2;
                }
            }
        }
        int[] begin = new int[2];
        string[] strs = start.Split(' ');
        begin[0] = int.Parse(strs[0]);
        begin[1] = int.Parse(strs[1]);
        strs = end.Split(' ');
        fin[0] = int.Parse(strs[0]);
        fin[1] = int.Parse(strs[1]);
        strs = watchtower.Split(' ');
        tower[0] = int.Parse(strs[0]);
        tower[1] = int.Parse(strs[1]);
        s = int.Parse(strs[2]);
        d = int.Parse(strs[3]);

        int best = go(begin[0], begin[1], 0);

        return best;
    }
}

namespace SRM252Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            WatchtowerSpotlight c = new WatchtowerSpotlight();
            System.Console.Out.WriteLine(c.quickestEscape("6 1", "6 3", "3 3 3 3"));
            System.Console.In.ReadLine();
        }
    }
}
