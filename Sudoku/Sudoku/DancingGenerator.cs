using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    public class DancingGenerator : Generator
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sizex">
        /// X width of blocks.
        /// </param>
        /// <param name="sizey">
        /// Y width of blocks.
        /// </param>
        /// <param name="rnd">
        /// Random number generator to use.
        /// </param>
        /// <param name="maxLookahead">
        /// Maximum lookahead to use.
        /// </param>
        public DancingGenerator(int sizex, int sizey, Random rnd, int maxLookahead)
            : base(sizex, sizey, rnd, maxLookahead)
        {
        }

        public override void Generate()
        {
            xs = new List<int>();
            ys = new List<int>();
            values = new List<int>();
            int width = sizex * sizey;
            int lastLookaheadUsed = 0;
            int loops = 0;

            SudokuDancingLinks dl = new SudokuDancingLinks(sizey, sizex);
            while (true)
            {
                int x = 0;
                int y = 0;
                bool match = true;
                while (match)
                {
                    x = rnd.Next(width);
                    y = rnd.Next(width);
                    match = false;
                    for (int i = 0; i < xs.Count; i++)
                    {
                        if (xs[i] == x && ys[i] == y)
                        {
                            match = true;
                            break;
                        }
                    }
                }
                int v = rnd.Next(width) + 1;
                xs.Add(x);
                ys.Add(y);
                values.Add(v);
                bool hasOpp = width % 2 == 0 || (x != width / 2 || y != width / 2);
                if (hasOpp)
                {
                    xs.Add(width - 1 - x);
                    ys.Add(width - 1 - y);
                    values.Add(rnd.Next(width) + 1);
                }

                dl.Clear();
                for (int i = 0; i < xs.Count; i++)
                {
                    dl.Grid[ys[i], xs[i]] = values[i];
                }
                dl.Solve();
                if (dl.Count <= 0)
                {
                    if (width % 2 == 0 || (xs[xs.Count - 1] != width / 2 || ys[ys.Count - 1] != width / 2))
                    {
                        xs.RemoveAt(xs.Count - 1);
                        ys.RemoveAt(ys.Count - 1);
                        values.RemoveAt(values.Count - 1);
                    }
                    xs.RemoveAt(xs.Count - 1);
                    ys.RemoveAt(ys.Count - 1);
                    values.RemoveAt(values.Count - 1);
                }
                else if (dl.Count == 1)
                {
                    break;
                }
                loops++;
                // if we go for a long time, try again.
                if (loops > width * width * width)
                {
                    Generate();
                    return;
                }

            }
            List<int> xs2 = new List<int>(xs);
            List<int> ys2 = new List<int>(ys);
            List<int> values2 = new List<int>(values);
            xs = new List<int>();
            ys = new List<int>();
            values = new List<int>();

            while (xs2.Count > 0)
            {
                int choice = rnd.Next(xs2.Count);
                int x = xs2[choice];
                int y = ys2[choice];
                int v = values2[choice];
                int x2 = -1;
                int y2 = -1;
                int v2 = -1;
                if (choice > 0 && xs2[choice - 1] == width - 1 - x && ys2[choice - 1] == width - 1 - y)
                {
                    x2 = width - 1 - x;
                    y2 = width - 1 - y;
                    v2 = values2[choice - 1];
                    xs2.RemoveAt(choice - 1);
                    xs2.RemoveAt(choice - 1);
                    ys2.RemoveAt(choice - 1);
                    ys2.RemoveAt(choice - 1);
                    values2.RemoveAt(choice - 1);
                    values2.RemoveAt(choice - 1);
                }
                else if (choice < xs2.Count - 1 && xs2[choice + 1] == width - 1 - x && ys2[choice + 1] == width - 1 - y)
                {
                    x2 = width - 1 - x;
                    y2 = width - 1 - y;
                    v2 = values2[choice + 1];
                    xs2.RemoveAt(choice);
                    xs2.RemoveAt(choice);
                    ys2.RemoveAt(choice);
                    ys2.RemoveAt(choice);
                    values2.RemoveAt(choice);
                    values2.RemoveAt(choice);

                }
                else
                {
                    xs2.RemoveAt(choice);
                    ys2.RemoveAt(choice);
                    values2.RemoveAt(choice);
                }


                dl.Clear();
                for (int i = 0; i < xs.Count; i++)
                {
                    dl.Grid[ys[i], xs[i]] = values[i];
                }
                for (int i = 0; i < xs2.Count; i++)
                {
                    dl.Grid[ys2[i], xs2[i]] = values2[i];
                }
                dl.Solve();
                if (dl.Count != 1)
                {
                    xs.Add(x);
                    ys.Add(y);
                    values.Add(v);
                    if (x2 != -1)
                    {
                        xs.Add(x2);
                        ys.Add(y2);
                        values.Add(v2);
                    }
                }

            }

            if (ensureDiff)
            {
                Board b = new Board(sizey, sizex);
                b.MaxLookahead = maxLookahead;
                b.Apply(xs, ys, values);
                SolveState result = b.Solve();
                if (result != SolveState.Solved)
                {
                    Generate();
                    return;
                }
                lastLookaheadUsed = b.LastLookaheadUsed;

                if (lastLookaheadUsed != maxLookahead)
                {
                    Generate();
                    return;
                }

            }
        }
    }
}
