using System.Threading.Tasks;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions.Tests.Utils
{
    public class OperationContextCollectingMiddleware : OwinMiddleware
    {
        public string OperationIdFromEnvironment { get; set; }
        public string OperationIdFromAmbientContext { get; set; }

        public string ParentOperationIdFromEnvironment { get; set; }
        public string ParentOperationIdFromAmbientContext { get; set; }

        public OperationContextCollectingMiddleware(OwinMiddleware next = null) : base(next)
        {
        }

        public override Task Invoke(IOwinContext context)
        {
            OperationIdFromEnvironment = context.Get<string>(Consts.OperationIdContextKey);
            OperationIdFromAmbientContext = OperationContext.Get().OperationId;

            ParentOperationIdFromEnvironment = context.Get<string>(Consts.ParentOperationIdContextKey);
            ParentOperationIdFromAmbientContext = OperationContext.Get().ParentOperationId;

            if(Next == null)
                return Task.FromResult<object>(null);

            return Next.Invoke(context);
        }
    }
}
