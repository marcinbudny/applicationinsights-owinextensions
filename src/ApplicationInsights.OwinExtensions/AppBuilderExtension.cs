using Microsoft.ApplicationInsights.Extensibility;
using Owin;

namespace ApplicationInsights.OwinExtensions
{
    public static class AppBuilderExtension
    {
        public static IAppBuilder UseApplicationInsights(this IAppBuilder builder,
            OperationIdContextMiddlewareConfiguration middlewareConfiguration = null,
            TelemetryConfiguration telemetryConfiguration = null)
        {
            builder.Use<OperationIdContextMiddleware>(middlewareConfiguration);
            builder.Use<HttpRequestTrackingMiddleware>(telemetryConfiguration);

            return builder;
        }

        public static IAppBuilder RestoreOperationIdContext(this IAppBuilder builder)
        {
            builder.Use<RestoreOperationIdContextMiddleware>();
            return builder;
        }
    }
}
