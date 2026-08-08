using DatabaseMastery.TransportMongoDb.HostedServices;
using DatabaseMastery.TransportMongoDb.Services.AboutServices;
using DatabaseMastery.TransportMongoDb.Services.AdminUserServices;
using DatabaseMastery.TransportMongoDb.Services.BrandServices;
using DatabaseMastery.TransportMongoDb.Services.GetInTouchServices;
using DatabaseMastery.TransportMongoDb.Services.HowItWorksServices;
using DatabaseMastery.TransportMongoDb.Services.OfferServices;
using DatabaseMastery.TransportMongoDb.Services.ProjectServices;
using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;
using DatabaseMastery.TransportMongoDb.Services.SliderServices;
using DatabaseMastery.TransportMongoDb.Services.TestimonialServices;
using DatabaseMastery.TransportMongoDb.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly HealthStatus _readyHealthStatus;

    public CustomWebApplicationFactory()
        : this(HealthStatus.Healthy)
    {
    }

    protected CustomWebApplicationFactory(HealthStatus readyHealthStatus)
    {
        _readyHealthStatus = readyHealthStatus;
    }

    public SpyShipmentService ShipmentServiceSpy { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DatabaseSettings:ConnectionString"] =
                    "mongodb://127.0.0.1:65535/?connectTimeoutMS=1&serverSelectionTimeoutMS=1",
                ["DatabaseSettings:DatabaseName"] = "TransportDb_SecurityHttpTests",
                ["DatabaseSettings:SliderCollectionName"] = "Sliders",
                ["DatabaseSettings:BrandCollectionName"] = "Brands",
                ["DatabaseSettings:OfferCollectionName"] = "Offers",
                ["DatabaseSettings:AboutCollectionName"] = "Abouts",
                ["DatabaseSettings:HowItWorksCollectionName"] = "HowItWorks",
                ["DatabaseSettings:TestimonialCollectionName"] = "Testimonials",
                ["DatabaseSettings:GetInTouchCollectionName"] = "GetInTouches",
                ["DatabaseSettings:ProjectCollectionName"] = "Projects",
                ["DatabaseSettings:ShipmentCollectionName"] = "Shipments",
                ["DatabaseSettings:AdminUserCollectionName"] = "AdminUsers",
            });
        });

        builder.ConfigureTestServices(services =>
        {
            RemoveHostedService<AdminBootstrapHostedService>(services);
            RemoveHostedService<ShipmentIndexHostedService>(services);

            services.RemoveAll<IMongoClient>();
            services.AddSingleton<IMongoClient>(_ =>
                throw new InvalidOperationException("MongoDB client must not be used in security HTTP tests."));

            services.RemoveAll<IShipmentService>();
            services.AddSingleton<IShipmentService>(ShipmentServiceSpy);

            services.RemoveAll<ISliderService>();
            services.AddSingleton<ISliderService, StubSliderService>();
            services.RemoveAll<IBrandService>();
            services.AddSingleton<IBrandService, StubBrandService>();
            services.RemoveAll<IOfferService>();
            services.AddSingleton<IOfferService, StubOfferService>();
            services.RemoveAll<IAboutService>();
            services.AddSingleton<IAboutService, StubAboutService>();
            services.RemoveAll<IGetInTouchServices>();
            services.AddSingleton<IGetInTouchServices, StubGetInTouchService>();
            services.RemoveAll<IHowItWorksService>();
            services.AddSingleton<IHowItWorksService, StubHowItWorksService>();
            services.RemoveAll<ITestimonialService>();
            services.AddSingleton<ITestimonialService, StubTestimonialService>();
            services.RemoveAll<IProjectService>();
            services.AddSingleton<IProjectService, StubProjectService>();

            services.RemoveAll<IAdminUserService>();
            services.AddSingleton<IAdminUserService, StubAdminUserService>();

            services.AddSingleton(new TestHealthCheck(_readyHealthStatus));
            services.Configure<HealthCheckServiceOptions>(options =>
            {
                options.Registrations.Clear();
                options.Registrations.Add(new HealthCheckRegistration(
                    "self",
                    _ => new TestHealthCheck(HealthStatus.Healthy),
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["live"],
                    timeout: TimeSpan.FromSeconds(5)));

                options.Registrations.Add(new HealthCheckRegistration(
                    "database_configuration",
                    _ => new TestHealthCheck(HealthStatus.Healthy),
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["ready"],
                    timeout: TimeSpan.FromSeconds(5)));

                options.Registrations.Add(new HealthCheckRegistration(
                    "ready",
                    sp => sp.GetRequiredService<TestHealthCheck>(),
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["ready"],
                    timeout: TimeSpan.FromSeconds(5)));
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var databaseSettings = scope.ServiceProvider
            .GetRequiredService<IOptions<DatabaseSettings>>()
            .Value;

        if (string.Equals(
                databaseSettings.DatabaseName,
                "TransportDb",
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Test host resolved production/default database name TransportDb.");
        }

        if (!IsLocalConnectionString(databaseSettings.ConnectionString))
        {
            throw new InvalidOperationException(
                "Test host resolved a non-local MongoDB connection string.");
        }

        return host;
    }

    private static bool IsLocalConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        return connectionString.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase)
            || connectionString.Contains("localhost", StringComparison.OrdinalIgnoreCase)
            || connectionString.Contains("::1", StringComparison.Ordinal);
    }

    private static void RemoveHostedService<TImplementation>(IServiceCollection services)
        where TImplementation : class, IHostedService
    {
        var descriptor = services.FirstOrDefault(service =>
            service.ServiceType == typeof(IHostedService) &&
            service.ImplementationType == typeof(TImplementation));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
    }
}

public sealed class UnhealthyReadyWebApplicationFactory : CustomWebApplicationFactory
{
    public UnhealthyReadyWebApplicationFactory()
        : base(HealthStatus.Unhealthy)
    {
    }
}
