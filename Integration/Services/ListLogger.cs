using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.Tests.Integration.Services;

public class ListLogger : ILogger
{
    public string CategoryName { get; }

    public ListLogger(string categoryName) =>
        CategoryName = categoryName;

    public IDisposable BeginScope<TState>(TState state) =>
        throw new NotSupportedException();

    public bool IsEnabled(LogLevel logLevel) =>
        throw new NotSupportedException();

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter) =>
        throw new NotSupportedException();
}

public class ListLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ListLogger> _loggers = new();

    public ILogger CreateLogger(string categoryName) =>
        _loggers.TryGetValue(categoryName, out var logger) ?
            logger :
            _loggers.GetOrAdd(categoryName, new ListLogger(categoryName));

    [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "Nothing to dispose.")]
    public void Dispose() => GC.SuppressFinalize(this);
}
