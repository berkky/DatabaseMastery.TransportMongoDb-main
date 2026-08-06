using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.AdminUserServices
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IMongoCollection<AdminUser> _adminUserCollection;
        private readonly AdminLoginSecurityOptions _loginSecurityOptions;

        public AdminUserService(
            IMongoClient mongoClient,
            IDatabaseSettings databaseSettings,
            IOptions<AdminLoginSecurityOptions> loginSecurityOptions)
        {
            var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);

            _adminUserCollection = database.GetCollection<AdminUser>(
                databaseSettings.AdminUserCollectionName);

            _loginSecurityOptions = loginSecurityOptions.Value;
        }

        public async Task<AdminUser?> GetByNormalizedUsernameAsync(
            string normalizedUsername)
        {
            return await _adminUserCollection
                .Find(x => x.NormalizedUsername == normalizedUsername)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AnyAsync()
        {
            return await _adminUserCollection
                .Find(FilterDefinition<AdminUser>.Empty)
                .Limit(1)
                .AnyAsync();
        }

        public async Task CreateAsync(AdminUser adminUser)
        {
            await _adminUserCollection.InsertOneAsync(adminUser);
        }

        public async Task EnsureIndexesAsync()
        {
            var indexKeys = Builders<AdminUser>.IndexKeys
                .Ascending(x => x.NormalizedUsername);

            var indexOptions = new CreateIndexOptions
            {
                Unique = true,
                Name = "ux_admin_users_normalized_username"
            };

            await _adminUserCollection.Indexes.CreateOneAsync(
                new CreateIndexModel<AdminUser>(indexKeys, indexOptions));
        }

        public async Task<AdminLoginFailureResult> RegisterFailedLoginAsync(
            string adminUserId,
            DateTime utcNow)
        {
            if (string.IsNullOrWhiteSpace(adminUserId))
            {
                throw new ArgumentException(
                    "Admin user id is required.",
                    nameof(adminUserId));
            }

            var maxFailedAttempts = _loginSecurityOptions.MaxFailedAttempts;
            var lockoutMinutes = _loginSecurityOptions.LockoutMinutes;

            for (var attempt = 0; attempt < 5; attempt++)
            {
                var current = await _adminUserCollection
                    .Find(x => x.AdminUserId == adminUserId)
                    .FirstOrDefaultAsync();

                if (current is null)
                {
                    return new AdminLoginFailureResult
                    {
                        FailedLoginCount = 0,
                        LockoutEndUtc = null,
                        IsLockedOut = false
                    };
                }

                int newFailedCount;
                DateTime? newLockoutEndUtc;

                if (current.LockoutEndUtc.HasValue &&
                    current.LockoutEndUtc.Value <= utcNow)
                {
                    newFailedCount = 1;
                    newLockoutEndUtc = null;
                }
                else
                {
                    newFailedCount = current.FailedLoginCount + 1;
                    newLockoutEndUtc = current.LockoutEndUtc;
                }

                if (newFailedCount >= maxFailedAttempts)
                {
                    newLockoutEndUtc = utcNow.AddMinutes(lockoutMinutes);
                }

                var filter = Builders<AdminUser>.Filter.And(
                    Builders<AdminUser>.Filter.Eq(x => x.AdminUserId, adminUserId),
                    Builders<AdminUser>.Filter.Eq(
                        x => x.FailedLoginCount,
                        current.FailedLoginCount),
                    Builders<AdminUser>.Filter.Eq(
                        x => x.LockoutEndUtc,
                        current.LockoutEndUtc));

                var update = Builders<AdminUser>.Update
                    .Set(x => x.FailedLoginCount, newFailedCount)
                    .Set(x => x.LockoutEndUtc, newLockoutEndUtc)
                    .Set(x => x.UpdatedAtUtc, utcNow);

                var updated = await _adminUserCollection.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<AdminUser>
                    {
                        ReturnDocument = ReturnDocument.After
                    });

                if (updated is not null)
                {
                    return new AdminLoginFailureResult
                    {
                        FailedLoginCount = updated.FailedLoginCount,
                        LockoutEndUtc = updated.LockoutEndUtc,
                        IsLockedOut = updated.LockoutEndUtc.HasValue &&
                                      updated.LockoutEndUtc.Value > utcNow
                    };
                }
            }

            throw new InvalidOperationException(
                "Failed to register admin login failure atomically.");
        }

        public async Task RegisterSuccessfulLoginAsync(
            string adminUserId,
            DateTime utcNow,
            string? newPasswordHash = null)
        {
            if (string.IsNullOrWhiteSpace(adminUserId))
            {
                throw new ArgumentException(
                    "Admin user id is required.",
                    nameof(adminUserId));
            }

            var filter = Builders<AdminUser>.Filter.Eq(
                x => x.AdminUserId,
                adminUserId);

            var update = Builders<AdminUser>.Update
                .Set(x => x.FailedLoginCount, 0)
                .Set(x => x.LockoutEndUtc, null)
                .Set(x => x.LastLoginAtUtc, utcNow)
                .Set(x => x.UpdatedAtUtc, utcNow);

            if (!string.IsNullOrEmpty(newPasswordHash))
            {
                update = update.Set(x => x.PasswordHash, newPasswordHash);
            }

            await _adminUserCollection.FindOneAndUpdateAsync(filter, update);
        }
    }
}
