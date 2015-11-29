using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace ApplicationInsights.OwinExtensions.Tests
{
    public class OperationIdContextTests : IDisposable
    {
        public OperationIdContextTests()
        {
            OperationIdContext.Clear();
        }

        [Fact]
        public void Can_Create_Context()
        {
            OperationIdContext.Create();
            OperationIdContext.Get().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Can_Clear_Context()
        {
            OperationIdContext.Create();
            OperationIdContext.Clear();
            OperationIdContext.Get().Should().BeNull();
        }

        [Fact]
        public void Can_Set_Context()
        {
            OperationIdContext.Set("test");
            OperationIdContext.Get().Should().Be("test");
        }

        [Fact]
        public async Task Operation_Context_Flows_With_Async()
        {
            OperationIdContext.Create();
            var expected = OperationIdContext.Get();

            using (var client = new HttpClient())
                await client.GetAsync("http://google.com");

            var actual = OperationIdContext.Get();

            actual.Should().NotBeNullOrEmpty();
            actual.Should().Be(expected);
        }

        public void Dispose()
        {
            OperationIdContext.Clear();
        }
    }
}
