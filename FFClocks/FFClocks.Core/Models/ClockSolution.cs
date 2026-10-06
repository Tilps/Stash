namespace FFClocks.Core.Models;

public sealed class ClockSolution
{
    public IReadOnlyList<ClockStep> Steps { get; }
    public int[] StepOrderPerNode { get; }

    public ClockSolution(IReadOnlyList<ClockStep> steps, int totalNodes)
    {
        Steps = steps;
        StepOrderPerNode = new int[totalNodes];
        for (int i = 0; i < steps.Count; i++)
        {
            // 1-based visit order for player clarity
            StepOrderPerNode[steps[i].NodeIndex] = steps[i].StepNumber;
        }
    }

    public int TotalSteps => Steps.Count;
    public int StartNodeIndex => Steps.Count > 0 ? Steps[0].NodeIndex : -1;
}

