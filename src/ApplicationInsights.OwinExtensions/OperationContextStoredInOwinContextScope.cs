using System;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions
{
    public  class OperationContextStoredInOwinContextScope : IDisposable
    {
        private readonly IOwinContext _owinContext;

        private readonly string _previousOperationId;
        private readonly string _previousParentOperationId;

        public OperationContextStoredInOwinContextScope(IOwinContext owinContext)
        {
            _owinContext = owinContext;

            _previousOperationId = _owinContext.Get<string>(Consts.OperationIdContextKey);
            _previousParentOperationId = _owinContext.Get<string>(Consts.ParentOperationIdContextKey);

            _owinContext.Set(Consts.OperationIdContextKey, OperationContext.Get().OperationId);
            _owinContext.Set(Consts.ParentOperationIdContextKey, OperationContext.Get().ParentOperationId);
        }

        public void Dispose()
        {
            _owinContext.Set(Consts.OperationIdContextKey, _previousOperationId);
            _owinContext.Set(Consts.ParentOperationIdContextKey, _previousParentOperationId);
        }
    }
}
