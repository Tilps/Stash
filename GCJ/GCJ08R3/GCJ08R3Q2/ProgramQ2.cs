using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCJ08R3Q2
{
    class ProgramQ2
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(args[0]);
            List<string> output = new List<string>();
            int cases = int.Parse(lines[0]);
            int index = 1;
            for (int i = 0; i < cases; i++)
            {
                // Parse
                string[] bits = lines[index].Split(' ');
                index++;
                int R = int.Parse(bits[0]);
                int C = int.Parse(bits[1]);
                int[,] map = new int[C, R];
                int startX=-1;
                int startY = -1;
                int cakeX = -1;
                int cakeY = -1;
                for (int j = 0; j < R; j++)
                {
                    for (int k = 0; k < C; k++)
                    {
                        bool cake;
                        bool start;
                        map[k, j] = Map(lines[index][k], out cake, out start);
                        if (start)
                        {
                            startX = k;
                            startY = j;
                        }
                        if (cake)
                        {
                            cakeX = k;
                            cakeY = j;
                        }
                    }
                    index++;
                }
                // Process
                output.Add(string.Format("Case #{0}: {1}", i + 1, Solve(R, C, map, startX, startY, cakeX, cakeY)));
            }
            File.WriteAllLines("output.txt", output.ToArray());
        }

        private struct PortalLoc
        {
            public int X;
            public int Y;
            public int dir;

            public override bool Equals(object obj)
            {
                PortalLoc other = (PortalLoc)obj;
                return other.X == X && other.Y == Y && other.dir == dir;
            }
        }

        private class Pos
        {
            public int X;
            public int Y;
            public PortalLoc B;
            public int depth = -1;
        }

        private static string Solve(int R, int C, int[,] map, int startX, int startY, int cakeX, int cakeY)
        {
            portalLocCache = new List<PortalLoc>[C,R];
            bool[,,,,] seen = new bool[C, R, C+1, R+1, 2];
            List<PortalLoc> portalLocs = GetPortalLocs(map, startX, startY, R, C);
            Queue<Pos> positions = new Queue<Pos>();
            for (int i = 0; i < portalLocs.Count; i++)
            {
                Pos pos = new Pos();
                pos.X = startX;
                pos.Y = startY;
                pos.B = portalLocs[i];
                pos.depth = 0;
                positions.Enqueue(pos);
                seen[startX, startY, portalLocs[i].X, portalLocs[i].Y, portalLocs[i].dir] = true;
            }
            while (positions.Count > 0)
            {
                Pos current = positions.Dequeue();
                if (current.X == cakeX && current.Y == cakeY)
                    return current.depth.ToString();
                List<PortalLoc> portals = GetPortalLocs(map, current.X, current.Y, R, C);
                for (int i = 0; i < portals.Count; i++)
                {
                    if (portals[i].Equals(current.B))
                        continue;
                    if (!Wall(current.X - 1, current.Y, map, R, C))
                    {
                        if (!seen[current.X - 1, current.Y, current.B.X, current.B.Y, current.B.dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = current.X - 1;
                            newPos.Y = current.Y;
                            newPos.B = current.B;
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[current.X - 1, current.Y, current.B.X, current.B.Y, current.B.dir] = true;
                        }
                        if (!seen[current.X - 1, current.Y, portals[i].X, portals[i].Y, portals[i].dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = current.X - 1;
                            newPos.Y = current.Y;
                            newPos.B = portals[i];
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[current.X - 1, current.Y, portals[i].X, portals[i].Y, portals[i].dir] = true;
                        }
                    }
                    if (!Wall(current.X + 1, current.Y, map, R, C))
                    {
                        if (!seen[current.X + 1, current.Y, current.B.X, current.B.Y, current.B.dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = current.X + 1;
                            newPos.Y = current.Y;
                            newPos.B = current.B;
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[current.X + 1, current.Y, current.B.X, current.B.Y, current.B.dir] = true;
                        }
                        if (!seen[current.X + 1, current.Y, portals[i].X, portals[i].Y, portals[i].dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = current.X + 1;
                            newPos.Y = current.Y;
                            newPos.B = portals[i];
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[current.X + 1, current.Y, portals[i].X, portals[i].Y, portals[i].dir] = true;
                        }
                    }
                    if (!Wall(current.X, current.Y - 1, map, R, C))
                    {
                        if (!seen[current.X, current.Y - 1, current.B.X, current.B.Y, current.B.dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = current.X;
                            newPos.Y = current.Y - 1;
                            newPos.B = current.B;
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[current.X, current.Y - 1, current.B.X, current.B.Y, current.B.dir] = true;
                        }
                        if (!seen[current.X, current.Y - 1, portals[i].X, portals[i].Y, portals[i].dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = current.X;
                            newPos.Y = current.Y - 1;
                            newPos.B = portals[i];
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[current.X, current.Y - 1, portals[i].X, portals[i].Y, portals[i].dir] = true;
                        }
                    }
                    if (!Wall(current.X, current.Y + 1, map, R, C))
                    {
                        if (!seen[current.X, current.Y + 1, portals[i].X, portals[i].Y, portals[i].dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = current.X;
                            newPos.Y = current.Y + 1;
                            newPos.B = portals[i];
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[current.X, current.Y + 1, portals[i].X, portals[i].Y, portals[i].dir] = true;
                        }
                        if (!seen[current.X, current.Y + 1, current.B.X, current.B.Y, current.B.dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = current.X;
                            newPos.Y = current.Y + 1;
                            newPos.B = current.B;
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[current.X, current.Y + 1, current.B.X, current.B.Y, current.B.dir] = true;
                        }
                    }
                    if (NextTo(current.X, current.Y, portals[i], map, R, C))
                    {
                        int nextX;
                        int nextY;
                        GetExit(current.B, out nextX, out nextY, map, R, C);
                        if (!seen[nextX, nextY, current.B.X, current.B.Y, current.B.dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = nextX;
                            newPos.Y = nextY;
                            newPos.B = current.B;
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[nextX, nextY, current.B.X, current.B.Y, current.B.dir] = true;
                        }
                        if (!seen[nextX, nextY, portals[i].X, portals[i].Y, portals[i].dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = nextX;
                            newPos.Y = nextY;
                            newPos.B = portals[i];
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[nextX, nextY, portals[i].X, portals[i].Y, portals[i].dir] = true;
                        }
                        for (int j = 0; j < portals.Count; j++)
                        {
                            if (portals[i].Equals(portals[j]))
                                continue;
                            GetExit(portals[j], out nextX, out nextY, map, R, C);
                            if (!seen[nextX, nextY, portals[j].X, portals[j].Y, portals[j].dir])
                            {
                                Pos newPos = new Pos();
                                newPos.X = nextX;
                                newPos.Y = nextY;
                                newPos.B = portals[j];
                                newPos.depth = current.depth + 1;
                                positions.Enqueue(newPos);
                                seen[nextX, nextY, portals[j].X, portals[j].Y, portals[j].dir] = true;
                            }
                            if (!seen[nextX, nextY, portals[i].X, portals[i].Y, portals[i].dir])
                            {
                                Pos newPos = new Pos();
                                newPos.X = nextX;
                                newPos.Y = nextY;
                                newPos.B = portals[j];
                                newPos.depth = current.depth + 1;
                                positions.Enqueue(newPos);
                                seen[nextX, nextY, portals[i].X, portals[i].Y, portals[i].dir] = true;
                            }
                        }
                    }
                    if (NextTo(current.X, current.Y, current.B, map, R, C))
                    {
                        int nextX;
                        int nextY;
                        GetExit(portals[i], out nextX, out nextY, map, R, C);
                        if (!seen[nextX, nextY, current.B.X, current.B.Y, current.B.dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = nextX;
                            newPos.Y = nextY;
                            newPos.B = current.B;
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[nextX, nextY, current.B.X, current.B.Y, current.B.dir] = true;
                        }
                        if (!seen[nextX, nextY, portals[i].X, portals[i].Y, portals[i].dir])
                        {
                            Pos newPos = new Pos();
                            newPos.X = nextX;
                            newPos.Y = nextY;
                            newPos.B = portals[i];
                            newPos.depth = current.depth + 1;
                            positions.Enqueue(newPos);
                            seen[nextX, nextY, portals[i].X, portals[i].Y, portals[i].dir] = true;
                        }
                    }
                }
            }
            return "THE CAKE IS A LIE";
        }

        private static void GetExit(PortalLoc portalLoc, out int nextX, out int nextY, int[,] map, int R, int C)
        {
            nextX = -1;
            nextY = -1;
            if (portalLoc.dir == 0)
            {
                if (Wall(portalLoc.X, portalLoc.Y - 1, map, R, C))
                {
                    nextX = portalLoc.X;
                    nextY = portalLoc.Y;
                    return;
                }
                if (Wall(portalLoc.X, portalLoc.Y, map, R, C))
                {
                    nextX = portalLoc.X;
                    nextY = portalLoc.Y-1;
                    return;
                } 
            }
            if (portalLoc.dir == 1)
            {
                if (Wall(portalLoc.X-1, portalLoc.Y, map, R, C))
                {
                    nextX = portalLoc.X;
                    nextY = portalLoc.Y;
                    return;
                }
                if (Wall(portalLoc.X, portalLoc.Y, map, R, C))
                {
                    nextX = portalLoc.X-1;
                    nextY = portalLoc.Y;
                    return;
                }
            }

        }

        private static bool NextTo(int X, int Y, PortalLoc portalLoc, int[,] map, int R, int C)
        {
            if (portalLoc.dir == 0)
            {
                if (portalLoc.X == X && portalLoc.Y == Y && Wall(X, Y - 1, map, R, C))
                    return true;
                if (portalLoc.X == X && portalLoc.Y == Y + 1 && Wall(X, Y + 1, map, R, C))
                    return true;
            }
            if (portalLoc.dir == 1)
            {
                if (portalLoc.Y == Y && portalLoc.X == X && Wall(X - 1, Y, map, R, C))
                    return true;
                if (portalLoc.Y == Y && portalLoc.X == X + 1 && Wall(X + 1, Y, map, R, C))
                    return true;
            }
            return false;
        }

        static List<PortalLoc>[,] portalLocCache;

        private static List<PortalLoc> GetPortalLocs(int[,] map, int startX, int startY, int R, int C)
        {
            if (portalLocCache[startX, startY] != null)
                return portalLocCache[startX, startY];
            List<PortalLoc> locs = new List<PortalLoc>();
            for (int i = startX + 1; i <= C; i++)
            {
                if (Wall(i, startY, map, R, C))
                {
                    PortalLoc loc = new PortalLoc();
                    loc.X = i;
                    loc.Y = startY;
                    loc.dir = 1;
                    locs.Add(loc);
                    break;
                }
            }
            for (int i = startX - 1; i >= -1; i--)
            {
                if (Wall(i, startY, map, R, C))
                {
                    PortalLoc loc = new PortalLoc();
                    loc.X = i + 1;
                    loc.Y = startY;
                    loc.dir = 1;
                    locs.Add(loc);
                    break;
                }
            }
            for (int i = startY + 1; i <= R; i++)
            {
                if (Wall(startX, i, map, R, C))
                {
                    PortalLoc loc = new PortalLoc();
                    loc.X = startX;
                    loc.Y = i;
                    loc.dir = 0;
                    locs.Add(loc);
                    break;
                }
            }
            for (int i = startY - 1; i >= -1; i--)
            {
                if (Wall(startX, i, map, R, C))
                {
                    PortalLoc loc = new PortalLoc();
                    loc.X = startX;
                    loc.Y = i+1;
                    loc.dir = 0;
                    locs.Add(loc);
                    break;
                }
            }
            portalLocCache[startX, startY] = locs;
            return locs;
        }

        private static bool Wall(int x, int y, int[,] map, int R, int C)
        {
            if (x < 0 || y < 0)
                return true;
            if (x >= C || y >= R)
                return true;
            return map[x, y] == 1;
        }

        private static int Map(char p, out bool cake, out bool start)
        {
            cake = false;
            start = false;
            switch (p)
            {
                case '.':
                    return 0;
                case '#':
                    return 1;
                case 'O':
                    start = true;
                    return 0;
                case 'X':
                    cake = true;
                    return 0;
            }
            throw new Exception();

        }
    }
}
