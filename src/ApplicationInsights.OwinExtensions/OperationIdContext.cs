using System.Runtime.Remoting.Messaging;

namespace ApplicationInsights.OwinExtensions
{
    public static class OperationIdContext
    {
        private const string OperationIdContextKey = "APPLICATION_INSIGHTS_OPERATION_ID_CONTEXT";

        private static IIdGenerationStrategy _strategy = new GuidIdGenerationStrategy();
        //  TODO: remove comments
        //private static readonly AsyncLocal<string> _operationContext = new AsyncLocal<string>();

        public static void UseIdGenerationStrategy(IIdGenerationStrategy strategy)
        {
            _strategy = strategy;
        }

        public static void Create()
        {
            CallContext.LogicalSetData(OperationIdContextKey, _strategy.GenerateId());
            //_operationContext.Value = _strategy.GenerateId();
        }

        public static void Set(string value)
        {
            CallContext.LogicalSetData(OperationIdContextKey, value);
            //_operationContext.Value = value;
        }

        public static string Get()
        {
            return CallContext.LogicalGetData(OperationIdContextKey) as string;
            //return _operationContext.Value;
        }

        public static void Clear()
        {
            CallContext.LogicalSetData(OperationIdContextKey, null);
            //_operationContext.Value = null;
        }
    }
}
