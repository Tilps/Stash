namespace Sudoku.Core.Models;

public sealed record PresetPuzzle(string Title, string Difficulty, string Clues)
{
    public static IReadOnlyList<PresetPuzzle> Presets { get; } = new List<PresetPuzzle>
    {
        new(
            "Beginner",
            "Easy",
            ".2....5.8.8..293..9..7..........87..3..1.2..6..64..........3..7..761..8.1.3....4."
        ),
        new(
            "Classic Easy",
            "Easy",
            "53..7....6..195....98....6.8...6...34..8.3..17...2...6.6....28....419..5....8..79"
        ),
        new(
            "Classic Medium",
            "Medium",
            "2.6........7.1..92.8...5.....576.9..9...4...6..1.394.....1...8.63..5.2........5.4"
        ),
        new(
            "Challenging",
            "Hard",
            "......6...32..1...5..9...7...3.2..8.2...5...7.4..8.9...8...7..6...3..51...9......"
        ),
        new(
            "AI Escargot",
            "Expert",
            "1....7.9..3..2...8..96..5....53..9...1..8...26....4...3......1..4......7..7...3.."
        )
    };
}
