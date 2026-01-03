using System.Diagnostics;
using LearnHub.Core.Models;
using Microsoft.Extensions.Logging;

namespace LearnHub.Infrastructure.Services;

public class YtDlpService
{
    private readonly ILogger<YtDlpService> _logger;
    public string? ExecutablePath { get; private set; }

    public YtDlpService(ILogger<YtDlpService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> DetectAsync(string? configuredPath, CancellationToken cancellationToken = default)
    {
        var candidates = new List<string>();
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            candidates.Add(configuredPath);
        }

        var pathEnv = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        candidates.AddRange(pathEnv.Split(Path.PathSeparator).Select(p => Path.Combine(p, OperatingSystem.IsWindows() ? "yt-dlp.exe" : "yt-dlp")));

        foreach (var candidate in candidates.Distinct())
        {
            if (!File.Exists(candidate))
            {
                continue;
            }

            var version = await RunProcessAsync(candidate, "--version", cancellationToken);
            if (version.ExitCode == 0)
            {
                ExecutablePath = candidate;
                return true;
            }
        }

        return false;
    }

    public async Task<DownloadRecord> DownloadAsync(VideoItem video, string outputFolder, CancellationToken cancellationToken = default)
    {
        if (ExecutablePath is null)
        {
            throw new InvalidOperationException("yt-dlp is not configured. Please set the path in Settings.");
        }

        Directory.CreateDirectory(outputFolder);
        var outputTemplate = Path.Combine(outputFolder, "%(title)s-%(id)s.%(ext)s");
        var args = $"-o \"{outputTemplate}\" -f bestaudio+best {video.Url}";

        var record = new DownloadRecord
        {
            VideoId = video.VideoId,
            Url = video.Url,
            FilePath = outputTemplate,
            Status = JobStatus.Queued
        };

        var result = await RunProcessAsync(ExecutablePath, args, cancellationToken);
        record = record with { Status = result.ExitCode == 0 ? JobStatus.Succeeded : JobStatus.Failed };
        return record;
    }

    private async Task<(int ExitCode, string Output)> RunProcessAsync(string fileName, string arguments, CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start process");
            using var reg = cancellationToken.Register(() =>
            {
                if (!process.HasExited)
                {
                    try { process.Kill(); } catch { }
                }
            });

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);
            _logger.LogInformation("yt-dlp output: {Output} {Error}", output, error);
            return (process.ExitCode, output + error);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to run yt-dlp");
            return (-1, ex.Message);
        }
    }
}
