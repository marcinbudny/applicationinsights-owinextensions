using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions
{
    public class OperationIdContextMiddlewareConfiguration
    {
        [Obsolete("Use OperationIdFactory property")]
        public bool ShouldTryGetIdFromHeader { get; set; } = false;
        [Obsolete("Use OperationIdFactory property")]
        public string OperationIdHeaderName { get; set; } = Consts.OperationIdHeaderName;

        public Func<IOwinContext, string> OperationIdFactory = IdFactory.NewGuid;
    }

    public class OperationIdContextMiddleware : OwinMiddleware
    {
        private readonly OperationIdContextMiddlewareConfiguration _configuration;

        public OperationIdContextMiddleware(
            OwinMiddleware next, 
            OperationIdContextMiddlewareConfiguration configuration) : base(next)
        {
            _configuration = configuration ?? new OperationIdContextMiddlewareConfiguration();

            // TODO: remove once obsolete configuration of opid header is removed
            if (_configuration.ShouldTryGetIdFromHeader)
                _configuration.OperationIdFactory = IdFactory.FromHeader(_configuration.OperationIdHeaderName);
        }

        public override async Task Invoke(IOwinContext context)
        {
            var operationId = _configuration.OperationIdFactory(context);

            using (new OperationContextScope(operationId))
            using (new OperationContextStoredInOwinContextScope(context))
            {
                await Next.Invoke(context);
            }
        }
    }
}
