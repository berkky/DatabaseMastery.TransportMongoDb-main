using DatabaseMastery.TransportMongoDb.Tests.Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

public sealed class ProductionErrorWebApplicationFactory : CustomWebApplicationFactory
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseEnvironment(Environments.Production);

        builder.ConfigureTestServices(services =>
        {
            services.AddControllersWithViews()
                .AddApplicationPart(typeof(TestFailureController).Assembly);
        });
    }
}
