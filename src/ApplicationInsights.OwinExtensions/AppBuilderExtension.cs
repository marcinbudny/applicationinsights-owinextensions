using Microsoft.ApplicationInsights.Extensibility;
using Owin;

namespace ApplicationInsights.OwinExtensions
{
    public static class AppBuilderExtension
    {
        public static IAppBuilder UseApplicationInsights(this IAppBuilder builer, TelemetryConfiguration configuration = null)
        {
            builer.Use<OperationIdContextMiddleware>();
            builer.Use<HttpRequestTrackingMiddleware>(configuration);
            return builer;
        }
    }
}
