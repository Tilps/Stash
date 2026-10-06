namespace Sudoku.Core.Models;

public enum SolveState
{
    Progressing,
    Solved,
    Unsolvable,
    MultipleSolutions,
    DefiniteMultipleSolutions
}
