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

        private static void InitializeOperationIdContext(IOwinContext context)
        {
            string idContextKey;

            if (TryGetOperationIdContextKey(context, out idContextKey))
            {
                OperationIdContext.Set(idContextKey);
            }
            else
            {
                OperationIdContext.Create();
            }

            context.Set(Consts.OperationIdContextKey, OperationIdContext.Get());
        }

        private static bool TryGetOperationIdContextKey(IOwinContext context, out string idContextKey)
        {
            idContextKey = context.Request?.Headers?.Get(Consts.OperationIdContextKey);
            return idContextKey != null;
        }
    }
}
