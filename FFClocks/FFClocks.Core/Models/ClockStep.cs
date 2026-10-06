namespace FFClocks.Core.Models;

public sealed record ClockStep(
    int StepNumber,
    int NodeIndex,
    int Value,
    int FromIndex = -1,
    MoveDirection Direction = MoveDirection.Start,
    int StepDistance = 0
)
{
    public string GetInstruction(int totalNodes)
    {
        string pos = FormatPosition(NodeIndex, totalNodes);
        return Direction switch
        {
            MoveDirection.Start => $"Start at {pos} (value {Value})",
            MoveDirection.Clockwise => $"Move clockwise {StepDistance} {(StepDistance == 1 ? "step" : "steps")} to {pos} (value {Value})",
            MoveDirection.CounterClockwise => $"Move counter-clockwise {StepDistance} {(StepDistance == 1 ? "step" : "steps")} to {pos} (value {Value})",
            MoveDirection.Both => $"Move {StepDistance} {(StepDistance == 1 ? "step" : "steps")} (either direction) to {pos} (value {Value})",
            _ => $"Select {pos}"
        };
    }

    public static string FormatPosition(int index, int totalNodes)
    {
        if (totalNodes == 12)
        {
            int hour = index == 0 ? 12 : index;
            return $"{hour} o'clock";
        }

        double degrees = (360.0 / totalNodes) * index;
        return degrees switch
        {
            0 => "12 o'clock (top)",
            90 => "3 o'clock (right)",
            180 => "6 o'clock (bottom)",
            270 => "9 o'clock (left)",
            _ => $"spot {index + 1} ({degrees:F0}°)"
        };
    }
}

