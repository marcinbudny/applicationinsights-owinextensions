using Microsoft.ApplicationInsights.Extensibility;
using Owin;

namespace ApplicationInsights.OwinExtensions
{
    public static class AppBuilderExtension
    {
        public static IAppBuilder UseApplicationInsights(this IAppBuilder builder,
            string componentName,
            TelemetryConfiguration configuration = null)
        {
            builder.UseApplicationInsights(configuration);

            UseComponentNameTelemetryInitializer(componentName);

            return builder;
        }

        public static IAppBuilder UseApplicationInsights(this IAppBuilder builder,
            TelemetryConfiguration configuration = null)
        {
            builder.Use<OperationIdContextMiddleware>();
            builder.Use<HttpRequestTrackingMiddleware>(configuration);

            UseOperationIdTelemetryInitializer();

            return builder;
        }

        public static IAppBuilder RestoreOperationIdContext(this IAppBuilder builder)
        {
            builder.Use<RestoreOperationIdContextMiddleware>();
            return builder;
        }

        private static void UseOperationIdTelemetryInitializer()
        {
            TelemetryConfiguration.Active
                .TelemetryInitializers.Add(new OperationIdTelemetryInitializer());
        }

        private static void UseComponentNameTelemetryInitializer(string componentName)
        {
            TelemetryConfiguration.Active
               .TelemetryInitializers.Add(new ComponentNameTelemetryInitializer(componentName));
        }

    }
}
