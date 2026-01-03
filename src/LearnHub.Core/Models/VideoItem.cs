namespace LearnHub.Core.Models;

public record VideoItem
{
    public string VideoId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Channel { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public TimeSpan Duration { get; init; }
    public DateTimeOffset? PublishedDate { get; init; }
    public IReadOnlyDictionary<string, string> Thumbnails { get; init; } = new Dictionary<string, string>();
}
