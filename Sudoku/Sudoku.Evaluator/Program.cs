using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Sudoku.Core.Models;

namespace Sudoku.Evaluator;

public record PuzzleEvaluationResult(
    string FileName,
    string OriginalRating,
    int PuzzleIndex,
    int ClueCount,
    SolveState State,
    int LookaheadUsed,
    int Score,
    int HighTuples,
    string NewRating,
    bool Changed,
    string ChangeType, // "Unchanged", "Easier", "Harder", "Unsolved"
    long ElapsedMs
);

public class Program
{
    public static int Main(string[] args)
    {
        string? fileFilter = null;
        int maxPerFile = int.MaxValue;
        int maxLookahead = 2;
        int threads = Environment.ProcessorCount;
        string? csvPath = null;
        bool verbose = false;
        bool summaryOnly = false;
        int timeoutMs = 30000;

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            if ((arg == "-f" || arg == "--file") && i + 1 < args.Length)
            {
                fileFilter = args[++i];
            }
            else if ((arg == "-n" || arg == "--limit" || arg == "--max-per-file") && i + 1 < args.Length)
            {
                int.TryParse(args[++i], out maxPerFile);
            }
            else if ((arg == "-l" || arg == "--lookahead") && i + 1 < args.Length)
            {
                int.TryParse(args[++i], out maxLookahead);
            }
            else if ((arg == "-t" || arg == "--threads") && i + 1 < args.Length)
            {
                int.TryParse(args[++i], out threads);
            }
            else if ((arg == "-o" || arg == "--csv" || arg == "--export-csv") && i + 1 < args.Length)
            {
                csvPath = args[++i];
            }
            else if (arg == "-v" || arg == "--verbose")
            {
                verbose = true;
            }
            else if (arg == "-s" || arg == "--summary-only")
            {
                summaryOnly = true;
            }
            else if (arg == "--timeout" && i + 1 < args.Length)
            {
                int.TryParse(args[++i], out timeoutMs);
            }
            else if (arg == "-h" || arg == "--help")
            {
                PrintHelp();
                return 0;
            }
        }

