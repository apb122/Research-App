using LearnHub.Core.Models;
using LearnHub.Core.Providers;
using LearnHub.Core.Services;
using Microsoft.Extensions.Logging;

namespace LearnHub.Infrastructure.Services;

public class JobRunner : IJobRunner
{
    private readonly ILogger<JobRunner> _logger;
    private readonly Dictionary<Guid, JobStatus> _statuses = new();

    public JobRunner(ILogger<JobRunner> logger)
    {
        _logger = logger;
    }

    public Task<JobHandle> QueueJobAsync(Func<CancellationToken, Task> work, string description, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        _statuses[id] = JobStatus.Queued;
        _ = RunAsync(id, work, description, cancellationToken);
        return Task.FromResult(new JobHandle(id, description, () => _statuses.GetValueOrDefault(id, JobStatus.Failed)));
    }

    private async Task RunAsync(Guid id, Func<CancellationToken, Task> work, string description, CancellationToken cancellationToken)
    {
        try
        {
            _statuses[id] = JobStatus.Running;
            _logger.LogInformation("Job {JobId} started: {Description}", id, description);
            await work(cancellationToken);
            _statuses[id] = JobStatus.Succeeded;
            _logger.LogInformation("Job {JobId} succeeded", id);
        }
        catch (Exception ex)
        {
            _statuses[id] = JobStatus.Failed;
            _logger.LogError(ex, "Job {JobId} failed", id);
        }
    }
}
