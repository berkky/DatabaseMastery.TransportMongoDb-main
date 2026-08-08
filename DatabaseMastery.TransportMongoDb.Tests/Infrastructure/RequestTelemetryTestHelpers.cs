using System.Net;
using DatabaseMastery.TransportMongoDb.Infrastructure.Operations;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure.Logging;
using Microsoft.Extensions.Logging;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

internal static class RequestTelemetryTestHelpers
{
    public static CapturedLogEntry GetSingleCompletionEntry(
        CapturingLoggerProvider logCapture,
        string expectedPath)
    {
        var entries = logCapture.GetRequestTelemetryEntries()
            .Where(entry => entry.Message.Contains("responded", StringComparison.Ordinal))
            .Where(entry =>
                entry.State.TryGetValue("Path", out var path) &&
                string.Equals(path?.ToString(), expectedPath, StringComparison.Ordinal))
            .ToArray();

        return Assert.Single(entries);
    }

    public static void AssertSafeCompletionFields(
        CapturedLogEntry entry,
        string expectedMethod,
        string expectedPath,
        int expectedStatusCode,
        string expectedCorrelationId)
    {
        Assert.Equal(LogLevel.Information, entry.LogLevel);
        Assert.Equal(expectedMethod, entry.State["Method"]?.ToString());
        Assert.Equal(expectedPath, entry.State["Path"]?.ToString());
        Assert.Equal(expectedStatusCode, entry.State["StatusCode"]);
        Assert.True(entry.State.ContainsKey("ElapsedMs"));
        Assert.True(Convert.ToInt64(entry.State["ElapsedMs"]) >= 0);
        Assert.Equal(expectedCorrelationId, entry.State["CorrelationId"]?.ToString());
        Assert.Contains(expectedMethod, entry.Message, StringComparison.Ordinal);
        Assert.Contains(expectedPath, entry.Message, StringComparison.Ordinal);
        Assert.Contains(expectedStatusCode.ToString(), entry.Message, StringComparison.Ordinal);
        Assert.Contains(expectedCorrelationId, entry.Message, StringComparison.Ordinal);
        Assert.Contains("ms", entry.Message, StringComparison.Ordinal);
    }

    public static void AssertWarningCompletionFields(
        CapturedLogEntry entry,
        string expectedPath,
        int expectedStatusCode,
        string expectedCorrelationId)
    {
        Assert.Equal(LogLevel.Warning, entry.LogLevel);
        Assert.Equal(expectedPath, entry.State["Path"]?.ToString());
        Assert.Equal(expectedStatusCode, entry.State["StatusCode"]);
        Assert.Equal(expectedCorrelationId, entry.State["CorrelationId"]?.ToString());
        Assert.DoesNotContain("?", entry.Message, StringComparison.Ordinal);
    }

    public static string GetResponseCorrelationId(HttpResponseMessage response)
    {
        Assert.True(
            response.Headers.TryGetValues(CorrelationIdMiddleware.HeaderName, out var values),
            $"Expected {CorrelationIdMiddleware.HeaderName} response header.");

        return Assert.Single(values!.ToArray());
    }

    public static void AssertSentinelsAbsent(
        IEnumerable<CapturedLogEntry> entries,
        params string[] sentinels)
    {
        foreach (var entry in entries)
        {
            foreach (var sentinel in sentinels)
            {
                Assert.False(
                    entry.ContainsValue(sentinel),
                    $"RequestTelemetry log must not contain sentinel '{sentinel}'. Message: {entry.Message}");
            }
        }
    }

    public static void AssertServerCorrelationPresent(
        IEnumerable<CapturedLogEntry> entries,
        string expectedCorrelationId)
    {
        Assert.Contains(
            entries,
            entry => string.Equals(
                entry.State.TryGetValue("CorrelationId", out var correlationId)
                    ? correlationId?.ToString()
                    : null,
                expectedCorrelationId,
                StringComparison.Ordinal));
    }
}
