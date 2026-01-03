using LearnHub.Core.Models;
using LearnHub.Core.Providers;

namespace LearnHub.Infrastructure.Providers;

public class DefaultWebSearchProvider : IWebSearchProvider
{
    public Task<IReadOnlyList<SourceItem>> SearchWebAsync(string query, CancellationToken cancellationToken = default)
    {
        // Placeholder implementation. Production code should call a compliant search API such as Bing Web Search.
        var demo = new List<SourceItem>
        {
            new()
            {
                Title = $"Official docs for {query}",
                Domain = "docs.example.com",
                Url = "https://docs.example.com",
                Snippet = "Getting started guide from official vendor",
                PublishedDate = DateTimeOffset.UtcNow.AddMonths(-1),
                CredibilityScore = 0.9
            },
            new()
            {
                Title = $"University course on {query}",
                Domain = "university.edu",
                Url = "https://university.edu/course",
                Snippet = "Syllabus and lecture notes from .edu provider",
                PublishedDate = DateTimeOffset.UtcNow.AddYears(-1),
                CredibilityScore = 1.0
            }
        };

        return Task.FromResult<IReadOnlyList<SourceItem>>(demo);
    }
}
