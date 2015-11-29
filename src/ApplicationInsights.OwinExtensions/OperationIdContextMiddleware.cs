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
            // TODO: use constant and add namespace
            context.Set("OperationIdContext", OperationIdContext.Get());

            try
            {
                await Next.Invoke(context);
            }
            finally
            {
                context.Set<string>("OperationIdContext", null);
                OperationIdContext.Clear();
            }
        }
    }
}
