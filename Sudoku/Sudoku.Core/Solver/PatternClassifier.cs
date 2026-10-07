using Sudoku.Core.Models;

namespace Sudoku.Core.Solver;

public static class PatternClassifier
{
    public record ClassificationResult(
        DeductionType Type,
        string Name,
        string Explanation,
        string GroupDescription,
        IReadOnlyList<(int Row, int Col)> InvolvedCells,
        IReadOnlyList<DeductionArrow> Arrows,
        IReadOnlyList<string>? ProofChain,
        IReadOnlyList<ChainStepInfo>? ChainSteps = null
    );

    public static ClassificationResult Classify(
        Board board,
        int targetRow,
        int targetCol,
        int targetVal,
        IReadOnlyList<int> branchRows,
        IReadOnlyList<int> branchCols,
        IReadOnlyList<int> branchVals,
        int lookaheadDepth,
        int scoring,
        IReadOnlyList<string>? rawProofChain = null,
        IReadOnlyList<IReadOnlyList<(int Row, int Col, int Val)>>? branchChains = null)
    {
        int width = board.Width;
        int sizex = board.SizeX;
        int sizey = board.SizeY;

        // 1. Check Pointing Pair / Triple (Locked Candidates Type 1) directly from trial branches
        var pointing = CheckPointing(board, targetRow, targetCol, targetVal, branchRows, branchCols, branchVals);
        if (pointing != null) return pointing;

        // 2. Check Box-Line Reduction (Claiming Pair / Triple, Locked Candidates Type 2) directly from trial branches
        var boxLine = CheckBoxLineReduction(board, targetRow, targetCol, targetVal, branchRows, branchCols, branchVals);
        if (boxLine != null) return boxLine;

        // 3. Check Naked Pair / Triple / Quad directly from trial branches & chains
        var naked = CheckNakedSubset(board, targetRow, targetCol, targetVal, branchRows, branchCols, branchVals, branchChains);
        if (naked != null) return naked;

        // 4. Check Hidden Pair / Triple directly from trial branches & chains
        var hidden = CheckHiddenSubset(board, targetRow, targetCol, targetVal, branchRows, branchCols, branchVals, branchChains);
        if (hidden != null) return hidden;

        // 5. Check X-Wing (2-Fish) directly from trial branches & chains
        var xwing = CheckXWing(board, targetRow, targetCol, targetVal, branchRows, branchCols, branchVals, branchChains);
        if (xwing != null) return xwing;

        // 6. Check XY-Wing directly from trial branches & chains
        var xywing = CheckXYWing(board, targetRow, targetCol, targetVal, branchRows, branchCols, branchVals, branchChains);
        if (xywing != null) return xywing;

        // 7. General Forcing Chain / Branching Logic
        if (lookaheadDepth > 1)
        {
            return BuildDeepLookaheadResult(board, targetRow, targetCol, targetVal, branchRows, branchCols, branchVals, lookaheadDepth, scoring, rawProofChain);
        }

        return BuildForcingChainResult(board, targetRow, targetCol, targetVal, branchRows, branchCols, branchVals, lookaheadDepth, scoring, rawProofChain, branchChains);
    }

    private static ClassificationResult? CheckPointing(
        Board board, int targetRow, int targetCol, int targetVal,
        IReadOnlyList<int> bRows, IReadOnlyList<int> bCols, IReadOnlyList<int> bVals)
    {
        if (bRows.Count < 2 || bRows.Count > 3) return null;
        int sizex = board.SizeX;
        int sizey = board.SizeY;

        // All branch values must match targetVal - 1
        if (bVals.Any(v => v != targetVal - 1)) return null;

        // All branch cells must belong to the same Box
        int boxR = (bRows[0] / sizey) * sizey;
        int boxC = (bCols[0] / sizex) * sizex;
        for (int i = 1; i < bRows.Count; i++)
        {
            if ((bRows[i] / sizey) * sizey != boxR || (bCols[i] / sizex) * sizex != boxC)
                return null;
        }

        int targetBoxR = (targetRow / sizey) * sizey;
        int targetBoxC = (targetCol / sizex) * sizex;
        if (targetBoxR == boxR && targetBoxC == boxC) return null; // Target must be outside the box

        int boxNum = (boxR / sizey) * (board.Width / sizex) + (boxC / sizex) + 1;
        bool isPair = bRows.Count == 2;
        string typeName = isPair ? "Pointing Pair" : "Pointing Triple";
        var type = isPair ? DeductionType.PointingPair : DeductionType.PointingTriple;

        // Case A: Same Row
        if (bRows.All(r => r == targetRow))
        {
            var involved = bRows.Zip(bCols, (r, c) => (r, c)).Concat(new[] { (targetRow, targetCol) }).ToList();
            var arrows = new List<DeductionArrow>();
            for (int i = 0; i < bRows.Count; i++)
            {
                arrows.Add(new DeductionArrow(bRows[i], bCols[i], targetRow, targetCol, $"locks {Board.FormatValue(targetVal)}", "#38bdf8"));
            }

            string cellsDesc = string.Join(" and ", bRows.Zip(bCols, (r, c) => $"R{r + 1}C{c + 1}"));
            string explanation = $"{typeName}: In Box {boxNum}, candidate {Board.FormatValue(targetVal)} is confined to Row {targetRow + 1} ({cellsDesc}), eliminating candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

            return new ClassificationResult(type, typeName, explanation, $"Box {boxNum} Pointing to Row {targetRow + 1}", involved, arrows, null);
        }

        // Case B: Same Column
        if (bCols.All(c => c == targetCol))
        {
            var involved = bRows.Zip(bCols, (r, c) => (r, c)).Concat(new[] { (targetRow, targetCol) }).ToList();
            var arrows = new List<DeductionArrow>();
            for (int i = 0; i < bRows.Count; i++)
            {
                arrows.Add(new DeductionArrow(bRows[i], bCols[i], targetRow, targetCol, $"locks {Board.FormatValue(targetVal)}", "#38bdf8"));
            }

            string cellsDesc = string.Join(" and ", bRows.Zip(bCols, (r, c) => $"R{r + 1}C{c + 1}"));
            string explanation = $"{typeName}: In Box {boxNum}, candidate {Board.FormatValue(targetVal)} is confined to Column {targetCol + 1} ({cellsDesc}), eliminating candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

            return new ClassificationResult(type, typeName, explanation, $"Box {boxNum} Pointing to Column {targetCol + 1}", involved, arrows, null);
        }

        return null;
    }


