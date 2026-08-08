using System.Security.Claims;
using System.Text.Encodings.Web;
using DatabaseMastery.TransportMongoDb.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

public static class TestAuthDefaults
{
    public const string Scheme = "Test";
}

public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "test-admin-id"),
            new Claim(ClaimTypes.Name, "test-admin"),
            new Claim(ClaimTypes.Role, AdminRoles.SuperAdmin),
        };

        var identity = new ClaimsIdentity(claims, TestAuthDefaults.Scheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestAuthDefaults.Scheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
