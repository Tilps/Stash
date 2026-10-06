using Sudoku.Core.Generator;
using Sudoku.Core.Models;
using Sudoku.Core.Solver;

namespace Sudoku.Tests;

public class BoardTests
{
    [Fact]
    public void SolveWithRating_BeginnerPuzzle_SolvesWithNakedAndHiddenSingles()
    {
        var preset = PresetPuzzle.Presets[0];
        var board = Board.Parse(preset.Clues);

        var solution = board.SolveWithRating();

        Assert.Equal(SolveState.Solved, solution.State);
        Assert.True(solution.IsSuccess);
        Assert.NotEmpty(solution.Steps);
        Assert.True(solution.Steps.Count > 0);

        // Verify all cells in solved grid are 1..9
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                Assert.InRange(solution.SolvedGrid[r, c], 1, 9);
            }
        }

        // Verify each step has an explanation
        foreach (var step in solution.Steps)
        {
            Assert.False(string.IsNullOrWhiteSpace(step.Explanation));
            Assert.InRange(step.Value, 1, 9);
            Assert.InRange(step.Row, 0, 8);
            Assert.InRange(step.Col, 0, 8);
        }
    }

    [Fact]
    public void SolveFast_DLX_SolvesInstantly()
    {
        var preset = PresetPuzzle.Presets[1];
        var board = Board.Parse(preset.Clues);

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
    public void Generator_GeneratesSolvableUniquePuzzle()
    {
        var generator = new SudokuGenerator();
        var board = generator.Generate(Difficulty.Easy);

        // Check with DLX that it has exactly 1 unique solution
        var dl = new SudokuDancingLinks(3, 3);
        dl.SetGrid(board.Cells);
        dl.Solve();

        Assert.Equal(1, dl.Count);
    }

    [Fact]
    public void SolveWithRating_ProducesStructuredSteps()
    {
        var board = Board.Parse(PresetPuzzle.Presets[0].Clues);
        var solution = board.SolveWithRating();

        Assert.True(solution.Steps.Count > 0);
        var firstStep = solution.Steps[0];
        Assert.Equal(1, firstStep.StepNumber);
        Assert.Contains(firstStep.Value.ToString(), firstStep.Explanation);
    }

    [Fact]
    public void AllPresets_SolveSuccessfullyWithDLX()
    {
        foreach (var preset in PresetPuzzle.Presets)
        {
            var board = Board.Parse(preset.Clues);
            var solution = board.SolveFast();
            Assert.True(solution.State == SolveState.Solved, $"Preset '{preset.Title}' failed with state: {solution.State}");
        }
    }

    [Fact]
    public void Generator_GeneratesUniqueMediumAndHardPuzzles()
    {
        var generator = new SudokuGenerator();
        foreach (var diff in new[] { Difficulty.Medium, Difficulty.Hard })
        {
            var board = generator.Generate(diff);
            var dl = new SudokuDancingLinks(3, 3);
            dl.SetGrid(board.Cells);
            dl.Solve();
            Assert.Equal(1, dl.Count);
        }
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
}
