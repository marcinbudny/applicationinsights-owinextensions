using System;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Owin;
using Owin;

namespace ApplicationInsights.OwinExtensions
{
    public static class AppBuilderExtension
    {
        public static IAppBuilder UseApplicationInsights(this IAppBuilder builder,
            OperationIdContextMiddlewareConfiguration middlewareConfiguration = null,
            TelemetryConfiguration telemetryConfiguration = null,
            Func<IOwinRequest, IOwinResponse, bool> shouldTraceRequest = null)
        {
            builder.Use<OperationIdContextMiddleware>(middlewareConfiguration);
            builder.Use<HttpRequestTrackingMiddleware>(telemetryConfiguration, shouldTraceRequest);

            return builder;
        }

        public static IAppBuilder RestoreOperationIdContext(this IAppBuilder builder)
        {
            builder.Use<RestoreOperationIdContextMiddleware>();
            return builder;
        }
    }
}
