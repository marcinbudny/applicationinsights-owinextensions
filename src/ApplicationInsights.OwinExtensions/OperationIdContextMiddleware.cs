using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions
{
    public class OperationIdContextMiddlewareConfiguration
    {
        public bool ShouldTryGetIdFromHeader { get; set; } = false;
        public string OperationIdHeaderName { get; set; } = Consts.OperationIdHeaderName;
    }

    public class OperationIdContextMiddleware : OwinMiddleware
    {
        private readonly OperationIdContextMiddlewareConfiguration _configuration;

        public OperationIdContextMiddleware(
            OwinMiddleware next, 
            OperationIdContextMiddlewareConfiguration configuration) : base(next)
        {
            _configuration = configuration ?? new OperationIdContextMiddlewareConfiguration();
        }

        public override async Task Invoke(IOwinContext context)
        {
            InitializeOperationIdContext(context);

            try
            {
                await Next.Invoke(context);
            }
            finally
            {
                context.Set<string>(Consts.OperationIdContextKey, null);
                OperationIdContext.Clear();
            }
        }

        private void InitializeOperationIdContext(IOwinContext context)
        {
            string idContextKey;

            if (_configuration.ShouldTryGetIdFromHeader && 
                TryGetIdFromHeader(context, out idContextKey))
            {
                OperationIdContext.Set(idContextKey);
            }
            else
            {
                OperationIdContext.Create();
            }

            context.Set(Consts.OperationIdContextKey, OperationIdContext.Get());
        }

        private bool TryGetIdFromHeader(IOwinContext context, out string idContextKey)
        {
            idContextKey = context.Request?.Headers?.Get(_configuration.OperationIdHeaderName);
            return idContextKey != null;
        }
    }
}
