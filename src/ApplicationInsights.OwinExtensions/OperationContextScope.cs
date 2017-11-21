using System;

namespace ApplicationInsights.OwinExtensions
{
    public class OperationContextScope : IDisposable
    {
        private readonly OperationContext.Item _previous;

        public OperationContextScope(string operationId, string parentOperationId = null)
        {
            _previous = OperationContext.Get();
            OperationContext.Set(new OperationContext.Item(operationId, parentOperationId ?? operationId));
        }

        public void Dispose()
        {
            OperationContext.Set(_previous);
        }
    }
}
