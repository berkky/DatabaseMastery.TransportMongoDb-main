using Microsoft.Extensions.Logging;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure.Logging;

public sealed class CapturedLogEntry
{
    public required string Category { get; init; }

    public required LogLevel LogLevel { get; init; }

    public EventId EventId { get; init; }

    public required string Message { get; init; }

    public IReadOnlyDictionary<string, object?> State { get; init; } =
        new Dictionary<string, object?>();

    public Exception? Exception { get; init; }

    public bool ContainsValue(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        if (Message.Contains(value, StringComparison.Ordinal))
        {
            return true;
        }

        foreach (var stateValue in State.Values)
        {
            if (stateValue?.ToString()?.Contains(value, StringComparison.Ordinal) == true)
            {
                return true;
            }
        }

        return Exception?.Message.Contains(value, StringComparison.Ordinal) == true
            || Exception?.ToString().Contains(value, StringComparison.Ordinal) == true;
    }
}
