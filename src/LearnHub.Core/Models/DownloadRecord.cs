namespace LearnHub.Core.Models;

public record DownloadRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string VideoId { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string FilePath { get; init; } = string.Empty;
    public JobStatus Status { get; init; } = JobStatus.Queued;
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
