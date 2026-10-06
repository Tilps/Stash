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
        IReadOnlyList<string>? ProofChain
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
        IReadOnlyList<string>? rawProofChain = null)
    {
        int width = board.Width;
        int sizex = board.SizeX;
        int sizey = board.SizeY;

        // 1. Check Pointing Pair / Triple (Locked Candidates Type 1)
        var pointing = CheckPointing(board, targetRow, targetCol, targetVal, branchRows, branchCols, branchVals)
                     ?? CheckPointingFromBoard(board, targetRow, targetCol, targetVal);
        if (pointing != null) return pointing;

        // 2. Check Box-Line Reduction (Claiming Pair / Triple, Locked Candidates Type 2)
        var boxLine = CheckBoxLineReduction(board, targetRow, targetCol, targetVal, branchRows, branchCols, branchVals)
                    ?? CheckBoxLineReductionFromBoard(board, targetRow, targetCol, targetVal);
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
        return BuildForcingChainResult(board, targetRow, targetCol, targetVal, branchRows, branchCols, branchVals, lookaheadDepth, scoring, rawProofChain);
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

    private static ClassificationResult? CheckPointingFromBoard(Board board, int targetRow, int targetCol, int targetVal)
    {
        int width = board.Width;
        int sizex = board.SizeX;
        int sizey = board.SizeY;

        for (int br = 0; br < width; br += sizey)
        {
            for (int bc = 0; bc < width; bc += sizex)
            {
                var boxCells = GetBoxCells(board, br, bc);
                var cands = boxCells.Where(c => board.Get(c.R, c.C) == 0 && board.CheckPossible(c.R, c.C, targetVal)).ToList();
                if (cands.Count >= 2 && cands.Count <= 3)
                {
                    int boxNum = (br / sizey) * (width / sizex) + (bc / sizex) + 1;
                    bool isPair = cands.Count == 2;
                    string typeName = isPair ? "Pointing Pair" : "Pointing Triple";
                    var type = isPair ? DeductionType.PointingPair : DeductionType.PointingTriple;

                    // Pointing along Row
                    if (cands.All(c => c.R == targetRow) && (targetRow < br || targetRow >= br + sizey || targetCol < bc || targetCol >= bc + sizex))
                    {
                        var involved = cands.Concat(new[] { (targetRow, targetCol) }).ToList();
                        var arrows = cands.Select(c => new DeductionArrow(c.R, c.C, targetRow, targetCol, $"locks {Board.FormatValue(targetVal)}", "#38bdf8")).ToList();
                        string cellsDesc = string.Join(" and ", cands.Select(c => $"R{c.R + 1}C{c.C + 1}"));
                        string explanation = $"{typeName}: In Box {boxNum}, candidate {Board.FormatValue(targetVal)} is confined to Row {targetRow + 1} ({cellsDesc}), eliminating candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";
                        return new ClassificationResult(type, typeName, explanation, $"Box {boxNum} Pointing to Row {targetRow + 1}", involved, arrows, null);
                    }

                    // Pointing along Col
                    if (cands.All(c => c.C == targetCol) && (targetRow < br || targetRow >= br + sizey || targetCol < bc || targetCol >= bc + sizex))
                    {
                        var involved = cands.Concat(new[] { (targetRow, targetCol) }).ToList();
                        var arrows = cands.Select(c => new DeductionArrow(c.R, c.C, targetRow, targetCol, $"locks {Board.FormatValue(targetVal)}", "#38bdf8")).ToList();
                        string cellsDesc = string.Join(" and ", cands.Select(c => $"R{c.R + 1}C{c.C + 1}"));
                        string explanation = $"{typeName}: In Box {boxNum}, candidate {Board.FormatValue(targetVal)} is confined to Column {targetCol + 1} ({cellsDesc}), eliminating candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";
                        return new ClassificationResult(type, typeName, explanation, $"Box {boxNum} Pointing to Column {targetCol + 1}", involved, arrows, null);
                    }
                }
            }
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

    private static ClassificationResult? CheckBoxLineReductionFromBoard(Board board, int targetRow, int targetCol, int targetVal)
    {
        int width = board.Width;
        int sizex = board.SizeX;
        int sizey = board.SizeY;
        int targetBoxR = (targetRow / sizey) * sizey;
        int targetBoxC = (targetCol / sizex) * sizex;
        int boxNum = (targetBoxR / sizey) * (width / sizex) + (targetBoxC / sizex) + 1;

        // Row claims Box
        for (int r = 0; r < width; r++)
        {
            if (r == targetRow) continue;
            if (r < targetBoxR || r >= targetBoxR + sizey) continue;

            var rowCands = Enumerable.Range(0, width)
                .Where(c => board.Get(r, c) == 0 && board.CheckPossible(r, c, targetVal))
                .Select(c => (R: r, C: c))
                .ToList();

            if (rowCands.Count >= 2 && rowCands.Count <= 3 && rowCands.All(c => c.C >= targetBoxC && c.C < targetBoxC + sizex))
            {
                var involved = rowCands.Concat(new[] { (targetRow, targetCol) }).ToList();
                var arrows = rowCands.Select(c => new DeductionArrow(c.R, c.C, targetRow, targetCol, $"claims {Board.FormatValue(targetVal)}", "#a855f7")).ToList();
                string cellsDesc = string.Join(" and ", rowCands.Select(c => $"R{c.R + 1}C{c.C + 1}"));
                string explanation = $"Box-Line Reduction: In Row {r + 1}, candidate {Board.FormatValue(targetVal)} only appears within Box {boxNum} ({cellsDesc}), eliminating candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";
                return new ClassificationResult(DeductionType.BoxLineReduction, "Box-Line Reduction", explanation, $"Row {r + 1} Claims Box {boxNum}", involved, arrows, null);
            }
        }

        // Col claims Box
        for (int c = 0; c < width; c++)
        {
            if (c == targetCol) continue;
            if (c < targetBoxC || c >= targetBoxC + sizex) continue;

            var colCands = Enumerable.Range(0, width)
                .Where(r => board.Get(r, c) == 0 && board.CheckPossible(r, c, targetVal))
                .Select(r => (R: r, C: c))
                .ToList();

            if (colCands.Count >= 2 && colCands.Count <= 3 && colCands.All(c => c.R >= targetBoxR && c.R < targetBoxR + sizey))
            {
                var involved = colCands.Concat(new[] { (targetRow, targetCol) }).ToList();
                var arrows = colCands.Select(c => new DeductionArrow(c.R, c.C, targetRow, targetCol, $"claims {Board.FormatValue(targetVal)}", "#a855f7")).ToList();
                string cellsDesc = string.Join(" and ", colCands.Select(c => $"R{c.R + 1}C{c.C + 1}"));
                string explanation = $"Box-Line Reduction: In Column {c + 1}, candidate {Board.FormatValue(targetVal)} only appears within Box {boxNum} ({cellsDesc}), eliminating candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.";
                return new ClassificationResult(DeductionType.BoxLineReduction, "Box-Line Reduction", explanation, $"Column {c + 1} Claims Box {boxNum}", involved, arrows, null);
            }
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
        IReadOnlyList<string>? rawProofChain)
    {
        var involved = new List<(int, int)> { (targetRow, targetCol) };
        for (int i = 0; i < bRows.Count; i++)
        {
            involved.Add((bRows[i], bCols[i]));
        }

        var arrows = new List<DeductionArrow>();
        for (int i = 0; i < bRows.Count; i++)
        {
            arrows.Add(new DeductionArrow(
                bRows[i],
                bCols[i],
                targetRow,
                targetCol,
                $"≠{Board.FormatValue(targetVal)}",
                "#ef4444"
            ));
        }

        List<string> proofChain;
        if (rawProofChain != null && rawProofChain.Count > 0)
        {
            proofChain = rawProofChain.ToList();
        }
        else
        {
            proofChain = new List<string>();
            for (int i = 0; i < bRows.Count; i++)
            {
                proofChain.Add($"Hypothesis {i + 1}: If R{bRows[i] + 1}C{bCols[i] + 1} = {Board.FormatValue(bVals[i] + 1)}:");
                proofChain.Add($"   → Directly eliminates candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}");
            }
            proofChain.Add($"Conclusion: All {bRows.Count} hypotheses eliminate candidate {Board.FormatValue(targetVal)} from R{targetRow + 1}C{targetCol + 1}.");
        }

        string branchesDesc = string.Join(" or ", bRows.Zip(bCols.Zip(bVals, (c, v) => (c, v)), (r, cv) => $"R{r + 1}C{cv.c + 1}={Board.FormatValue(cv.v + 1)}"));
        string expl = $"Forcing Chain: Evaluating all {bRows.Count} options ({branchesDesc}) proves candidate {Board.FormatValue(targetVal)} is impossible in R{targetRow + 1}C{targetCol + 1}.";
        string groupDesc = (lookaheadDepth <= 1) ? $"Forcing Chain (Depth 1)" : $"Forcing Chain (Depth {lookaheadDepth})";

        return new ClassificationResult(
            DeductionType.ForcingChain,
            "Forcing Chain",
            expl,
            groupDesc,
            involved,
            arrows,
            proofChain
        );
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

