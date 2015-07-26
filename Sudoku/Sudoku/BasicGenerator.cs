using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    public class BasicGenerator : Generator
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
        public BasicGenerator(int sizex, int sizey, Random rnd, int maxLookahead)
            : base(sizex, sizey, rnd, maxLookahead)
        {
        }

        public override void Generate()
        {
            xs = new List<int>();
            ys = new List<int>();
            values = new List<int>();
            int lastLookaheadUsed = 0;
            SolveState result = SolveState.MultipleSolutions;
            int width = sizex * sizey;
            int loops = 0;
            Board workingBoard = null;
            int possibilityCount = 0;
            List<List<int>> a = new List<List<int>>();
            while (result != SolveState.Solved)
            {
                if (result == SolveState.MultipleSolutions)
                {

                    if (workingBoard != null)
                    {
                        int rndnum = rnd.Next(possibilityCount);
                        int i2 = a[rndnum][0];
                        int j2 = a[rndnum][1];
                        int k2 = a[rndnum][2];
                        bool hasOpp = width % 2 == 0 || (i2 != width / 2 || j2 != width / 2);
                        xs.Add(i2);
                        ys.Add(j2);
                        values.Add(k2 + 1);
                        if (hasOpp)
                        {
                            int rnd2 = rnd.Next(a[rndnum].Count - 3) + 3;
                            int l2 = a[rndnum][rnd2];
                            xs.Add(width - 1 - i2);
                            ys.Add(width - 1 - j2);
                            values.Add(l2 + 1);
                        }
                    }
                    else
                    {
                        int x = rnd.Next(width);
                        int y = rnd.Next(width);
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
                    }

                    Board board = new Board(sizey, sizex);
                    // This line used to generate harder games.
                    board.MaxLookahead = maxLookahead;

                    board.Apply(xs, ys, values);
                    result = board.Solve();
                    if (result == SolveState.MultipleSolutions)
                    {
                        workingBoard = board;
                        a = new List<List<int>>();
                        possibilityCount = 0;
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                if (workingBoard.Get(i, j) == 0)
                                {
                                    if (workingBoard.Get(width - 1 - i, width - 1 - j) == 0)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            if (workingBoard.CheckPossible(i, j, k + 1))
                                            {
                                                List<int> aInner = new List<int>();
                                                aInner.Add(i);
                                                aInner.Add(j);
                                                aInner.Add(k);
                                                for (int l = 0; l < width; l++)
                                                {
                                                    if (workingBoard.CheckPossible(width - 1 - i, width - 1 - j, l + 1))
                                                    {
                                                        aInner.Add(l);
                                                    }
                                                }
                                                if (aInner.Count >= 4)
                                                {
                                                    a.Add(aInner);
                                                    possibilityCount++;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        if (possibilityCount == 0)
                        {
                            a = new List<List<int>>();
                            possibilityCount = 0;
                            for (int i = 0; i < width; i++)
                            {
                                for (int j = 0; j < width; j++)
                                {
                                    if (workingBoard.Get(i, j) == 0)
                                    {
                                        for (int k = 0; k < width; k++)
                                        {
                                            if (workingBoard.CheckPossible(i, j, k + 1))
                                            {
                                                List<int> aInner = new List<int>();
                                                aInner.Add(i);
                                                aInner.Add(j);
                                                aInner.Add(k);
                                                for (int l = 0; l < width; l++)
                                                {
                                                    if (workingBoard.CheckPossible(width - 1 - i, width - 1 - j, l + 1))
                                                    {
                                                        aInner.Add(l);
                                                    }
                                                }
                                                if (aInner.Count >= 4)
                                                {
                                                    a.Add(aInner);
                                                    possibilityCount++;
                                                }
                                            }
                                        }

                                    }
                                }
                            }
                            if (possibilityCount == 0)
                            {
                                // Deadend, try again.  Faster then walking backwards?
                                Generate();
                                return;
                            }
                        }
                    }
                    lastLookaheadUsed = board.LastLookaheadUsed;
                }
                else if (result == SolveState.Unsolvable)
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
                    result = SolveState.MultipleSolutions;
                }
                loops++;
                // if we go for a long time, try again.
                if (loops > width * width * width)
                {
                    Generate();
                    return;
                }
            }

            if (ensureDiff && lastLookaheadUsed != maxLookahead)
            {
                Generate();
                return;
            }
        }
    }
}
