using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;
using DatabaseMastery.TransportMongoDb.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DatabaseMastery.TransportMongoDb.Services.AdminLoginRateLimiting
{
    public sealed class AdminLoginRateLimiter : IAdminLoginRateLimiter, IDisposable
    {
        private const char PartitionSeparator = '\u001f';

        private readonly AdminLoginRateLimitOptions _options;
        private readonly MemoryCache _cache;
        private readonly object _createLock = new();
        private bool _disposed;

        public AdminLoginRateLimiter(IOptions<AdminLoginRateLimitOptions> options)
        {
            _options = options.Value;
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = _options.MaxPartitions
            });
        }

        public async ValueTask<AdminLoginRateLimitResult> AcquireAsync(
            string normalizedUsername,
            IPAddress? remoteIpAddress,
            CancellationToken cancellationToken)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (string.IsNullOrWhiteSpace(normalizedUsername))
            {
                throw new ArgumentException(
                    "Normalized username is required.",
                    nameof(normalizedUsername));
            }

            var partitionKey = CreatePartitionKey(normalizedUsername, remoteIpAddress);
            var limiter = GetOrCreateLimiter(partitionKey);

            using RateLimitLease lease = await limiter
                .AcquireAsync(1, cancellationToken)
                .ConfigureAwait(false);

            if (lease.IsAcquired)
            {
                return new AdminLoginRateLimitResult
                {
                    IsAllowed = true,
                    RetryAfter = TimeSpan.Zero
                };
            }

            var retryAfter = TimeSpan.FromSeconds(_options.IdentityWindowSeconds);
            if (lease.TryGetMetadata(MetadataName.RetryAfter, out var metadataRetryAfter))
            {
                retryAfter = metadataRetryAfter;
            }

            var retrySeconds = Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));

            return new AdminLoginRateLimitResult
            {
                IsAllowed = false,
                RetryAfter = TimeSpan.FromSeconds(retrySeconds)
            };
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _cache.Dispose();
        }

        private FixedWindowRateLimiter GetOrCreateLimiter(string partitionKey)
        {
            if (_cache.TryGetValue(partitionKey, out FixedWindowRateLimiter? existing) &&
                existing is not null)
            {
                return existing;
            }

            lock (_createLock)
            {
                if (_cache.TryGetValue(partitionKey, out existing) &&
                    existing is not null)
                {
                    return existing;
                }

                var limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
                {
                    PermitLimit = _options.IdentityPermitLimit,
                    Window = TimeSpan.FromSeconds(_options.IdentityWindowSeconds),
                    QueueLimit = 0,
                    AutoReplenishment = true
                });

                var entryOptions = new MemoryCacheEntryOptions
                {
                    Size = 1,
                    SlidingExpiration = TimeSpan.FromMinutes(_options.PartitionIdleMinutes)
                };

                entryOptions.RegisterPostEvictionCallback(
                    static (_, value, _, _) =>
                    {
                        if (value is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    });

                _cache.Set(partitionKey, limiter, entryOptions);
                return limiter;
            }
        }

        private static string CreatePartitionKey(
            string normalizedUsername,
            IPAddress? remoteIpAddress)
        {
            var ipPart = remoteIpAddress?.ToString() ?? "unknown";
            var composite = string.Concat(ipPart, PartitionSeparator, normalizedUsername);
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(composite));
            return Convert.ToHexString(hashBytes);
        }
    }
}
