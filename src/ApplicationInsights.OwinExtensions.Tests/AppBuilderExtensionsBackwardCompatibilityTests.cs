using System.Collections.Generic;
using Microsoft.ApplicationInsights.Extensibility;
using Owin;

namespace ApplicationInsights.OwinExtensions.Tests
{
    public class AppBuilderExtensionsBackwardCompatibilityTests
    {
        public void This_method_should_compile()
        {
            IAppBuilder app = null;

#pragma warning disable CS0618 // Type or member is obsolete
// ReSharper disable ExpressionIsAlwaysNull

            app.UseApplicationInsights();

            app.UseApplicationInsights(new OperationIdContextMiddlewareConfiguration());

            app.UseApplicationInsights(middlewareConfiguration: new OperationIdContextMiddlewareConfiguration());

            app.UseApplicationInsights(
                new OperationIdContextMiddlewareConfiguration(), 
                new TelemetryConfiguration());

            app.UseApplicationInsights(
                telemetryConfiguration: new TelemetryConfiguration());

            app.UseApplicationInsights(
                new OperationIdContextMiddlewareConfiguration(),
                new TelemetryConfiguration(),
                (request, response) => true);

            app.UseApplicationInsights(
                telemetryConfiguration: new TelemetryConfiguration(),
                shouldTraceRequest: (request, response) => true);

            app.UseApplicationInsights(
                shouldTraceRequest: (request, response) => true);

            app.UseApplicationInsights(
                new OperationIdContextMiddlewareConfiguration(),
                new TelemetryConfiguration(),
                (request, response) => true,
                (request, response) => new KeyValuePair<string, string>[] {});

            app.UseApplicationInsights(
                getContextProperties: (request, response) => new KeyValuePair<string, string>[] { });

            app.UseApplicationInsights(
                telemetryConfiguration: new TelemetryConfiguration(),
                getContextProperties: (request, response) => new KeyValuePair<string, string>[] { });

            app.UseApplicationInsights(
                new OperationIdContextMiddlewareConfiguration(),
                telemetryConfiguration: new TelemetryConfiguration(),
                shouldTraceRequest: (request, response) => true,
                getContextProperties: (request, response) => new KeyValuePair<string, string>[] { });

            app.UseApplicationInsights(
                middlewareConfiguration: new OperationIdContextMiddlewareConfiguration(),
                telemetryConfiguration: new TelemetryConfiguration(),
                shouldTraceRequest: (request, response) => true,
                getContextProperties: (request, response) => new KeyValuePair<string, string>[] { });

            // ReSharper restore ExpressionIsAlwaysNull
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
