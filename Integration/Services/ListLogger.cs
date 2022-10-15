using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.Tests.Integration.Services;

public class ListLogger : ILogger
{
    public string CategoryName { get; }
    public IList<LogEntry> Logs { get; } = new List<LogEntry>();

    public ListLogger(string categoryName) => CategoryName = categoryName;

    public IDisposable BeginScope<TState>(TState state) => throw new NotSupportedException();
    public bool IsEnabled(LogLevel logLevel) => throw new NotSupportedException();

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter) =>
        Logs.Add(new LogEntry(logLevel, eventId, state.ToString(), exception));

    public record LogEntry(LogLevel LogLevel, EventId EventId, string Message, Exception Exception);
}

[SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "Nothing to dispose.")]
[SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Nothing to dispose.")]
public class ListLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ListLogger> _loggers = new();

    public ILogger CreateLogger(string categoryName) =>
        _loggers.TryGetValue(categoryName, out var logger) ?
            logger :
            _loggers.GetOrAdd(categoryName, new ListLogger(categoryName));

    public void Dispose() => GC.SuppressFinalize(this);
}
