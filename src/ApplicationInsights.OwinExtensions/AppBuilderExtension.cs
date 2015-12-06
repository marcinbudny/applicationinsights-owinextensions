using Microsoft.ApplicationInsights.Extensibility;
using Owin;

namespace ApplicationInsights.OwinExtensions
{
    public static class AppBuilderExtension
    {
        public static IAppBuilder UseApplicationInsights(this IAppBuilder builder, TelemetryConfiguration configuration = null)
        {
            builder.Use<OperationIdContextMiddleware>();
            builder.Use<HttpRequestTrackingMiddleware>(configuration);
            return builder;
        }

        public static IAppBuilder RestoreOperationIdContext(this IAppBuilder builder)
        {
            builder.Use<RestoreOperationIdContextMiddleware>();
            return builder;
        }
    }
}
