using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace DatabaseMastery.TransportMongoDb.Tests.Infrastructure;

internal sealed class TestRemoteIpStartupFilter : IStartupFilter
{
    public const string HeaderName = "X-Test-Remote-Ip";

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.Use(async (context, nextMiddleware) =>
            {
                if (context.Request.Headers.TryGetValue(HeaderName, out var values))
                {
                    var headerValue = values.ToString();
                    if (IPAddress.TryParse(headerValue, out var ipAddress))
                    {
                        context.Connection.RemoteIpAddress = ipAddress;
                    }
                }

                await nextMiddleware(context);
            });

            next(app);
        };
    }
}
