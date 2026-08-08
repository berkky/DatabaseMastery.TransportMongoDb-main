using Microsoft.AspNetCore.Mvc;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure.ReverseProxy;

[ApiController]
[Route("[controller]")]
public sealed class ForwardedHeadersProbeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new ForwardedHeadersProbeResponse
        {
            RemoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            Scheme = HttpContext.Request.Scheme,
            IsHttps = HttpContext.Request.IsHttps,
        });
    }

    public sealed class ForwardedHeadersProbeResponse
    {
        public string? RemoteIpAddress { get; init; }

        public string Scheme { get; init; } = string.Empty;

        public bool IsHttps { get; init; }
    }
}
