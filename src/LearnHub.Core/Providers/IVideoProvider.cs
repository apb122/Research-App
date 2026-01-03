using LearnHub.Core.Models;

namespace LearnHub.Core.Providers;

public interface IVideoProvider
{
    Task<IReadOnlyList<VideoItem>> SearchVideosAsync(string query, CancellationToken cancellationToken = default);
}
