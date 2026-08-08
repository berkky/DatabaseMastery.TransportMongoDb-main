using System.Net;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure.Logging;
using Microsoft.Extensions.Logging;

namespace DatabaseMastery.TransportMongoDb.Tests.Telemetry;

public sealed class RequestTelemetryTests
{
  private const string QuerySentinel = "TELEMETRY_QUERY_SENTINEL_48";
  private const string BodySentinel = "TELEMETRY_BODY_SENTINEL_48";
  private const string UsernameSentinel = "TELEMETRY_USERNAME_SENTINEL_48";
  private const string PasswordSentinel = "TELEMETRY_PASSWORD_SENTINEL_48";
  private const string TrackingSentinel = "TELEMETRY_TRACKING_SENTINEL_48";
  private const string CookieSentinel = "TELEMETRY_COOKIE_SENTINEL_48";
  private const string AuthSentinel = "TELEMETRY_AUTH_SENTINEL_48";
  private const string ForwardedIpSentinel = "TELEMETRY_FORWARDED_IP_SENTINEL_48";
  private const string ClientCorrelationSentinel = "TELEMETRY_CLIENT_CORRELATION_SENTINEL_48";
  private const string ConnectionStringSentinel = "mongodb://TELEMETRY_CONNECTION_SENTINEL_48";
  private const string DatabaseNameSentinel = "TransportDb_TELEMETRY_SENTINEL_48";

  [Fact]
  public async Task SuccessfulMvcRequest_LogsExpectedSafeTelemetry()
  {
    using var factory = new TelemetryWebApplicationFactory();
    var client = factory.CreateClient();

    var response = await client.GetAsync("/");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var correlationId = RequestTelemetryTestHelpers.GetResponseCorrelationId(response);
    var entry = RequestTelemetryTestHelpers.GetSingleCompletionEntry(factory.LogCapture, "/");

    RequestTelemetryTestHelpers.AssertSafeCompletionFields(
      entry,
      expectedMethod: "GET",
      expectedPath: "/",
      expectedStatusCode: 200,
      expectedCorrelationId: correlationId);
  }

  [Fact]
  public async Task SensitiveRequestData_IsNotLoggedByRequestTelemetry()
  {
    using var factory = new TelemetryWebApplicationFactory();
    var client = factory.CreateClient();

    var request = new HttpRequestMessage(
      HttpMethod.Get,
      $"/telemetry-test-privacy-route?trackingNumber={TrackingSentinel}&username={UsernameSentinel}&password={PasswordSentinel}");

    request.Headers.Add("Cookie", $".TransportAdmin.Auth={CookieSentinel}");
    request.Headers.Add("Authorization", $"Bearer {AuthSentinel}");
    request.Headers.Add("X-Forwarded-For", ForwardedIpSentinel);
    request.Headers.Add("X-Correlation-ID", ClientCorrelationSentinel);

    var response = await client.SendAsync(request);

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

    var correlationId = RequestTelemetryTestHelpers.GetResponseCorrelationId(response);
    var telemetryEntries = factory.LogCapture.GetRequestTelemetryEntries();

    RequestTelemetryTestHelpers.AssertSentinelsAbsent(
      telemetryEntries,
      QuerySentinel,
      BodySentinel,
      UsernameSentinel,
      PasswordSentinel,
      TrackingSentinel,
      CookieSentinel,
      AuthSentinel,
      ForwardedIpSentinel,
      ClientCorrelationSentinel,
      ConnectionStringSentinel,
      DatabaseNameSentinel,
      "?trackingNumber=",
      "Bearer ",
      "X-Forwarded-For");

    RequestTelemetryTestHelpers.AssertServerCorrelationPresent(telemetryEntries, correlationId);
    Assert.NotEqual(ClientCorrelationSentinel, correlationId);
  }

  [Fact]
  public async Task QueryString_IsExcludedFromTelemetryPath()
  {
    using var factory = new TelemetryWebApplicationFactory();
    var client = factory.CreateClient();

    var response = await client.GetAsync($"/?trackingNumber={QuerySentinel}");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var entry = RequestTelemetryTestHelpers.GetSingleCompletionEntry(factory.LogCapture, "/");

    Assert.Equal("/", entry.State["Path"]?.ToString());
    RequestTelemetryTestHelpers.AssertSentinelsAbsent(factory.LogCapture.GetRequestTelemetryEntries(), QuerySentinel, "?trackingNumber=");
  }

  [Fact]
  public async Task ClientError_IsLoggedAsWarning()
  {
    using var factory = new TelemetryWebApplicationFactory();
    var client = factory.CreateClient();

    var response = await client.GetAsync("/telemetry-test-missing-route");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

    var correlationId = RequestTelemetryTestHelpers.GetResponseCorrelationId(response);
    var entry = RequestTelemetryTestHelpers.GetSingleCompletionEntry(
      factory.LogCapture,
      "/telemetry-test-missing-route");

    RequestTelemetryTestHelpers.AssertWarningCompletionFields(
      entry,
      expectedPath: "/telemetry-test-missing-route",
      expectedStatusCode: 404,
      expectedCorrelationId: correlationId);
  }

  [Fact]
  public async Task StaticAsset_IsExcludedFromRequestTelemetry()
  {
    var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    var assetPath = Path.Combine(repoRoot, "wwwroot", "templates", "assets", "css", "style.css");
    Assert.True(File.Exists(assetPath), "Expected static asset file to exist.");

    using var factory = new TelemetryWebApplicationFactory();
    var client = factory.CreateClient();

    var response = await client.GetAsync("/templates/assets/css/style.css");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.DoesNotContain(
      factory.LogCapture.GetRequestTelemetryEntries(),
      entry => entry.State.TryGetValue("Path", out var path) &&
               string.Equals(path?.ToString(), "/templates/assets/css/style.css", StringComparison.OrdinalIgnoreCase));
  }
}
