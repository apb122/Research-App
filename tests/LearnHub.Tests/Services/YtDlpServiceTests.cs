using LearnHub.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace LearnHub.Tests.Services;

public class YtDlpServiceTests
{
    [Fact]
    public async Task DetectReturnsFalseWhenBinaryMissing()
    {
        var svc = new YtDlpService(new NullLogger<YtDlpService>());
        var found = await svc.DetectAsync("C:/non-existent/yt-dlp.exe");
        Assert.False(found);
    }
}
