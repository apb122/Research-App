using LearnHub.Core.Models;

namespace LearnHub.Core.Services;

public class JobHandle
{
    private readonly Func<JobStatus> _statusProvider;
    public Guid CorrelationId { get; }
    public string Description { get; }

    public JobHandle(Guid correlationId, string description, Func<JobStatus> statusProvider)
    {
        CorrelationId = correlationId;
        Description = description;
        _statusProvider = statusProvider;
    }

    public JobStatus Status => _statusProvider();
}
