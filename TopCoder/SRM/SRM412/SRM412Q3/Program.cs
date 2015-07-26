using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ErrantKnight
{
    public struct Loc
    {
        public Loc(int x, int y)
        {
            X = x;
            Y = y;
        }
        public int X;
        public int Y;
    }
    public string whoWins(int[] x, int[] y)
    {
        int[] mdx = new int[8];
        int[] mdy = new int[8];
        mdx[0] = 2; mdy[0] = 1;
        mdx[1] = 1; mdy[1] = 2;
        mdx[2] = 2; mdy[2] = -1;
        mdx[3] = 1; mdy[3] = -2;
        mdx[4] = -2; mdy[4] = 1;
        mdx[5] = -1; mdy[5] = 2;
        mdx[6] = -2; mdy[6] = -1;
        mdx[7] = -1; mdy[7] = -2;
        int a = 501;
        int b = 250;
        int[,] movesToWin = new int[a,a];
        for (int i=0; i < a;i++) {
            for (int j=0; j < a; j++) {
                movesToWin[i,j] = -1;
            }
        }
        movesToWin[b,b] = 0;
        movesToWin[b-1,b] = 0;
        movesToWin[b+1,b] = 0;
        movesToWin[b,b-1] = 0;
        movesToWin[b,b+1] = 0;
        int next = 1;
        List<Loc> locs = new List<Loc>();
        locs.Add(new Loc(b, b));
        locs.Add(new Loc(b-1, b));
        locs.Add(new Loc(b+1, b));
        locs.Add(new Loc(b, b-1));
        locs.Add(new Loc(b, b+1));
        List<Loc> newLocs = new List<Loc>();
        while (locs.Count > 0)
        {
            for (int i = 0; i < locs.Count; i++)
            {
                int dx = locs[i].X - b;
                int dy = locs[i].Y - b;
                for (int j = 0; j < 8; j++)
                {
                    if (Dist(dx, dy) < Dist(dx + mdx[j], dy + mdy[j]))
                    {
                        Fill(movesToWin, dx, dy, mdx[j], mdy[j], next, a, newLocs);
                    }
                }
            }
            List<Loc> temp = locs;
            locs = newLocs;
            newLocs = temp;
            newLocs.Clear();
            next++;
        }
        return "";
    }

    private int Dist(int dx, int dy)
    {
        return dx * dx + dy * dy;
    }

    private void Fill(int[,] movesToWin, int dx, int dy, int mdx, int mdy, int next, int a, List<Loc> newLocs)
    {
        while (true)
        {
            dx += mdx;
            dy += mdy;
            if (dx >= a || dx < 0 || dy < 0 || dy >= a)
                break;
            if (movesToWin[dx, dy] < 0)
            {
                movesToWin[dx, dy] = next;
                newLocs.Add(new Loc(dx, dy));
            }
        }
    }
}

namespace SRM412Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            ErrantKnight instance = new ErrantKnight();
            Console.Out.WriteLine(instance.whoWins(new int[] {}, new int[] {}));
            Console.ReadKey();
        }
    }
}
