using LearnHub.Core.Services;

namespace LearnHub.Core.Providers;

public interface IJobRunner
{
    Task<JobHandle> QueueJobAsync(Func<CancellationToken, Task> work, string description, CancellationToken cancellationToken = default);
}
