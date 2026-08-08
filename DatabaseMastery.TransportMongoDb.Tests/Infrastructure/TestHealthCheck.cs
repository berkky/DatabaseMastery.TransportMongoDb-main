using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

public sealed class TestHealthCheck : IHealthCheck
{
    private readonly HealthStatus _status;

    public TestHealthCheck(HealthStatus status)
    {
        _status = status;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return _status == HealthStatus.Healthy
            ? Task.FromResult(HealthCheckResult.Healthy())
            : Task.FromResult(HealthCheckResult.Unhealthy());
    }
}
