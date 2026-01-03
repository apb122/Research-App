using System.Text.Json.Serialization;

namespace LearnHub.Core.Models;

public record LearningPlan
{
    [JsonPropertyName("planTitle")]
    public string PlanTitle { get; init; } = string.Empty;

    [JsonPropertyName("prerequisites")]
    public IReadOnlyList<string> Prerequisites { get; init; } = Array.Empty<string>();

    [JsonPropertyName("weeklySchedule")]
    public IReadOnlyList<WeeklySchedule> WeeklySchedule { get; init; } = Array.Empty<WeeklySchedule>();

    [JsonPropertyName("milestones")]
    public IReadOnlyList<string> Milestones { get; init; } = Array.Empty<string>();

    [JsonPropertyName("spacedRepetitionReminders")]
    public IReadOnlyList<string>? SpacedRepetitionReminders { get; init; }
    ;
}

public record WeeklySchedule
{
    [JsonPropertyName("weekLabel")]
    public string WeekLabel { get; init; } = string.Empty;

    [JsonPropertyName("days")]
    public IReadOnlyList<PlanDay> Days { get; init; } = Array.Empty<PlanDay>();
}

public record PlanDay
{
    [JsonPropertyName("dayLabel")]
    public string DayLabel { get; init; } = string.Empty;

    [JsonPropertyName("reading")]
    public IReadOnlyList<string> Reading { get; init; } = Array.Empty<string>();

    [JsonPropertyName("watching")]
    public IReadOnlyList<string> Watching { get; init; } = Array.Empty<string>();

    [JsonPropertyName("practiceTasks")]
    public IReadOnlyList<string> PracticeTasks { get; init; } = Array.Empty<string>();

    [JsonPropertyName("checkpoints")]
    public IReadOnlyList<string> Checkpoints { get; init; } = Array.Empty<string>();
}
