namespace Sudoku.Core.Generator;

public sealed record DifficultyCriteria
{
    public string TargetPattern { get; init; } = "";
    public int? ExactLookahead { get; init; }
    public int? MinScore { get; init; }
    public int? MaxScore { get; init; }

    public static DifficultyCriteria Easy { get; } = new()
    {
        ExactLookahead = 0,
        TargetPattern = "0"
    };

    public static DifficultyCriteria Medium { get; } = new()
    {
        ExactLookahead = 1,
        MaxScore = 3,
        TargetPattern = "1.0..1.3"
    };

    public static DifficultyCriteria Hard { get; } = new()
    {
        ExactLookahead = 1,
        MinScore = 4,
        TargetPattern = "1.4+ / 2"
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
        Difficulty.Easy => Easy,
        Difficulty.Medium => Medium,
        Difficulty.Hard => Hard,
        Difficulty.Expert => Expert,
        _ => Medium
    };

    public bool Matches(int lookahead, int score, int highTuples)
    {
        if (ExactLookahead.HasValue)
        {
            if (this == Hard)
            {
                // Hard allows either deep lookahead 1 (score >= 4) OR lookahead >= 2
                return lookahead >= 2 || (lookahead == 1 && score >= 4);
            }

            if (this == Expert)
            {
                return lookahead >= 2;
            }

            if (lookahead != ExactLookahead.Value)
                return false;

            if (MinScore.HasValue && score < MinScore.Value) return false;
            if (MaxScore.HasValue && score > MaxScore.Value) return false;

            return true;
        }

        if (string.IsNullOrWhiteSpace(TargetPattern) || TargetPattern is "0" or "Easy" or "Medium" or "Hard" or "Expert")
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
