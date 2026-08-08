using System.Net;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DatabaseMastery.TransportMongoDb.Tests.ErrorHandling;

public sealed class ProductionErrorTests
{
  [Fact]
  public async Task UnhandledException_ReturnsGenericProductionError()
  {
    using var factory = new ProductionErrorWebApplicationFactory();
    var client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
      AllowAutoRedirect = false,
      BaseAddress = new Uri("https://localhost"),
    });

    var response = await client.GetAsync("/TestFailure/Index");

    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

    var body = await response.Content.ReadAsStringAsync();

    Assert.Contains("Bir hata oluştu", body, StringComparison.Ordinal);
    Assert.Contains("Beklenmeyen bir sorun oluştu", body, StringComparison.Ordinal);
    Assert.Contains("İstek Referansı", body, StringComparison.Ordinal);

    Assert.DoesNotContain(TestFailureController.ExceptionMessage, body, StringComparison.Ordinal);
    Assert.DoesNotContain("InvalidOperationException", body, StringComparison.Ordinal);
    Assert.DoesNotContain("System.", body, StringComparison.Ordinal);
    Assert.DoesNotContain(".cs:line", body, StringComparison.Ordinal);
    Assert.DoesNotContain("mongodb://", body, StringComparison.OrdinalIgnoreCase);
    Assert.DoesNotContain("TransportDb", body, StringComparison.Ordinal);
    Assert.DoesNotContain("password", body, StringComparison.OrdinalIgnoreCase);

    Assert.Contains("İstek Referansı", body, StringComparison.Ordinal);
    Assert.Matches(
      "<code>(?<reference>[0-9a-fA-F]{32})</code>",
      body);

    RequestTelemetryTestHelpers.GetResponseCorrelationId(response);
  }

  [Fact]
  public async Task UnhandledException_PreservesSecurityHeadersAndNoStore()
  {
    using var factory = new ProductionErrorWebApplicationFactory();
    var client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
      AllowAutoRedirect = false,
      BaseAddress = new Uri("https://localhost"),
    });

    var response = await client.GetAsync("/TestFailure/Index");

    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

    HttpTestHelpers.AssertBaselineSecurityHeaders(response);
    HttpTestHelpers.AssertSingleHeader(response, "Referrer-Policy", "strict-origin-when-cross-origin");
    HttpTestHelpers.AssertCorrelationId(response);
    HttpTestHelpers.AssertHeaderContains(response, "Cache-Control", "no-store");
    HttpTestHelpers.AssertHeaderContains(response, "Cache-Control", "no-cache");
  }
}
