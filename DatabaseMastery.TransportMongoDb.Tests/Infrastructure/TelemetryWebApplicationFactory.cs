using DatabaseMastery.TransportMongoDb.Tests.Infrastructure.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

public sealed class TelemetryWebApplicationFactory : CustomWebApplicationFactory
{
    public CapturingLoggerProvider LogCapture { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton(LogCapture);
            services.AddSingleton<ILoggerProvider>(LogCapture);
        });
    }
}
