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
            if (MaxLookaheadUsed == 0)
                return "Trivial";
            if (MaxLookaheadUsed == 1)
            {
                if (Score == 0) return "Easy";
                if (Score == 1) return "Medium";
                if (Score == 2) return "Hard";
                return "Challenging";
            }
            return "Expert";
        }
    }
}
