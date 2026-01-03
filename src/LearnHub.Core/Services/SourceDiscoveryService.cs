using LearnHub.Core.Models;
using LearnHub.Core.Providers;

namespace LearnHub.Core.Services;

public class SourceDiscoveryService
{
    private readonly IWebSearchProvider _webSearchProvider;

    public SourceDiscoveryService(IWebSearchProvider webSearchProvider)
    {
        _webSearchProvider = webSearchProvider;
    }

    public async Task<IReadOnlyList<SourceItem>> SearchAsync(string query, bool preferReputableDomains, CancellationToken cancellationToken = default)
    {
        var results = await _webSearchProvider.SearchWebAsync(query, cancellationToken);
        return preferReputableDomains
            ? results.OrderByDescending(r => r.CredibilityScore).ToList()
            : results.OrderByDescending(r => r.PublishedDate ?? DateTimeOffset.MinValue).ToList();
    }
}
