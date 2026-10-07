namespace Sudoku.Core.Generator;

public sealed record DifficultyCriteria
{
    public string TargetPattern { get; init; } = "";
    public int? ExactLookahead { get; init; }
    public int? MinScore { get; init; }
    public int? MaxScore { get; init; }
    public bool NamedStrategiesOnly { get; init; } = false;

    public static DifficultyCriteria Trivial { get; } = new()
    {
        ExactLookahead = 0,
        TargetPattern = "0"
    };

    public static DifficultyCriteria Easy { get; } = new()
    {
        ExactLookahead = 1,
        MinScore = 0,
        MaxScore = 0,
        TargetPattern = "1.0"
    };

    public static DifficultyCriteria Medium { get; } = new()
    {
        ExactLookahead = 1,
        MinScore = 1,
        MaxScore = 1,
        TargetPattern = "1.1"
    };

    public static DifficultyCriteria Hard { get; } = new()
    {
        ExactLookahead = 1,
        MinScore = 2,
        MaxScore = 2,
        TargetPattern = "1.2"
    };

    public static DifficultyCriteria Challenging { get; } = new()
    {
        TargetPattern = "1.3+ / 2"
    };

    public static DifficultyCriteria Expert { get; } = new()
    {
        ExactLookahead = 2,
        TargetPattern = "2"
    };

    public static DifficultyCriteria Custom(string pattern) => new()
    {
        TargetPattern = pattern.Trim()
    };

    public static DifficultyCriteria FromDifficulty(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Trivial => Trivial,
        Difficulty.Easy => Easy,
        Difficulty.Medium => Medium,
        Difficulty.Hard => Hard,
        Difficulty.Challenging => Challenging,
        Difficulty.Expert => Expert,
        _ => Medium
    };

    public bool Matches(int lookahead, int score, int highTuples)
    {
        if (this == Trivial || TargetPattern is "0" or "Trivial")
        {
            return lookahead == 0;
        }

        if (this == Easy || TargetPattern is "1.0" or "Easy")
        {
            return lookahead == 1 && score == 0;
        }

        if (this == Medium || TargetPattern is "1.1" or "Medium")
        {
            return lookahead == 1 && score == 1;
        }

        if (this == Hard || TargetPattern is "1.2" or "Hard")
        {
            return lookahead == 1 && score == 2;
        }

        if (this == Challenging || TargetPattern is "1.3+ / 2" or "Challenging")
        {
            return lookahead >= 2 || (lookahead == 1 && score >= 3);
        }

        if (this == Expert || TargetPattern is "2" or "Expert")
        {
            return lookahead >= 2;
        }

        if (ExactLookahead.HasValue)
        {
            if (lookahead != ExactLookahead.Value)
                return false;

            if (MinScore.HasValue && score < MinScore.Value) return false;
            if (MaxScore.HasValue && score > MaxScore.Value) return false;

            return true;
        }

        if (string.IsNullOrWhiteSpace(TargetPattern))
            return true;

        string currentRating = (lookahead != 1) ? lookahead.ToString() : $"{lookahead}.{score}.{highTuples}";

        // Support wildcard prefix: e.g. "1.*" or "1.2.*"
        if (TargetPattern.EndsWith("*"))
        {
            string prefix = TargetPattern.TrimEnd('*');
            return currentRating.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        // Support range: e.g. "1.0..1.3"
        if (TargetPattern.Contains(".."))
        {
            var parts = TargetPattern.Split("..", StringSplitOptions.TrimEntries);
            if (parts.Length == 2)
            {
                return string.Compare(currentRating, parts[0], StringComparison.OrdinalIgnoreCase) >= 0 &&
                       string.Compare(currentRating, parts[1], StringComparison.OrdinalIgnoreCase) <= 0;
            }
        }

        // Support minimum lookahead: e.g. ">=2"
        if (TargetPattern.StartsWith(">="))
        {
            if (int.TryParse(TargetPattern.Substring(2), out int minL))
                return lookahead >= minL;
        }

        // Exact match e.g. "1.0.2" or "2" or "0"
        if (currentRating.Equals(TargetPattern, StringComparison.OrdinalIgnoreCase)) return true;
        if ($"{lookahead}.{score}".Equals(TargetPattern, StringComparison.OrdinalIgnoreCase)) return true;

        return false;
    }
}
