using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    public class StraightGenerator : Generator
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
        public StraightGenerator(int sizex, int sizey, Random rnd, int maxLookahead)
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
            int width = sizex*sizey;
            Board workingBoard = null;
            int possibilityCount = 0;
            List<List<int>> a = new List<List<int>>();
            while (result != SolveState.Solved)
            {
                if (result == SolveState.MultipleSolutions)
                {
                    bool needsFollowup = false;
                    int altx = 0;
                    int alty = 0;
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
                            needsFollowup = true;
                            altx = width - 1 - i2;
                            alty = width - 1 - j2;
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
                            int x2 = width - 1- x;
                            int y2 = width - 1 -y;
                            xs.Add(x2);
                            ys.Add(y2);
                            int v2 = rnd.Next(width) + 1;
                            if (x2 == x || y2 == y || (x2/sizex==x/sizex && y2/sizey==y/sizey))
                            {
                                while (v2 == v)
                                {
                                    v2 = rnd.Next(width) + 1;
                                }
                            }
                            values.Add(v2);
                        }
                    }

                    if (workingBoard == null)
                    {
                        workingBoard = new Board(sizey,sizex);
                        // This line used to generate harder games.
                        workingBoard.MaxLookahead = maxLookahead;
                    }

                    // Doesnt hurt to apply the same values more then once.
                    workingBoard.Apply(xs, ys, values);
                    result = workingBoard.Solve();
                    if (result == SolveState.MultipleSolutions || result == SolveState.Solved)
                    {
                        if (needsFollowup)
                        {
                            List<int> poss = new List<int>();
                            for (int i = 0; i < width; i++)
                            {
                                if (workingBoard.CheckPossible(altx, alty, i + 1))
                                {
                                    poss.Add(i + 1);
                                }
                            }
                            if (poss.Count == 0)
                            {
                                result = SolveState.Unsolvable;
                            }
                            else
                            {
                                int choice = rnd.Next(poss.Count);
                                xs.Add(altx);
                                ys.Add(alty);
                                values.Add(poss[choice]);
                                workingBoard.Set(altx, alty, poss[choice]);
                                result = workingBoard.Solve();
                            }
                        }
                    }
                    if (result == SolveState.MultipleSolutions)
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
                                            possibilityCount++;
                                            a.Add(aInner);
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
                    lastLookaheadUsed = workingBoard.LastLookaheadUsed;
                }
                else if (result == SolveState.Unsolvable)
                {
                    // Deadend, try again.  Faster then walking backwards?
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
