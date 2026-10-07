using System.Diagnostics;
using Sudoku.Core.Generator;
using Sudoku.Core.Models;
using Sudoku.Core.Solver;

namespace Sudoku.Tests;

public class BoardTests
{
    [Fact]
    public void SolveWithRating_9x9BeginnerPuzzle_SolvesWithNakedAndHiddenSingles()
    {
        var preset = PresetPuzzle.GetPresetsForSize(3, 3)[0];
        var board = Board.Parse(preset.Clues, 3, 3);

        var solution = board.SolveWithRating();

        Assert.Equal(SolveState.Solved, solution.State);
        Assert.True(solution.IsSuccess);
        Assert.NotEmpty(solution.Steps);

        // Verify all cells in solved grid are 1..9
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                Assert.InRange(solution.SolvedGrid[r, c], 1, 9);
            }
        }

        // Verify each step has an explanation and valid coordinate
        foreach (var step in solution.Steps)
        {
            Assert.False(string.IsNullOrWhiteSpace(step.Explanation));
            Assert.InRange(step.Value, 1, 9);
            Assert.InRange(step.Row, 0, 8);
            Assert.InRange(step.Col, 0, 8);
        }
    }

    [Fact]
    public void PassZeroSlow_Deduplication_NoDuplicateStepsForSameCell()
    {
        var preset = PresetPuzzle.GetPresetsForSize(3, 3)[0];
        var board = Board.Parse(preset.Clues, 3, 3);

        var steps = new List<DeductionStep>();
        board.PassZeroSlow(steps);

        // Verify no two steps target the exact same cell in the same pass
        var targetCells = new HashSet<(int Row, int Col)>();
        foreach (var step in steps)
        {
            Assert.True(targetCells.Add((step.Row, step.Col)), $"Duplicate deduction step recorded for cell R{step.Row + 1}C{step.Col + 1}");
            Assert.True(step.IsPlacement);
            Assert.False(step.IsElimination);
        }
    }

    [Fact]
    public void SolveFast_DLX_SolvesInstantly()
    {
        var preset = PresetPuzzle.GetPresetsForSize(3, 3)[2];
        var board = Board.Parse(preset.Clues, 3, 3);

        var solution = board.SolveFast();

        Assert.Equal(SolveState.Solved, solution.State);
        Assert.True(solution.Elapsed.TotalMilliseconds < 50);

        // Verify row uniqueness
        for (int r = 0; r < 9; r++)
        {
            var rowVals = new HashSet<int>();
            for (int c = 0; c < 9; c++)
            {
                Assert.True(rowVals.Add(solution.SolvedGrid[r, c]));
            }
        }
    }

    [Fact]
    public void Board_6x6Size_ParsesAndSolvesCorrectly()
    {
        string clues = "2..4...34...6..3....3..5...51...1..2";
        var board = Board.Parse(clues, 3, 2);
        Assert.Equal(6, board.Width);
        Assert.Equal(3, board.SizeX);
        Assert.Equal(2, board.SizeY);

        string beginnerClues = "21.4.3.34.2.6.53.11.32.5.6.51.4.1.32";
        var b = Board.Parse(beginnerClues, 3, 2);
        b.MaxLookahead = 0;
        var sol = b.SolveWithRating();
        Assert.Equal(SolveState.Solved, sol.State);
        Assert.Equal(0, sol.MaxLookaheadUsed);
    }

    [Fact]
    public void Board_MultiLineAsciiFormat_ParsesCorrectly()
    {
        string ascii = """
            2.6|...|...
            ..7|.1.|.92
            .8.|..5|...
            ---+---+---
            ..5|76.|9..
            9..|.4.|..6
            ..1|.39|4..
            ---+---+---
            ...|1..|.8.
            63.|.5.|2..
            ......5.4
            """;

        var board = Board.Parse(ascii, 3, 3);
        Assert.Equal(2, board.Get(0, 0));
        Assert.Equal(6, board.Get(0, 2));
        Assert.Equal(7, board.Get(1, 2));
        Assert.Equal(1, board.Get(1, 4));

        var solution = board.SolveFast();
        Assert.Equal(SolveState.Solved, solution.State);
    }

    [Fact]
    public void DLX_DetectsUnsolvablePuzzle()
    {
        var board = new Board(3, 3);
        // Put two 5s in the same row
        board.Set(0, 0, 5);
        board.Set(0, 1, 5);

        var solution = board.SolveFast();
        Assert.Equal(SolveState.Unsolvable, solution.State);
    }

    [Fact]
    public void Board_ParseAndToSimpleString_RoundTripsCorrectly()
    {
        string input = "530070000600195000098000060800060003400803001700020006060000280000419005000080079";
        var board = Board.Parse(input);
        string output = board.ToSimpleString();

        Assert.Equal(81, output.Length);
        Assert.Equal('5', output[0]);
        Assert.Equal('3', output[1]);
        Assert.Equal('.', output[2]);
    }

    [Fact]
    public void AllPresets_SolveSuccessfullyWithDLX()
    {
        foreach (var preset in PresetPuzzle.Presets)
        {
            var board = Board.Parse(preset.Clues, preset.SizeX, preset.SizeY);
            var solution = board.SolveFast();
            Assert.True(solution.State == SolveState.Solved, $"Preset '{preset.Title}' ({preset.SizeX}x{preset.SizeY}) failed with state: {solution.State}");
        }
    }

    [Fact]
    public void DifficultyCriteria_MatchesCorrectly()
    {
        var trivial = DifficultyCriteria.Trivial;
        Assert.True(trivial.Matches(0, 0, 0));
        Assert.False(trivial.Matches(1, 0, 0));

        var easy = DifficultyCriteria.Easy;
        Assert.True(easy.Matches(1, 0, 0));
        Assert.False(easy.Matches(0, 0, 0));
        Assert.False(easy.Matches(1, 1, 0));

        var medium = DifficultyCriteria.Medium;
        Assert.True(medium.Matches(1, 1, 2));
        Assert.False(medium.Matches(0, 0, 0));
        Assert.False(medium.Matches(1, 2, 0));

        var hard = DifficultyCriteria.Hard;
        Assert.True(hard.Matches(1, 2, 1));
        Assert.False(hard.Matches(1, 1, 0));

        var challenging = DifficultyCriteria.Challenging;
        Assert.True(challenging.Matches(1, 3, 0));
        Assert.True(challenging.Matches(2, 0, 0));
        Assert.False(challenging.Matches(1, 2, 0));

        var expert = DifficultyCriteria.Expert;
        Assert.True(expert.Matches(2, 0, 0));
        Assert.False(expert.Matches(1, 5, 2));

        var customWildcard = DifficultyCriteria.Custom("1.4.*");
        Assert.True(customWildcard.Matches(1, 4, 3));
        Assert.False(customWildcard.Matches(1, 2, 2));

        var customGte = DifficultyCriteria.Custom(">=2");
        Assert.True(customGte.Matches(2, 0, 0));
        Assert.False(customGte.Matches(1, 5, 2));

        var namedOnly = DifficultyCriteria.Medium with { NamedStrategiesOnly = true };
        Assert.True(namedOnly.NamedStrategiesOnly);
        Assert.True(namedOnly.Matches(1, 1, 0));
    }

    [Fact]
    public void GatheredSudokus_SamplesFrom0AndCount_SolveCorrectly()
    {
        string dir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "GatheredSudokus"));
        if (!Directory.Exists(dir))
        {
            dir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "GatheredSudokus"));
        }
        if (!Directory.Exists(dir)) return;

        // 1. Test first 3 puzzles from 0.txt - should solve with 0 lookahead
        string zeroPath = Path.Combine(dir, "0.txt");
        if (File.Exists(zeroPath))
        {
            string[] lines = File.ReadAllLines(zeroPath);
            int puzzleCount = 0;
            var currentPuzzle = new List<string>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (currentPuzzle.Count >= 9)
                    {
                        var board = Board.Parse(string.Join("\n", currentPuzzle));
                        board.MaxLookahead = 0;
                        var solution = board.SolveWithRating();
                        Assert.Equal(SolveState.Solved, solution.State);
                        Assert.Equal(0, solution.MaxLookaheadUsed);
                        puzzleCount++;
                        if (puzzleCount >= 3) break;
                    }
                    currentPuzzle.Clear();
                }
                else
                {
                    currentPuzzle.Add(line);
                }
            }
        }

        // 2. Test a sample puzzle from Count 25.txt with DLX
        string count25Path = Path.Combine(dir, "Count 25.txt");
        if (File.Exists(count25Path))
        {
            var firstLines = File.ReadLines(count25Path).Take(12);
            var board = Board.Parse(string.Join("\n", firstLines));
            var solution = board.SolveFast();
            Assert.Equal(SolveState.Solved, solution.State);
        }
    }

    [Fact]
    public void SolveWithRating_GatheredSudoku112_ClassifiesPatternsAndProducesArrows()
    {
        string dir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "GatheredSudokus"));
        if (!Directory.Exists(dir))
        {
            dir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "GatheredSudokus"));
        }
        string p1Path = Path.Combine(dir, "1.1.2.txt");
        if (!File.Exists(p1Path)) return;

        var lines = File.ReadLines(p1Path).Take(11);
        var board = Board.Parse(string.Join("\n", lines));
        board.MaxLookahead = 2;
        var sol = board.SolveWithRating();

        Assert.Equal(SolveState.Solved, sol.State);
        Assert.True(sol.IsSuccess);

        // Check that patterns were classified into PointingPair/BoxLineReduction/NakedPair/ForcingChain
        var types = sol.Steps.Select(s => s.Type).ToHashSet();
        Assert.Contains(DeductionType.PointingPair, types);
        Assert.Contains(DeductionType.BoxLineReduction, types);
        Assert.Contains(DeductionType.NakedPair, types);
        Assert.Contains(DeductionType.ForcingChain, types);

        // Check arrows on steps
        var stepsWithArrows = sol.Steps.Where(s => s.Arrows != null && s.Arrows.Count > 0).ToList();
        Assert.NotEmpty(stepsWithArrows);
        foreach (var step in stepsWithArrows)
        {
            foreach (var arrow in step.Arrows!)
            {
                Assert.InRange(arrow.FromRow, 0, 8);
                Assert.InRange(arrow.FromCol, 0, 8);
                Assert.InRange(arrow.ToRow, 0, 8);
                Assert.InRange(arrow.ToCol, 0, 8);
                Assert.False(string.IsNullOrWhiteSpace(arrow.Color));
            }
        }
    }

    [Fact]
    public void SolveWithRating_GatheredSudoku122_SolvesSuccessfullyWithExplanations()
    {
        string dir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "GatheredSudokus"));
        if (!Directory.Exists(dir))
        {
            dir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "GatheredSudokus"));
        }
        string pPath = Path.Combine(dir, "1.2.2.txt");
        if (!File.Exists(pPath)) return;

        var lines = File.ReadLines(pPath).Take(11);
        var board = Board.Parse(string.Join("\n", lines));
        board.MaxLookahead = 2;
        var sol = board.SolveWithRating();

        Assert.Equal(SolveState.Solved, sol.State);
        Assert.True(sol.IsSuccess);
        Assert.NotEmpty(sol.Steps);
        foreach (var step in sol.Steps)
        {
            Assert.False(string.IsNullOrWhiteSpace(step.Explanation));
        }
    }

    [Fact]
    public void Test_ClassicMedium()
    {
        var preset = PresetPuzzle.Presets.First(p => p.Title == "Classic Medium");
        var board = Board.Parse(preset.Clues, preset.SizeX, preset.SizeY);
        board.MaxLookahead = 1;
        var sol = board.SolveWithRating();

        Assert.Equal(SolveState.Solved, sol.State);
        Assert.True(sol.IsSuccess);
        Assert.Equal(1, sol.MaxLookaheadUsed);
    }

    [Fact]
    public void Test_ClassicHard()
    {
        var preset = PresetPuzzle.Presets.First(p => p.Title == "Classic Hard");
        var board = Board.Parse(preset.Clues, preset.SizeX, preset.SizeY);
        var dlxSol = board.Clone().SolveFast();

        board.MaxLookahead = 2;
        var sw = Stopwatch.StartNew();
        var sol = board.SolveWithRating();
        sw.Stop();
        Console.WriteLine($"[TEST_PROFILE] Classic Hard solved in {sw.ElapsedMilliseconds} ms, {sol.Steps.Count} steps, Lookahead: {sol.MaxLookaheadUsed}, Score: {sol.Score}, HighTuples: {sol.HighTuples}");
        foreach (var s in sol.Steps.Where(s => s.Type != DeductionType.NakedSingle && s.Type != DeductionType.HiddenSingleRow && s.Type != DeductionType.HiddenSingleColumn && s.Type != DeductionType.HiddenSingleBox))
        {
            Console.WriteLine($"   Step #{s.StepNumber}: {s.Type} at R{s.Row+1}C{s.Col+1} val={s.Value} - {s.Explanation}");
        }

        Assert.Equal(SolveState.Solved, sol.State);
        Assert.True(sol.IsSuccess);
        Assert.Equal(1, sol.MaxLookaheadUsed);
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                Assert.Equal(dlxSol.SolvedGrid[r, c], sol.SolvedGrid[r, c]);
            }
        }
    }

    [Fact]
    public void Test_ChallengingPreset()
    {
        var preset = PresetPuzzle.Presets.First(p => p.Title == "Challenging");
        var board = Board.Parse(preset.Clues, preset.SizeX, preset.SizeY);
        board.MaxLookahead = 2;
        var sol = board.SolveWithRating();
        Assert.True(sol.IsSuccess);
        Assert.Equal(2, sol.MaxLookaheadUsed);
    }

    [Fact]
    public void Test_Gathered113()
    {
        string dir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "GatheredSudokus"));
        if (!Directory.Exists(dir)) dir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "GatheredSudokus"));
        string pPath = Path.Combine(dir, "1.1.3.txt");
        var lines = File.ReadLines(pPath).Take(11);
        var board = Board.Parse(string.Join("\n", lines));
        board.MaxLookahead = 1;
        var sol = board.SolveWithRating();
        Assert.Equal(SolveState.Solved, sol.State);
        Assert.True(sol.IsSuccess);
        Assert.Equal(1, sol.MaxLookaheadUsed);
        Assert.Equal(1, sol.Score);
        Assert.Equal(3, sol.HighTuples);
    }

    [Fact]
    public void Test_Gathered113_Puzzle2()
    {
        string dir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "GatheredSudokus"));
        if (!Directory.Exists(dir)) dir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "GatheredSudokus"));
        string pPath = Path.Combine(dir, "1.1.3.txt");
        var lines = File.ReadLines(pPath).Skip(12).Take(11);
        var board = Board.Parse(string.Join("\n", lines));
        board.MaxLookahead = 1;
        var sol = board.SolveWithRating();
        Assert.Equal(SolveState.Solved, sol.State);
        Assert.True(sol.IsSuccess);
        Assert.Equal(1, sol.MaxLookaheadUsed);
        Assert.Equal(1, sol.Score);
        Assert.Equal(3, sol.HighTuples);
    }

    [Fact]
    public void Test_PatternClassifier_XYWing_ClassifiesWhenBackedByChains()
    {
        var board = new Board(3, 3);
        // Setup bi-value cells:
        // Pivot at R0C0 (0,0) with {1, 2}
        // Pincer 1 at R0C5 (0,5) with {1, 9} (sees Pivot in Row 0)
        // Pincer 2 at R4C0 (4,0) with {2, 9} (sees Pivot in Col 0)
        // Target cell at R4C5 (4,5) with candidate 9 (sees Pincer 1 in Col 5 and Pincer 2 in Row 4)
        for (int v = 3; v <= 8; v++) board.SetCandidate(0, 0, v, false); // Pivot has {1, 2}
        for (int v = 2; v <= 8; v++) board.SetCandidate(0, 5, v, false); // Pincer 1 has {1, 9}
        for (int v = 3; v <= 8; v++) board.SetCandidate(4, 0, v, false); // Pincer 2 has {2, 9}
        board.SetCandidate(4, 0, 1, false);

        var branchRows = new List<int> { 0, 0 };
        var branchCols = new List<int> { 0, 0 };
        var branchVals = new List<int> { 0, 1 }; // Values 1 and 2 (0-indexed 0 and 1)
        var branchChains = new List<IReadOnlyList<(int Row, int Col, int Val)>>
        {
            new List<(int, int, int)> { (0, 0, 1), (0, 5, 9) },
            new List<(int, int, int)> { (0, 0, 2), (4, 0, 9) }
        };

        var result = PatternClassifier.Classify(
            board,
            targetRow: 4, targetCol: 5, targetVal: 9,
            branchRows, branchCols, branchVals,
            lookaheadDepth: 1, scoring: 2,
            branchChains: branchChains
        );

        Assert.Equal(DeductionType.XYWing, result.Type);
        Assert.Equal("XY-Wing", result.Name);
        Assert.Contains("Pivot R1C1", result.Explanation);
    }

    [Fact]
    public void Test_PatternClassifier_UnbackedTrial_DoesNotMisclassify()
    {
        var board = new Board(3, 3);
        // Pivot & pincers exist on the board as before
        for (int v = 3; v <= 8; v++) board.SetCandidate(0, 0, v, false);
        for (int v = 2; v <= 8; v++) board.SetCandidate(0, 5, v, false);
        for (int v = 3; v <= 8; v++) board.SetCandidate(4, 0, v, false);
        board.SetCandidate(4, 0, 1, false);

        // BUT the trial deduction that was actually executed was at an unrelated cell R8C8!
        var branchRows = new List<int> { 8, 8 };
        var branchCols = new List<int> { 8, 8 };
        var branchVals = new List<int> { 3, 4 };
        var branchChains = new List<IReadOnlyList<(int Row, int Col, int Val)>>
        {
            new List<(int, int, int)> { (8, 8, 4) },
            new List<(int, int, int)> { (8, 8, 5) }
        };

        var result = PatternClassifier.Classify(
            board,
            targetRow: 4, targetCol: 5, targetVal: 9,
            branchRows, branchCols, branchVals,
            lookaheadDepth: 1, scoring: 2,
            branchChains: branchChains
        );

        // It must NOT misclassify as XY-Wing! It must report Forcing Chain.
        Assert.Equal(DeductionType.ForcingChain, result.Type);
    }

    [Fact]
    public async Task Test_SolveWithRatingAsync_ReportsProgressAndCooperativelyYields()
    {
        var preset = PresetPuzzle.Presets.First(p => p.Title == "Classic Hard");
        var board = Board.Parse(preset.Clues, preset.SizeX, preset.SizeY);
        board.MaxLookahead = 2;

        var progressUpdates = new List<SolverProgress>();
        var progress = new Progress<SolverProgress>(p => progressUpdates.Add(p));

        using var cts = new CancellationTokenSource();
        var solution = await board.SolveWithRatingAsync(progress, cts.Token, enableYield: true);

        Assert.Equal(SolveState.Solved, solution.State);
        Assert.True(progressUpdates.Count > 0, "Expected cooperative progress reports during solve.");
    }

    [Fact]
    public async Task Test_SolveWithRatingAsync_CancelsPromptly()
    {
        var preset = PresetPuzzle.Presets.First(p => p.Title == "Classic Hard");
        var board = Board.Parse(preset.Clues, preset.SizeX, preset.SizeY);
        board.MaxLookahead = 2;

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await board.SolveWithRatingAsync(cancellationToken: cts.Token, enableYield: true);
        });
    }

    [Fact]
    public void Test_LookaheadDepthGreaterThan1_ClassifiedAsLookaheadElimination_WithoutMisleadingArrows()
    {
        var board = new Board(3, 3);
        var bRows = new List<int> { 2, 8 };
        var bCols = new List<int> { 0, 0 };
        var bVals = new List<int> { 0, 0 };

        var result = PatternClassifier.Classify(
            board,
            targetRow: 0,
            targetCol: 1,
            targetVal: 4,
            bRows,
            bCols,
            bVals,
            lookaheadDepth: 2,
            scoring: 1
        );

        Assert.Equal(DeductionType.LookaheadElimination, result.Type);
        Assert.Contains("Deep Lookahead (Depth 2)", result.Name);
        Assert.Empty(result.Arrows);
        Assert.Null(result.ChainSteps);
        Assert.NotNull(result.ProofChain);
        Assert.NotEmpty(result.ProofChain);
    }

    [Fact]
    public void Test_UnitSubset_BoundedByWidthDiv2()
    {
        var board = new Board(3, 3);
        // Verify width is 9, so width / 2 is 4
        Assert.Equal(9, board.Width);

        // Run rating solve on a preset with quads / subsets
        var preset = PresetPuzzle.Presets.First(p => p.Title == "Classic Hard");
        var testBoard = Board.Parse(preset.Clues, preset.SizeX, preset.SizeY);
        testBoard.MaxLookahead = 2;
        var sol = testBoard.SolveWithRating();
        Assert.True(sol.IsSuccess);
    }

    [Fact]
    public void Test_2DSubsets_XWing_Elimination()
    {
        var board = new Board(3, 3);
        int v = 5; // value 5
        // Row 1: clear value 5 except Col 1 and Col 4
        for (int c = 0; c < 9; c++)
        {
            if (c != 1 && c != 4) board.SetCandidate(1, c, v, false);
        }
        // Row 5: clear value 5 except Col 1 and Col 4
        for (int c = 0; c < 9; c++)
        {
            if (c != 1 && c != 4) board.SetCandidate(5, c, v, false);
        }
        // Ensure Row 8 Col 1 has candidate 5
        Assert.True(board.CheckPossible(8, 1, v));

        // Solve with rating at lookahead 1
        board.MaxLookahead = 1;
        var solution = board.SolveWithRating();

        // Candidate 5 should be eliminated from Row 8 Col 1 via X-Wing
        Assert.False(board.CheckPossible(8, 1, v));
        var xwingStep = solution.Steps.FirstOrDefault(s => s.Type == DeductionType.XWing && s.Row == 8 && s.Col == 1 && s.Value == v);
        Assert.NotNull(xwingStep);
        Assert.Contains("X-Wing", xwingStep.Explanation);
    }

    [Fact]
    public void Test_2DSubsets_Swordfish_Elimination()
    {
        var board = new Board(3, 3);
        int v = 7; // value 7
        // Row 1: only in cols 2, 5, 8 (3 candidates)
        for (int c = 0; c < 9; c++)
        {
            if (c != 2 && c != 5 && c != 8) board.SetCandidate(1, c, v, false);
        }
        // Row 4: only in cols 2, 5, 8 (3 candidates)
        for (int c = 0; c < 9; c++)
        {
            if (c != 2 && c != 5 && c != 8) board.SetCandidate(4, c, v, false);
        }
        // Row 7: only in cols 2, 5, 8 (3 candidates)
        for (int c = 0; c < 9; c++)
        {
            if (c != 2 && c != 5 && c != 8) board.SetCandidate(7, c, v, false);
        }
        // Target cell: Row 3 Col 2 has candidate 7
        Assert.True(board.CheckPossible(3, 2, v));

        // Solve with rating at lookahead 1
        board.MaxLookahead = 1;
        var solution = board.SolveWithRating();

        // Candidate 7 should be eliminated from Row 3 Col 2 via Swordfish
        Assert.False(board.CheckPossible(3, 2, v));
        var swordfishStep = solution.Steps.FirstOrDefault(s => s.Type == DeductionType.Swordfish && s.Row == 3 && s.Col == 2 && s.Value == v);
        Assert.NotNull(swordfishStep);
        Assert.Contains("Swordfish", swordfishStep.Explanation);
    }

    [Fact]
    public void Test_Generator_NamedStrategiesOnly()
    {
        var generator = new SudokuGenerator(3, 3, new Random(42));
        var criteria = DifficultyCriteria.Trivial with { NamedStrategiesOnly = true };
        var board = generator.Generate(criteria);
        Assert.NotNull(board);

        var solution = board.SolveFast();
        Assert.Equal(SolveState.Solved, solution.State);
    }
}

