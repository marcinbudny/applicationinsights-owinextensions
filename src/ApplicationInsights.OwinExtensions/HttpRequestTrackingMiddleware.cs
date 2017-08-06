using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions
{
    public class HttpRequestTrackingMiddleware : OwinMiddleware
    {
        private readonly TelemetryClient _client;
        private readonly RequestTrackingConfiguration _configuration;

        [Obsolete("Use the overload accepting RequestTrackingConfiguration")]
        public HttpRequestTrackingMiddleware(
            OwinMiddleware next, 
            TelemetryConfiguration configuration = null, 
            Func<IOwinRequest, IOwinResponse, bool> shouldTraceRequest = null, 
            Func<IOwinRequest, IOwinResponse, KeyValuePair<string,string>[]> getContextProperties = null) : 
            this(next, new RequestTrackingConfiguration
            {
                TelemetryConfiguration = configuration,
                ShouldTrackRequest = shouldTraceRequest != null 
                    ? (IOwinContext cts) => Task.FromResult(shouldTraceRequest(cts.Request, cts.Response))
                    : (Func<IOwinContext, Task<bool>>) null,
                GetAdditionalContextProperties = getContextProperties != null 
                    ? (IOwinContext ctx) => Task.FromResult(getContextProperties(ctx.Request, ctx.Response).AsEnumerable())
                    : (Func<IOwinContext, Task<IEnumerable<KeyValuePair<string, string>>>>) null,
            })
        {
        }

        public HttpRequestTrackingMiddleware(
            OwinMiddleware next,
            RequestTrackingConfiguration configuration = null) : base(next)
        {
            _configuration = configuration ?? new RequestTrackingConfiguration();

            _configuration.ShouldTrackRequest = _configuration.ShouldTrackRequest ?? (ctx => Task.FromResult(true));

            _configuration.GetAdditionalContextProperties = _configuration.GetAdditionalContextProperties ?? 
                (ctx => Task.FromResult(Enumerable.Empty<KeyValuePair<string, string>>()));

            _client = _configuration.TelemetryConfiguration != null 
                ? new TelemetryClient(_configuration.TelemetryConfiguration) 
                : new TelemetryClient();
        }

        public override async Task Invoke(IOwinContext context)
        {
            // following request properties have to be accessed before other middlewares run
            // otherwise access could result in ObjectDisposedException
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();
            var uri = context.Request.Uri;

            var requestStartDate = DateTimeOffset.Now;
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                await Next.Invoke(context);

                stopWatch.Stop();

                if (await _configuration.ShouldTrackRequest(context))
                    await TrackRequest(method, path, uri, context, context.Response.StatusCode, requestStartDate, stopWatch.Elapsed);

            }
            catch (Exception e)
            {
                stopWatch.Stop();

                TraceException(e);

                if (await _configuration.ShouldTrackRequest(context))
                    await TrackRequest(method, path, uri, context, (int)HttpStatusCode.InternalServerError, requestStartDate, stopWatch.Elapsed);

                throw;
            }
        }

        private async Task TrackRequest(
            string method,
            string path,
            Uri uri,
            IOwinContext context, 
            int responseCode, 
            DateTimeOffset requestStartDate, 
            TimeSpan duration)
        {
            var name = $"{method} {path}";

            var telemetry = new RequestTelemetry(
                name,
                requestStartDate,
                duration,
                responseCode.ToString(),
                success: responseCode < 400)
            {
                Id = OperationIdContext.Get(),
                HttpMethod = method,
                Url = uri
            };

            telemetry.Context.Operation.Name = name;

            foreach (var kvp in await _configuration.GetAdditionalContextProperties(context))
                telemetry.Context.Properties.Add(kvp);

            _client.TrackRequest(telemetry);
        }

        private void TraceException(Exception e)
        {
            var telemetry = new ExceptionTelemetry(e);
            telemetry.Context.Operation.Id = OperationIdContext.Get();

            _client.TrackException(telemetry);
        }
    }

}