        string baseDir = AppContext.BaseDirectory;
        string gatheredDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "GatheredSudokus"));
        if (!Directory.Exists(gatheredDir))
        {
            gatheredDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Sudoku", "GatheredSudokus"));
        }
        if (!Directory.Exists(gatheredDir))
        {
            gatheredDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "GatheredSudokus"));
        }

        if (!Directory.Exists(gatheredDir))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: Could not find GatheredSudokus directory at '{gatheredDir}'.");
            Console.ResetColor();
            return 1;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("================================================================================");
        Console.WriteLine("                 SUDOKU GATHERED PUZZLES RE-EVALUATION TOOL                     ");
        Console.WriteLine("================================================================================");
        Console.ResetColor();
        Console.WriteLine($"Directory:      {gatheredDir}");
        Console.WriteLine($"Lookahead:      {maxLookahead}");
        Console.WriteLine($"Concurrency:    {threads} threads");
        Console.WriteLine($"File filter:    {(string.IsNullOrEmpty(fileFilter) ? "All 1.x.y and 2 buckets" : fileFilter)}");
        if (maxPerFile < int.MaxValue) Console.WriteLine($"Limit/file:     {maxPerFile}");
        if (!string.IsNullOrEmpty(csvPath)) Console.WriteLine($"CSV Output:     {csvPath}");
        Console.WriteLine("--------------------------------------------------------------------------------\n");

        // Collect matching files in 1.x.y or 2 buckets
        var allFiles = Directory.GetFiles(gatheredDir, "*.txt")
            .Select(f => new FileInfo(f))
            .OrderBy(f => f.Name)
            .ToList();

        var targetFiles = new List<FileInfo>();
        foreach (var fi in allFiles)
        {
            string name = fi.Name;
            if (!string.IsNullOrEmpty(fileFilter))
            {
                if (MatchesFilter(name, fileFilter))
                {
                    targetFiles.Add(fi);
                }
            }
            else
            {
                // Default: 1.x.y files and 2.txt / 2.*.txt (skip 0.txt and Count *.txt)
                if (name.StartsWith("1.") || name.StartsWith("2.") || name == "2.txt")
                {
                    targetFiles.Add(fi);
                }
            }
        }

        if (targetFiles.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No files matched the specified filter.");
            Console.ResetColor();
            return 0;
        }

        Console.WriteLine($"Found {targetFiles.Count} matching bucket files to evaluate:\n");

        var allResults = new ConcurrentBag<PuzzleEvaluationResult>();
        var totalSw = Stopwatch.StartNew();

        int totalPuzzlesToRun = 0;
        var filePuzzles = new Dictionary<string, List<(int Index, string PuzzleText, int Clues)>>();

        foreach (var file in targetFiles)
        {
            var lines = File.ReadAllLines(file.FullName);
            int count = (lines.Length + 1) / 12;
            int take = Math.Min(count, maxPerFile);

            var list = new List<(int Index, string PuzzleText, int Clues)>();
            for (int i = 0; i < take; i++)
            {
                var pLines = lines.Skip(i * 12).Take(11).ToArray();
                string text = string.Join("\n", pLines);
                int clues = 0;
                foreach (var line in pLines)
                {
                    foreach (char ch in line)
                    {
                        if (ch >= '1' && ch <= '9') clues++;
                    }
                }
                list.Add((i + 1, text, clues));
            }

            filePuzzles[file.Name] = list;
            totalPuzzlesToRun += list.Count;
        }

        Console.WriteLine($"Total puzzles queued for evaluation: {totalPuzzlesToRun}\n");

        int completedCount = 0;

        foreach (var file in targetFiles)
        {
            string fileName = file.Name;
            string originalRating = Path.GetFileNameWithoutExtension(fileName);
            var puzzles = filePuzzles[fileName];

            if (puzzles.Count == 0) continue;

            Console.Write($"Evaluating {fileName} ({puzzles.Count} puzzles)... ");
            var fileSw = Stopwatch.StartNew();

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Max(1, threads)
            };

            var fileResults = new ConcurrentBag<PuzzleEvaluationResult>();

            Parallel.ForEach(puzzles, parallelOptions, p =>
            {
                var sw = Stopwatch.StartNew();
                try
                {
                    var board = Board.Parse(p.PuzzleText);
                    board.MaxLookahead = maxLookahead;

                    using var cts = new CancellationTokenSource(timeoutMs);
                    var sol = board.SolveWithRating(cts.Token);
                    sw.Stop();

                    string newRating;
                    if (sol.State == SolveState.Solved)
                    {
                        newRating = (sol.MaxLookaheadUsed != 1)
                            ? sol.MaxLookaheadUsed.ToString()
                            : $"{sol.MaxLookaheadUsed}.{sol.Score}.{sol.HighTuples}";
                    }
                    else
                    {
                        newRating = sol.State.ToString();
                    }

                    bool changed = (newRating != originalRating);
                    string changeType = "Unchanged";
                    if (changed)
                    {
                        if (sol.State != SolveState.Solved)
                        {
                            changeType = "Unsolved";
                        }
                        else
                        {
                            changeType = CompareRating(originalRating, newRating) > 0 ? "Easier" : "Harder";
                        }
                    }

                    var res = new PuzzleEvaluationResult(
                        FileName: fileName,
                        OriginalRating: originalRating,
                        PuzzleIndex: p.Index,
                        ClueCount: p.Clues,
                        State: sol.State,
                        LookaheadUsed: sol.MaxLookaheadUsed,
                        Score: sol.Score,
                        HighTuples: sol.HighTuples,
                        NewRating: newRating,
                        Changed: changed,
                        ChangeType: changeType,
                        ElapsedMs: sw.ElapsedMilliseconds
                    );

                    fileResults.Add(res);
                    allResults.Add(res);

                    int c = Interlocked.Increment(ref completedCount);
                    if (verbose)
                    {
                        Console.WriteLine($"[{c}/{totalPuzzlesToRun}] {fileName} #{p.Index}: {originalRating} -> {newRating} ({(changed ? changeType : "Unchanged")}, {sw.ElapsedMilliseconds} ms)");
                    }
                }
                catch (OperationCanceledException)
                {
                    sw.Stop();
                    var res = new PuzzleEvaluationResult(
                        FileName: fileName,
                        OriginalRating: originalRating,
                        PuzzleIndex: p.Index,
                        ClueCount: p.Clues,
                        State: SolveState.MultipleSolutions,
                        LookaheadUsed: -1,
                        Score: -1,
                        HighTuples: -1,
                        NewRating: "Timeout",
                        Changed: true,
                        ChangeType: "Unsolved",
                        ElapsedMs: sw.ElapsedMilliseconds
                    );
                    fileResults.Add(res);
                    allResults.Add(res);
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    var res = new PuzzleEvaluationResult(
                        FileName: fileName,
                        OriginalRating: originalRating,
                        PuzzleIndex: p.Index,
                        ClueCount: p.Clues,
                        State: SolveState.MultipleSolutions,
                        LookaheadUsed: -1,
                        Score: -1,
                        HighTuples: -1,
                        NewRating: $"Error: {ex.Message}",
                        Changed: true,
                        ChangeType: "Unsolved",
                        ElapsedMs: sw.ElapsedMilliseconds
                    );
                    fileResults.Add(res);
                    allResults.Add(res);
                }
            });

            fileSw.Stop();
            int fileChanged = fileResults.Count(r => r.Changed);
            Console.WriteLine($"Done in {fileSw.ElapsedMilliseconds} ms ({(fileChanged > 0 ? $"{fileChanged} changed" : "all unchanged")}).");
        }

        totalSw.Stop();

        // Print Summary Tables
        var sortedResults = allResults.OrderBy(r => r.FileName).ThenBy(r => r.PuzzleIndex).ToList();

        PrintSummaryTables(sortedResults, totalSw.Elapsed, summaryOnly);

        // Export to CSV if requested
        if (!string.IsNullOrEmpty(csvPath))
        {
            ExportCsv(sortedResults, csvPath);
        }

        return 0;
    }

    private static bool MatchesFilter(string filename, string filter)
    {
        if (string.Equals(filename, filter, StringComparison.OrdinalIgnoreCase)) return true;
        if (string.Equals(Path.GetFileNameWithoutExtension(filename), filter, StringComparison.OrdinalIgnoreCase)) return true;
        string pattern = "^" + Regex.Escape(filter).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        return Regex.IsMatch(filename, pattern, RegexOptions.IgnoreCase);
    }

    /// <summary>
    /// Compares two ratings. Returns > 0 if orig is strictly harder than new (so new is Easier), < 0 if new is Harder, 0 if equal.
    /// </summary>
    private static int CompareRating(string orig, string @new)
    {
        if (orig == @new) return 0;
        // Parse "1.x.y" or "2" or "0"
        var (la1, sc1, tp1) = ParseRating(orig);
        var (la2, sc2, tp2) = ParseRating(@new);

        if (la1 != la2) return la1.CompareTo(la2);
        if (sc1 != sc2) return sc1.CompareTo(sc2);
        return tp1.CompareTo(tp2);
    }

    private static (int Lookahead, int Score, int Tuples) ParseRating(string rating)
    {
        if (rating == "0") return (0, 0, 0);
        if (rating == "2") return (2, 0, 0);

        var parts = rating.Split('.');
        if (parts.Length == 3 && int.TryParse(parts[0], out int la) && int.TryParse(parts[1], out int sc) && int.TryParse(parts[2], out int tp))
        {
            return (la, sc, tp);
        }
        if (parts.Length == 2 && int.TryParse(parts[0], out int la2) && int.TryParse(parts[1], out int sc2))
        {
            return (la2, sc2, 0);
        }
        if (int.TryParse(rating, out int single))
        {
            return (single, 0, 0);
        }
        return (999, 999, 999);
    }

    private static void PrintSummaryTables(List<PuzzleEvaluationResult> results, TimeSpan totalTime, bool summaryOnly)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n================================================================================");
        Console.WriteLine("                              EVALUATION SUMMARY                                ");
        Console.WriteLine("================================================================================");
        Console.ResetColor();

        int totalPuzzles = results.Count;
        int unchangedCount = results.Count(r => !r.Changed);
        int changedCount = results.Count(r => r.Changed);
        int easierCount = results.Count(r => r.ChangeType == "Easier");
        int harderCount = results.Count(r => r.ChangeType == "Harder");
        int unsolvedCount = results.Count(r => r.ChangeType == "Unsolved");

        Console.WriteLine($"Total Puzzles:     {totalPuzzles}");
        Console.WriteLine($"Unchanged Rating:  {unchangedCount} ({(totalPuzzles > 0 ? (unchangedCount * 100.0 / totalPuzzles):0):F1}%)");
        Console.ForegroundColor = changedCount > 0 ? ConsoleColor.Yellow : ConsoleColor.Green;
        Console.WriteLine($"Changed Rating:    {changedCount} ({(totalPuzzles > 0 ? (changedCount * 100.0 / totalPuzzles):0):F1}%)");
        Console.ResetColor();
        Console.WriteLine($"  - Easier rating: {easierCount}");
        Console.WriteLine($"  - Harder rating: {harderCount}");
        Console.WriteLine($"  - Unsolved:      {unsolvedCount}");
        Console.WriteLine($"Total Time:        {totalTime.TotalSeconds:F2} seconds ({(totalPuzzles > 0 ? (totalTime.TotalMilliseconds / totalPuzzles):0):F1} ms/puzzle)");
        Console.WriteLine("--------------------------------------------------------------------------------\n");

        // Per-file breakdown
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("FILE-BY-FILE BREAKDOWN:");
        Console.ResetColor();
        Console.WriteLine("{0,-12} {1,7} {2,10} {3,8} {4,7} {5,7} {6,9} {7,12}",
            "File", "Total", "Unchanged", "Changed", "Easier", "Harder", "Unsolved", "Avg Time");
        Console.WriteLine(new string('-', 80));

        var byFile = results.GroupBy(r => r.FileName).OrderBy(g => g.Key);
        foreach (var group in byFile)
        {
            int tot = group.Count();
            int unc = group.Count(r => !r.Changed);
            int chg = group.Count(r => r.Changed);
            int eas = group.Count(r => r.ChangeType == "Easier");
            int hrd = group.Count(r => r.ChangeType == "Harder");
            int uns = group.Count(r => r.ChangeType == "Unsolved");
            double avgMs = group.Average(r => r.ElapsedMs);

            if (chg > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            Console.WriteLine("{0,-12} {1,7} {2,10} {3,8} {4,7} {5,7} {6,9} {7,10:F1} ms",
                group.Key, tot, unc, chg, eas, hrd, uns, avgMs);
            if (chg > 0)
            {
                Console.ResetColor();
            }
        }
        Console.WriteLine(new string('-', 80));

        // Rating Migration Matrix
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nRATING MIGRATIONS (Transitions from Original -> New):");
        Console.ResetColor();
        Console.WriteLine("{0,-12} -> {1,-12} {2,8}   {3,-10}",
            "Old Rating", "New Rating", "Count", "Shift");
        Console.WriteLine(new string('-', 50));

        var transitions = results
            .GroupBy(r => (r.OriginalRating, r.NewRating, r.ChangeType))
            .OrderByDescending(g => g.Key.ChangeType != "Unchanged")
            .ThenBy(g => g.Key.OriginalRating)
            .ThenBy(g => g.Key.NewRating);

        foreach (var t in transitions)
        {
            if (t.Key.ChangeType != "Unchanged")
            {
                Console.ForegroundColor = t.Key.ChangeType == "Easier" ? ConsoleColor.Green : ConsoleColor.Magenta;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }

            Console.WriteLine("{0,-12} -> {1,-12} {2,8}   {3,-10}",
                t.Key.OriginalRating, t.Key.NewRating, t.Count(), t.Key.ChangeType);
            Console.ResetColor();
        }
        Console.WriteLine(new string('-', 50));

        // List of changed puzzles
        var changedList = results.Where(r => r.Changed).ToList();
        if (changedList.Count > 0 && !summaryOnly)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nCHANGED PUZZLES DETAIL ({changedList.Count} puzzles):");
            Console.ResetColor();
            Console.WriteLine("{0,-12} {1,5} {2,6} {3,-10} -> {4,-10} {5,-10} {6,9}",
                "File", "#", "Clues", "Old Rating", "New Rating", "Shift", "Time");
            Console.WriteLine(new string('-', 72));

            foreach (var chg in changedList.Take(100))
            {
                Console.WriteLine("{0,-12} {1,5} {2,6} {3,-10} -> {4,-10} {5,-10} {6,7} ms",
                    chg.FileName, chg.PuzzleIndex, chg.ClueCount, chg.OriginalRating, chg.NewRating, chg.ChangeType, chg.ElapsedMs);
            }

            if (changedList.Count > 100)
            {
                Console.WriteLine($"... and {changedList.Count - 100} more changed puzzles (see CSV export for complete list).");
            }
            Console.WriteLine(new string('-', 72));
        }
        else if (changedList.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nAll evaluated puzzles maintained their exact original ratings!");
            Console.ResetColor();
        }
    }

    private static void ExportCsv(List<PuzzleEvaluationResult> results, string path)
    {
        try
        {
            string? dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using var writer = new StreamWriter(path);
            writer.WriteLine("File,OriginalRating,PuzzleIndex,Clues,State,LookaheadUsed,Score,HighTuples,NewRating,Changed,ChangeType,ElapsedMs");
            foreach (var r in results)
            {
                writer.WriteLine($"\"{r.FileName}\",\"{r.OriginalRating}\",{r.PuzzleIndex},{r.ClueCount},\"{r.State}\",{r.LookaheadUsed},{r.Score},{r.HighTuples},\"{r.NewRating}\",{r.Changed},\"{r.ChangeType}\",{r.ElapsedMs}");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSuccessfully exported full results to CSV: {Path.GetFullPath(path)}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error exporting to CSV: {ex.Message}");
            Console.ResetColor();
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("Usage: Sudoku.Evaluator [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -f, --file <filter>         Filter files (e.g. '2.txt', '1.1.*', '1.1.3.txt'). Default: all 1.x and 2 files.");
        Console.WriteLine("  -n, --limit <count>         Maximum puzzles to evaluate per file (default: all).");
        Console.WriteLine("  -l, --lookahead <max>       Max lookahead depth allowed (default: 2).");
        Console.WriteLine("  -t, --threads <count>       Degree of concurrency (default: CPU count).");
        Console.WriteLine("  -o, --csv <file>            Export full results to a CSV file.");
        Console.WriteLine("  -v, --verbose               Log each puzzle evaluation in real-time.");
        Console.WriteLine("  -s, --summary-only          Print only summary tables.");
        Console.WriteLine("  --timeout <ms>              Timeout per puzzle in milliseconds (default: 30000).");
        Console.WriteLine("  -h, --help                  Show this help message.");
    }
}
