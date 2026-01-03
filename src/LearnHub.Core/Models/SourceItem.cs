namespace LearnHub.Core.Models;

public record SourceItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = string.Empty;
    public string Snippet { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Domain { get; init; } = string.Empty;
    public DateTimeOffset? PublishedDate { get; init; }
    ;
    public double CredibilityScore { get; init; }
    ;
}
