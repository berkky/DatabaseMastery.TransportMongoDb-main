using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

public sealed class LoginIpRateLimitWebApplicationFactory : CustomWebApplicationFactory
{
    public int IpPermitLimit { get; init; } = 5;

    public int IpWindowSeconds { get; init; } = 10;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AdminLoginRateLimit:IpPermitLimit"] = IpPermitLimit.ToString(),
                ["AdminLoginRateLimit:IpWindowSeconds"] = IpWindowSeconds.ToString(),
                ["AdminLoginRateLimit:IdentityPermitLimit"] = "100",
                ["AdminLoginRateLimit:IdentityWindowSeconds"] = "60",
                ["AdminLoginRateLimit:PartitionIdleMinutes"] = "30",
                ["AdminLoginRateLimit:MaxPartitions"] = "10000",
            });
        });

        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IStartupFilter, TestRemoteIpStartupFilter>();
        });
    }
}
