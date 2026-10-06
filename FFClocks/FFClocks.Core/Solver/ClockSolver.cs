using System.Diagnostics;
using FFClocks.Core.Models;

namespace FFClocks.Core.Solver;

public static class ClockSolver
{
    public const int MaxSupportedClockSize = 64;

    public static SolverResult Solve(IReadOnlyList<int> values, int maxSolutions = 1)
    {
        return SolveInternal(values, forcedStartIndex: null, maxSolutions: Math.Max(1, maxSolutions));
    }

    public static SolverResult SolveAll(IReadOnlyList<int> values, int maxSolutions = 50)
    {
        return SolveInternal(values, forcedStartIndex: null, maxSolutions: Math.Max(1, maxSolutions));
    }

    public static SolverResult SolveFromStart(IReadOnlyList<int> values, int startIndex, int maxSolutions = 1)
    {
        return SolveInternal(values, forcedStartIndex: startIndex, maxSolutions: Math.Max(1, maxSolutions));
    }

    private static SolverResult SolveInternal(IReadOnlyList<int> values, int? forcedStartIndex, int maxSolutions)
    {
        if (values == null || values.Count == 0)
        {
            return SolverResult.Failed("Clock cannot be empty.");
        }

        int n = values.Count;

        if (n < 2)
        {
            return SolverResult.Failed("Clock must have at least 2 spots.");
        }

        if (n > MaxSupportedClockSize)
        {
            return SolverResult.Failed($"Clocks up to {MaxSupportedClockSize} spots are supported.");
        }

        for (int i = 0; i < n; i++)
        {
            if (values[i] <= 0)
            {
                return SolverResult.Failed($"Spot {i + 1} has invalid value {values[i]}. All values must be positive.");
            }

            if (values[i] % n == 0)
            {
                return SolverResult.Failed($"Spot {i + 1} has value {values[i]}, which lands back on itself (0 relative steps).");
            }
        }

        if (forcedStartIndex.HasValue && (forcedStartIndex.Value < 0 || forcedStartIndex.Value >= n))
        {
            return SolverResult.Failed($"Invalid starting spot {forcedStartIndex.Value + 1}.");
        }

        var sw = Stopwatch.StartNew();

        int[] path = new int[n];
        List<ClockSolution> solutions = new();

        if (forcedStartIndex.HasValue)
        {
            int start = forcedStartIndex.Value;
            path[0] = start;
            ulong visited = 1UL << start;
            Dfs(values, n, 1, start, visited, path, solutions, maxSolutions);
        }
        else
        {
            for (int start = 0; start < n; start++)
            {
                path[0] = start;
                ulong visited = 1UL << start;
                Dfs(values, n, 1, start, visited, path, solutions, maxSolutions);

                if (solutions.Count >= maxSolutions)
                {
                    break;
                }
            }
        }

        sw.Stop();

        if (solutions.Count == 0)
        {
            return SolverResult.Failed("No solution exists for this clock configuration.", sw.Elapsed);
        }

        return SolverResult.Succeeded(solutions, sw.Elapsed);
    }

    private static void Dfs(
        IReadOnlyList<int> values,
        int n,
        int depth,
        int current,
        ulong visited,
        int[] path,
        List<ClockSolution> solutions,
        int maxSolutions)
    {
        if (solutions.Count >= maxSolutions)
        {
            return;
        }

        if (depth == n)
        {
            solutions.Add(BuildSolution(values, path, n));
            return;
        }

        int step = values[current] % n;
        int nextCw = (current + step) % n;
        int nextCcw = (current - step + n) % n;

        // Try Clockwise
        if ((visited & (1UL << nextCw)) == 0)
        {
            path[depth] = nextCw;
            Dfs(values, n, depth + 1, nextCw, visited | (1UL << nextCw), path, solutions, maxSolutions);
            if (solutions.Count >= maxSolutions) return;
        }

        // Try Counter-Clockwise (if different from CW)
        if (nextCw != nextCcw && (visited & (1UL << nextCcw)) == 0)
        {
            path[depth] = nextCcw;
            Dfs(values, n, depth + 1, nextCcw, visited | (1UL << nextCcw), path, solutions, maxSolutions);
            if (solutions.Count >= maxSolutions) return;
        }
    }

    private static ClockSolution BuildSolution(IReadOnlyList<int> values, int[] path, int n)
    {
        var steps = new List<ClockStep>(n);

        // Step 1: Start node
        steps.Add(new ClockStep(
            StepNumber: 1,
            NodeIndex: path[0],
            Value: values[path[0]],
            FromIndex: -1,
            Direction: MoveDirection.Start,
            StepDistance: 0
        ));

        for (int i = 1; i < n; i++)
        {
            int prev = path[i - 1];
            int curr = path[i];
            int dist = values[prev];
            int stepModulo = dist % n;

            int cw = (prev + stepModulo) % n;
            int ccw = (prev - stepModulo + n) % n;

            MoveDirection dir;
            if (cw == ccw)
            {
                dir = MoveDirection.Both;
            }
            else if (curr == cw)
            {
                dir = MoveDirection.Clockwise;
            }
            else
            {
                dir = MoveDirection.CounterClockwise;
            }

            steps.Add(new ClockStep(
                StepNumber: i + 1,
                NodeIndex: curr,
                Value: values[curr],
                FromIndex: prev,
                Direction: dir,
                StepDistance: dist
            ));
        }

        return new ClockSolution(steps, n);
    }
}

