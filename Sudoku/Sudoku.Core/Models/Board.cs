using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Sudoku.Core.Solver;

namespace Sudoku.Core.Models;

public record SolverProgress(int StepsCount, int LookaheadDepth, int Score, string CurrentAction);

public class Board
{
    private readonly int sizex;
    private readonly int sizey;
    private readonly int width;
    private int[,] cells;
    private bool[,,] possibles;

    public Board(int size = 3) : this(size, size) { }

    public Board(int sizex, int sizey)
    {
        this.sizex = sizex;
        this.sizey = sizey;
        this.width = sizex * sizey;
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

    public int SizeX => sizex;
    public int SizeY => sizey;
    public int Width => width;
    public int[,] Cells => (int[,])cells.Clone();

    public int Get(int x, int y) => cells[x, y];

    public bool CheckPossible(int x, int y, int v)
    {
        if (v < 1 || v > width) return false;
        return possibles[x, y, v - 1];
    }

    public IReadOnlyList<int> GetCandidates(int x, int y)
    {
        if (cells[x, y] != 0) return Array.Empty<int>();
        var list = new List<int>();
        for (int v = 1; v <= width; v++)
        {
            if (possibles[x, y, v - 1]) list.Add(v);
        }
        return list;
    }

    public void Set(int x, int y, int value)
    {
        cells[x, y] = value;
        if (value <= 0) return;

        int cx = (x / sizey) * sizey;
        int cy = (y / sizex) * sizex;
        for (int i = 0; i < width; i++)
        {
            possibles[i, y, value - 1] = false;
            possibles[x, i, value - 1] = false;
            int tx = cx + (i / sizex);
            int ty = cy + (i % sizex);
            possibles[tx, ty, value - 1] = false;
            possibles[x, y, i] = false;
        }
        possibles[x, y, value - 1] = true;
    }

    public bool Full
    {
        get
        {
            foreach (int cell in cells)
            {
                if (cell == 0) return false;
            }
            return true;
        }
    }

    public bool UseEliminations { get; set; } = false;
    public bool UseLogging { get; set; } = true;

    private readonly StringBuilder log = new();
    public string Log => log.ToString();

    private int lastLookaheadUsed;
    public int LastLookaheadUsed => lastLookaheadUsed;

    private int maxLookahead = 2;
    public int MaxLookahead
    {
        get => maxLookahead;
        set => maxLookahead = value;
    }

    private int maxScore = 1;
    public int Score => Math.Max(0, maxScore - 1);

    private int scoring;
    private int highTuples;
    public int HighTuples => highTuples;

    public string DifficultyRating => (lastLookaheadUsed != 1)
        ? lastLookaheadUsed.ToString()
        : $"{lastLookaheadUsed}.{Score}.{highTuples}";

    private readonly List<int> lastxs = new();
    private readonly List<int> lastys = new();
    private readonly List<int> lastvalues = new();

    public void Apply(List<int> xs, List<int> ys, List<int> values)
    {
        for (int i = 0; i < xs.Count; i++)
        {
            Set(xs[i], ys[i], values[i]);
        }
    }

    public void ApplyZeroBased(List<int> xs, List<int> ys, List<int> values)
    {
        for (int i = 0; i < xs.Count; i++)
        {
            Set(xs[i], ys[i], values[i] + 1);
        }
    }

    /// <summary>
    /// Applies zeroth-order logical deductions (Naked Singles and Hidden Singles).
    /// Prevents duplicate deductions across rows/columns/boxes within the same pass.
    /// </summary>
    public SolveState PassZeroSlow(List<DeductionStep>? structuredSteps = null)
    {
        SolveState result = SolveState.MultipleSolutions;
        List<int> xs = new();
        List<int> ys = new();
        List<int> values = new();
        HashSet<(int r, int c)> deducedCells = new();
        bool unsolvable = false;

        // 1. Naked Singles
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
                            if (value == -1) value = k;
                            else value = -2;
                        }
                    }
                    if (value == -1)
                    {
                        unsolvable = true;
                    }
                    else if (value >= 0)
                    {
                        xs.Add(i);
                        ys.Add(j);
                        values.Add(value + 1);
                        deducedCells.Add((i, j));

                        if (UseLogging)
                        {
                            log.AppendFormat("{0} only possible in {1},{2}\n", value + 1, i, j);
                        }
                        if (structuredSteps != null)
                        {
                            structuredSteps.Add(new DeductionStep(
                                StepNumber: structuredSteps.Count + 1,
                                Row: i,
                                Col: j,
                                Value: value + 1,
                                Type: DeductionType.NakedSingle,
                                Explanation: $"Cell R{i + 1}C{j + 1} must be {FormatValue(value + 1)}: it is the only remaining valid candidate for this cell.",
                                GroupDescription: $"Cell R{i + 1}C{j + 1}"
                            ));
                        }
                        result = SolveState.Progressing;
                    }
                }
            }
        }

        // 2. Hidden Singles in Row, Column, Box
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int rid = -1;
                int cid = -1;
                int oxid = -1;
                int oyid = -1;
                int cx = (j / sizey) * sizey;
                int cy = (j % sizey) * sizex;

                for (int k = 0; k < width; k++)
                {
                    if (possibles[j, k, i])
                    {
                        if (rid == -1) rid = k;
                        else rid = -2;
                    }
                    if (possibles[k, j, i])
                    {
                        if (cid == -1) cid = k;
                        else cid = -2;
                    }
                    int tx = cx + (k / sizex);
                    int ty = cy + (k % sizex);
                    if (possibles[tx, ty, i])
                    {
                        if (oxid == -1) { oxid = tx; oyid = ty; }
                        else oxid = -2;
                    }
                }

                if (cid == -1 || rid == -1 || oxid == -1)
                {
                    unsolvable = true;
                    continue;
                }

                int valToPlace = i + 1;

                // Hidden single in row j, column rid:
                if (rid != -2 && cells[j, rid] == 0 && deducedCells.Add((j, rid)))
                {
                    xs.Add(j);
                    ys.Add(rid);
                    values.Add(valToPlace);
                    if (UseLogging)
                    {
                        log.AppendFormat("{0} only row possible in {1},{2}\n", valToPlace, j, rid);
                    }
                    if (structuredSteps != null)
                    {
                        structuredSteps.Add(new DeductionStep(
                            StepNumber: structuredSteps.Count + 1,
                            Row: j,
                            Col: rid,
                            Value: valToPlace,
                            Type: DeductionType.HiddenSingleRow,
                            Explanation: $"Place {FormatValue(valToPlace)} at R{j + 1}C{rid + 1}: within Row {j + 1}, {FormatValue(valToPlace)} cannot go anywhere else.",
                            GroupDescription: $"Row {j + 1}"
                        ));
                    }
                    result = SolveState.Progressing;
                }

                // Hidden single in col j, row cid:
                if (cid != -2 && cells[cid, j] == 0 && deducedCells.Add((cid, j)))
                {
                    xs.Add(cid);
                    ys.Add(j);
                    values.Add(valToPlace);
                    if (UseLogging)
                    {
                        log.AppendFormat("{0} only column possible in {1},{2}\n", valToPlace, cid, j);
                    }
                    if (structuredSteps != null)
                    {
                        structuredSteps.Add(new DeductionStep(
                            StepNumber: structuredSteps.Count + 1,
                            Row: cid,
                            Col: j,
                            Value: valToPlace,
                            Type: DeductionType.HiddenSingleColumn,
                            Explanation: $"Place {FormatValue(valToPlace)} at R{cid + 1}C{j + 1}: within Column {j + 1}, {FormatValue(valToPlace)} cannot go anywhere else.",
                            GroupDescription: $"Column {j + 1}"
                        ));
                    }
                    result = SolveState.Progressing;
                }

                // Hidden single in box j, cell (oxid, oyid):
                if (oxid != -2 && cells[oxid, oyid] == 0 && deducedCells.Add((oxid, oyid)))
                {
                    xs.Add(oxid);
                    ys.Add(oyid);
                    values.Add(valToPlace);
                    if (UseLogging)
                    {
                        log.AppendFormat("{0} only cell possible in {1},{2}\n", valToPlace, oxid, oyid);
                    }
                    if (structuredSteps != null)
                    {
                        int boxNum = j + 1;
                        structuredSteps.Add(new DeductionStep(
                            StepNumber: structuredSteps.Count + 1,
                            Row: oxid,
                            Col: oyid,
                            Value: valToPlace,
                            Type: DeductionType.HiddenSingleBox,
                            Explanation: $"Place {FormatValue(valToPlace)} at R{oxid + 1}C{oyid + 1}: within Box {boxNum}, {FormatValue(valToPlace)} cannot go anywhere else.",
                            GroupDescription: $"Box {boxNum}"
                        ));
                    }
                    result = SolveState.Progressing;
                }
            }
        }

        Apply(xs, ys, values);
        lastxs.Clear(); lastxs.AddRange(xs);
        lastys.Clear(); lastys.AddRange(ys);
        lastvalues.Clear(); lastvalues.AddRange(values);

        if (unsolvable) return SolveState.Unsolvable;
        return result;
    }

    public SolveState SolveProper()
    {
        SolveState result = SolveState.Progressing;
        while (result == SolveState.Progressing)
        {
            result = PassZeroSlow();
            if (result == SolveState.MultipleSolutions && Full)
                result = SolveState.Solved;
        }
        return result;
    }

    /// <summary>
    /// Performs a lookahead logic branch test with delta-debugging minimization of deductions.
    /// </summary>
    private SolveState PassPartLookaheadLogic(List<int> ys, List<int> xs, List<int> values, int lookahead, List<DeductionStep>? structuredSteps = null)
    {
        Board[] boards = new Board[ys.Count];
        SolveState[] results = new SolveState[ys.Count];
        SolveState result = SolveState.MultipleSolutions;

        for (int i = 0; i < boards.Length; i++)
        {
            boards[i] = Clone();
            boards[i].maxLookahead = lookahead - 1;
            boards[i].scoring = 0;
            boards[i].Set(xs[i], ys[i], values[i] + 1);

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
            {
                allUnsolvable = false;
            }
            else
            {
                if (!UseEliminations)
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

        if (allUnsolvable) return SolveState.Unsolvable;

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
                                if (results[b] != SolveState.Unsolvable && boards[b].possibles[i, j, k])
                                {
                                    allFalse = false;
                                }
                            }

                            if (allFalse)
                            {
                                if (scoring != 0)
                                {
                                    var proofChain = new List<string>();

                                    if (UseLogging)
                                    {
                                        for (int a = 0; a < xs.Count; a++)
                                        {
                                            log.AppendFormat("({0},{1} {2}) ", xs[a], ys[a], values[a] + 1);
                                        }
                                        if (lookahead <= 1)
                                            log.AppendFormat("Eliminated {0},{1} {2} in {3} steps\n", i, j, k + 1, scoring - 1);
                                        else
                                            log.AppendFormat("Eliminated {0},{1} {2} at {3} branches\n", i, j, k + 1, lookahead);

                                        // Step Minimization (Delta-Debugging)
                                        if (scoring > 1)
                                        {
                                            for (int a = 0; a < xs.Count; a++)
                                            {
                                                List<int> peggedxs = new() { xs[a] };
                                                List<int> peggedys = new() { ys[a] };
                                                List<int> peggedvs = new() { values[a] + 1 };
                                                List<int> peggedwhen = new() { 0 };

                                                for (int pegged = 0; pegged < scoring - 1; pegged++)
                                                {
                                                    Board tb = Clone();
                                                    tb.Apply(peggedxs, peggedys, peggedvs);
                                                    tb.PassZeroSlow();

                                                    List<int> possiblexs = new(tb.lastxs);
                                                    List<int> possibleys = new(tb.lastys);
                                                    List<int> possiblevs = new(tb.lastvalues);
                                                    List<int> bestxs = new(possiblexs);
                                                    List<int> bestys = new(possibleys);
                                                    List<int> bestvs = new(possiblevs);

                                                    // Prune unnecessary deduction steps from the chain
                                                    for (int trial = possiblexs.Count - 1; trial >= 0; trial--)
                                                    {
                                                        tb = Clone();
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

                                                        if (tb.possibles[i, j, k])
                                                        {
                                                            // Step was required, revert
                                                            possiblexs = new(bestxs);
                                                            possibleys = new(bestys);
                                                            possiblevs = new(bestvs);
                                                        }
                                                        else
                                                        {
                                                            // Step was redundant! Keep it pruned
                                                            bestxs = new(possiblexs);
                                                            bestys = new(possibleys);
                                                            bestvs = new(possiblevs);
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
                                                for (int b = 0; b < peggedxs.Count; b++)
                                                {
                                                    string chainEntry = $"Step {peggedwhen[b]}: R{peggedxs[b] + 1}C{peggedys[b] + 1}={FormatValue(peggedvs[b])}";
                                                    proofChain.Add(chainEntry);
                                                    log.AppendFormat(" {3}: {0}, {1} {2}\n", peggedxs[b], peggedys[b], peggedvs[b], peggedwhen[b]);
                                                }
                                            }
                                        }
                                    }

                                    if (structuredSteps != null && possibles[i, j, k])
                                    {
                                        structuredSteps.Add(new DeductionStep(
                                            StepNumber: structuredSteps.Count + 1,
                                            Row: i,
                                            Col: j,
                                            Value: k + 1,
                                            Type: DeductionType.LookaheadElimination,
                                            Explanation: $"Eliminated candidate {FormatValue(k + 1)} from R{i + 1}C{j + 1}: branching logic demonstrates {FormatValue(k + 1)} leads to contradiction.",
                                            GroupDescription: $"Lookahead Depth {lookahead}",
                                            ProofChain: proofChain.Count > 0 ? proofChain : null
                                        ));
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

    private SolveState PassLookaheadLogic(int depth, int lookahead, List<DeductionStep>? structuredSteps = null)
    {
        List<int> rowSpots = new();
        List<int> columnSpots = new();
        List<int> values = new();
        SolveState result = SolveState.MultipleSolutions;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                // Column check
                rowSpots.Clear(); columnSpots.Clear(); values.Clear();
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
                    SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead, structuredSteps);
                    if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultipleSolutions) return temp;
                    if (temp == SolveState.Progressing)
                    {
                        result = SolveState.Progressing;
                        if (scoring > 0) return result;
                    }
                }

                // Row check
                rowSpots.Clear(); columnSpots.Clear(); values.Clear();
                for (int k = 0; k < width; k++)
                {
                    if (possibles[k, j, i])
                    {
                        rowSpots.Add(j);
                        columnSpots.Add(k);
                        values.Add(i);
                    }
                }
                if (rowSpots.Count == depth)
                {
                    SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead, structuredSteps);
                    if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultipleSolutions) return temp;
                    if (temp == SolveState.Progressing)
                    {
                        result = SolveState.Progressing;
                        if (scoring > 0) return result;
                    }
                }

                // Box check
                rowSpots.Clear(); columnSpots.Clear(); values.Clear();
                int cx = (j / sizey) * sizey;
                int cy = (j % sizey) * sizex;
                for (int k = 0; k < width; k++)
                {
                    int tx = cx + (k / sizex);
                    int ty = cy + (k % sizex);
                    if (possibles[tx, ty, i])
                    {
                        columnSpots.Add(tx);
                        rowSpots.Add(ty);
                        values.Add(i);
                    }
                }
                if (rowSpots.Count == depth)
                {
                    SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead, structuredSteps);
                    if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultipleSolutions) return temp;
                    if (temp == SolveState.Progressing)
                    {
                        result = SolveState.Progressing;
                        if (scoring > 0) return result;
                    }
                }

                // Cell candidates check
                if (cells[i, j] == 0)
                {
                    rowSpots.Clear(); columnSpots.Clear(); values.Clear();
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
                        SolveState temp = PassPartLookaheadLogic(rowSpots, columnSpots, values, lookahead, structuredSteps);
                        if (temp == SolveState.Unsolvable || temp == SolveState.DefiniteMultipleSolutions) return temp;
                        if (temp == SolveState.Progressing)
                        {
                            result = SolveState.Progressing;
                            if (scoring > 0) return result;
                        }
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Solves the puzzle using logical deductions, records minimized explanation steps, and rates difficulty.
    /// Supports cooperative yielding and cancellation.
    /// </summary>
    public async Task<SudokuSolution> SolveWithRatingAsync(IProgress<SolverProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();
        int[,] initial = (int[,])cells.Clone();
        var structuredSteps = new List<DeductionStep>();

        log.Clear();
        lastLookaheadUsed = 0;
        maxScore = 1;
        highTuples = 0;

        SolveState result = SolveState.Progressing;
        int passCount = 0;

        while (result == SolveState.Progressing)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if ((++passCount % 5) == 0)
            {
                progress?.Report(new SolverProgress(structuredSteps.Count, lastLookaheadUsed, Score, "Evaluating deductions..."));
                await Task.Yield();
            }

            result = PassZeroSlow(structuredSteps);
            if (result == SolveState.MultipleSolutions && Full)
            {
                result = SolveState.Solved;
            }

            int counter = 0;
            while (result == SolveState.MultipleSolutions && counter < maxLookahead)
            {
                cancellationToken.ThrowIfCancellationRequested();
                scoring = 1;
                int max = counter > 0 ? 2 : width * width + 1;

                while (result == SolveState.MultipleSolutions && scoring < max)
                {
                    int tuples = 2;
                    int maxTuples = scoring < 2 ? Math.Max(sizex, sizey) : width;

                    while (result == SolveState.MultipleSolutions && tuples <= maxTuples)
                    {
                        result = PassLookaheadLogic(tuples, counter + 1, structuredSteps);
                        if (result != SolveState.MultipleSolutions)
                        {
                            if (scoring > maxScore)
                            {
                                maxScore = scoring;
                                highTuples = tuples;
                            }
                            else if (scoring == maxScore && tuples > highTuples)
                            {
                                highTuples = tuples;
                            }

                            if (counter + 1 > lastLookaheadUsed)
                            {
                                lastLookaheadUsed = counter + 1;
                            }
                        }
                        tuples++;
                    }
                    scoring++;
                }
                counter++;
            }
        }

        if (result == SolveState.DefiniteMultipleSolutions)
        {
            result = SolveState.MultipleSolutions;
        }

        sw.Stop();

        return new SudokuSolution(
            State: result,
            InitialGrid: initial,
            SolvedGrid: (int[,])cells.Clone(),
            Steps: structuredSteps,
            MaxLookaheadUsed: lastLookaheadUsed,
            Score: Score,
            HighTuples: highTuples,
            Elapsed: sw.Elapsed,
            FullLog: Log
        );
    }

    public SudokuSolution SolveWithRating(CancellationToken cancellationToken = default)
    {
        return SolveWithRatingAsync(null, cancellationToken).GetAwaiter().GetResult();
    }

    /// <summary>
    /// High-speed exact cover solve using Knuth's Algorithm X (DLX).
    /// </summary>
    public SudokuSolution SolveFast()
    {
        var sw = Stopwatch.StartNew();
        int[,] initial = (int[,])cells.Clone();

        var dl = new SudokuDancingLinks(sizex, sizey);
        dl.SetGrid(initial);
        dl.Solve();

        sw.Stop();

        SolveState state;
        int[,] solutionGrid;

        if (dl.Count == 1)
        {
            state = SolveState.Solved;
            solutionGrid = dl.Solution;
            for (int r = 0; r < width; r++)
                for (int c = 0; c < width; c++)
                    Set(r, c, solutionGrid[r, c]);
        }
        else if (dl.Count > 1)
        {
            state = SolveState.MultipleSolutions;
            solutionGrid = (int[,])cells.Clone();
        }
        else
        {
            state = SolveState.Unsolvable;
            solutionGrid = (int[,])cells.Clone();
        }

        return new SudokuSolution(
            State: state,
            InitialGrid: initial,
            SolvedGrid: solutionGrid,
            Steps: Array.Empty<DeductionStep>(),
            MaxLookaheadUsed: 0,
            Score: 0,
            HighTuples: 0,
            Elapsed: sw.Elapsed,
            FullLog: dl.Count == 1 ? "Solved instantly via Dancing Links (DLX)." : "Dancing Links: " + state
        );
    }

    public Board Clone()
    {
        var b = new Board(sizex, sizey)
        {
            maxLookahead = this.maxLookahead,
            UseEliminations = this.UseEliminations,
            UseLogging = this.UseLogging,
            scoring = this.scoring
        };
        b.cells = (int[,])cells.Clone();
        b.possibles = (bool[,,])possibles.Clone();
        return b;
    }

    /// <summary>
    /// Parses any Sudoku board: flat strings (81 / 36 / 256 chars), or formatted multi-line ASCII drawings with |, +, -, spaces.
    /// Supports values 1-9 and letters A-G for boards up to 16x16.
    /// </summary>
    public static Board Parse(string input, int sizex = 3, int sizey = 3)
    {
        var board = new Board(sizex, sizey);
        int width = sizex * sizey;

        // Split by lines to check for multi-line ASCII format
        string[] rawLines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        var contentLines = new List<string>();

        foreach (var line in rawLines)
        {
            string trimmed = line.Trim();
            // Ignore horizontal divider lines like "---+---+---" or "===+===+==="
            if (trimmed.Length > 0 && trimmed.All(ch => ch is '-' or '+' or '=' or '_' or ' '))
                continue;
            contentLines.Add(line);
        }

        if (contentLines.Count == width)
        {
            // Multi-line line-by-line parsing
            for (int r = 0; r < width; r++)
            {
                int c = 0;
                string line = contentLines[r];
                for (int i = 0; i < line.Length && c < width; i++)
                {
                    char ch = line[i];
                    if (ch is '|' or '+' or '-' or ' ' or '\t') continue;
                    int val = ParseValue(ch);
                    if (val > 0 && val <= width)
                    {
                        board.Set(r, c, val);
                    }
                    c++;
                }
            }
            return board;
        }

        // Fallback: continuous character stream
        int cellIndex = 0;
        for (int i = 0; i < input.Length && cellIndex < width * width; i++)
        {
            char ch = input[i];
            if (ch is '|' or '+' or '-' or '=' or '_' or ' ' or '\t' or '\r' or '\n') continue;

            int r = cellIndex / width;
            int c = cellIndex % width;
            int val = ParseValue(ch);

            if (val > 0 && val <= width)
            {
                board.Set(r, c, val);
            }
            cellIndex++;
        }

        return board;
    }

    public static string FormatValue(int value)
    {
        if (value <= 0) return ".";
        if (value < 10) return value.ToString();
        return ((char)('A' + value - 10)).ToString();
    }

    public static int ParseValue(char ch)
    {
        if (ch is '.' or '0') return 0;
        if (char.IsDigit(ch)) return ch - '0';
        if (char.IsLetter(ch)) return char.ToUpperInvariant(ch) - 'A' + 10;
        return 0;
    }

    public string ToSimpleString()
    {
        var sb = new StringBuilder(width * width);
        for (int r = 0; r < width; r++)
        {
            for (int c = 0; c < width; c++)
            {
                sb.Append(FormatValue(cells[r, c]));
            }
        }
        return sb.ToString();
    }

    public string ToAsciiString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < width; i++)
        {
            if (i > 0 && i % sizey == 0)
            {
                for (int j = 0; j < width; j++)
                {
                    if (j > 0 && j % sizex == 0) sb.Append('+');
                    sb.Append('-');
                }
                sb.AppendLine();
            }

            for (int j = 0; j < width; j++)
            {
                if (j > 0 && j % sizex == 0) sb.Append('|');
                sb.Append(FormatValue(cells[i, j]));
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

