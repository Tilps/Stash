using FFClocks.Core.Models;

namespace FFClocks.Core.Solver;

public static class PuzzleGenerator
{
    private static readonly Random Rng = new();

    public static int[] GenerateSolvablePuzzle(int size, int maxStepSize = 0)
    {
        if (size < 3) size = 3;
        if (size > ClockSolver.MaxSupportedClockSize) size = ClockSolver.MaxSupportedClockSize;

        int limit = maxStepSize > 0 ? maxStepSize : Math.Max(1, size / 2);

        for (int attempt = 0; attempt < 100; attempt++)
        {
            // Create random visit sequence
            int[] sequence = Enumerable.Range(0, size).OrderBy(_ => Rng.Next()).ToArray();
            int[] values = new int[size];

            bool valid = true;
            for (int i = 0; i < size - 1; i++)
            {
                int curr = sequence[i];
                int next = sequence[i + 1];

                int cwDist = (next - curr + size) % size;
                int ccwDist = (curr - next + size) % size;

                // Pick cw or ccw, preferentially one within limit
                List<int> validDistances = new();
                if (cwDist > 0 && cwDist <= limit) validDistances.Add(cwDist);
                if (ccwDist > 0 && ccwDist <= limit && ccwDist != cwDist) validDistances.Add(ccwDist);

                if (validDistances.Count == 0)
                {
                    // Fallback to min distance
                    int minD = Math.Min(cwDist, ccwDist);
                    if (minD > 0) validDistances.Add(minD);
                }

                if (validDistances.Count == 0)
                {
                    valid = false;
                    break;
                }

                values[curr] = validDistances[Rng.Next(validDistances.Count)];
            }

            if (!valid) continue;

            // Last spot can be any random valid number 1..limit
            int lastSpot = sequence[size - 1];
            values[lastSpot] = Rng.Next(1, Math.Max(2, limit + 1));

            // Verify with solver
            var res = ClockSolver.Solve(values);
            if (res.Success)
            {
                return values;
            }
        }

        // Fallback default simple solvable puzzle
        int[] fallback = new int[size];
        for (int i = 0; i < size; i++) fallback[i] = 1;
        return fallback;
    }
}

