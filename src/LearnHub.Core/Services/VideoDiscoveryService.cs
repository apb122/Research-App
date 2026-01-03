using LearnHub.Core.Models;
using LearnHub.Core.Providers;

namespace LearnHub.Core.Services;

public class VideoDiscoveryService
{
    private readonly IVideoProvider _videoProvider;

    public VideoDiscoveryService(IVideoProvider videoProvider)
    {
        _videoProvider = videoProvider;
    }

    public async Task<IReadOnlyList<VideoItem>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var items = await _videoProvider.SearchVideosAsync(query, cancellationToken);
        return items.OrderBy(i => i.Duration).ToList();
    }
}
