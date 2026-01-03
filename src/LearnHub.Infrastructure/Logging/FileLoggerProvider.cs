using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace LearnHub.Infrastructure.Logging;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _logFile;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new();

    public FileLoggerProvider(string logFile)
    {
        _logFile = logFile;
        Directory.CreateDirectory(Path.GetDirectoryName(logFile)!);
    }

    public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, _ => new FileLogger(_logFile, categoryName));

    public void Dispose()
    {
        _loggers.Clear();
    }
}

internal class FileLogger : ILogger
{
    private readonly string _path;
    private readonly string _category;
    private static readonly object _lock = new();

    public FileLogger(string path, string category)
    {
        _path = path;
        _category = category;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        lock (_lock)
        {
            File.AppendAllText(_path, $"{DateTimeOffset.UtcNow:o}\t{logLevel}\t{_category}\t{formatter(state, exception)}\t{exception}\n");
        }
    }
}
