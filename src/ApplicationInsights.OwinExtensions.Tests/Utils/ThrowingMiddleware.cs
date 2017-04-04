using System.Threading.Tasks;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions.Tests.Utils
{
    public class ThrowingMiddleware : OwinMiddleware
    {
        public ThrowingMiddleware(OwinMiddleware next = null) : base(next) { }

        public override Task Invoke(IOwinContext context) => throw new OlabogaException();
    }
}
