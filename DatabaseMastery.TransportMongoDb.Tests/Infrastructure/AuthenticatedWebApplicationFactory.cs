using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

public sealed class AuthenticatedWebApplicationFactory : CustomWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(TestAuthDefaults.Scheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthDefaults.Scheme,
                    _ => { });

            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthDefaults.Scheme;
                options.DefaultChallengeScheme = TestAuthDefaults.Scheme;
                options.DefaultSignInScheme = TestAuthDefaults.Scheme;
            });
        });
    }
}
