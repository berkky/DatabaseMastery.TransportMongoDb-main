using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure.ReverseProxy;

public sealed class ForwardedHeadersWebApplicationFactory : CustomWebApplicationFactory
{
    public int IpPermitLimit { get; init; } = 5;

    public int IpWindowSeconds { get; init; } = 10;

    public bool UseProductionEnvironment { get; init; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        if (UseProductionEnvironment)
        {
            builder.UseEnvironment(Environments.Production);
        }

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
            services.AddControllers()
                .AddApplicationPart(typeof(ForwardedHeadersProbeController).Assembly);
        });
    }
}
