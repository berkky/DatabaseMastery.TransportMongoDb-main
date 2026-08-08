using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

internal static class HttpTestHelpers
{
    private static readonly Regex CorrelationIdRegex = new(
        "^[0-9a-fA-F]{32}$",
        RegexOptions.Compiled);

    private static readonly Regex AntiforgeryTokenRegex = new(
        "name=\"__RequestVerificationToken\"\\s+type=\"hidden\"\\s+value=\"([^\"]+)\"",
        RegexOptions.Compiled);

    public static void AssertSingleHeader(HttpResponseMessage response, string headerName, string expectedValue)
    {
        Assert.True(
            response.Headers.TryGetValues(headerName, out var values) ||
            response.Content.Headers.TryGetValues(headerName, out values),
            $"Expected header '{headerName}' to be present.");

        var headerValues = values!.ToArray();
        Assert.Single(headerValues);
        Assert.Equal(expectedValue, headerValues[0]);
    }

    public static void AssertHeaderContains(HttpResponseMessage response, string headerName, string expectedPart)
    {
        Assert.True(
            response.Headers.TryGetValues(headerName, out var values) ||
            response.Content.Headers.TryGetValues(headerName, out values),
            $"Expected header '{headerName}' to be present.");

        var headerValue = Assert.Single(values!.ToArray());
        Assert.Contains(expectedPart, headerValue, StringComparison.OrdinalIgnoreCase);
    }

    public static void AssertHeaderAbsent(HttpResponseMessage response, string headerName, string forbiddenValue)
    {
        if (!response.Headers.TryGetValues(headerName, out var values) &&
            !response.Content.Headers.TryGetValues(headerName, out values))
        {
            return;
        }

        Assert.DoesNotContain(
            forbiddenValue,
            values!,
            StringComparer.OrdinalIgnoreCase);
    }

    public static void AssertBaselineSecurityHeaders(HttpResponseMessage response)
    {
        AssertSingleHeader(response, "X-Content-Type-Options", "nosniff");
        AssertSingleHeader(response, "X-Frame-Options", "DENY");
        AssertSingleHeader(response, "Content-Security-Policy", "frame-ancestors 'none'");
    }

    public static void AssertCorrelationId(HttpResponseMessage response)
    {
        Assert.True(
            response.Headers.TryGetValues("X-Correlation-ID", out var values),
            "Expected X-Correlation-ID response header.");

        var correlationId = Assert.Single(values!.ToArray());
        Assert.Matches(CorrelationIdRegex, correlationId);
    }

    public static void AssertNoStore(HttpResponseMessage response)
    {
        AssertHeaderContains(response, "Cache-Control", "no-store");
    }

    public static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content);
    }

    public static void AssertMinimalHealthBody(JsonDocument document, string expectedStatus)
    {
        Assert.Equal(JsonValueKind.Object, document.RootElement.ValueKind);
        Assert.Equal(1, document.RootElement.EnumerateObject().Count());
        Assert.True(document.RootElement.TryGetProperty("status", out var statusProperty));
        Assert.Equal(expectedStatus, statusProperty.GetString());
    }

    public static string ExtractAntiforgeryToken(string html)
    {
        var match = AntiforgeryTokenRegex.Match(html);
        Assert.True(match.Success, "Antiforgery token hidden input was not found.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }
}
