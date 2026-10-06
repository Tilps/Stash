namespace Sudoku.Core.Models;

public sealed record PresetPuzzle(string Title, string Difficulty, string Clues, int SizeX = 3, int SizeY = 3)
{
    public int Width => SizeX * SizeY;

    public static IReadOnlyList<PresetPuzzle> Presets { get; } = new List<PresetPuzzle>
    {
        // 2x3 (6x6) Presets: 3 columns wide, 2 rows high
        new(
            "6x6 Beginner",
            "Easy",
            "21.4.3.34.2.6.53.11.32.5.6.51.4.1.32",
            SizeX: 3,
            SizeY: 2
        ),
        new(
            "6x6 Classic",
            "Medium",
            "2..4...34...6..3....3..5...51...1..2",
            SizeX: 3,
            SizeY: 2
        ),

        // 3x3 (9x9) Presets
        new(
            "Beginner (Singles Only)",
            "Easy",
            ".2....5.8.8..293..9..7..........87..3..1.2..6..64..........3..7..761..8.1.3....4.",
            SizeX: 3,
            SizeY: 3
        ),
        new(
            "Classic Easy",
            "Easy",
            "53..7....6..195....98....6.8...6...34..8.3..17...2...6.6....28....419..5....8..79",
            SizeX: 3,
            SizeY: 3
        ),
        new(
            "Classic Medium",
            "Medium",
            ".759.4......1....418...5.....9.83...8.......2...56.7.....3...756....7......8.263.",
            SizeX: 3,
            SizeY: 3
        ),
        new(
            "Classic Hard",
            "Hard",
            "2.6........7.1..92.8...5.....576.9..9...4...6..1.394.....1...8.63..5.2........5.4",
            SizeX: 3,
            SizeY: 3
        ),
        new(
            "Challenging",
            "Hard",
            "......6...32..1...5..9...7...3.2..8.2...5...7.4..8.9...8...7..6...3..51...9......",
            SizeX: 3,
            SizeY: 3
        ),
        new(
            "AI Escargot",
            "Expert",
            "1....7.9..3..2...8..96..5....53..9...1..8...26....4...3......1..4......7..7...3..",
            SizeX: 3,
            SizeY: 3
        )
    };

    public static IReadOnlyList<PresetPuzzle> GetPresetsForSize(int sizex, int sizey)
    {
        var list = Presets.Where(p => p.SizeX == sizex && p.SizeY == sizey).ToList();
        if (list.Count == 0)
        {
            // Generate empty board preset for unsupported sizes
            list.Add(new PresetPuzzle("Empty Board", "Custom", new string('.', sizex * sizey * sizex * sizey), sizex, sizey));
        }
        return list;
    }

    public static Board GetDefaultPreset(int sizex, int sizey)
    {
        var preset = Presets.FirstOrDefault(p => p.SizeX == sizex && p.SizeY == sizey);
        if (preset != null)
        {
            return Board.Parse(preset.Clues, sizex, sizey);
        }
        return new Board(sizex, sizey);
    }
}
