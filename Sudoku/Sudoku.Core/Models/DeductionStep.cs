namespace Sudoku.Core.Models;

public sealed record DeductionStep(
    int StepNumber,
    int Row,
    int Col,
    int Value,
    DeductionType Type,
    string Explanation,
    IReadOnlyList<(int Row, int Col)>? HighlightCells = null,
    string? GroupDescription = null
)
{
    public string CellCoordinate => $"R{Row + 1}C{Col + 1}";
}
