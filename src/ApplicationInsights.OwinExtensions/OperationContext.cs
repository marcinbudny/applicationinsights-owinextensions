using System;
using System.Threading;

namespace ApplicationInsights.OwinExtensions
{
    public static class OperationContext
    {
        private static readonly AsyncLocal<Item> CurrentContext = new AsyncLocal<Item>();

        internal static void Set(Item context) => CurrentContext.Value = context;

        public static Item Get() => CurrentContext.Value;

        public class Item
        {
            public string OperationId { get; }

            public string ParentOperationId { get; }

            public Item(string operationId)
            {
                OperationId = operationId;
            }

            public Item(string operationId, string parentOperationId)
            {
                OperationId = operationId;
                ParentOperationId = parentOperationId;
            }
        }
    }

    [Obsolete("Use OperationContext to get and OperationContextScope to set current operation context")]
    public static class OperationIdContext
    {
        private static IIdGenerationStrategy _strategy = new GuidIdGenerationStrategy();

        public static void UseIdGenerationStrategy(IIdGenerationStrategy strategy)
        {
            _strategy = strategy;
        }

        public static void Create()
        {
            OperationContext.Set(new OperationContext.Item(_strategy.GenerateId()));
        }

        public static void Set(string value)
        {
            OperationContext.Set(new OperationContext.Item(value));
        }

        public static string Get()
        {
            return OperationContext.Get()?.OperationId;
        }

        public static void Clear()
        {
            OperationContext.Set(null);
        }
    }

}
