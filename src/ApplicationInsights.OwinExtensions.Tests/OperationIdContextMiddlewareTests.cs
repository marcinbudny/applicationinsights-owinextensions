using System.Threading.Tasks;
using ApplicationInsights.OwinExtensions.Tests.Utils;
using FluentAssertions;
using Xunit;

namespace ApplicationInsights.OwinExtensions.Tests
{
    public class OperationIdContextMiddlewareTests
    {
        [Fact]
        public async Task Can_Establish_Id_Context()
        {
            var actual = new OperationContextCollectingMiddleware();

            var sut = new OperationIdContextMiddleware(
                actual,
                new OperationIdContextMiddlewareConfiguration
                {
                    OperationIdFactory = _ => "operationid"
                });

            await sut.Invoke(new MockOwinContext());

            actual.OperationIdFromAmbientContext.Should().Be("operationid");
            actual.OperationIdFromEnvironment.Should().Be("operationid");

            actual.ParentOperationIdFromAmbientContext.Should().Be("operationid");
            actual.ParentOperationIdFromEnvironment.Should().Be("operationid");
        }

        [Fact]
        public async Task Can_Clean_Context()
        {
            var context = new MockOwinContext();

            var sut = new OperationIdContextMiddleware(
                new NoopMiddleware(),
                new OperationIdContextMiddlewareConfiguration());

            await sut.Invoke(context);

            context.Get<string>(Consts.OperationIdContextKey).Should().BeNull();
            context.Get<string>(Consts.ParentOperationIdContextKey).Should().BeNull();
            OperationContext.Get().Should().BeNull();
        }
    }
}