    private static ClassificationResult? CheckBoxLineReduction(
        Board board, int targetRow, int targetCol, int targetVal,
        IReadOnlyList<int> bRows, IReadOnlyList<int> bCols, IReadOnlyList<int> bVals)
    {
        if (bRows.Count < 2 || bRows.Count > 3) return null;
        int sizex = board.SizeX;
        int sizey = board.SizeY;

        if (bVals.Any(v => v != targetVal - 1)) return null;

        // Branch cells must belong to the same Box
        int boxR = (bRows[0] / sizey) * sizey;
        int boxC = (bCols[0] / sizex) * sizex;
        for (int i = 1; i < bRows.Count; i++)
        {
            if ((bRows[i] / sizey) * sizey != boxR || (bCols[i] / sizex) * sizex != boxC)
                return null;
        }

        // Target cell must be in the same box
        int targetBoxR = (targetRow / sizey) * sizey;
        int targetBoxC = (targetCol / sizex) * sizex;
        if (targetBoxR != boxR || targetBoxC != boxC) return null;

        int boxNum = (boxR / sizey) * (board.Width / sizex) + (boxC / sizex) + 1;

        // Case A: Branch cells lie in the same row, but target cell is in a different row
        if (bRows.All(r => r == bRows[0]) && targetRow != bRows[0])
        {
            var involved = bRows.Zip(bCols, (r, c) => (r, c)).Concat(new[] { (targetRow, targetCol) }).ToList();
            var arrows = new List<DeductionArrow>();
            for (int i = 0; i < bRows.Count; i++)
            {
                arrows.Add(new DeductionArrow(bRows[i], bCols[i], targetRow, targetCol, $"claims {Board.FormatValue(targetVal)}", "#a855f7"));
            }

            string cellsDesc = string.Join(" and ", bRows.Zip(bCols, (r, c) => $"R{r + 1}C{c + 1}"));
            string explanation = $"Box-Line Reduction: In Row {bRows[0] + 1}, candidate {Board.FormatValue(targetVal)} only appears within Box {boxNum} ({cellsDesc}), eliminating candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

            return new ClassificationResult(DeductionType.BoxLineReduction, "Box-Line Reduction", explanation, $"Row {bRows[0] + 1} Claims Box {boxNum}", involved, arrows, null);
        }

        // Case B: Branch cells lie in the same col, but target cell is in a different col
        if (bCols.All(c => c == bCols[0]) && targetCol != bCols[0])
        {
            var involved = bRows.Zip(bCols, (r, c) => (r, c)).Concat(new[] { (targetRow, targetCol) }).ToList();
            var arrows = new List<DeductionArrow>();
            for (int i = 0; i < bRows.Count; i++)
            {
                arrows.Add(new DeductionArrow(bRows[i], bCols[i], targetRow, targetCol, $"claims {Board.FormatValue(targetVal)}", "#a855f7"));
            }

            string cellsDesc = string.Join(" and ", bRows.Zip(bCols, (r, c) => $"R{r + 1}C{c + 1}"));
            string explanation = $"Box-Line Reduction: In Column {bCols[0] + 1}, candidate {Board.FormatValue(targetVal)} only appears within Box {boxNum} ({cellsDesc}), eliminating candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

            return new ClassificationResult(DeductionType.BoxLineReduction, "Box-Line Reduction", explanation, $"Column {bCols[0] + 1} Claims Box {boxNum}", involved, arrows, null);
        }

        return null;
    }


