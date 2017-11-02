using System;

namespace ApplicationInsights.OwinExtensions
{
    [Obsolete("Use id generation strategy when configuring operation id middleware")]
    public interface IIdGenerationStrategy
    {
        string GenerateId();
    }

    [Obsolete("Use id generation strategy when configuring operation id middleware")]
    public class GuidIdGenerationStrategy : IIdGenerationStrategy
    {
        public string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
