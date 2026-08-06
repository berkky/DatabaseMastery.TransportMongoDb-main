using DatabaseMastery.TransportMongoDb.Entities;

namespace DatabaseMastery.TransportMongoDb.Services.AdminUserServices
{
    public interface IAdminUserService
    {
        Task<AdminUser?> GetByNormalizedUsernameAsync(string normalizedUsername);

        Task<bool> AnyAsync();

        Task CreateAsync(AdminUser adminUser);

        Task EnsureIndexesAsync();
    }
}
