using DatabaseMastery.TransportMongoDb.Entities;
using DatabaseMastery.TransportMongoDb.Settings;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Services.AdminUserServices
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IMongoCollection<AdminUser> _adminUserCollection;

        public AdminUserService(
            IMongoClient mongoClient,
            IDatabaseSettings databaseSettings)
        {
            var database = mongoClient.GetDatabase(databaseSettings.DatabaseName);

            _adminUserCollection = database.GetCollection<AdminUser>(
                databaseSettings.AdminUserCollectionName);
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
    }
}
