using System.Threading.Tasks;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions.Tests.Utils
{
    public class NoopMiddleware : OwinMiddleware
    {
        public NoopMiddleware(OwinMiddleware next = null) : base(next) { }

        public override Task Invoke(IOwinContext context)
        {
            return Task.FromResult<object>(null);
        }
    }
}
