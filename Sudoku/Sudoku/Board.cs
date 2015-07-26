using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{

    /// <summary>
    /// A solveable sudoku board.
    /// </summary>
    public class Board
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="size">
        /// Size of game to be played.
        /// </param>
        public Board(int size) : this(size, size)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sizex">
        /// Size of game to be played.
        /// </param>
        /// <param name="sizey">
        /// Second size of game to be played.
        /// </param>
        public Board(int sizex, int sizey) {
            width = sizex*sizey;
            this.sizex = sizex;
            this.sizey = sizey;
            cells = new int[width, width];
            possibles = new bool[width, width, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        possibles[i, j, k] = true;
                    }
                }
            }
        }
        private int sizex;
        private int sizey;
        private int width;

        /// <summary>
        /// Gets the value of a location on the board.
        /// </summary>
        /// <param name="x">
        /// x coordinate.
        /// </param>
        /// <param name="y">
        /// y coordinate.
        /// </param>
        /// <returns>
        /// Value at the coordinates.
        /// </returns>
        public int Get(int x, int y)
        {
            return cells[x, y];
        }

        /// <summary>
        /// Gets whether a particualar value could be set.
        /// </summary>
        /// <param name="x">
        /// x coordinate.
        /// </param>
        /// <param name="y">
        /// y coordinate.
        /// </param>
        /// <param name="v">
        /// Value to check.
        /// </param>
        /// <returns>
        /// If the combination is possible still.
        /// </returns>
        public bool CheckPossible(int x, int y, int v)
        {
            return possibles[x, y, v-1];
        }

        /// <summary>
        /// Sets a location on the board, updating the possibles.
        /// </summary>
        /// <param name="x">
        /// X coordinate to set.
        /// </param>
        /// <param name="y">
        /// y coordianate of location to set.
        /// </param>
        /// <param name="value">
        /// Non-zero value to set at the location.
        /// </param>
        public void Set(int x, int y, int value)
        {
            cells[x,y] = value;
            int cx = (x / sizex) * sizex;
            int cy = (y / sizey) * sizey;
            for (int i = 0; i < width; i++)
            {
                possibles[i,y,value-1] = false;
                possibles[x,i,value-1] = false;
                int tx = (i / sizey) + cx;
                int ty = (i % sizey) + cy;
                possibles[tx,ty,value-1] = false;
                possibles[x, y, i] = false;
            }
            possibles[x,y,value-1] = true;
        }

        /// <summary>
        /// Gets whether the puzzle is full.
        /// </summary>
        public bool Full
        {
            get
            {
                foreach (int cell in cells)
                {
                    if (cell == 0)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Apply the 'only possible solution for this cell' pass.
        /// </summary>
        /// <returns>
        /// Either unsolvable if there is no possible number for a group, or progressing if progress is made, or multiple solutions if no progress is made.
        /// </returns>
        private SolveState PassZeroTrivial()
        {
            SolveState result = SolveState.MultipleSolutions;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (cells[i,j] == 0)
                    {
                        int value = -1;
                        for (int k = 0; k < width && value >= -1; k++)
                        {
                            if (possibles[i, j, k])
                            {
                                if (value == -1)
                                    value = k;
                                else
                                    value = -2;
                            }
                        }
                        if (value == -1)
                            return SolveState.Unsolvable;
                        if (value >= 0)
                        {
                            Set(i, j, value + 1);
                            result = SolveState.Progressing;
                        }
                    }
                }
            }
            return result;
        }

        private List<int> lastxs;
        private List<int> lastys;
        private List<int> lastvalues;

        /// <summary>
        /// Applies both zeroth order passes using delayed application.
        /// </summary>
        /// <returns>
        /// Either unsolvable if there is no possible number for a group, or progressing if progress is made, or multiple solutions if no progress is made.
        /// </returns>
        private SolveState PassZeroSlow()
        {
            SolveState result = SolveState.MultipleSolutions;
            List<int> xs = new List<int>();
            List<int> ys = new List<int>();
            List<int> values = new List<int>();
            bool unsolvable = false;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (cells[i, j] == 0)
                    {
                        int value = -1;
                        for (int k = 0; k < width && value >= -1; k++)
                        {
                            if (possibles[i, j, k])
                            {
                                if (value == -1)
                                    value = k;
                                else
                                    value = -2;
                            }
                        }
                        if (value == -1)
                        {
                            unsolvable = true;
                        }
                        if (value >= 0)
                        {
                            xs.Add(i);
                            ys.Add(j);
                            values.Add(value + 1);
                            if (UseLogging)
                            {
                                log.AppendFormat("{0} only possible in {1},{2}\n", value + 1, i, j);
                            }
                            result = SolveState.Progressing;
                        }
                    }
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int rid = -1;
                    int cid = -1;
                    int oxid = -1;
                    int oyid = -1;
                    int cx = (j / sizex) * sizex;
                    int cy = (j % sizex) * sizey;
                    for (int k = 0; k < width; k++)
                    {
                        if (possibles[j, k, i])
                        {
                            if (rid == -1)
                                rid = k;
                            else
                                rid = -2;
                        }
                        if (possibles[k, j, i])
                        {
                            if (cid == -1)
                                cid = k;
                            else
                                cid = -2;
                        }
                        int tx = (k / sizey) + cx;
                        int ty = (k % sizey) + cy;
                        if (possibles[tx, ty, i])
                        {
                            if (oxid == -1)
                            {
                                oxid = tx;
                                oyid = ty;
                            }
                            else
                                oxid = -2;
                        }
                    }
                    if (cid == -1)
                    {
                        unsolvable = true;
                        continue;
                    }
                    if (rid == -1)
                    {
                        unsolvable = true;
                        continue;
                    }
                    if (oxid == -1)
                    {
                        unsolvable = true;
                        continue;
                    }
                    if (rid != -2 && cells[j, rid] == 0)
                    {
                        xs.Add(j);
                        ys.Add(rid);
                        values.Add(i + 1);
                        if (UseLogging)
                        {
                            log.AppendFormat("{0} only row possible in {1},{2}\n", i + 1, j, rid);
                        }
                        result = SolveState.Progressing;
                    }
                    if (cid != -2 && cells[cid, j] == 0)
                    {
                        xs.Add(cid);
                        ys.Add(j);
                        values.Add(i + 1);
                        if (UseLogging)
                        {
                            log.AppendFormat("{0} only column possible in {1},{2}\n", i + 1, cid, j);
                        }
                        result = SolveState.Progressing;
                    }
                    if (oxid != -2 && cells[oxid, oyid] == 0)
                    {
                        xs.Add(oxid);
                        ys.Add(oyid);
                        values.Add(i + 1);
                        if (UseLogging)
                        {
                            log.AppendFormat("{0} only cell possible in {1},{2}\n", i + 1, oxid, oyid);
                        }
                        result = SolveState.Progressing;
                    }
                }
            }
            Apply(xs, ys, values);
            lastxs = xs;
            lastys = ys;
            lastvalues = values;
            if (unsolvable)
                return SolveState.Unsolvable;
            return result;
        }

        /// <summary>
        /// Applies the 'only place for this number in this group' pass.
        /// </summary>
        /// <returns>
        /// Either unsolvable if there is no possible number for a group, or progressing if progress is made, or multiple solutions if no progress is made.
        /// </returns>
        private SolveState PassZeroNonTrivial()
        {
            SolveState result = SolveState.MultipleSolutions;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int rid = -1;
                    int cid = -1;
                    int oxid = -1;
                    int oyid = -1;
                    int cx = (j / sizex) * sizex;
                    int cy = (j % sizex) * sizey;
                    for (int k = 0; k < width; k++)
                    {
                        if (possibles[j,k,i])
                        {
                            if (rid == -1)
                                rid = k;
                            else
                                rid = -2;
                        }
                        if (possibles[k,j,i])
                        {
                            if (cid == -1)
                                cid = k;
                            else
                                cid = -2;
                        }
                        int tx = (k / sizey) + cx;
                        int ty = (k % sizey) + cy;
                        if (possibles[tx,ty,i])
                        {
                            if (oxid == -1)
                            {
                                oxid = tx;
                                oyid = ty;
                            }
                            else
                                oxid = -2;
                        }
                    }
                    if (cid == -1)
                        return SolveState.Unsolvable;
                    if (rid == -1)
                        return SolveState.Unsolvable;
                    if (oxid == -1)
                        return SolveState.Unsolvable;
                    if (rid != -2 && cells[j,rid] == 0)
                    {
                        Set(j, rid, i + 1);
                        result = SolveState.Progressing;
                    }
                    if (cid != -2 && cells[cid, j] == 0)
                    {
                        Set(cid, j, i + 1);
                        result = SolveState.Progressing;
                    }
                    if (oxid != -2 && cells[oxid, oyid] == 0)
                    {
                        Set(oxid, oyid, i + 1);
                        result = SolveState.Progressing;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets or sets whether to use eliminations found in making progress.
        /// </summary>
        public bool UseEliminations
        {
            get
            {
                return useEliminations;
            }
            set
            {
                useEliminations = value;
            }
        }
        private bool useEliminations = false;

        /// <summary>
        /// Gets or sets whether to log progress during a rating solve.
        /// </summary>
        public bool UseLogging
        {
            get
            {
                return useLogging;
            }
            set
            {
                useLogging = value;
            }
        }
        private bool useLogging = false;

        /// <summary>
        /// Log string generated if using logging.
        /// </summary>
        public string Log
        {
            get
            {
                return log.ToString();
            }
        }
        private StringBuilder log = new StringBuilder();


        /// <summary>
        /// Applies a batch of values.
        /// </summary>
        /// <param name="xs">
        /// X coordinates.
        /// </param>
        /// <param name="ys">
        /// Y coordinates.
        /// </param>
        /// <param name="values">
        /// Values for each coordinate.
        /// </param>
        public void Apply(List<int> xs, List<int> ys, List<int> values)
        {
            for (int i = 0; i < xs.Count; i++)
            {
                Set(xs[i], ys[i], values[i]);
            }
        }


        /// <summary>
        /// Applies a batch of values.
        /// </summary>
        /// <param name="xs">
        /// X coordinates.
        /// </param>
        /// <param name="ys">
        /// Y coordinates.
        /// </param>
        /// <param name="values">
        /// Values for each coordinate.
        /// </param>
        public void ApplyZB(List<int> xs, List<int> ys, List<int> values)
        {
            for (int i = 0; i < xs.Count; i++)
            {
                Set(xs[i], ys[i], values[i]+1);
            }
        }

        /// <summary>
        /// Performs a single 'lookahead' logic attempt.
        /// </summary>
        /// <param name="ys">
        /// Y coordinates to try.
        /// </param>
        /// <param name="xs">
        /// X coordinates to try.
        /// </param>
        /// <param name="values">
        /// Values to try in each location.
        /// </param>
        /// <returns></returns>
        private SolveState PassPartLookaheadLogic(List<int> ys, List<int> xs, List<int> values, int lookahead)
        {
            Board[] boards = new Board[ys.Count];
            SolveState[] results = new SolveState[ys.Count];
            SolveState result = SolveState.MultipleSolutions;
            for (int i = 0; i < boards.Length; i++)
            {
                boards[i] = this.Clone();
                boards[i].maxLookahead = lookahead - 1;
                boards[i].scoring = 0;
                boards[i].Set(xs[i], ys[i], values[i]+1);
                if (scoring == 0 || lookahead > 1)
                {
                    results[i] = boards[i].SolveProper();
                }
                else
                {
                    for (int j = 0; j < scoring - 1; j++)
                    {
                        results[i] = boards[i].PassZeroSlow();
                        if (results[i] != SolveState.Progressing)
                            break;
                    }
                }
            }
            bool allUnsolvable = true;
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] != SolveState.Unsolvable)
                    allUnsolvable = false;
                else
                {
                    if (!useEliminations)
                    {
                        results[i] = SolveState.MultipleSolutions;
                        allUnsolvable = false;
                    }
                    else
                    {
                        if (possibles[xs[i], ys[i], values[i]])
                        {
                            possibles[xs[i], ys[i], values[i]] = false;
                            result = SolveState.Progressing;
                        }
                    }
                }
            }
            if (allUnsolvable)
                return SolveState.Unsolvable;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (cells[i, j] == 0)
                    {
                        for (int k = 0; k < width; k++)
                        {
                            if (possibles[i, j, k])
                            {
                                bool allFalse = true;
                                for (int b = 0; b < boards.Length; b++)
                                {
                                    if (results[b] != SolveState.Unsolvable)
                                    {
                                        if (boards[b].possibles[i, j, k])
                                            allFalse = false;
                                    }
                                }
                                if (allFalse)
                                {
                                    if (scoring != 0)
                                    {
                                        if (useLogging)
                                        {
                                            for (int a = 0; a < xs.Count; a++)
                                            {
                                                log.AppendFormat("({0},{1} {2}) ", xs[a], ys[a], values[a] + 1);
                                            }
                                            if (lookahead <=1)
                                                log.AppendFormat("Eliminated {0},{1} {2} in {3} steps\n", i, j, k + 1, scoring - 1);
                                            else
                                                log.AppendFormat("Eliminated {0},{1} {2} at {3} branches\n", i, j, k + 1, lookahead);
                                            if (scoring > 1)
                                            {
                                                for (int a = 0; a < xs.Count; a++)
                                                {
                                                    List<int> peggedxs = new List<int>();
                                                    peggedxs.Add(xs[a]);
                                                    List<int> peggedys = new List<int>();
                                                    peggedys.Add(ys[a]);
                                                    List<int> peggedvs = new List<int>();
                                                    peggedvs.Add(values[a]+1);
                                                    List<int> peggedwhen = new List<int>();
                                                    peggedwhen.Add(0);
                                                    for (int pegged = 0; pegged < scoring - 1; pegged++)
                                                    {
                                                        Board tb = this.Clone();
                                                        tb.Apply(peggedxs, peggedys, peggedvs);
                                                        tb.PassZeroSlow();
                                                        List<int> possiblexs = new List<int>(tb.lastxs);
                                                        List<int> possibleys = new List<int>(tb.lastys);
                                                        List<int> possiblevs = new List<int>(tb.lastvalues);
                                                        List<int> bestxs = new List<int>(possiblexs);
                                                        List<int> bestys = new List<int>(possibleys);
                                                        List<int> bestvs = new List<int>(possiblevs);
                                                        for (int trial = possiblexs.Count - 1; trial >= 0; trial--)
                                                        {
                                                            tb = this.Clone();
                                                            tb.Apply(peggedxs, peggedys, peggedvs);
                                                            possiblexs.RemoveAt(trial);
                                                            possibleys.RemoveAt(trial);
                                                            possiblevs.RemoveAt(trial);
                                                            tb.Apply(possiblexs, possibleys, possiblevs);
                                                            for (int nextp = pegged + 1; nextp < scoring - 1; nextp++)
                                                            {
                                                                if (tb.PassZeroSlow() != SolveState.Progressing)
                                                                    break;
                                                            }
                                                            if (tb.possibles[i, j, k] != false)
                                                            {
                                                                possiblexs = new List<int>(bestxs);
                                                                possibleys = new List<int>(bestys);
                                                                possiblevs = new List<int>(bestvs);
                                                            }
                                                            else
                                                            {
                                                                bestxs = new List<int>(possiblexs);
                                                                bestys = new List<int>(possibleys);
                                                                bestvs = new List<int>(possiblevs);
                                                            }
                                                        }
                                                        peggedxs.AddRange(bestxs);
                                                        peggedys.AddRange(bestys);
                                                        peggedvs.AddRange(bestvs);
                                                        for (int index = 0; index < bestvs.Count; index++)
                                                        {
                                                            peggedwhen.Add(pegged + 1);
                                                        }
                                                    }
                                                    log.AppendFormat("Trial ({0},{1} {2}):\n", xs[a], ys[a], values[a] + 1);
                                                    string[] splits = boards[a].Log.Split('\n');
                                                    for (int b = 0; b < peggedxs.Count; b++)
                                                    {
                                                        log.AppendFormat(" {3}: {0}, {1} {2}\n", peggedxs[b], peggedys[b], peggedvs[b], peggedwhen[b]);
                                                    }
                                                }

                                            }
                                        }
                                        possibles[i, j, k] = false;
                                        return SolveState.Progressing;
                                    }
                                    possibles[i, j, k] = false;
                                    result = SolveState.Progressing;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Uses a lookahead 'common difference' logic attack for pruning possibilities.
        /// </summary>
        /// <param name="lookahead">
        /// Lookahead to use in testing.
        /// </param>
        /// <returns>
        /// Either unsolvable if there is no possible number for a group, or progressing if progress is made, or multiple solutions if no progress is made.
        /// </returns>
        private SolveState PassLookaheadLogic(int lookahead)
        {
            List<int> rowSpots = new List<int>();
            List<int> columnSpots = new List<int>();
            List<int> values = new List<int>();
            SolveState result = SolveState.MultipleSolutions;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // Multiple possibilities for a number in a column.
                    rowSpots.Clear();
                    columnSpots.Clear();
                    values.Clear();
                    for (int k = 0; k < width; k++)
                    {
                        if (possibles[j, k, i])
                        {
                            rowSpots.Add(k);
                            columnSpots.Add(j);
                            values.Add(i);
                        }
                    }
                    if (rowSpots.Count > 1)
                    {
                        SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead);
                        if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultiplesolutions)
                            return temp;
                        else if (temp == SolveState.Progressing)
                        {
                            result = SolveState.Progressing;
                            if (scoring > 0)
                                return result;
                        }
                    }
                    // Multiple possibilities for a number in a row.
                    rowSpots.Clear();
                    columnSpots.Clear();
                    values.Clear();
                    for (int k = 0; k < width; k++)
                    {
                        if (possibles[k, j, i])
                        {
                            columnSpots.Add(k);
                            rowSpots.Add(j);
                            values.Add(i);
                        }
                    }
                    if (rowSpots.Count > 1)
                    {
                        SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead);
                        if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultiplesolutions)
                            return temp;
                        else if (temp == SolveState.Progressing)
                        {
                            result = SolveState.Progressing;
                            if (scoring > 0)
                                return result;
                        }
                    }
                    // multiple possibilities for a number in a group.
                    rowSpots.Clear();
                    columnSpots.Clear();
                    values.Clear();
                    int cx = (j / sizex) * sizex;
                    int cy = (j % sizex) * sizey;
                    for (int k = 0; k < width; k++)
                    {
                        int tx = (k / sizey) + cx;
                        int ty = (k % sizey) + cy;
                        if (possibles[tx, ty, i])
                        {
                            columnSpots.Add(tx);
                            rowSpots.Add(ty);
                            values.Add(i);
                        }
                    }
                    if (rowSpots.Count > 1)
                    {
                        SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead);
                        if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultiplesolutions)
                            return temp;
                        else if (temp == SolveState.Progressing)
                        {
                            result = SolveState.Progressing;
                            if (scoring > 0)
                                return result;
                        }
                    }
                    if (cells[i, j] == 0)
                    {
                        // multiple possibilities in a single cell.
                        rowSpots.Clear();
                        columnSpots.Clear();
                        values.Clear();
                        for (int k = 0; k < width; k++)
                        {
                            if (possibles[i, j, k])
                            {
                                columnSpots.Add(i);
                                rowSpots.Add(j);
                                values.Add(k);
                            }
                        }
                        if (rowSpots.Count > 1)
                        {
                            SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead);
                            if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultiplesolutions)
                                return temp;
                            else if (temp == SolveState.Progressing)
                            {
                                result = SolveState.Progressing;
                                if (scoring > 0)
                                    return result;
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Uses a lookahead 'common difference' logic attack for pruning possibilities.
        /// </summary>
        /// <param name="depth">
        /// Number of numbers to test simultaneous.
        /// </param>
        /// <param name="lookahead">
        /// Lookahead to use in testing.
        /// </param>
        /// <returns>
        /// Either unsolvable if there is no possible number for a group, or progressing if progress is made, or multiple solutions if no progress is made.
        /// </returns>
        private SolveState PassLookaheadLogic(int depth, int lookahead)
        {
            List<int> rowSpots = new List<int>();
            List<int> columnSpots = new List<int>();
            List<int> values = new List<int>();
            SolveState result = SolveState.MultipleSolutions;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // Multiple possibilities for a number in a column.
                    rowSpots.Clear();
                    columnSpots.Clear();
                    values.Clear();
                    for (int k = 0; k < width; k++)
                    {
                        if (possibles[j, k, i])
                        {
                            rowSpots.Add(k);
                            columnSpots.Add(j);
                            values.Add(i);
                        }
                    }
                    if (rowSpots.Count == depth)
                    {
                        SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead);
                        if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultiplesolutions)
                            return temp;
                        else if (temp == SolveState.Progressing)
                        {
                            result = SolveState.Progressing;
                            if (scoring > 0)
                                return result;
                        }
                    }
                    // Multiple possibilities for a number in a row.
                    rowSpots.Clear();
                    columnSpots.Clear();
                    values.Clear();
                    for (int k = 0; k < width; k++)
                    {
                        if (possibles[k, j, i])
                        {
                            columnSpots.Add(k);
                            rowSpots.Add(j);
                            values.Add(i);
                        }
                    }
                    if (rowSpots.Count == depth)
                    {
                        SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead);
                        if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultiplesolutions)
                            return temp;
                        else if (temp == SolveState.Progressing)
                        {
                            result = SolveState.Progressing;
                            if (scoring > 0)
                                return result;
                        }
                    }
                    // multiple possibilities for a number in a group.
                    rowSpots.Clear();
                    columnSpots.Clear();
                    values.Clear();
                    int cx = (j / sizex) * sizex;
                    int cy = (j % sizex) * sizey;
                    for (int k = 0; k < width; k++)
                    {
                        int tx = (k / sizey) + cx;
                        int ty = (k % sizey) + cy;
                        if (possibles[tx, ty, i])
                        {
                            columnSpots.Add(tx);
                            rowSpots.Add(ty);
                            values.Add(i);
                        }
                    }
                    if (rowSpots.Count == depth)
                    {
                        SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead);
                        if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultiplesolutions)
                            return temp;
                        else if (temp == SolveState.Progressing)
                        {
                            result = SolveState.Progressing;
                            if (scoring > 0)
                                return result;
                        }
                    }
                    if (cells[i, j] == 0)
                    {
                        // multiple possibilities in a single cell.
                        rowSpots.Clear();
                        columnSpots.Clear();
                        values.Clear();
                        for (int k = 0; k < width; k++)
                        {
                            if (possibles[i, j, k])
                            {
                                columnSpots.Add(i);
                                rowSpots.Add(j);
                                values.Add(k);
                            }
                        }
                        if (rowSpots.Count == depth)
                        {
                            SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead);
                            if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultiplesolutions)
                                return temp;
                            else if (temp == SolveState.Progressing)
                            {
                                result = SolveState.Progressing;
                                if (scoring > 0)
                                    return result;
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the highest lookahead used during the solve.
        /// </summary>
        public int LastLookaheadUsed
        {
            get
            {
                return lastLookaheadUsed;
            }
        }
        private int lastLookaheadUsed;

        /// <summary>
        /// Gets/Sets how much to 'cheat' by to find solutions.
        /// </summary>
        public int MaxLookahead
        {
            get
            {
                return maxLookahead;
            }
            set
            {
                maxLookahead = value;
            }
        }
        private int maxLookahead = 0;

        /// <summary>
        /// Attempts to solve the board.
        /// </summary>
        /// <returns>
        /// An enumeration value representing the state of the board after solving.
        /// Progressing will not be returned.
        /// MultipleSolutions may be erroneously returned if maxLookahead is too low.
        /// </returns>
        public SolveState Solve()
        {
            lastLookaheadUsed = 0;
            SolveState result = SolveState.Progressing;
            while (result == SolveState.Progressing)
            {
                result = PassZeroTrivial();
                if (result == SolveState.MultipleSolutions)
                {
                    // the trivial 'solved' pass.
                    if (Full)
                        result = SolveState.Solved;
                    result = PassZeroNonTrivial();
                    // the trivial 'solved' pass.
                    if (result == SolveState.MultipleSolutions && Full)
                        result = SolveState.Solved;
                }
                int counter = 0;
                while (result == SolveState.MultipleSolutions && counter < maxLookahead)
                {
                    result = PassLookaheadLogic(counter + 1);
                    if (lastLookaheadUsed < counter + 1)
                        lastLookaheadUsed = counter + 1;
                    counter++;
                }
            }
            if (result == SolveState.DefiniteMultiplesolutions)
                result = SolveState.MultipleSolutions;
            return result;
        }

        /// <summary>
        /// Attempts to solve the board.
        /// </summary>
        /// <returns>
        /// An enumeration value representing the state of the board after solving.
        /// Progressing will not be returned.
        /// MultipleSolutions may be erroneously returned if maxLookahead is too low.
        /// </returns>
        public SolveState SolveProper()
        {
            lastLookaheadUsed = 0;
            SolveState result = SolveState.Progressing;
            while (result == SolveState.Progressing)
            {
                result = PassZeroSlow();
                if (result == SolveState.MultipleSolutions)
                {
                    // the trivial 'solved' pass.
                    if (Full)
                        result = SolveState.Solved;
                }
                int counter = 0;
                while (result == SolveState.MultipleSolutions && counter < maxLookahead)
                {
                    result = PassLookaheadLogic(counter + 1);
                    if (lastLookaheadUsed < counter + 1)
                        lastLookaheadUsed = counter + 1;
                    counter++;
                }
            }
            if (result == SolveState.DefiniteMultiplesolutions)
                result = SolveState.MultipleSolutions;
            return result;
        }

        /// <summary>
        /// Tracks the difficulty score for nontrivial cases.
        /// </summary>
        public int Score
        {
            get
            {
                return maxScore-1;
            }
        }
        private int maxScore = 0;
        private int scoring = 0;

        /// <summary>
        /// Tracks the difficulty score for nontrivial cases.
        /// </summary>
        public int HighTuples
        {
            get
            {
                return highTuples;
            }
        }
        private int highTuples = 0;

        /// <summary>
        /// Attempts to solve the board, using slower more rating useful methods.
        /// </summary>
        /// <returns>
        /// An enumeration value representing the state of the board after solving.
        /// Progressing will not be returned.
        /// MultipleSolutions may be erroneously returned if maxLookahead is too low.
        /// </returns>
        public SolveState SolveWithRating()
        {
            log = new StringBuilder();
            lastLookaheadUsed = 0;
            maxScore = 1;
            highTuples = 0;
            SolveState result = SolveState.Progressing;
            while (result == SolveState.Progressing)
            {
                result = PassZeroSlow();
                if (result == SolveState.MultipleSolutions)
                {
                    // the trivial 'solved' pass.
                    if (Full)
                        result = SolveState.Solved;
                }
                int counter = 0;
                while (result == SolveState.MultipleSolutions && counter < maxLookahead)
                {
                    scoring = 1;
                    int max = width * width + 1;
                    if (counter > 0)
                        max = 2;
                    while (result == SolveState.MultipleSolutions && scoring < max)
                    {
                        int tuples = 2;
                        int maxTuples = width;
                        if (scoring < 2)
                            maxTuples = sizex > sizey ? sizex : sizey;
                        while (result == SolveState.MultipleSolutions && tuples <= maxTuples)
                        {
                            result = PassLookaheadLogic(tuples, counter + 1);
                            if (result != SolveState.MultipleSolutions)
                            {
                                if (scoring > maxScore)
                                {
                                    maxScore = scoring;
                                    highTuples = tuples;
                                }
                                else if (scoring == maxScore)
                                {
                                    if (tuples > highTuples)
                                    {
                                        highTuples = tuples;
                                    }
                                }
                                if (counter + 1 > lastLookaheadUsed)
                                    lastLookaheadUsed = counter + 1;
                            }
                            tuples++;
                        }
                        scoring++;
                    }
                    counter++;

                }
            }
            if (result == SolveState.DefiniteMultiplesolutions)
                result = SolveState.MultipleSolutions;
            return result;
        }

        private int[,] cells;
        private bool[,,] possibles;

        public Board Clone()
        {
            Board result = (Board)this.MemberwiseClone();
            result.cells = (int[,])cells.Clone();
            result.possibles = (bool[,,])possibles.Clone();
            result.log = new StringBuilder();
            return result;
        }

    }

    /// <summary>
    /// Enumerates the possible states of which solving the board can be in.
    /// </summary>
    public enum SolveState
    {
        Progressing,
        Solved,
        Unsolvable,
        MultipleSolutions,
        DefiniteMultiplesolutions,
    }
}
