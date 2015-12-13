using System.Runtime.Remoting.Messaging;

namespace ApplicationInsights.OwinExtensions
{
    public static class OperationIdContext
    {
        private static IIdGenerationStrategy _strategy = new GuidIdGenerationStrategy();

        public static void UseIdGenerationStrategy(IIdGenerationStrategy strategy)
        {
            _strategy = strategy;
        }

        public static void Create()
        {
            CallContext.LogicalSetData(Consts.OperationIdContextKey, _strategy.GenerateId());
        }

        public static void Set(string value)
        {
            CallContext.LogicalSetData(Consts.OperationIdContextKey, value);
        }

        public static string Get()
        {
            return CallContext.LogicalGetData(Consts.OperationIdContextKey) as string;
        }

        public static void Clear()
        {
            CallContext.LogicalSetData(Consts.OperationIdContextKey, null);
        }
    }
}
