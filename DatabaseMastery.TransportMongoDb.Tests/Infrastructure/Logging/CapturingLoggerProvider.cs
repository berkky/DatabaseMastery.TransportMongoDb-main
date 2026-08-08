using System.Collections.Concurrent;
using DatabaseMastery.TransportMongoDb.Infrastructure.Operations;
using Microsoft.Extensions.Logging;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure.Logging;

public sealed class CapturingLoggerProvider : ILoggerProvider
{
    public const string RequestTelemetryCategory =
        "DatabaseMastery.TransportMongoDb.Infrastructure.Operations.RequestTelemetryMiddleware";

    private readonly ConcurrentQueue<CapturedLogEntry> _entries = new();
    private readonly ConcurrentDictionary<string, CapturingLogger> _loggers = new(StringComparer.Ordinal);

    public IReadOnlyCollection<CapturedLogEntry> Entries => _entries.ToArray();

    public IReadOnlyCollection<CapturedLogEntry> GetRequestTelemetryEntries()
    {
        return _entries
            .Where(entry => string.Equals(entry.Category, RequestTelemetryCategory, StringComparison.Ordinal))
            .ToArray();
    }

    public void Clear()
    {
        while (_entries.TryDequeue(out _))
        {
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, name => new CapturingLogger(name, _entries));
    }

    public void Dispose()
    {
        _loggers.Clear();
        Clear();
    }

    private sealed class CapturingLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly ConcurrentQueue<CapturedLogEntry> _entries;

        public CapturingLogger(string categoryName, ConcurrentQueue<CapturedLogEntry> entries)
        {
            _categoryName = categoryName;
            _entries = entries;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var stateValues = new Dictionary<string, object?>(StringComparer.Ordinal);

            if (state is IEnumerable<KeyValuePair<string, object?>> keyValuePairs)
            {
                foreach (var pair in keyValuePairs)
                {
                    if (pair.Key == "{OriginalFormat}")
                    {
                        continue;
                    }

                    stateValues[pair.Key] = pair.Value;
                }
            }

            _entries.Enqueue(new CapturedLogEntry
            {
                Category = _categoryName,
                LogLevel = logLevel,
                EventId = eventId,
                Message = formatter(state, exception),
                State = stateValues,
                Exception = exception,
            });
        }
    }
}
