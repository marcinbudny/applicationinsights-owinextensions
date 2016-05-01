using System.Net.Http;

namespace ApplicationInsights.OwinExtensions
{
    public static class HttpRequestMessageExtension
    {
        public static void AppendOperationId(this HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Add(Consts.OperationIdContextKey, OperationIdContext.Get());
        }
    }
}