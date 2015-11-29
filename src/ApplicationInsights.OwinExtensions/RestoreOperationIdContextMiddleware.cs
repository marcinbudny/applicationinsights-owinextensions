using System.Threading.Tasks;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions
{
    public class RestoreOperationIdContextMiddleware : OwinMiddleware
    {
        public RestoreOperationIdContextMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            var operationId = context.Get<string>("OperationIdContext");
            if(operationId != null)
                OperationIdContext.Set(operationId);

            await Next.Invoke(context);
        }
    }
}
