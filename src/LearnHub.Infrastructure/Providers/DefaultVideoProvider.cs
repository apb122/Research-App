using LearnHub.Core.Models;
using LearnHub.Core.Providers;

namespace LearnHub.Infrastructure.Providers;

public class DefaultVideoProvider : IVideoProvider
{
    public Task<IReadOnlyList<VideoItem>> SearchVideosAsync(string query, CancellationToken cancellationToken = default)
    {
        // Replace with YouTube Data API integration in production.
        var demo = new List<VideoItem>
        {
            new()
            {
                VideoId = "demo1",
                Title = $"Intro to {query}",
                Channel = "LearnHub Channel",
                Description = "High-level overview",
                Url = "https://video.example.com/watch?v=demo1",
                Duration = TimeSpan.FromMinutes(12),
                PublishedDate = DateTimeOffset.UtcNow.AddDays(-10)
            },
            new()
            {
                VideoId = "demo2",
                Title = $"{query} deep dive",
                Channel = "Pro Tutorials",
                Description = "Long-form tutorial",
                Url = "https://video.example.com/watch?v=demo2",
                Duration = TimeSpan.FromMinutes(45),
                PublishedDate = DateTimeOffset.UtcNow.AddMonths(-2)
            }
        };

        return Task.FromResult<IReadOnlyList<VideoItem>>(demo);
    }
}
