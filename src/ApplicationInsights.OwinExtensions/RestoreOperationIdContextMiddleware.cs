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
            var operationId = context.Get<string>(Consts.OperationIdContextKey);
            var parentOperationId = context.Get<string>(Consts.ParentOperationIdContextKey);

            if(operationId != null)
                using(new OperationContextScope(operationId, parentOperationId))
                    await Next.Invoke(context);
            else
                await Next.Invoke(context);
        }
    }
}
