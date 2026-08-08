using System.Diagnostics;

namespace DatabaseMastery.TransportMongoDb.Infrastructure.Operations
{
    public sealed class RequestTelemetryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTelemetryMiddleware> _logger;

        public RequestTelemetryMiddleware(
            RequestDelegate next,
            ILogger<RequestTelemetryMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (ShouldSkipTelemetry(context.Request.Path))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();

            var correlationId = context.Items[CorrelationIdMiddleware.ItemKey] as string ?? string.Empty;
            var method = context.Request.Method;
            var path = context.Request.Path.Value ?? "/";
            var statusCode = context.Response.StatusCode;
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (statusCode >= 500)
            {
                _logger.LogError(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms (CorrelationId={CorrelationId})",
                    method,
                    path,
                    statusCode,
                    elapsedMs,
                    correlationId);
            }
            else if (statusCode >= 400)
            {
                _logger.LogWarning(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms (CorrelationId={CorrelationId})",
                    method,
                    path,
                    statusCode,
                    elapsedMs,
                    correlationId);
            }
            else
            {
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms (CorrelationId={CorrelationId})",
                    method,
                    path,
                    statusCode,
                    elapsedMs,
                    correlationId);
            }
        }

        private static bool ShouldSkipTelemetry(PathString path)
        {
            var value = path.Value ?? string.Empty;

            return value.StartsWith("/templates", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("/Silva-Admin", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("/lib", StringComparison.OrdinalIgnoreCase);
        }
    }
}
