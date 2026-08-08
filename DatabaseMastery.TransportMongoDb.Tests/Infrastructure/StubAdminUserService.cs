using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Services.AdminUserServices;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

public sealed class StubAdminUserService : IAdminUserService
{
    public Task<AdminUser?> GetByNormalizedUsernameAsync(string normalizedUsername) =>
        Task.FromResult<AdminUser?>(null);

    public Task<bool> AnyAsync() => Task.FromResult(false);

    public Task CreateAsync(AdminUser adminUser) => Task.CompletedTask;

    public Task EnsureIndexesAsync() => Task.CompletedTask;

    public Task<AdminLoginFailureResult> RegisterFailedLoginAsync(
        string adminUserId,
        DateTime utcNow) =>
        Task.FromResult(new AdminLoginFailureResult());

    public Task RegisterSuccessfulLoginAsync(
        string adminUserId,
        DateTime utcNow,
        string? newPasswordHash = null) =>
        Task.CompletedTask;
}
