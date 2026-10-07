using Sudoku.Core.Models;
using Sudoku.Core.Solver;

namespace Sudoku.Core.Generator;

public enum Difficulty
{
    Trivial,
    Easy,
    Medium,
    Hard,
    Challenging,
    Expert
}

public record GenerationProgress(int Attempt, int MaxAttempts, string LastRating, string Status);

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

    public async Task<Board> GenerateAsync(
        DifficultyCriteria criteria,
        IProgress<GenerationProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        int targetClues = GetTargetClues(criteria);
        int maxLookahead = criteria.ExactLookahead ?? 2;
        int maxAttempts = (width > 9) ? 15 : 60;
        Board? bestFallback = null;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            progress?.Report(new GenerationProgress(
                Attempt: attempt,
                MaxAttempts: maxAttempts,
                LastRating: bestFallback != null ? bestFallback.DifficultyRating : "None",
                Status: $"Attempt {attempt}/{maxAttempts}: Generating puzzle candidate..."
            ));

            await Task.Yield();

            var candidate = GenerateSymmetricPuzzle(targetClues);
            if (candidate != null)
            {
                bestFallback ??= candidate;

                // Rate the candidate using the solver
                var testBoard = candidate.Clone();
                testBoard.MaxLookahead = Math.Max(2, maxLookahead);
                var solution = await testBoard.SolveWithRatingAsync(cancellationToken: cancellationToken);

                if (solution.IsSuccess)
                {
                    string rating = testBoard.DifficultyRating;
                    progress?.Report(new GenerationProgress(
                        Attempt: attempt,
                        MaxAttempts: maxAttempts,
                        LastRating: rating,
                        Status: $"Attempt {attempt}: Rated as {rating}"
                    ));

                    if (criteria.Matches(testBoard.LastLookaheadUsed, testBoard.Score, testBoard.HighTuples))
                    {
                        if (criteria.NamedStrategiesOnly)
                        {
                            bool hasGenericChain = solution.Steps.Any(s =>
                                s.Type is DeductionType.ForcingChain or DeductionType.LookaheadElimination);
                            if (hasGenericChain)
                            {
                                bestFallback ??= candidate;
                                continue;
                            }
                        }

                        return candidate;
                    }
                    else
                    {
                        // Keep best fallback based on lookahead closeness
                        if (bestFallback == null || testBoard.LastLookaheadUsed > bestFallback.LastLookaheadUsed)
                        {
                            bestFallback = candidate;
                        }
                    }
                }
            }
        }

        // Return best fallback if difficulty match was not found within max attempts
        return bestFallback ?? GenerateSymmetricPuzzle(targetClues) ?? PresetPuzzle.GetDefaultPreset(sizex, sizey);
    }

    public Board Generate(Difficulty difficulty = Difficulty.Medium)
    {
        return Generate(DifficultyCriteria.FromDifficulty(difficulty));
    }

    public Board Generate(DifficultyCriteria criteria)
    {
        return GenerateAsync(criteria).GetAwaiter().GetResult();
    }

    private int GetTargetClues(DifficultyCriteria criteria)
    {
        // For 9x9: target 37 clues for trivial down to 23 for expert
        // Scale proportionally for other board sizes
        double ratio = criteria.TargetPattern switch
        {
            "0" or "Trivial" => 0.46,
            "1.0" or "Easy" => 0.42,
            "1.1" or "Medium" => 0.36,
            "1.2" or "Hard" => 0.32,
            "1.3+ / 2" or "Challenging" => 0.30,
            "2" or "Expert" => 0.28,
            _ => 0.36
        };

        return Math.Max(width + 2, (int)(width * width * ratio));
    }

    private Board? GenerateSymmetricPuzzle(int targetClues)
    {
        var dl = new SudokuDancingLinks(sizex, sizey);
        List<int> xs = new();
        List<int> ys = new();
        List<int> values = new();

        int loops = 0;
        int maxLoops = width * width * 8;

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
            for (int rr = 0; rr < width; rr++)
            {
                for (int cc = 0; cc < width; cc++)
                {
                    dl.Grid[rr, cc] = puzzleGrid[rr, cc];
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
