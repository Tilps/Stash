namespace Sudoku.Core.Models;

public sealed record SudokuSolution(
    SolveState State,
    int[,] InitialGrid,
    int[,] SolvedGrid,
    IReadOnlyList<DeductionStep> Steps,
    int MaxLookaheadUsed,
    int Score,
    int HighTuples,
    TimeSpan Elapsed,
    string FullLog
)
{
    public bool IsSuccess => State == SolveState.Solved;
    public int StepCount => Steps.Count;

    public string DifficultyRating
    {
        get
        {
            if (MaxLookaheadUsed == 0 && Score <= 1)
                return "Easy";
            if (MaxLookaheadUsed <= 1 && Score <= 3)
                return "Medium";
            if (MaxLookaheadUsed <= 2)
                return "Hard";
            return "Expert";
        }
    }
}
