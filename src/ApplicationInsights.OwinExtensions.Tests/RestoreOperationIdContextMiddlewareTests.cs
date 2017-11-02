using System.Threading.Tasks;
using ApplicationInsights.OwinExtensions.Tests.Utils;
using FluentAssertions;
using Xunit;

namespace ApplicationInsights.OwinExtensions.Tests
{
    public class RestoreOperationIdContextMiddlewareTests
    {
        [Fact]
        public async Task Can_Restore_Operation_Context_Id_From_Owin_Context()
        {
            var context = new MockOwinContext();
            context.Set(Consts.OperationIdContextKey, "opid");
            context.Set(Consts.ParentOperationIdContextKey, "parentid");

            var collector = new OperationContextCollectingMiddleware();
            var sut = new RestoreOperationIdContextMiddleware(collector);

            await sut.Invoke(context);

            collector.OperationIdFromAmbientContext.Should().Be("opid");
            collector.ParentOperationIdFromAmbientContext.Should().Be("parentid");
        }

        [Fact]
        public async Task Should_Not_Substitue_Existing_Operation_Id_If_Context_Contains_Null()
        {
            using (new OperationContextScope("opid", "parentid"))
            {
                var context = new MockOwinContext();

                var collector = new OperationContextCollectingMiddleware();
                var sut = new RestoreOperationIdContextMiddleware(collector);

                await sut.Invoke(context);

                collector.OperationIdFromAmbientContext.Should().Be("opid");
                collector.ParentOperationIdFromAmbientContext.Should().Be("parentid");
            }
        }
    }
}
