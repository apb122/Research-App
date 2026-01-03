using LearnHub.Core.Models;

namespace LearnHub.Core.Providers;

public interface IWebSearchProvider
{
    Task<IReadOnlyList<SourceItem>> SearchWebAsync(string query, CancellationToken cancellationToken = default);
}
