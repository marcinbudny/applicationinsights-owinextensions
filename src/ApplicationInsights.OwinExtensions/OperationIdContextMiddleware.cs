using System.Threading.Tasks;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions
{
    public class OperationIdContextMiddleware : OwinMiddleware
    {
        public OperationIdContextMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            OperationIdContext.Create();
            context.Set(Consts.OperationIdContextKey, OperationIdContext.Get());

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
    }
}
