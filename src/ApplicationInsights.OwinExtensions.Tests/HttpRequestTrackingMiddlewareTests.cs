using System;
using System.Linq;
using System.Threading.Tasks;
using ApplicationInsights.OwinExtensions.Tests.Utils;
using FluentAssertions;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Owin;
using Moq;
using Xunit;

namespace ApplicationInsights.OwinExtensions.Tests
{
    public class HttpRequestTrackingMiddlewareTests
    {
        [Fact]
        public async Task Can_Send_Request_Telemetry()
        {
            // given
            var channel = new MockTelemetryChannel();

            var request = Mock.Of<IOwinRequest>(r =>
                r.Method == "GET" &&
                r.Path == new PathString("/path") &&
                r.Uri == new Uri("http://google.com/path")
                );

            var response = Mock.Of<IOwinResponse>(r => r.StatusCode == 200);

            var context = new MockOwinContextBuilder()
                .WithRequest(request)
                .WithResponse(response)
                .Build();

            var configuration = new TelemetryConfigurationBuilder()
                .WithChannel(channel)
                .Build();


            var sut = new OperationIdContextMiddleware(
                new HttpRequestTrackingMiddleware(
                    new NoopMiddleware(), configuration));

            // when
            await sut.Invoke(context);

            // then
            channel.SentTelemetries.Count.Should().Be(1);

            var telemetry = channel.SentTelemetries.First() as RequestTelemetry;
            telemetry.Should().NotBeNull();

            telemetry.HttpMethod.Should().Be("GET");
            telemetry.Name.Should().Be("GET /path");
            telemetry.Context.Operation.Name.Should().Be("GET /path");
            telemetry.Id.Should().NotBeNullOrEmpty();
            telemetry.Success.Should().BeTrue();
            telemetry.Url.Should().Be(new Uri("http://google.com/path"));
            telemetry.StartTime.Date.Should().Be(DateTimeOffset.Now.Date);
        }

        [Theory]
        [InlineData(200, true )]
        [InlineData(201, true )]
        [InlineData(204, true )]
        [InlineData(299, true )]
        [InlineData(300, true )]
        [InlineData(301, true )]
        [InlineData(302, true )]
        [InlineData(399, true )]
        [InlineData(400, false)]
        [InlineData(401, false)]
        [InlineData(403, false)]
        [InlineData(404, false)]
        [InlineData(499, false)]
        [InlineData(500, false)]
        [InlineData(503, false)]
        [InlineData(599, false)]
        public async Task Can_Send_Request_Telemetry(int statusCode, bool expectedSuccess)
        {
            // given
            var channel = new MockTelemetryChannel();

            var response = Mock.Of<IOwinResponse>(r => r.StatusCode == statusCode);

            var context = new MockOwinContextBuilder()
                .WithResponse(response)
                .Build();

            var configuration = new TelemetryConfigurationBuilder()
                .WithChannel(channel)
                .Build();

            var sut = new HttpRequestTrackingMiddleware(new NoopMiddleware(), configuration);

            // when
            await sut.Invoke(context);

            // then
            var telemetry = channel.SentTelemetries.First() as RequestTelemetry;
            telemetry.Success.Should().Be(expectedSuccess);
        }

    }
}
