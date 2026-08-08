using System.Net;
using DatabaseMastery.TransportMongoDb.Tests.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseMastery.TransportMongoDb.Tests.CacheProtection;

public sealed class AuthenticatedNoStoreTests
{
    [Fact]
    public async Task AuthenticatedAdminMvcResponse_ReturnsNoStoreHeaders()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/AdminLayout/Index");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpTestHelpers.AssertAuthenticatedNoStoreHeaders(response);
        HttpTestHelpers.AssertBaselineSecurityHeaders(response);
    }

    [Fact]
    public async Task AuthenticatedPublicMvcResponse_AlsoReturnsNoStore()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpTestHelpers.AssertAuthenticatedNoStoreHeaders(response);
    }

    [Fact]
    public async Task AnonymousLanding_DoesNotReceiveAuthenticatedNoStore()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpTestHelpers.AssertNoAuthenticatedNoStoreSignature(response);

        if (HttpTestHelpers.TryGetHeaderValue(response, "Pragma", out var pragma))
        {
            Assert.NotEqual("no-cache", pragma, StringComparer.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task AuthenticatedStaticAsset_IsNotAffectedByMvcNoStoreFilter()
    {
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var assetPath = Path.Combine(repoRoot, "wwwroot", "templates", "assets", "css", "style.css");
        Assert.True(File.Exists(assetPath), "Expected static asset file to exist.");

        using var factory = new AuthenticatedWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/templates/assets/css/style.css");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpTestHelpers.AssertNoAuthenticatedNoStoreSignature(response);
    }

    [Fact]
    public void AuthenticatedAdminResponse_DoesNotUseRealAdminCookie()
    {
        using var factory = new AuthenticatedWebApplicationFactory();
        var client = factory.CreateClient();

        Assert.Equal(TestAuthDefaults.Scheme, factory.Services
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<AuthenticationOptions>>()
            .Value.DefaultAuthenticateScheme);

        Assert.DoesNotContain(
            client.DefaultRequestHeaders,
            header => string.Equals(header.Key, "Cookie", StringComparison.OrdinalIgnoreCase));
    }
}
