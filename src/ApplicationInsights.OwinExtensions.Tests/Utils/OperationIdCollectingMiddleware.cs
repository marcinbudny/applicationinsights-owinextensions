using System.Threading.Tasks;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions.Tests.Utils
{
    public class OperationIdCollectingMiddleware : OwinMiddleware
    {
        public string OperationIdFromEnvironment { get; set; }
        public string OperationIdFromAmbientContext { get; set; }

        public OperationIdCollectingMiddleware(OwinMiddleware next = null) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            OperationIdFromEnvironment = context.Get<string>(Consts.OperationIdContextKey);
            OperationIdFromAmbientContext = OperationIdContext.Get();

            if(Next == null)
                return Task.FromResult<object>(null);

            return Next.Invoke(context);
        }
    }
}
