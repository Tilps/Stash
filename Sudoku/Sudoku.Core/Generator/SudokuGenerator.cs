using Sudoku.Core.Models;
using Sudoku.Core.Solver;

namespace Sudoku.Core.Generator;

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Expert
}

public class SudokuGenerator
{
    private readonly int sizex;
    private readonly int sizey;
    private readonly int width;
    private readonly Random rng;

    public SudokuGenerator(int sizex = 3, int sizey = 3, Random? rng = null)
    {
        this.sizex = sizex;
        this.sizey = sizey;
        this.width = sizex * sizey;
        this.rng = rng ?? new Random();
    }

    public Board Generate(Difficulty difficulty = Difficulty.Medium)
    {
        int targetClues = difficulty switch
        {
            Difficulty.Easy => 38,
            Difficulty.Medium => 32,
            Difficulty.Hard => 28,
            Difficulty.Expert => 24,
            _ => 32
        };

        int maxLookahead = difficulty switch
        {
            Difficulty.Easy => 0,
            Difficulty.Medium => 1,
            Difficulty.Hard => 2,
            Difficulty.Expert => 3,
            _ => 1
        };

        for (int attempt = 0; attempt < 20; attempt++)
        {
            var board = GenerateSymmetricPuzzle(targetClues, maxLookahead);
            if (board != null)
            {
                return board;
            }
        }

        // Fallback: return a well-formed puzzle if random generation timed out
        return Board.Parse(PresetPuzzle.Presets[0].Clues, sizex, sizey);
    }

    private Board? GenerateSymmetricPuzzle(int targetClues, int maxLookahead)
    {
        var dl = new SudokuDancingLinks(sizey, sizex);
        List<int> xs = new();
        List<int> ys = new();
        List<int> values = new();

        int loops = 0;
        int maxLoops = width * width * 10;

        // Step 1: Add random symmetric pairs until puzzle has <= 1 solution
        while (loops++ < maxLoops)
        {
            int x = rng.Next(width);
            int y = rng.Next(width);
            if (xs.Count > 0 && xs.Zip(ys, (a, b) => a == x && b == y).Any(m => m))
            {
                continue;
            }

            int v = rng.Next(width) + 1;
            xs.Add(x);
            ys.Add(y);
            values.Add(v);

            bool hasOpp = width % 2 == 0 || (x != width / 2 || y != width / 2);
            if (hasOpp)
            {
                xs.Add(width - 1 - x);
                ys.Add(width - 1 - y);
                values.Add(rng.Next(width) + 1);
            }

            dl.Clear();
            for (int i = 0; i < xs.Count; i++)
            {
                dl.Grid[ys[i], xs[i]] = values[i];
            }
            dl.Solve();

            if (dl.Count <= 0)
            {
                // Contradiction: pop the added clues
                if (hasOpp)
                {
                    xs.RemoveAt(xs.Count - 1);
                    ys.RemoveAt(ys.Count - 1);
                    values.RemoveAt(values.Count - 1);
                }
                xs.RemoveAt(xs.Count - 1);
                ys.RemoveAt(ys.Count - 1);
                values.RemoveAt(values.Count - 1);
            }
            else if (dl.Count == 1)
            {
                break;
            }
        }

        if (dl.Count != 1) return null;

        // Step 2: Use the full solution from DLX to seed the grid, then prune clues symmetrically
        int[,] solved = (int[,])dl.Solution.Clone();
        var candidateCoords = new List<(int R, int C)>();
        for (int r = 0; r < width; r++)
        {
            for (int c = 0; c < width; c++)
            {
                candidateCoords.Add((r, c));
            }
        }

        candidateCoords = candidateCoords.OrderBy(_ => rng.Next()).ToList();
        var puzzleGrid = (int[,])solved.Clone();

        foreach (var (r, c) in candidateCoords)
        {
            if (puzzleGrid[r, c] == 0) continue;

            int oppR = width - 1 - r;
            int oppC = width - 1 - c;

            int backup = puzzleGrid[r, c];
            int oppBackup = puzzleGrid[oppR, oppC];

            puzzleGrid[r, c] = 0;
            puzzleGrid[oppR, oppC] = 0;

            dl.Clear();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    dl.Grid[j, i] = puzzleGrid[i, j];
                }
            }
            dl.Solve();

            if (dl.Count != 1)
            {
                // Uniqueness lost! Restore clues
                puzzleGrid[r, c] = backup;
                puzzleGrid[oppR, oppC] = oppBackup;
            }

            // Count remaining clues
            int remaining = 0;
            foreach (int cell in puzzleGrid)
            {
                if (cell != 0) remaining++;
            }
            if (remaining <= targetClues)
            {
                break;
            }
        }

        var resultBoard = new Board(sizex, sizey);
        for (int r = 0; r < width; r++)
        {
            for (int c = 0; c < width; c++)
            {
                if (puzzleGrid[r, c] > 0)
                {
                    resultBoard.Set(r, c, puzzleGrid[r, c]);
                }
            }
        }
        return resultBoard;
    }
}
