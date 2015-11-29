using System;

namespace ApplicationInsights.OwinExtensions
{
    public interface IIdGenerationStrategy
    {
        string GenerateId();
    }

    public class GuidIdGenerationStrategy : IIdGenerationStrategy
    {
        public string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
