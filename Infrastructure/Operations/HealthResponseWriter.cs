using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DatabaseMastery.TransportMongoDb.Infrastructure.Operations
{
    public static class HealthResponseWriter
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static Task WriteResponseAsync(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.Headers.CacheControl = "no-store, no-cache";
            context.Response.Headers.Pragma = "no-cache";
            context.Response.Headers.Expires = "0";

            context.Response.StatusCode = report.Status switch
            {
                HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
                _ => StatusCodes.Status200OK
            };

            var status = report.Status switch
            {
                HealthStatus.Healthy => "Healthy",
                HealthStatus.Degraded => "Degraded",
                _ => "Unhealthy"
            };

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(new { status }, SerializerOptions));
        }
    }
}