    private static ClassificationResult? CheckNakedSubset(
        Board board,
        int targetRow,
        int targetCol,
        int targetVal,
        IReadOnlyList<int> bRows,
        IReadOnlyList<int> bCols,
        IReadOnlyList<int> bVals,
        IReadOnlyList<IReadOnlyList<(int Row, int Col, int Val)>>? branchChains)
    {
        int width = board.Width;
        int sizex = board.SizeX;
        int sizey = board.SizeY;

        var units = new (string Name, List<(int R, int C)> Cells)[]
        {
            ($"Row {targetRow + 1}", Enumerable.Range(0, width).Select(c => (targetRow, c)).ToList()),
            ($"Column {targetCol + 1}", Enumerable.Range(0, width).Select(r => (r, targetCol)).ToList()),
            ($"Box {(targetRow / sizey) * (width / sizex) + (targetCol / sizex) + 1}",
                GetBoxCells(board, (targetRow / sizey) * sizey, (targetCol / sizex) * sizex))
        };

        // Scenario 1: Trial branches all test candidate targetVal in a unit containing targetCell
        if (bVals.All(v => v == targetVal - 1) && bRows.Count >= 2 && bRows.Count <= 4)
        {
            foreach (var (unitName, unitCells) in units)
            {
                bool allInUnit = true;
                for (int i = 0; i < bRows.Count; i++)
                {
                    if (!unitCells.Any(c => c.R == bRows[i] && c.C == bCols[i]))
                    {
                        allInUnit = false;
                        break;
                    }
                }

                if (allInUnit && !bRows.Zip(bCols, (r, c) => (r, c)).Contains((targetRow, targetCol)))
                {
                    var emptyCells = unitCells.Where(c => board.Get(c.R, c.C) == 0 && (c.R != targetRow || c.C != targetCol)).ToList();
                    var trialCells = bRows.Zip(bCols, (r, c) => (r, c)).Distinct().ToList();

                    for (int size = trialCells.Count; size <= 4 && size <= emptyCells.Count; size++)
                    {
                        foreach (var combo in Combinations(emptyCells, size))
                        {
                            if (!trialCells.All(tc => combo.Contains(tc))) continue;

                            var combinedCands = new HashSet<int>();
                            bool valid = true;
                            foreach (var c in combo)
                            {
                                var cands = board.GetCandidates(c.R, c.C);
                                if (cands.Count < 2) { valid = false; break; }
                                foreach (int cand in cands) combinedCands.Add(cand);
                            }

                            if (valid && combinedCands.Count == size && combinedCands.Contains(targetVal))
                            {
                                string subsetName = size switch
                                {
                                    2 => "Naked Pair",
                                    3 => "Naked Triple",
                                    _ => "Naked Quad"
                                };
                                var dType = size switch
                                {
                                    2 => DeductionType.NakedPair,
                                    3 => DeductionType.NakedTriple,
                                    _ => DeductionType.NakedQuad
                                };

                                var involved = combo.Concat(new[] { (targetRow, targetCol) }).ToList();
                                var arrows = combo.Select(c => new DeductionArrow(c.R, c.C, targetRow, targetCol, $"locks {Board.FormatValue(targetVal)}", "#10b981")).ToList();
                                string cellsStr = string.Join(", ", combo.Select(c => $"R{c.R + 1}C{c.C + 1}"));
                                string candsStr = "{" + string.Join(", ", combinedCands.OrderBy(v => v).Select(Board.FormatValue)) + "}";
                                string expl = $"{subsetName}: Cells {cellsStr} in {unitName} are locked to candidates {candsStr}, eliminating candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

                                return new ClassificationResult(dType, subsetName, expl, $"{subsetName} in {unitName}", involved, arrows, null);
                            }
                        }
                    }
                }
            }
        }

        // Scenario 2: Trial on a single cell A in unit, testing its candidates, which forces cell B in the unit to targetVal
        if (bRows.Count == 2 && bRows[0] == bRows[1] && bCols[0] == bCols[1])
        {
            int rA = bRows[0];
            int cA = bCols[0];
            int v0 = bVals[0] + 1;
            int v1 = bVals[1] + 1;

            if (v0 == targetVal || v1 == targetVal)
            {
                int otherVal = (v0 == targetVal) ? v1 : v0;
                int branchOtherIdx = (v0 == targetVal) ? 1 : 0;

                if (branchChains != null && branchOtherIdx < branchChains.Count)
                {
                    foreach (var step in branchChains[branchOtherIdx].Skip(1))
                    {
                        if (step.Val == targetVal && (step.Row != rA || step.Col != cA))
                        {
                            int rB = step.Row;
                            int cB = step.Col;

                            foreach (var (unitName, unitCells) in units)
                            {
                                if (unitCells.Any(c => c.R == rA && c.C == cA) && unitCells.Any(c => c.R == rB && c.C == cB))
                                {
                                    var candsA = board.GetCandidates(rA, cA);
                                    var candsB = board.GetCandidates(rB, cB);
                                    if (candsA.Count == 2 && candsB.Count == 2 &&
                                        candsA.Contains(targetVal) && candsA.Contains(otherVal) &&
                                        candsB.Contains(targetVal) && candsB.Contains(otherVal))
                                    {
                                        var involved = new List<(int, int)> { (rA, cA), (rB, cB), (targetRow, targetCol) };
                                        var arrows = new List<DeductionArrow>
                                        {
                                            new(rA, cA, targetRow, targetCol, $"locks {Board.FormatValue(targetVal)}", "#10b981"),
                                            new(rB, cB, targetRow, targetCol, $"locks {Board.FormatValue(targetVal)}", "#10b981")
                                        };
                                        string expl = $"Naked Pair: Cells R{rA + 1}C{cA + 1} and R{rB + 1}C{cB + 1} in {unitName} are locked to candidates {{{Board.FormatValue(Math.Min(targetVal, otherVal))}, {Board.FormatValue(Math.Max(targetVal, otherVal))}}}, eliminating candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

                                        return new ClassificationResult(DeductionType.NakedPair, "Naked Pair", expl, $"Naked Pair in {unitName}", involved, arrows, null);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    private static ClassificationResult? CheckHiddenSubset(
        Board board,
        int targetRow,
        int targetCol,
        int targetVal,
        IReadOnlyList<int> bRows,
        IReadOnlyList<int> bCols,
        IReadOnlyList<int> bVals,
        IReadOnlyList<IReadOnlyList<(int Row, int Col, int Val)>>? branchChains)
    {
        int width = board.Width;
        int sizex = board.SizeX;
        int sizey = board.SizeY;

        var units = new (string Name, List<(int R, int C)> Cells)[]
        {
            ($"Row {targetRow + 1}", Enumerable.Range(0, width).Select(c => (targetRow, c)).ToList()),
            ($"Column {targetCol + 1}", Enumerable.Range(0, width).Select(r => (r, targetCol)).ToList()),
            ($"Box {(targetRow / sizey) * (width / sizex) + (targetCol / sizex) + 1}",
                GetBoxCells(board, (targetRow / sizey) * sizey, (targetCol / sizex) * sizex))
        };

        // Scenario 1: Trial on cell (targetRow, targetCol) itself, testing the hidden candidates
        if (bRows.All(r => r == targetRow) && bCols.All(c => c == targetCol) && bRows.Count >= 2 && bRows.Count <= 3)
        {
            var trialVals = bVals.Select(v => v + 1).ToHashSet();
            if (!trialVals.Contains(targetVal))
            {
                foreach (var (unitName, unitCells) in units)
                {
                    var emptyCells = unitCells.Where(c => board.Get(c.R, c.C) == 0).ToList();
                    for (int size = trialVals.Count; size <= 3 && size <= emptyCells.Count; size++)
                    {
                        var eligibleDigits = new List<int>();
                        for (int d = 1; d <= width; d++)
                        {
                            if (d == targetVal) continue;
                            int count = emptyCells.Count(c => board.CheckPossible(c.R, c.C, d));
                            if (count >= 2 && count <= size) eligibleDigits.Add(d);
                        }

                        if (trialVals.All(tv => eligibleDigits.Contains(tv)))
                        {
                            foreach (var dCombo in Combinations(eligibleDigits, size))
                            {
                                if (!trialVals.All(tv => dCombo.Contains(tv))) continue;

                                var cellsUnion = new HashSet<(int R, int C)>();
                                foreach (var d in dCombo)
                                {
                                    foreach (var c in emptyCells.Where(cell => board.CheckPossible(cell.R, cell.C, d)))
                                        cellsUnion.Add(c);
                                }

                                if (cellsUnion.Count == size && cellsUnion.Contains((targetRow, targetCol)))
                                {
                                    string subsetName = size == 2 ? "Hidden Pair" : "Hidden Triple";
                                    var dType = size == 2 ? DeductionType.HiddenPair : DeductionType.HiddenTriple;

                                    var involved = cellsUnion.ToList();
                                    string digitsStr = "{" + string.Join(", ", dCombo.OrderBy(v => v).Select(Board.FormatValue)) + "}";
                                    var arrows = cellsUnion.Where(c => c != (targetRow, targetCol))
                                        .Select(c => new DeductionArrow(c.R, c.C, targetRow, targetCol, $"hidden {digitsStr}", "#ec4899")).ToList();
                                    string cellsStr = string.Join(", ", cellsUnion.Select(c => $"R{c.R + 1}C{c.C + 1}"));
                                    string expl = $"{subsetName}: In {unitName}, digits {digitsStr} appear only in cells {cellsStr}, eliminating other candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

                                    return new ClassificationResult(dType, subsetName, expl, $"{subsetName} in {unitName}", involved, arrows, null);
                                }
                            }
                        }
                    }
                }
            }
        }

        // Scenario 2: Trial on a hidden value v1 across cells in unit U containing targetCell,
        // and the other branch forces targetCell to take hidden value v2
        if (bVals.All(v => v == bVals[0]) && bRows.Count == 2)
        {
            int v1 = bVals[0] + 1;
            if (v1 != targetVal)
            {
                int targetIdx = -1;
                for (int i = 0; i < 2; i++)
                {
                    if (bRows[i] == targetRow && bCols[i] == targetCol) { targetIdx = i; break; }
                }

                if (targetIdx >= 0)
                {
                    int otherIdx = 1 - targetIdx;
                    int rOther = bRows[otherIdx];
                    int cOther = bCols[otherIdx];

                    if (branchChains != null && otherIdx < branchChains.Count)
                    {
                        foreach (var step in branchChains[otherIdx].Skip(1))
                        {
                            if (step.Row == targetRow && step.Col == targetCol && step.Val != targetVal && step.Val != v1)
                            {
                                int v2 = step.Val;
                                foreach (var (unitName, unitCells) in units)
                                {
                                    if (unitCells.Any(c => c.R == rOther && c.C == cOther))
                                    {
                                        int countV1 = unitCells.Count(c => board.Get(c.R, c.C) == 0 && board.CheckPossible(c.R, c.C, v1));
                                        int countV2 = unitCells.Count(c => board.Get(c.R, c.C) == 0 && board.CheckPossible(c.R, c.C, v2));

                                        if (countV1 == 2 && countV2 == 2)
                                        {
                                            var involved = new List<(int, int)> { (targetRow, targetCol), (rOther, cOther) };
                                            var arrows = new List<DeductionArrow>
                                            {
                                                new(rOther, cOther, targetRow, targetCol, $"hidden {{{Board.FormatValue(Math.Min(v1, v2))}, {Board.FormatValue(Math.Max(v1, v2))}}}", "#ec4899")
                                            };
                                            string expl = $"Hidden Pair: In {unitName}, digits {{{Board.FormatValue(Math.Min(v1, v2))}, {Board.FormatValue(Math.Max(v1, v2))}}} appear only in cells R{targetRow + 1}C{targetCol + 1} and R{rOther + 1}C{cOther + 1}, eliminating other candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

                                            return new ClassificationResult(DeductionType.HiddenPair, "Hidden Pair", expl, $"Hidden Pair in {unitName}", involved, arrows, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return null;
    }

    private static ClassificationResult? CheckXWing(
        Board board,
        int targetRow,
        int targetCol,
        int targetVal,
        IReadOnlyList<int> bRows,
        IReadOnlyList<int> bCols,
        IReadOnlyList<int> bVals,
        IReadOnlyList<IReadOnlyList<(int Row, int Col, int Val)>>? branchChains)
    {
        if (bRows.Count != 2) return null;
        if (bVals[0] != targetVal - 1 || bVals[1] != targetVal - 1) return null;

        int width = board.Width;

        // Case A: Row-based X-Wing trial (trial cells in same row r1)
        if (bRows[0] == bRows[1] && bCols[0] != bCols[1])
        {
            int r1 = bRows[0];
            int c0 = bCols[0];
            int c1 = bCols[1];

            if ((targetCol == c0 || targetCol == c1) && targetRow != r1)
            {
                int cTarget = targetCol;
                int cOther = (targetCol == c0) ? c1 : c0;
                int branchOtherIdx = (targetCol == c0) ? 1 : 0;

                int r2 = -1;
                if (branchChains != null && branchOtherIdx < branchChains.Count)
                {
                    foreach (var step in branchChains[branchOtherIdx].Skip(1))
                    {
                        if (step.Col == cTarget && step.Val == targetVal && step.Row != r1 && step.Row != targetRow)
                        {
                            r2 = step.Row;
                            break;
                        }
                    }
                }

                if (r2 < 0)
                {
                    for (int r = 0; r < width; r++)
                    {
                        if (r != r1 && r != targetRow && board.Get(r, cTarget) == 0 && board.CheckPossible(r, cTarget, targetVal) &&
                            board.Get(r, cOther) == 0 && board.CheckPossible(r, cOther, targetVal))
                        {
                            int candCount = 0;
                            for (int c = 0; c < width; c++)
                            {
                                if (board.Get(r, c) == 0 && board.CheckPossible(r, c, targetVal)) candCount++;
                            }
                            if (candCount == 2)
                            {
                                r2 = r;
                                break;
                            }
                        }
                    }
                }

                if (r2 >= 0)
                {
                    int candCountR1 = 0;
                    for (int c = 0; c < width; c++)
                    {
                        if (board.Get(r1, c) == 0 && board.CheckPossible(r1, c, targetVal)) candCountR1++;
                    }
                    int candCountR2 = 0;
                    for (int c = 0; c < width; c++)
                    {
                        if (board.Get(r2, c) == 0 && board.CheckPossible(r2, c, targetVal)) candCountR2++;
                    }

                    if (candCountR1 == 2 && candCountR2 == 2)
                    {
                        var involved = new List<(int, int)> { (r1, cTarget), (r1, cOther), (r2, cTarget), (r2, cOther), (targetRow, targetCol) };
                        var arrows = new List<DeductionArrow>
                        {
                            new(r1, cOther, r2, cTarget, $"forces ={Board.FormatValue(targetVal)}", "#f59e0b", BranchIndex: 1, StepIndex: 0, EnablingCells: GetEnablingCells(board, r1, cOther, r2, cTarget)),
                            new(r1, cTarget, targetRow, targetCol, $"≠{Board.FormatValue(targetVal)}", "#38bdf8", BranchIndex: 0, StepIndex: 0, EnablingCells: GetEnablingCells(board, r1, cTarget, targetRow, targetCol)),
                            new(r2, cTarget, targetRow, targetCol, $"≠{Board.FormatValue(targetVal)}", "#f59e0b", BranchIndex: 1, StepIndex: 1, EnablingCells: GetEnablingCells(board, r2, cTarget, targetRow, targetCol))
                        };

                        string expl = $"X-Wing: Candidate {Board.FormatValue(targetVal)} in Rows {r1 + 1} and {r2 + 1} is locked into Columns {Math.Min(c0, c1) + 1} and {Math.Max(c0, c1) + 1}, forming an X-Wing that eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";
                        return new ClassificationResult(
                            DeductionType.XWing,
                            "X-Wing",
                            expl,
                            $"X-Wing in Rows {r1 + 1}, {r2 + 1}",
                            involved,
                            arrows,
                            null
                        );
                    }
                }
            }
        }

        // Case B: Column-based X-Wing trial (trial cells in same column c1)
        if (bCols[0] == bCols[1] && bRows[0] != bRows[1])
        {
            int c1 = bCols[0];
            int r0 = bRows[0];
            int r1 = bRows[1];

            if ((targetRow == r0 || targetRow == r1) && targetCol != c1)
            {
                int rTarget = targetRow;
                int rOther = (targetRow == r0) ? r1 : r0;
                int branchOtherIdx = (targetRow == r0) ? 1 : 0;

                int c2 = -1;
                if (branchChains != null && branchOtherIdx < branchChains.Count)
                {
                    foreach (var step in branchChains[branchOtherIdx].Skip(1))
                    {
                        if (step.Row == rTarget && step.Val == targetVal && step.Col != c1 && step.Col != targetCol)
                        {
                            c2 = step.Col;
                            break;
                        }
                    }
                }

                if (c2 < 0)
                {
                    for (int c = 0; c < width; c++)
                    {
                        if (c != c1 && c != targetCol && board.Get(rTarget, c) == 0 && board.CheckPossible(rTarget, c, targetVal) &&
                            board.Get(rOther, c) == 0 && board.CheckPossible(rOther, c, targetVal))
                        {
                            int candCount = 0;
                            for (int r = 0; r < width; r++)
                            {
                                if (board.Get(r, c) == 0 && board.CheckPossible(r, c, targetVal)) candCount++;
                            }
                            if (candCount == 2)
                            {
                                c2 = c;
                                break;
                            }
                        }
                    }
                }

                if (c2 >= 0)
                {
                    int candCountC1 = 0;
                    for (int r = 0; r < width; r++)
                    {
                        if (board.Get(r, c1) == 0 && board.CheckPossible(r, c1, targetVal)) candCountC1++;
                    }
                    int candCountC2 = 0;
                    for (int r = 0; r < width; r++)
                    {
                        if (board.Get(r, c2) == 0 && board.CheckPossible(r, c2, targetVal)) candCountC2++;
                    }

                    if (candCountC1 == 2 && candCountC2 == 2)
                    {
                        var involved = new List<(int, int)> { (rTarget, c1), (rOther, c1), (rTarget, c2), (rOther, c2), (targetRow, targetCol) };
                        var arrows = new List<DeductionArrow>
                        {
                            new(rOther, c1, rTarget, c2, $"forces ={Board.FormatValue(targetVal)}", "#f59e0b", BranchIndex: 1, StepIndex: 0, EnablingCells: GetEnablingCells(board, rOther, c1, rTarget, c2)),
                            new(rTarget, c1, targetRow, targetCol, $"≠{Board.FormatValue(targetVal)}", "#38bdf8", BranchIndex: 0, StepIndex: 0, EnablingCells: GetEnablingCells(board, rTarget, c1, targetRow, targetCol)),
                            new(rTarget, c2, targetRow, targetCol, $"≠{Board.FormatValue(targetVal)}", "#f59e0b", BranchIndex: 1, StepIndex: 1, EnablingCells: GetEnablingCells(board, rTarget, c2, targetRow, targetCol))
                        };

                        string expl = $"X-Wing: Candidate {Board.FormatValue(targetVal)} in Columns {c1 + 1} and {c2 + 1} is locked into Rows {Math.Min(r0, r1) + 1} and {Math.Max(r0, r1) + 1}, forming an X-Wing that eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";
                        return new ClassificationResult(
                            DeductionType.XWing,
                            "X-Wing",
                            expl,
                            $"X-Wing in Columns {c1 + 1}, {c2 + 1}",
                            involved,
                            arrows,
                            null
                        );
                    }
                }
            }
        }

        return null;
    }

    private static ClassificationResult? CheckXYWing(
        Board board,
        int targetRow,
        int targetCol,
        int targetVal,
        IReadOnlyList<int> bRows,
        IReadOnlyList<int> bCols,
        IReadOnlyList<int> bVals,
        IReadOnlyList<IReadOnlyList<(int Row, int Col, int Val)>>? branchChains)
    {
        if (bRows.Count != 2) return null;
        if (bRows[0] != bRows[1] || bCols[0] != bCols[1]) return null;
        if (branchChains == null || branchChains.Count != 2) return null;

        int pR = bRows[0];
        int pC = bCols[0];
        int valA = bVals[0] + 1;
        int valB = bVals[1] + 1;
        if (valA == targetVal || valB == targetVal) return null;

        // Branch 0 (assumes P = valA) must force a pincer Q1 to targetVal
        (int R, int C)? q1 = null;
        foreach (var step in branchChains[0].Skip(1))
        {
            if (step.Val == targetVal && (step.Row != pR || step.Col != pC))
            {
                if (CanSee(board, pR, pC, step.Row, step.Col) && CanSee(board, targetRow, targetCol, step.Row, step.Col))
                {
                    q1 = (step.Row, step.Col);
                    break;
                }
            }
        }
        if (q1 == null) return null;

        // Branch 1 (assumes P = valB) must force a pincer Q2 to targetVal
        (int R, int C)? q2 = null;
        foreach (var step in branchChains[1].Skip(1))
        {
            if (step.Val == targetVal && (step.Row != pR || step.Col != pC))
            {
                if (CanSee(board, pR, pC, step.Row, step.Col) && CanSee(board, targetRow, targetCol, step.Row, step.Col))
                {
                    q2 = (step.Row, step.Col);
                    break;
                }
            }
        }
        if (q2 == null) return null;

        if (q1.Value.R == q2.Value.R && q1.Value.C == q2.Value.C) return null;

        var q1Cands = board.GetCandidates(q1.Value.R, q1.Value.C);
        var q2Cands = board.GetCandidates(q2.Value.R, q2.Value.C);
        if (!q1Cands.Contains(targetVal) || !q1Cands.Contains(valA)) return null;
        if (!q2Cands.Contains(targetVal) || !q2Cands.Contains(valB)) return null;

        var involved = new List<(int, int)> { (pR, pC), (q1.Value.R, q1.Value.C), (q2.Value.R, q2.Value.C), (targetRow, targetCol) };
        var arrows = new List<DeductionArrow>
        {
            new(pR, pC, q1.Value.R, q1.Value.C, Board.FormatValue(valA), "#38bdf8", BranchIndex: 0, StepIndex: 0, EnablingCells: GetEnablingCells(board, pR, pC, q1.Value.R, q1.Value.C)),
            new(pR, pC, q2.Value.R, q2.Value.C, Board.FormatValue(valB), "#f59e0b", BranchIndex: 1, StepIndex: 0, EnablingCells: GetEnablingCells(board, pR, pC, q2.Value.R, q2.Value.C)),
            new(q1.Value.R, q1.Value.C, targetRow, targetCol, $"≠{Board.FormatValue(targetVal)}", "#38bdf8", BranchIndex: 0, StepIndex: 1, EnablingCells: GetEnablingCells(board, q1.Value.R, q1.Value.C, targetRow, targetCol)),
            new(q2.Value.R, q2.Value.C, targetRow, targetCol, $"≠{Board.FormatValue(targetVal)}", "#f59e0b", BranchIndex: 1, StepIndex: 1, EnablingCells: GetEnablingCells(board, q2.Value.R, q2.Value.C, targetRow, targetCol))
        };

        var chainSteps = new List<ChainStepInfo>
        {
            new(0, 0, $"Assume Pivot R{pR + 1}C{pC + 1} = {Board.FormatValue(valA)} ➔ Forces Pincer R{q1.Value.R + 1}C{q1.Value.C + 1} = {Board.FormatValue(targetVal)}", pR, pC, q1.Value.R, q1.Value.C, $"={Board.FormatValue(targetVal)}", GetEnablingCells(board, pR, pC, q1.Value.R, q1.Value.C)),
            new(0, 1, $"Pincer R{q1.Value.R + 1}C{q1.Value.C + 1} = {Board.FormatValue(targetVal)} ➔ Eliminates {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}", q1.Value.R, q1.Value.C, targetRow, targetCol, $"≠{Board.FormatValue(targetVal)}", GetEnablingCells(board, q1.Value.R, q1.Value.C, targetRow, targetCol)),
            new(1, 0, $"Assume Pivot R{pR + 1}C{pC + 1} = {Board.FormatValue(valB)} ➔ Forces Pincer R{q2.Value.R + 1}C{q2.Value.C + 1} = {Board.FormatValue(targetVal)}", pR, pC, q2.Value.R, q2.Value.C, $"={Board.FormatValue(targetVal)}", GetEnablingCells(board, pR, pC, q2.Value.R, q2.Value.C)),
            new(1, 1, $"Pincer R{q2.Value.R + 1}C{q2.Value.C + 1} = {Board.FormatValue(targetVal)} ➔ Eliminates {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}", q2.Value.R, q2.Value.C, targetRow, targetCol, $"≠{Board.FormatValue(targetVal)}", GetEnablingCells(board, q2.Value.R, q2.Value.C, targetRow, targetCol)),
        };

        var proofChain = new List<string>
        {
            $"Hypothesis 1: If R{pR + 1}C{pC + 1} = {Board.FormatValue(valA)} ➔ Pincer R{q1.Value.R + 1}C{q1.Value.C + 1} = {Board.FormatValue(targetVal)} ➔ Eliminates {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}",
            $"Hypothesis 2: If R{pR + 1}C{pC + 1} = {Board.FormatValue(valB)} ➔ Pincer R{q2.Value.R + 1}C{q2.Value.C + 1} = {Board.FormatValue(targetVal)} ➔ Eliminates {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}",
            $"Conclusion: In all hypotheses, candidate {Board.FormatValue(targetVal)} is eliminated from R{targetRow + 1}C{targetCol + 1}."
        };

        string expl = $"XY-Wing: Pivot R{pR + 1}C{pC + 1} ({Board.FormatValue(valA)}, {Board.FormatValue(valB)}) with pincers R{q1.Value.R + 1}C{q1.Value.C + 1} ({Board.FormatValue(valA)}, {Board.FormatValue(targetVal)}) and R{q2.Value.R + 1}C{q2.Value.C + 1} ({Board.FormatValue(valB)}, {Board.FormatValue(targetVal)}) eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

        return new ClassificationResult(
            DeductionType.XYWing,
            "XY-Wing",
            expl,
            $"XY-Wing Pivot R{pR + 1}C{pC + 1}",
            involved,
            arrows,
            proofChain,
            chainSteps
        );
    }

    private static ClassificationResult BuildForcingChainResult(
        Board board,
        int targetRow,
        int targetCol,
        int targetVal,
        IReadOnlyList<int> bRows,
        IReadOnlyList<int> bCols,
        IReadOnlyList<int> bVals,
        int lookaheadDepth,
        int scoring,
        IReadOnlyList<string>? rawProofChain,
        IReadOnlyList<IReadOnlyList<(int Row, int Col, int Val)>>? branchChains)
    {
        var involved = new HashSet<(int, int)> { (targetRow, targetCol) };
        for (int i = 0; i < bRows.Count; i++)
        {
            involved.Add((bRows[i], bCols[i]));
        }

        var arrows = new List<DeductionArrow>();
        var chainSteps = new List<ChainStepInfo>();
        var proofChain = new List<string>();

        string[] branchColors = new[] { "#38bdf8", "#f59e0b", "#10b981", "#a855f7", "#ec4899", "#06b6d4", "#eab308" };

        for (int a = 0; a < bRows.Count; a++)
        {
            string color = branchColors[a % branchColors.Length];
            var chain = (branchChains != null && a < branchChains.Count) ? branchChains[a] : null;

            int rootR = bRows[a];
            int rootC = bCols[a];
            int rootV = bVals[a] + 1;

            if (chain != null && chain.Count > 1)
            {
                // Multi-step chain!
                proofChain.Add($"Hypothesis {a + 1}: If R{rootR + 1}C{rootC + 1} = {Board.FormatValue(rootV)}:");

                // Root step info
                var rootEnabling = GetEnablingCells(board, rootR, rootC, chain[1].Row, chain[1].Col);
                chainSteps.Add(new ChainStepInfo(
                    BranchIndex: a,
                    StepIndex: 0,
                    Text: $"Assume R{rootR + 1}C{rootC + 1} = {Board.FormatValue(rootV)} ➔ Forces R{chain[1].Row + 1}C{chain[1].Col + 1} = {Board.FormatValue(chain[1].Val)}",
                    FromRow: rootR,
                    FromCol: rootC,
                    ToRow: chain[1].Row,
                    ToCol: chain[1].Col,
                    ValueLabel: $"={Board.FormatValue(chain[1].Val)}",
                    EnablingCells: rootEnabling
                ));

                // Intermediate forced steps
                for (int b = 0; b < chain.Count - 1; b++)
                {
                    int fromR = chain[b].Row;
                    int fromC = chain[b].Col;
                    int toR = chain[b + 1].Row;
                    int toC = chain[b + 1].Col;
                    int toV = chain[b + 1].Val;

                    involved.Add((fromR, fromC));
                    involved.Add((toR, toC));

                    var enabling = GetEnablingCells(board, fromR, fromC, toR, toC);
                    arrows.Add(new DeductionArrow(
                        fromR, fromC, toR, toC,
                        $"={Board.FormatValue(toV)}",
                        color,
                        BranchIndex: a,
                        StepIndex: b,
                        EnablingCells: enabling
                    ));

                    if (b > 0)
                    {
                        chainSteps.Add(new ChainStepInfo(
                            BranchIndex: a,
                            StepIndex: b,
                            Text: $"Forces R{toR + 1}C{toC + 1} = {Board.FormatValue(toV)} (via R{fromR + 1}C{fromC + 1})",
                            FromRow: fromR,
                            FromCol: fromC,
                            ToRow: toR,
                            ToCol: toC,
                            ValueLabel: $"={Board.FormatValue(toV)}",
                            EnablingCells: enabling
                        ));
                    }
                    proofChain.Add($"   → Forces R{toR + 1}C{toC + 1} = {Board.FormatValue(toV)}");
                }

                // Final step from last forced cell to target cell
                int lastR = chain[chain.Count - 1].Row;
                int lastC = chain[chain.Count - 1].Col;
                var finalEnabling = GetEnablingCells(board, lastR, lastC, targetRow, targetCol);

                arrows.Add(new DeductionArrow(
                    lastR, lastC, targetRow, targetCol,
                    $"≠{Board.FormatValue(targetVal)}",
                    color,
                    BranchIndex: a,
                    StepIndex: chain.Count - 1,
                    EnablingCells: finalEnabling
                ));

                chainSteps.Add(new ChainStepInfo(
                    BranchIndex: a,
                    StepIndex: chain.Count - 1,
                    Text: $"Forces R{lastR + 1}C{lastC + 1} = {Board.FormatValue(chain[chain.Count - 1].Val)} ➔ Eliminates {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}",
                    FromRow: lastR,
                    FromCol: lastC,
                    ToRow: targetRow,
                    ToCol: targetCol,
                    ValueLabel: $"≠{Board.FormatValue(targetVal)}",
                    EnablingCells: finalEnabling
                ));
                proofChain.Add($"   → Eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}");
            }
            else
            {
                // Direct hypothesis elimination (chainLength == 0 or 1-step)
                var directEnabling = GetEnablingCells(board, rootR, rootC, targetRow, targetCol);
                arrows.Add(new DeductionArrow(
                    rootR, rootC, targetRow, targetCol,
                    $"≠{Board.FormatValue(targetVal)}",
                    color,
                    BranchIndex: a,
                    StepIndex: 0,
                    EnablingCells: directEnabling
                ));

                proofChain.Add($"Hypothesis {a + 1}: If R{rootR + 1}C{rootC + 1} = {Board.FormatValue(rootV)}:");
                proofChain.Add($"   → Eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}");

                chainSteps.Add(new ChainStepInfo(
                    BranchIndex: a,
                    StepIndex: 0,
                    Text: $"Assume R{rootR + 1}C{rootC + 1} = {Board.FormatValue(rootV)} ➔ Eliminates {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}",
                    FromRow: rootR,
                    FromCol: rootC,
                    ToRow: targetRow,
                    ToCol: targetCol,
                    ValueLabel: $"≠{Board.FormatValue(targetVal)}",
                    EnablingCells: directEnabling
                ));
            }
        }

        proofChain.Add($"Conclusion: All {bRows.Count} hypotheses eliminate candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.");

        string branchesDesc = string.Join(" or ", bRows.Zip(bCols.Zip(bVals, (c, v) => (c, v)), (r, cv) => $"R{r + 1}C{cv.c + 1}={Board.FormatValue(cv.v + 1)}"));
        string expl = $"Forcing Chain: Evaluating all {bRows.Count} options ({branchesDesc}) proves candidate {Board.FormatValue(targetVal)} is impossible in R{targetRow + 1}C{targetCol + 1}.";
        string groupDesc = (lookaheadDepth <= 1) ? $"Forcing Chain (Depth 1)" : $"Forcing Chain (Depth {lookaheadDepth})";

        return new ClassificationResult(
            DeductionType.ForcingChain,
            "Forcing Chain",
            expl,
            groupDesc,
            involved.ToList(),
            arrows,
            proofChain,
            chainSteps
        );
    }

    private static ClassificationResult BuildDeepLookaheadResult(
        Board board,
        int targetRow,
        int targetCol,
        int targetVal,
        IReadOnlyList<int> bRows,
        IReadOnlyList<int> bCols,
        IReadOnlyList<int> bVals,
        int lookaheadDepth,
        int scoring,
        IReadOnlyList<string>? rawProofChain)
    {
        var involved = new HashSet<(int, int)> { (targetRow, targetCol) };
        for (int i = 0; i < bRows.Count; i++)
        {
            involved.Add((bRows[i], bCols[i]));
        }

        var proofChain = new List<string>();
        for (int a = 0; a < bRows.Count; a++)
        {
            int r = bRows[a];
            int c = bCols[a];
            int v = bVals[a] + 1;
            proofChain.Add($"Hypothesis {a + 1}: If R{r + 1}C{c + 1} = {Board.FormatValue(v)} ➔ Multi-level search proves candidate {Board.FormatValue(targetVal)} impossible in R{targetRow + 1}C{targetCol + 1}");
        }
        proofChain.Add($"Conclusion: All {bRows.Count} branching hypotheses eliminate candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.");

        string branchesDesc = string.Join(" or ", bRows.Zip(bCols.Zip(bVals, (c, v) => (c, v)), (r, cv) => $"R{r + 1}C{cv.c + 1}={Board.FormatValue(cv.v + 1)}"));
        string expl = $"Deep Lookahead (Depth {lookaheadDepth}): Exhaustively testing all {bRows.Count} options ({branchesDesc}) eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1} via multi-level recursive search.";
        string groupDesc = $"Nested Lookahead (Depth {lookaheadDepth})";

        return new ClassificationResult(
            DeductionType.LookaheadElimination,
            $"Deep Lookahead (Depth {lookaheadDepth})",
            expl,
            groupDesc,
            involved.ToList(),
            new List<DeductionArrow>(),
            proofChain,
            null
        );
    }

    private static List<(int Row, int Col)> GetEnablingCells(Board board, int fromR, int fromC, int toR, int toC)
    {
        var result = new HashSet<(int Row, int Col)>();
        int width = board.Width;
        int sizex = board.SizeX;
        int sizey = board.SizeY;

        bool sameRow = (fromR == toR);
        bool sameCol = (fromC == toC);
        bool sameBox = (fromR / sizey == toR / sizey) && (fromC / sizex == toC / sizex);

        if (sameRow)
        {
            for (int c = 0; c < width; c++)
            {
                if (c != fromC && c != toC && board.Get(fromR, c) == 0)
                {
                    result.Add((fromR, c));
                }
            }
        }

        if (sameCol)
        {
            for (int r = 0; r < width; r++)
            {
                if (r != fromR && r != toR && board.Get(r, fromC) == 0)
                {
                    result.Add((r, fromC));
                }
            }
        }

        if (sameBox)
        {
            int boxR = (fromR / sizey) * sizey;
            int boxC = (fromC / sizex) * sizex;
            for (int r = boxR; r < boxR + sizey; r++)
            {
                for (int c = boxC; c < boxC + sizex; c++)
                {
                    if ((r != fromR || c != fromC) && (r != toR || c != toC) && board.Get(r, c) == 0)
                    {
                        result.Add((r, c));
                    }
                }
            }
        }

        // If not in a direct row, column or box, include intersection cells between them
        if (!sameRow && !sameCol && !sameBox)
        {
            if (board.Get(fromR, toC) == 0) result.Add((fromR, toC));
            if (board.Get(toR, fromC) == 0) result.Add((toR, fromC));
        }

        return result.ToList();
    }

    private static bool CanSee(Board board, int r1, int c1, int r2, int c2)
    {
        if (r1 == r2) return true;
        if (c1 == c2) return true;
        int sizey = board.SizeY;
        int sizex = board.SizeX;
        if ((r1 / sizey == r2 / sizey) && (c1 / sizex == c2 / sizex)) return true;
        return false;
    }

    private static List<(int R, int C)> GetBoxCells(Board board, int startR, int startC)
    {
        var list = new List<(int, int)>();
        for (int r = startR; r < startR + board.SizeY; r++)
        {
            for (int c = startC; c < startC + board.SizeX; c++)
            {
                list.Add((r, c));
            }
        }
        return list;
    }

    private static IEnumerable<List<T>> Combinations<T>(List<T> list, int length)
    {
        if (length == 0) return new[] { new List<T>() };
        if (list.Count == 0) return Enumerable.Empty<List<T>>();

        var head = list[0];
        var tail = list.Skip(1).ToList();

        var withHead = Combinations(tail, length - 1).Select(c => { c.Insert(0, head); return c; });
        var withoutHead = Combinations(tail, length);

        return withHead.Concat(withoutHead);
    }
}

