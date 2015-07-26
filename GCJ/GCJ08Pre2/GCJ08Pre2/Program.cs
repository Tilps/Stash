using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GCJ08Pre2
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(args[0]);
            int count = int.Parse(lines[0]);
            List<string> output = new List<string>();
            for (int i = 1; i <= count; i++)
            {
                string[] parts = lines[i].Split(' ');
                output.Add(string.Format("Case #{0}:", i));
                output.AddRange(Solve(parts));
            }
            File.WriteAllLines("output.txt", output.ToArray());
            Console.Out.WriteLine("Press any key.");
            Console.ReadKey();
        }

        private static string[] Solve(string[] parts)
        {
            int x = 0;
            int y = 0;
            int minx = 0;
            int maxx = 0;
            int miny = 0;
            int maxy = 0;
            int facing = 0;
            foreach (char c in parts[0])
            {
                if (c == 'L')
                {
                    facing--;
                    if (facing < 0)
                        facing = 3;
                }
                else if (c == 'R')
                {
                    facing++;
                    if (facing > 3)
                        facing = 0;
                }
                else
                {
                    switch (facing)
                    {
                        case 0:
                            y++;
                            break;
                        case 1:
                            x--;
                            break;
                        case 2:
                            y--;
                            break;
                        case 3:
                            x++;
                            break;
                    }
                    if (x > maxx)
                        maxx = x;
                    if (y > maxy)
                        maxy = y;
                    if (x < minx)
                        minx = x;
                    if (y < miny)
                        miny = y;
                }
            }
            facing = (facing + 2) % 4;
            foreach (char c in parts[1])
            {
                if (c == 'L')
                {
                    facing--;
                    if (facing < 0)
                        facing = 3;
                }
                else if (c == 'R')
                {
                    facing++;
                    if (facing > 3)
                        facing = 0;
                }
                else
                {
                    switch (facing)
                    {
                        case 0:
                            y++;
                            break;
                        case 1:
                            x--;
                            break;
                        case 2:
                            y--;
                            break;
                        case 3:
                            x++;
                            break;
                    }
                    if (x > maxx)
                        maxx = x;
                    if (y > maxy)
                        maxy = y;
                    if (x < minx)
                        minx = x;
                    if (y < miny)
                        miny = y;
                }
            }
            int xsize = maxx - minx + 3;
            int ysize = maxy - miny + 3;
            int[,] walls = new int[xsize, ysize];
            /*for (int i = 0; i < xsize; i++)
            {
                AddWallLeft(walls, i, 0, 1);
                AddWallLeft(walls, i, ysize - 1, 3);
            }
            for (int i = 0; i < ysize; i++)
            {
                AddWallLeft(walls, 0, i, 0);
                AddWallLeft(walls, xsize-1, i, 2);
            }*/
            x = -minx+1;
            y = 1;
            facing = 0;
            //RemoveWallFront(walls, x, y, facing);
            bool noLeftWallWalk = false;
            foreach (char c in parts[0])
            {
                if (c == 'L')
                {
                    noLeftWallWalk = true;
                    facing--;
                    if (facing < 0)
                        facing = 3;
                }
                else if (c == 'R')
                {
                    AddWallLeft(walls, x, y, facing);
                    facing++;
                    if (facing > 3)
                        facing = 0;
                    AddWallLeft(walls, x, y, facing);
                }
                else
                {
                    if (!noLeftWallWalk)
                        AddWallLeft(walls, x, y, facing);
                    else
                        noLeftWallWalk = false;
                    switch (facing)
                    {
                        case 0:
                            y++;
                            break;
                        case 1:
                            x--;
                            break;
                        case 2:
                            y--;
                            break;
                        case 3:
                            x++;
                            break;
                    }
                }
            }
            int exitFacing = facing;
            facing = (facing + 2) % 4;
            //RemoveWallFront(walls, x, y, facing);
            noLeftWallWalk = false;
            foreach (char c in parts[1])
            {
                if (c == 'L')
                {
                    noLeftWallWalk = true;
                    facing--;
                    if (facing < 0)
                        facing = 3;
                }
                else if (c == 'R')
                {
                    AddWallLeft(walls, x, y, facing);
                    facing++;
                    if (facing > 3)
                        facing = 0;
                    AddWallLeft(walls, x, y, facing);
                }
                else
                {
                    if (!noLeftWallWalk)
                        AddWallLeft(walls, x, y, facing);
                    else
                        noLeftWallWalk = false;
                    switch (facing)
                    {
                        case 0:
                            y++;
                            break;
                        case 1:
                            x--;
                            break;
                        case 2:
                            y--;
                            break;
                        case 3:
                            x++;
                            break;
                    }
                }
            }
            string[] res = new string[ysize-3 - (exitFacing==0 ? 1 : 0)];
            for (int i = 2; i < ysize - 1 - (exitFacing == 0 ? 1 : 0); i++)
            {
                for (int j = 1 + (exitFacing == 1 ? 1 : 0); j < xsize - 1 - (exitFacing == 3 ? 1 : 0); j++)
                {
                    res[i-2] += Map(walls[j, i]);
                }
            }
            return res;
        }

        private static string Map(int p)
        {
            switch (p)
            {
                case 0:
                    return "f";
                case 1:
                    return "d";
                case 2:
                    return "b";
                case 3:
                    return "9";
                case 4:
                    return "e";
                case 5:
                    return "c";
                case 6:
                    return "a";
                case 7:
                    return "8";
                case 8:
                    return "7";
                case 9:
                    return "5";
                case 10:
                    return "3";
                case 11:
                    return "1";
                case 12:
                    return "6";
                case 13:
                    return "4";
                case 14:
                    return "2";
                case 15:
                    return "0";
            }
            throw new Exception("Invalid arg");
        }

        private static void AddWallLeft(int[,] walls, int x, int y, int facing)
        {
            facing--;
            if (facing < 0)
                facing = 3;
            switch (facing) {
                case 0:
                    walls[x, y] |= 1;
                    walls[x, y + 1] |= 4;
                    break;
                case 1:
                    walls[x, y] |= 2;
                    walls[x - 1, y] |= 8;
                    break;
                case 2:
                    walls[x, y] |= 4;
                    walls[x, y - 1] |= 1;
                    break;
                case 3:
                    walls[x, y] |= 8;
                    walls[x + 1, y] |= 2;
                    break;
            }
        }
    }
}
