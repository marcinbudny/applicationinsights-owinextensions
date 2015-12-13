using System;
using System.Threading.Tasks;
using ApplicationInsights.OwinExtensions.Tests.Utils;
using FluentAssertions;
using Xunit;

namespace ApplicationInsights.OwinExtensions.Tests
{
    public class RestoreOperationIdContextMiddlewareTests : IDisposable
    {
        public RestoreOperationIdContextMiddlewareTests()
        {
            OperationIdContext.Clear();
        }

        [Fact]
        public async Task Can_Restore_Operation_Context_Id_From_Owin_Context()
        {
            var context = new MockOwinContext();
            context.Set(Consts.OperationIdContextKey, "test");

            var collector = new OperationIdCollectingMiddleware();
            var sut = new RestoreOperationIdContextMiddleware(collector);

            await sut.Invoke(context);

            collector.OperationIdFromAmbientContext.Should().Be("test");
        }

        [Fact]
        public async Task Should_Not_Substitue_Existing_Operation_Id_If_Context_Contains_Null()
        {
            OperationIdContext.Set("test");
            var context = new MockOwinContext();

            var collector = new OperationIdCollectingMiddleware();
            var sut = new RestoreOperationIdContextMiddleware(collector);

            await sut.Invoke(context);

            collector.OperationIdFromAmbientContext.Should().Be("test");
        }

        public void Dispose()
        {
            OperationIdContext.Clear();
        }
    }
}
