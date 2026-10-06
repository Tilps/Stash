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
        var easy = DifficultyCriteria.Easy;
        Assert.True(easy.Matches(0, 0, 0));
        Assert.False(easy.Matches(1, 2, 2));

        var medium = DifficultyCriteria.Medium;
        Assert.True(medium.Matches(1, 1, 2));
        Assert.False(medium.Matches(0, 0, 0));

        var customWildcard = DifficultyCriteria.Custom("1.4.*");
        Assert.True(customWildcard.Matches(1, 4, 3));
        Assert.False(customWildcard.Matches(1, 2, 2));

        var customGte = DifficultyCriteria.Custom(">=2");
        Assert.True(customGte.Matches(2, 0, 0));
        Assert.False(customGte.Matches(1, 5, 2));
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
}
