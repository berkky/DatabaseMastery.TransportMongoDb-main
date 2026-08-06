using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Security;
using DatabaseMastery.TransportMongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.AdminUserServices
{
    public sealed class AdminBootstrapHostedService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IAdminCredentialService _credentialService;
        private readonly IOptions<AdminBootstrapOptions> _bootstrapOptions;
        private readonly ILogger<AdminBootstrapHostedService> _logger;

        public AdminBootstrapHostedService(
            IServiceScopeFactory scopeFactory,
            IAdminCredentialService credentialService,
            IOptions<AdminBootstrapOptions> bootstrapOptions,
            ILogger<AdminBootstrapHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _credentialService = credentialService;
            _bootstrapOptions = bootstrapOptions;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var adminUserService = scope.ServiceProvider
                .GetRequiredService<IAdminUserService>();

            await adminUserService.EnsureIndexesAsync();
            _logger.LogInformation("Admin user indexes ensured.");

            var options = _bootstrapOptions.Value;
            if (string.IsNullOrWhiteSpace(options.Username) ||
                string.IsNullOrWhiteSpace(options.Password))
            {
                _logger.LogInformation(
                    "Admin bootstrap skipped because configuration is missing.");
                return;
            }

            if (await adminUserService.AnyAsync())
            {
                _logger.LogInformation(
                    "Admin bootstrap skipped because an admin user already exists.");
                return;
            }

            var username = options.Username.Trim();
            if (username.Length is < 3 or > 64)
            {
                _logger.LogWarning(
                    "Admin bootstrap skipped because username length is invalid.");
                return;
            }

            if (options.Password.Length < 14)
            {
                _logger.LogWarning(
                    "Admin bootstrap skipped because password length is invalid.");
                return;
            }

            var utcNow = DateTime.UtcNow;
            var adminUser = new AdminUser
            {
                Username = username,
                NormalizedUsername = _credentialService.NormalizeUsername(username),
                Role = AdminRoles.SuperAdmin,
                IsActive = true,
                CreatedAtUtc = utcNow,
                UpdatedAtUtc = utcNow,
                LastLoginAtUtc = null,
                FailedLoginCount = 0,
                LockoutEndUtc = null
            };

            adminUser.PasswordHash = _credentialService.HashPassword(
                adminUser,
                options.Password);

            try
            {
                await adminUserService.CreateAsync(adminUser);
                _logger.LogInformation("Initial SuperAdmin user created.");
            }
            catch (MongoWriteException ex)
                when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                _logger.LogInformation(
                    "Admin bootstrap skipped because of concurrent creation.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
