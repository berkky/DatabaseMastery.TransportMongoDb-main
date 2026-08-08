using DatabaseMastery.TransportMongoDb.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DatabaseMastery.TransportMongoDb.Infrastructure.Operations
{
    public sealed class MongoDbHealthCheck : IHealthCheck
    {
        private readonly IMongoClient _mongoClient;
        private readonly ILogger<MongoDbHealthCheck> _logger;

        public MongoDbHealthCheck(
            IMongoClient mongoClient,
            ILogger<MongoDbHealthCheck> logger)
        {
            _mongoClient = mongoClient;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _mongoClient
                    .GetDatabase("admin")
                    .RunCommandAsync<BsonDocument>(
                        new BsonDocument("ping", 1),
                        cancellationToken: cancellationToken);

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    "Dependency health check failed: {ExceptionType}",
                    ex.GetType().Name);

                return HealthCheckResult.Unhealthy();
            }
        }
    }

    public sealed class DatabaseConfigurationHealthCheck : IHealthCheck
    {
        private readonly IDatabaseSettings _databaseSettings;

        public DatabaseConfigurationHealthCheck(IDatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_databaseSettings.ConnectionString)
                || string.IsNullOrWhiteSpace(_databaseSettings.DatabaseName))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy());
            }

            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
