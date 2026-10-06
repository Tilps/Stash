namespace Sudoku.Core.Models;

public enum DeductionType
{
    NakedSingle,
    HiddenSingleRow,
    HiddenSingleColumn,
    HiddenSingleBox,
    LookaheadElimination,
    DirectPlacement
}
