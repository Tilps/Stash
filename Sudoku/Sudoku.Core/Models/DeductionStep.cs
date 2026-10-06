namespace Sudoku.Core.Models;

public sealed record DeductionArrow(
    int FromRow,
    int FromCol,
    int ToRow,
    int ToCol,
    string? Label = null,
    string Color = "#38bdf8"
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
    IReadOnlyList<DeductionArrow>? Arrows = null
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

