namespace FFClocks.Core.Models;

public sealed record SolverResult(
    bool Success,
    IReadOnlyList<ClockSolution> Solutions,
    TimeSpan Elapsed,
    string? Message = null
)
{
    public ClockSolution? PrimarySolution => Solutions.Count > 0 ? Solutions[0] : null;
    public int SolutionCount => Solutions.Count;

    public static SolverResult Failed(string message, TimeSpan elapsed = default) =>
        new(false, Array.Empty<ClockSolution>(), elapsed, message);

    public static SolverResult Succeeded(IReadOnlyList<ClockSolution> solutions, TimeSpan elapsed) =>
        new(true, solutions, elapsed, solutions.Count == 1 ? "Found 1 solution." : $"Found {solutions.Count} solutions.");
}

