using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions
{
    public class HttpRequestTrackingMiddleware : OwinMiddleware
    {
        private readonly Func<IOwinRequest, IOwinResponse, bool> _shouldTraceRequest;
        private readonly Func<IOwinRequest, IOwinResponse, KeyValuePair<string, string>[]> _getContextProperties;
        private readonly TelemetryClient _client;

        public HttpRequestTrackingMiddleware(
            OwinMiddleware next, 
            TelemetryConfiguration configuration = null, 
            Func<IOwinRequest, IOwinResponse, bool> shouldTraceRequest = null, 
            Func<IOwinRequest, IOwinResponse, KeyValuePair<string,string>[]> getContextProperties = null) : base(next)
        {
            _shouldTraceRequest = shouldTraceRequest;
            _getContextProperties = getContextProperties;
            _client = configuration != null ? new TelemetryClient(configuration) : new TelemetryClient();
        }

        public override async Task Invoke(IOwinContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();
            var uri = context.Request.Uri;

            var requestStartDate = DateTimeOffset.Now;
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                await Next.Invoke(context);
            }
            finally
            {
                stopWatch.Stop();
                if (ShouldTraceRequest(context))
                {
                    var contextProperties = GetContextProperties(context);
                    TraceRequest(method, path, uri, context.Response.StatusCode, requestStartDate, stopWatch.Elapsed, contextProperties);
                }
                    
            }
        }

        private KeyValuePair<string,string>[] GetContextProperties(IOwinContext context)
        {
            if (_getContextProperties == null)
            {
                return new KeyValuePair<string, string>[0];
            }
            return _getContextProperties(context.Request, context.Response);
        }

        private bool ShouldTraceRequest(IOwinContext context)
        {
            if (_shouldTraceRequest == null)
                return true;
            return _shouldTraceRequest(context.Request, context.Response);
        }

        private void TraceRequest(string method, string path, Uri uri, int responseCode, DateTimeOffset requestStartDate, TimeSpan duration, KeyValuePair<string, string>[] contextProperties)
        {
            var name = $"{method} {path}";

            var telemetry = new RequestTelemetry(
                name,
                requestStartDate,
                duration,
                responseCode.ToString(),
                responseCode < 400)
            {
                Id = OperationIdContext.Get(),
                HttpMethod = method,
                Url = uri
            };
            telemetry.Context.Operation.Name = name;
            foreach (var kvp in contextProperties)
            {
                telemetry.Context.Properties.Add(kvp);
            }
            _client.TrackRequest(telemetry);
        }
    }

}
