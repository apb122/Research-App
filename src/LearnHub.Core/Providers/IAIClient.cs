namespace LearnHub.Core.Providers;

public interface IAIClient
{
    Task<string> GeneratePlanJsonAsync(string prompt, CancellationToken cancellationToken = default);
}
