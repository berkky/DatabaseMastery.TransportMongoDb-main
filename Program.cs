using System.Globalization;
using System.Threading.RateLimiting;
using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Filters;
using DatabaseMastery.TransportMongoDb.HostedServices;
using DatabaseMastery.TransportMongoDb.Middleware;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Services.AboutServices;
using DatabaseMastery.TransportMongoDb.Services.AdminLoginRateLimiting;
using DatabaseMastery.TransportMongoDb.Services.PublicTrackingRateLimiting;
using DatabaseMastery.TransportMongoDb.Services.AdminUserServices;
using DatabaseMastery.TransportMongoDb.Services.BrandServices;
using DatabaseMastery.TransportMongoDb.Services.GetInTouchServices;
using DatabaseMastery.TransportMongoDb.Services.HowItWorksServices;
using DatabaseMastery.TransportMongoDb.Services.OfferServices;
using DatabaseMastery.TransportMongoDb.Services.ProjectServices;
using DatabaseMastery.TransportMongoDb.Services.ShipmentServices;
using DatabaseMastery.TransportMongoDb.Services.ShipmentTrackingServices;
using DatabaseMastery.TransportMongoDb.Services.SliderServices;
using DatabaseMastery.TransportMongoDb.Services.TestimonialServices;
using DatabaseMastery.TransportMongoDb.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<IGetInTouchServices, GetInTouchService>();
builder.Services.AddScoped<IHowItWorksService, HowItWorksService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IShipmentTrackingService, ShipmentTrackingService>();
builder.Services.AddScoped<ITestimonialService, TestimonialService>();
builder.Services.AddScoped<IAdminUserService, AdminUserService>();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));
builder.Services.Configure<AdminBootstrapOptions>(
    builder.Configuration.GetSection(AdminBootstrapOptions.SectionName));
builder.Services.AddOptions<AdminLoginSecurityOptions>()
    .Bind(builder.Configuration.GetSection(AdminLoginSecurityOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<AdminLoginRateLimitOptions>()
    .Bind(builder.Configuration.GetSection(AdminLoginRateLimitOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddOptions<PublicTrackingRateLimitOptions>()
    .Bind(builder.Configuration.GetSection(PublicTrackingRateLimitOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddScoped<IDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddSingleton<IPasswordHasher<AdminUser>, PasswordHasher<AdminUser>>();
builder.Services.AddSingleton<IAdminCredentialService, AdminCredentialService>();
builder.Services.AddSingleton<IAdminLoginRateLimiter, AdminLoginRateLimiter>();
builder.Services.AddSingleton<IPublicTrackingRateLimiter, PublicTrackingRateLimiter>();
builder.Services.AddHostedService<AdminBootstrapHostedService>();
builder.Services.AddHostedService<ShipmentIndexHostedService>();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        var response = context.HttpContext.Response;
        if (response.HasStarted)
        {
            return;
        }

        response.StatusCode = StatusCodes.Status429TooManyRequests;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            var retrySeconds = Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));
            response.Headers.RetryAfter =
                retrySeconds.ToString(CultureInfo.InvariantCulture);
        }

        response.ContentType = "text/plain; charset=utf-8";
        await response.WriteAsync(
            "Çok fazla giriş denemesi. Lütfen kısa süre sonra tekrar deneyin.",
            cancellationToken);
    };

    options.AddPolicy(AdminRateLimitPolicies.AdminLoginIp, httpContext =>
    {
        var rateOptions = httpContext.RequestServices
            .GetRequiredService<IOptions<AdminLoginRateLimitOptions>>()
            .Value;

        var partitionKey = httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = rateOptions.IpPermitLimit,
                Window = TimeSpan.FromSeconds(rateOptions.IpWindowSeconds),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
});

const string transportAdminScheme = "TransportAdmin";

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = transportAdminScheme;
        options.DefaultChallengeScheme = transportAdminScheme;
        options.DefaultSignInScheme = transportAdminScheme;
    })
    .AddCookie(transportAdminScheme, options =>
    {
        options.Cookie.Name = ".TransportAdmin.Auth";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();
builder.Services.AddAntiforgery(options =>
{
    options.SuppressXFrameOptionsHeader = true;
});
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AuthenticatedNoStoreFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
