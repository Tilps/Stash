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

        // 3. Check Naked Pair / Triple / Quad
        var naked = CheckNakedSubset(board, targetRow, targetCol, targetVal);
        if (naked != null) return naked;

        // 4. Check Hidden Pair / Triple
        var hidden = CheckHiddenSubset(board, targetRow, targetCol, targetVal);
        if (hidden != null) return hidden;

        // 5. Check X-Wing (2-Fish)
        var xwing = CheckXWing(board, targetRow, targetCol, targetVal);
        if (xwing != null) return xwing;

        // 6. Check XY-Wing
        var xywing = CheckXYWing(board, targetRow, targetCol, targetVal);
        if (xywing != null) return xywing;

        // 7. Check Swordfish (3-Fish)
        var swordfish = CheckSwordfish(board, targetRow, targetCol, targetVal);
        if (swordfish != null) return swordfish;

        // 8. General Forcing Chain / Branching Logic
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


    private static ClassificationResult? CheckNakedSubset(Board board, int targetRow, int targetCol, int targetVal)
    {
        int width = board.Width;
        int sizex = board.SizeX;
        int sizey = board.SizeY;

        // Check units: Row, Column, Box
        var units = new (string Name, List<(int R, int C)> Cells)[]
        {
            ($"Row {targetRow + 1}", Enumerable.Range(0, width).Select(c => (targetRow, c)).ToList()),
            ($"Column {targetCol + 1}", Enumerable.Range(0, width).Select(r => (r, targetCol)).ToList()),
            ($"Box {(targetRow / sizey) * (width / sizex) + (targetCol / sizex) + 1}",
                GetBoxCells(board, (targetRow / sizey) * sizey, (targetCol / sizex) * sizex))
        };

        foreach (var (unitName, unitCells) in units)
        {
            var emptyCells = unitCells.Where(cell => board.Get(cell.R, cell.C) == 0 && (cell.R != targetRow || cell.C != targetCol)).ToList();

            for (int size = 2; size <= 4 && size <= emptyCells.Count; size++)
            {
                foreach (var combo in Combinations(emptyCells, size))
                {
                    var combinedCands = new HashSet<int>();
                    bool allHaveAtLeastTwo = true;
                    foreach (var c in combo)
                    {
                        var cands = board.GetCandidates(c.R, c.C);
                        if (cands.Count < 2) { allHaveAtLeastTwo = false; break; }
                        foreach (int cand in cands)
                        {
                            combinedCands.Add(cand);
                        }
                    }

                    if (allHaveAtLeastTwo && combinedCands.Count == size && combinedCands.Contains(targetVal))
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
                        var arrows = combo.Select(c => new DeductionArrow(c.R, c.C, targetRow, targetCol, null, "#10b981")).ToList();
                        string cellsStr = string.Join(", ", combo.Select(c => $"R{c.R + 1}C{c.C + 1}"));
                        string candsStr = "{" + string.Join(", ", combinedCands.OrderBy(v => v).Select(Board.FormatValue)) + "}";
                        string expl = $"{subsetName}: Cells {cellsStr} in {unitName} are locked to candidates {candsStr}, eliminating candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

                        return new ClassificationResult(dType, subsetName, expl, $"{subsetName} in {unitName}", involved, arrows, null);
                    }
                }
            }
        }

        return null;
    }

    private static ClassificationResult? CheckHiddenSubset(Board board, int targetRow, int targetCol, int targetVal)
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

        foreach (var (unitName, unitCells) in units)
        {
            var targetCell = (targetRow, targetCol);
            var emptyCells = unitCells.Where(c => board.Get(c.R, c.C) == 0).ToList();

            for (int size = 2; size <= 3 && size <= emptyCells.Count; size++)
            {
                var candidateMap = new Dictionary<int, List<(int R, int C)>>();
                for (int d = 1; d <= width; d++)
                {
                    if (d == targetVal) continue;
                    var pos = emptyCells.Where(c => board.CheckPossible(c.R, c.C, d)).ToList();
                    if (pos.Count >= 2 && pos.Count <= size && pos.Contains(targetCell))
                    {
                        candidateMap[d] = pos;
                    }
                }

                if (candidateMap.Count >= size)
                {
                    foreach (var dCombo in Combinations(candidateMap.Keys.ToList(), size))
                    {
                        var cellsUnion = new HashSet<(int R, int C)>();
                        foreach (var d in dCombo)
                        {
                            foreach (var cell in candidateMap[d]) cellsUnion.Add(cell);
                        }

                        if (cellsUnion.Count == size && cellsUnion.Contains(targetCell))
                        {
                            string subsetName = size == 2 ? "Hidden Pair" : "Hidden Triple";
                            var dType = size == 2 ? DeductionType.HiddenPair : DeductionType.HiddenTriple;

                            var involved = cellsUnion.ToList();
                            var arrows = cellsUnion.Where(c => c != targetCell).Select(c => new DeductionArrow(c.R, c.C, targetRow, targetCol, null, "#ec4899")).ToList();
                            string cellsStr = string.Join(", ", cellsUnion.Select(c => $"R{c.R + 1}C{c.C + 1}"));
                            string digitsStr = "{" + string.Join(", ", dCombo.OrderBy(v => v).Select(Board.FormatValue)) + "}";
                            string expl = $"{subsetName}: In {unitName}, digits {digitsStr} appear only in cells {cellsStr}, eliminating other candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

                            return new ClassificationResult(dType, subsetName, expl, $"{subsetName} in {unitName}", involved, arrows, null);
                        }
                    }
                }
            }
        }

        return null;
    }

    private static ClassificationResult? CheckXWing(Board board, int targetRow, int targetCol, int targetVal)
    {
        int width = board.Width;

        // 1. Row-based X-Wing (eliminates in columns)
        var rowCandidateCols = new Dictionary<int, List<int>>();
        for (int r = 0; r < width; r++)
        {
            var cols = new List<int>();
            for (int c = 0; c < width; c++)
            {
                if (board.Get(r, c) == 0 && board.CheckPossible(r, c, targetVal))
                {
                    cols.Add(c);
                }
            }
            if (cols.Count == 2)
            {
                rowCandidateCols[r] = cols;
            }
        }

        var rKeys = rowCandidateCols.Keys.ToList();
        for (int i = 0; i < rKeys.Count; i++)
        {
            for (int j = i + 1; j < rKeys.Count; j++)
            {
                int r1 = rKeys[i];
                int r2 = rKeys[j];
                var c1List = rowCandidateCols[r1];
                var c2List = rowCandidateCols[r2];

                if (c1List[0] == c2List[0] && c1List[1] == c2List[1])
                {
                    int c1 = c1List[0];
                    int c2 = c1List[1];

                    if ((targetCol == c1 || targetCol == c2) && targetRow != r1 && targetRow != r2)
                    {
                        var involved = new List<(int, int)> { (r1, c1), (r1, c2), (r2, c1), (r2, c2), (targetRow, targetCol) };
                        var arrows = new List<DeductionArrow>
                        {
                            new(r1, c1, r2, c2, "X", "#f59e0b"),
                            new(r1, c2, r2, c1, "X", "#f59e0b"),
                            new(r1, targetCol, targetRow, targetCol, $"eliminates {Board.FormatValue(targetVal)}", "#ef4444")
                        };

                        string expl = $"X-Wing: Candidate {Board.FormatValue(targetVal)} in Rows {r1 + 1} and {r2 + 1} is locked into Columns {c1 + 1} and {c2 + 1}, forming an X-Wing that eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";
                        return new ClassificationResult(DeductionType.XWing, "X-Wing", expl, $"X-Wing in Rows {r1 + 1}, {r2 + 1}", involved, arrows, null);
                    }
                }
            }
        }

        // 2. Column-based X-Wing (eliminates in rows)
        var colCandidateRows = new Dictionary<int, List<int>>();
        for (int c = 0; c < width; c++)
        {
            var rows = new List<int>();
            for (int r = 0; r < width; r++)
            {
                if (board.Get(r, c) == 0 && board.CheckPossible(r, c, targetVal))
                {
                    rows.Add(r);
                }
            }
            if (rows.Count == 2)
            {
                colCandidateRows[c] = rows;
            }
        }

        var cKeys = colCandidateRows.Keys.ToList();
        for (int i = 0; i < cKeys.Count; i++)
        {
            for (int j = i + 1; j < cKeys.Count; j++)
            {
                int c1 = cKeys[i];
                int c2 = cKeys[j];
                var r1List = colCandidateRows[c1];
                var r2List = colCandidateRows[c2];

                if (r1List[0] == r2List[0] && r1List[1] == r2List[1])
                {
                    int r1 = r1List[0];
                    int r2 = r1List[1];

                    if ((targetRow == r1 || targetRow == r2) && targetCol != c1 && targetCol != c2)
                    {
                        var involved = new List<(int, int)> { (r1, c1), (r1, c2), (r2, c1), (r2, c2), (targetRow, targetCol) };
                        var arrows = new List<DeductionArrow>
                        {
                            new(r1, c1, r2, c2, "X", "#f59e0b"),
                            new(r1, c2, r2, c1, "X", "#f59e0b"),
                            new(targetRow, c1, targetRow, targetCol, $"eliminates {Board.FormatValue(targetVal)}", "#ef4444")
                        };

                        string expl = $"X-Wing: Candidate {Board.FormatValue(targetVal)} in Columns {c1 + 1} and {c2 + 1} is locked into Rows {r1 + 1} and {r2 + 1}, forming an X-Wing that eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";
                        return new ClassificationResult(DeductionType.XWing, "X-Wing", expl, $"X-Wing in Columns {c1 + 1}, {c2 + 1}", involved, arrows, null);
                    }
                }
            }
        }

        return null;
    }

    private static ClassificationResult? CheckXYWing(Board board, int targetRow, int targetCol, int targetVal)
    {
        int width = board.Width;

        // Find bi-value cells
        var bivalueCells = new List<(int R, int C, int V1, int V2)>();
        for (int r = 0; r < width; r++)
        {
            for (int c = 0; c < width; c++)
            {
                if (board.Get(r, c) == 0)
                {
                    var cands = board.GetCandidates(r, c);
                    if (cands.Count == 2)
                    {
                        bivalueCells.Add((r, c, cands[0], cands[1]));
                    }
                }
            }
        }

        // Pivot P has {A, B} (neither is targetVal)
        foreach (var p in bivalueCells)
        {
            if (p.V1 == targetVal || p.V2 == targetVal) continue;
            int A = p.V1;
            int B = p.V2;

            // Pincer Q1 must have {A, targetVal} and see P
            var q1Candidates = bivalueCells.Where(q => q != p && ((q.V1 == A && q.V2 == targetVal) || (q.V2 == A && q.V1 == targetVal)) && CanSee(board, p.R, p.C, q.R, q.C)).ToList();

            // Pincer Q2 must have {B, targetVal} and see P
            var q2Candidates = bivalueCells.Where(q => q != p && ((q.V1 == B && q.V2 == targetVal) || (q.V2 == B && q.V1 == targetVal)) && CanSee(board, p.R, p.C, q.R, q.C)).ToList();

            foreach (var q1 in q1Candidates)
            {
                foreach (var q2 in q2Candidates)
                {
                    if (q1.R == q2.R && q1.C == q2.C) continue;

                    // Target must see both Q1 and Q2
                    if (CanSee(board, targetRow, targetCol, q1.R, q1.C) && CanSee(board, targetRow, targetCol, q2.R, q2.C))
                    {
                        var involved = new List<(int, int)> { (p.R, p.C), (q1.R, q1.C), (q2.R, q2.C), (targetRow, targetCol) };
                        var arrows = new List<DeductionArrow>
                        {
                            new(p.R, p.C, q1.R, q1.C, Board.FormatValue(A), "#38bdf8"),
                            new(p.R, p.C, q2.R, q2.C, Board.FormatValue(B), "#38bdf8"),
                            new(q1.R, q1.C, targetRow, targetCol, $"≠{Board.FormatValue(targetVal)}", "#ef4444"),
                            new(q2.R, q2.C, targetRow, targetCol, $"≠{Board.FormatValue(targetVal)}", "#ef4444")
                        };

                        string expl = $"XY-Wing: Pivot R{p.R + 1}C{p.C + 1} ({Board.FormatValue(A)}, {Board.FormatValue(B)}) with pincers R{q1.R + 1}C{q1.C + 1} ({Board.FormatValue(A)}, {Board.FormatValue(targetVal)}) and R{q2.R + 1}C{q2.C + 1} ({Board.FormatValue(B)}, {Board.FormatValue(targetVal)}) eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";
                        return new ClassificationResult(DeductionType.XYWing, "XY-Wing", expl, $"XY-Wing Pivot R{p.R + 1}C{p.C + 1}", involved, arrows, null);
                    }
                }
            }
        }

        return null;
    }

    private static ClassificationResult? CheckSwordfish(Board board, int targetRow, int targetCol, int targetVal)
    {
        int width = board.Width;

        // 1. Row-based Swordfish
        var rowCandidateCols = new Dictionary<int, List<int>>();
        for (int r = 0; r < width; r++)
        {
            var cols = new List<int>();
            for (int c = 0; c < width; c++)
            {
                if (board.Get(r, c) == 0 && board.CheckPossible(r, c, targetVal))
                {
                    cols.Add(c);
                }
            }
            if (cols.Count >= 2 && cols.Count <= 3)
            {
                rowCandidateCols[r] = cols;
            }
        }

        var rKeys = rowCandidateCols.Keys.ToList();
        if (rKeys.Count >= 3)
        {
            foreach (var combo in Combinations(rKeys, 3))
            {
                var unionCols = new HashSet<int>();
                foreach (var r in combo)
                {
                    foreach (var c in rowCandidateCols[r]) unionCols.Add(c);
                }

                if (unionCols.Count == 3 && unionCols.Contains(targetCol) && !combo.Contains(targetRow))
                {
                    var involved = new List<(int, int)> { (targetRow, targetCol) };
                    foreach (var r in combo)
                    {
                        foreach (var c in rowCandidateCols[r]) involved.Add((r, c));
                    }

                    var arrows = new List<DeductionArrow>();
                    foreach (var r in combo)
                    {
                        if (rowCandidateCols[r].Contains(targetCol))
                        {
                            arrows.Add(new DeductionArrow(r, targetCol, targetRow, targetCol, $"locks {Board.FormatValue(targetVal)}", "#f59e0b"));
                        }
                    }

                    string rowsStr = string.Join(", ", combo.Select(r => $"Row {r + 1}"));
                    string colsStr = string.Join(", ", unionCols.OrderBy(c => c).Select(c => $"Col {c + 1}"));
                    string expl = $"Swordfish: Candidate {Board.FormatValue(targetVal)} across {rowsStr} is locked into {colsStr}, forming a Swordfish that eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

                    return new ClassificationResult(DeductionType.Swordfish, "Swordfish", expl, $"Swordfish (3-Fish)", involved, arrows, null);
                }
            }
        }

        // 2. Column-based Swordfish
        var colCandidateRows = new Dictionary<int, List<int>>();
        for (int c = 0; c < width; c++)
        {
            var rows = new List<int>();
            for (int r = 0; r < width; r++)
            {
                if (board.Get(r, c) == 0 && board.CheckPossible(r, c, targetVal))
                {
                    rows.Add(r);
                }
            }
            if (rows.Count >= 2 && rows.Count <= 3)
            {
                colCandidateRows[c] = rows;
            }
        }

        var cKeys = colCandidateRows.Keys.ToList();
        if (cKeys.Count >= 3)
        {
            foreach (var combo in Combinations(cKeys, 3))
            {
                var unionRows = new HashSet<int>();
                foreach (var c in combo)
                {
                    foreach (var r in colCandidateRows[c]) unionRows.Add(r);
                }

                if (unionRows.Count == 3 && unionRows.Contains(targetRow) && !combo.Contains(targetCol))
                {
                    var involved = new List<(int, int)> { (targetRow, targetCol) };
                    foreach (var c in combo)
                    {
                        foreach (var r in colCandidateRows[c]) involved.Add((r, c));
                    }

                    var arrows = new List<DeductionArrow>();
                    foreach (var c in combo)
                    {
                        if (colCandidateRows[c].Contains(targetRow))
                        {
                            arrows.Add(new DeductionArrow(targetRow, c, targetRow, targetCol, $"locks {Board.FormatValue(targetVal)}", "#f59e0b"));
                        }
                    }

                    string colsStr = string.Join(", ", combo.Select(c => $"Col {c + 1}"));
                    string rowsStr = string.Join(", ", unionRows.OrderBy(r => r).Select(r => $"Row {r + 1}"));
                    string expl = $"Swordfish: Candidate {Board.FormatValue(targetVal)} across {colsStr} is locked into {rowsStr}, forming a Swordfish that eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";

                    return new ClassificationResult(DeductionType.Swordfish, "Swordfish", expl, $"Swordfish (3-Fish)", involved, arrows, null);
                }
            }
        }

        return null;
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

