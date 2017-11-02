using System;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions
{
    public static class IdFactory
    {
        public static Func<IOwinContext, string> NewGuid => ctx => Guid.NewGuid().ToString();

        public static Func<IOwinContext, string> FromHeader(
            string header = Consts.OperationIdHeaderName,
            Func<IOwinContext, string> factoryIfHeaderNotAvailable = null)
        {
            return ctx => ctx?.Request?.Headers?.Get(header) ?? (factoryIfHeaderNotAvailable ?? NewGuid)(ctx);
        }
    }
}
