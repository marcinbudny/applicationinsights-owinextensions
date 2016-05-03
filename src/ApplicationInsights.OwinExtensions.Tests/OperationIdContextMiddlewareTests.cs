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
            var actual = new OperationIdCollectingMiddleware();

            var sut = new OperationIdContextMiddleware(
                actual,
                new OperationIdContextMiddlewareConfiguration());

            await sut.Invoke(new MockOwinContext());

            actual.OperationIdFromAmbientContext.Should().NotBeNullOrEmpty();
            actual.OperationIdFromAmbientContext.Should()
                .Be(actual.OperationIdFromEnvironment);
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
            OperationIdContext.Get().Should().BeNull();
        }
    }
}
