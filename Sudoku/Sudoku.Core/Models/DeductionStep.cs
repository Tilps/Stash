namespace Sudoku.Core.Models;

public sealed record DeductionArrow(
    int FromRow,
    int FromCol,
    int ToRow,
    int ToCol,
    string? Label = null,
    string Color = "#38bdf8",
    int BranchIndex = 0,
    int StepIndex = 0,
    IReadOnlyList<(int Row, int Col)>? EnablingCells = null
);

public sealed record ChainStepInfo(
    int BranchIndex,
    int StepIndex,
    string Text,
    int FromRow,
    int FromCol,
    int ToRow,
    int ToCol,
    string? ValueLabel = null,
    IReadOnlyList<(int Row, int Col)>? EnablingCells = null
);

public sealed record DeductionStep(
    int StepNumber,
    int Row,
    int Col,
    int Value,
    DeductionType Type,
    string Explanation,
    IReadOnlyList<(int Row, int Col)>? HighlightCells = null,
    string? GroupDescription = null,
    IReadOnlyList<string>? ProofChain = null,
    IReadOnlyList<DeductionArrow>? Arrows = null,
    IReadOnlyList<ChainStepInfo>? ChainSteps = null
)
{
    public string CellCoordinate => $"R{Row + 1}C{Col + 1}";

    public bool IsPlacement => Type is DeductionType.NakedSingle
        or DeductionType.HiddenSingleRow
        or DeductionType.HiddenSingleColumn
        or DeductionType.HiddenSingleBox
        or DeductionType.DirectPlacement;

    public bool IsElimination => !IsPlacement;
}
