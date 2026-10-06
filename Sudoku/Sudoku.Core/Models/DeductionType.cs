namespace Sudoku.Core.Models;

public enum DeductionType
{
    NakedSingle,
    HiddenSingleRow,
    HiddenSingleColumn,
    HiddenSingleBox,
    PointingPair,
    PointingTriple,
    BoxLineReduction,
    NakedPair,
    NakedTriple,
    NakedQuad,
    HiddenPair,
    HiddenTriple,
    XWing,
    Swordfish,
    Jellyfish,
    XYWing,
    ForcingChain,
    LookaheadElimination,
    DirectPlacement
}
