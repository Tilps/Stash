using FFClocks.Core.Models;
using FFClocks.Core.Solver;

namespace FFClocks.Tests;

public class SolverTests
{
    [Fact]
    public void Solve_Known6SpotPuzzle_FindsValidSolution()
    {
        // Bresha ruins style
        int[] puzzle = [1, 2, 3, 1, 2, 2];
        var result = ClockSolver.Solve(puzzle);

        Assert.True(result.Success);
        Assert.NotNull(result.PrimarySolution);
        Assert.Equal(6, result.PrimarySolution.TotalSteps);

        // Verify each step follows the rules
        var steps = result.PrimarySolution.Steps;
        var visited = new HashSet<int>();

        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            Assert.True(visited.Add(step.NodeIndex), $"Node {step.NodeIndex} visited multiple times");

            if (i > 0)
            {
                var prev = steps[i - 1];
                int prevVal = puzzle[prev.NodeIndex];
                int n = puzzle.Length;

                int cw = (prev.NodeIndex + prevVal) % n;
                int ccw = (prev.NodeIndex - prevVal % n + n) % n;

                Assert.True(step.NodeIndex == cw || step.NodeIndex == ccw,
                    $"Step {i + 1} ({step.NodeIndex}) is neither CW ({cw}) nor CCW ({ccw}) from {prev.NodeIndex}");
            }
        }
    }

    [Fact]
    public void Solve_VariousSizePuzzles_SolvesCorrectly()
    {
        int[][] testPuzzles =
        [
            [1, 2, 1, 2, 1],
            [1, 2, 3, 1, 2, 2],
            [2, 1, 3, 1, 2, 3, 1],
            [2, 4, 1, 3, 2, 1, 3, 2],
            [3, 1, 4, 2, 5, 2, 3, 1, 4, 2, 5, 1]
        ];

        foreach (var puzzle in testPuzzles)
        {
            var result = ClockSolver.Solve(puzzle);
            Assert.True(result.Success, $"Puzzle of size {puzzle.Length} failed to solve: {result.Message}");
            Assert.NotNull(result.PrimarySolution);
            Assert.Equal(puzzle.Length, result.PrimarySolution.TotalSteps);
        }
    }

    [Fact]
    public void Solve_InvalidInputs_ReturnsDescriptiveFailure()
    {
        // Zero value
        var res1 = ClockSolver.Solve([1, 0, 2, 1]);
        Assert.False(res1.Success);
        Assert.Contains("positive", res1.Message, StringComparison.OrdinalIgnoreCase);

        // Multiple of size (lands back on self)
        var res2 = ClockSolver.Solve([1, 4, 2, 1]); // 4 % 4 == 0
        Assert.False(res2.Success);
        Assert.Contains("itself", res2.Message, StringComparison.OrdinalIgnoreCase);

        // Less than 2 spots
        var res3 = ClockSolver.Solve([1]);
        Assert.False(res3.Success);
    }

    [Fact]
    public void Solve_UnsolvablePuzzle_ReturnsFailed()
    {
        // 4 spots where all values are 2: only ever toggles between 0 and 2, or 1 and 3. Never visits all 4.
        // Wait, 2 % 4 == 2. (0+2)%4 = 2, (2+2)%4 = 0.
        // Spots 1 and 3 are never reachable.
        int[] unsolvable = [2, 2, 2, 2];
        var result = ClockSolver.Solve(unsolvable);

        Assert.False(result.Success);
        Assert.Contains("No solution", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void SolveAll_FindsMultipleSolutionsWhenTheyExist()
    {
        // [1, 1, 1, 1] has multiple valid solutions (both directions and all starting positions)
        int[] allOnes = [1, 1, 1, 1];
        var result = ClockSolver.SolveAll(allOnes, maxSolutions: 10);

        Assert.True(result.Success);
        Assert.True(result.Solutions.Count > 1, "Should find multiple solutions");
    }

    [Fact]
    public void SolveFromStart_ForcesGivenStartingNode()
    {
        int[] puzzle = [1, 2, 3, 1, 2, 2];
        var allSolutions = ClockSolver.SolveAll(puzzle, maxSolutions: 10);
        Assert.True(allSolutions.Success);

        int startNode = allSolutions.Solutions[0].StartNodeIndex;
        var startResult = ClockSolver.SolveFromStart(puzzle, startNode);

        Assert.True(startResult.Success);
        Assert.Equal(startNode, startResult.PrimarySolution!.StartNodeIndex);
    }

    [Fact]
    public void PuzzleGenerator_GeneratesSolvablePuzzles()
    {
        for (int size = 5; size <= 12; size++)
        {
            int[] puzzle = PuzzleGenerator.GenerateSolvablePuzzle(size);
            Assert.Equal(size, puzzle.Length);

            var result = ClockSolver.Solve(puzzle);
            Assert.True(result.Success, $"Generated puzzle of size {size} was not solvable!");
        }
    }

    [Fact]
    public void Solve_Large13SpotPuzzle_SolvesInFewMilliseconds()
    {
        int[] large = [4, 2, 5, 1, 3, 6, 2, 4, 1, 3, 5, 2, 1];
        var result = ClockSolver.Solve(large);

        Assert.True(result.Success);
        Assert.True(result.Elapsed.TotalMilliseconds < 100, $"Solve took {result.Elapsed.TotalMilliseconds}ms which is too slow");
    }
}

