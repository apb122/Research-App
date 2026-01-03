using System.Threading.Tasks;
using LearnHub.Infrastructure.Providers;
using Xunit;

namespace LearnHub.Tests.Services;

public class WebSearchProviderTests
{
    [Fact]
    public async Task ReturnsDemoSources()
    {
        var provider = new DefaultWebSearchProvider();
        var results = await provider.SearchWebAsync("test topic");
        Assert.NotEmpty(results);
        Assert.All(results, r => Assert.False(string.IsNullOrWhiteSpace(r.Title)));
    }
}
