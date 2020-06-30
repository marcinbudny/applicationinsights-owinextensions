using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions
{
    public static class DefaultFilters
    {
        public static Task<bool> ShouldTrackException(IOwinContext context, Exception e)
            => Task.FromResult(!(e is OperationCanceledException && context.Request.CallCancelled.IsCancellationRequested));
    }
}