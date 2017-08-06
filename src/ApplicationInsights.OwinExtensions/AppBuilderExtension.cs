using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Owin;
using Owin;

namespace ApplicationInsights.OwinExtensions
{
    public static class AppBuilderExtension
    {
        public static IAppBuilder UseApplicationInsights(
            this IAppBuilder builder,
            RequestTrackingConfiguration requestTrackingConfiguration = null,
            OperationIdContextMiddlewareConfiguration operationIdConfiguration = null)
        {
            builder.Use<OperationIdContextMiddleware>(operationIdConfiguration);
            builder.Use<HttpRequestTrackingMiddleware>(requestTrackingConfiguration);

            return builder;
        }


        public static IAppBuilder RestoreOperationIdContext(this IAppBuilder builder)
        {
            builder.Use<RestoreOperationIdContextMiddleware>();
            return builder;
        }

        // method overloads to ensure non breaking api change

        [Obsolete("Use the overload accepting RequestTrackingConfiguration")]
        public static IAppBuilder UseApplicationInsights(
            this IAppBuilder builder,
            OperationIdContextMiddlewareConfiguration middlewareConfiguration,
            TelemetryConfiguration telemetryConfiguration = null,
            Func<IOwinRequest, IOwinResponse, bool> shouldTraceRequest = null,
            Func<IOwinRequest, IOwinResponse, KeyValuePair<string, string>[]> getContextProperties = null)
        {
            builder.Use<OperationIdContextMiddleware>(middlewareConfiguration);
            builder.Use<HttpRequestTrackingMiddleware>(telemetryConfiguration, shouldTraceRequest, getContextProperties);

            return builder;
        }

        [Obsolete("Use the overload accepting RequestTrackingConfiguration")]
        public static IAppBuilder UseApplicationInsights(
            this IAppBuilder builder,
            TelemetryConfiguration telemetryConfiguration,
            Func<IOwinRequest, IOwinResponse, bool> shouldTraceRequest = null,
            Func<IOwinRequest, IOwinResponse, KeyValuePair<string, string>[]> getContextProperties = null)
        {
            builder.Use<OperationIdContextMiddleware>(new object[] { null });
            builder.Use<HttpRequestTrackingMiddleware>(telemetryConfiguration, shouldTraceRequest, getContextProperties);

            return builder;
        }

        [Obsolete("Use the overload accepting RequestTrackingConfiguration")]
        public static IAppBuilder UseApplicationInsights(
            this IAppBuilder builder,
            Func<IOwinRequest, IOwinResponse, bool> shouldTraceRequest,
            Func<IOwinRequest, IOwinResponse, KeyValuePair<string, string>[]> getContextProperties = null)
        {
            builder.Use<OperationIdContextMiddleware>(new object[] { null });
            builder.Use<HttpRequestTrackingMiddleware>(null, shouldTraceRequest, getContextProperties);

            return builder;
        }

        [Obsolete("Use the overload accepting RequestTrackingConfiguration")]
        public static IAppBuilder UseApplicationInsights(
            this IAppBuilder builder,
            Func<IOwinRequest, IOwinResponse, KeyValuePair<string, string>[]> getContextProperties)
        {
            builder.Use<OperationIdContextMiddleware>(new object[] { null });
            builder.Use<HttpRequestTrackingMiddleware>(null, null, getContextProperties);

            return builder;
        }
    }
}
