using System;
using System.Collections.Generic;
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
        private static void VerifyDefaultsOnRequestTelemetry(RequestTelemetry telemetry)
        {
            telemetry.HttpMethod.Should().Be("GET");
            telemetry.Name.Should().Be("GET /path");
            telemetry.Context.Operation.Name.Should().Be("GET /path");
            telemetry.Id.Should().NotBeNullOrEmpty();
            telemetry.Url.Should().Be(new Uri("http://google.com/path"));
            telemetry.StartTime.Date.Should().Be(DateTimeOffset.Now.Date);
        }

        [Fact]
        public async Task Can_Send_Request_Telemetry()
        {
            // given
            var context = new MockOwinContextBuilder().Build();

            var configuration = new TelemetryConfigurationBuilder().Build();


            var sut = new OperationIdContextMiddleware(
                new HttpRequestTrackingMiddleware(
                    new NoopMiddleware(), configuration),
                new OperationIdContextMiddlewareConfiguration());

            // when
            await sut.Invoke(context);

            // then
            var channel = configuration.TelemetryChannel as MockTelemetryChannel;
            channel.SentTelemetries.Count.Should().Be(1);

            var telemetry = channel.SentTelemetries.First() as RequestTelemetry;
            telemetry.Should().NotBeNull();

            VerifyDefaultsOnRequestTelemetry(telemetry);
            telemetry.ResponseCode.Should().Be("200");
            telemetry.Success.Should().BeTrue();

        }

        [Fact]
        public async Task Should_Send_Request_Telemetry_When_Not_Filtered_Out()
        {
            // given
            var context = new MockOwinContextBuilder().Build();

            var configuration = new TelemetryConfigurationBuilder().Build();


            var sut = new OperationIdContextMiddleware(
                new HttpRequestTrackingMiddleware(
                    new NoopMiddleware(), configuration, (req, resp) => true),
                new OperationIdContextMiddlewareConfiguration());

            // when
            await sut.Invoke(context);

            // then
            var channel = configuration.TelemetryChannel as MockTelemetryChannel;
            channel.SentTelemetries.Count.Should().Be(1);

            var telemetry = channel.SentTelemetries.First() as RequestTelemetry;
            telemetry.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_Skip_Request_Telemetry_When_Filtered_Out()
        {
            // given
            var context = new MockOwinContextBuilder().Build();

            var configuration = new TelemetryConfigurationBuilder().Build();


            var sut = new OperationIdContextMiddleware(
                new HttpRequestTrackingMiddleware(
                    new NoopMiddleware(), configuration, (req, resp) => false),
                new OperationIdContextMiddlewareConfiguration());

            // when
            await sut.Invoke(context);

            // then
            var channel = configuration.TelemetryChannel as MockTelemetryChannel;
            channel.SentTelemetries.Count.Should().Be(0);
        }

        [Fact]
        public async Task Can_Pass_Request_Details_For_Filtering()
        {
            // given
            var context = new MockOwinContextBuilder().Build();

            var configuration = new TelemetryConfigurationBuilder().Build();
            IOwinRequest filteredRequest = null;
            IOwinResponse filteredResponse = null;

            var sut = new OperationIdContextMiddleware(
                new HttpRequestTrackingMiddleware(
                    new NoopMiddleware(), configuration, (req, resp) =>
                    {
                        filteredRequest = req;
                        filteredResponse = resp;
                        return false;
                    }),
                new OperationIdContextMiddlewareConfiguration());

            // when
            await sut.Invoke(context);

            // then
            filteredRequest.ShouldBeEquivalentTo(context.Request);
            filteredResponse.ShouldBeEquivalentTo(context.Response);

        }

        [Fact]
        public async Task Should_Add_Properties_To_Request_Telemetry_Context_When_They_Are_Provided()
        {
            // given
            var context = new MockOwinContextBuilder().Build();

            var configuration = new TelemetryConfigurationBuilder().Build();


            var sut = new OperationIdContextMiddleware(
                new HttpRequestTrackingMiddleware(
                    new NoopMiddleware(), configuration, getContextProperties: (req, res) => new[]
                    {
                        new KeyValuePair<string, string>("key1", "val1"),
                        new KeyValuePair<string, string>("key2", "val2"),
                    }),
                new OperationIdContextMiddlewareConfiguration());

            // when
            await sut.Invoke(context);

            // then
            var channel = configuration.TelemetryChannel as MockTelemetryChannel;
            channel.SentTelemetries.Count.Should().Be(1);

            var telemetry = channel.SentTelemetries.First() as RequestTelemetry;
            telemetry.Should().NotBeNull();

            telemetry.Context.Properties.Should().Contain(new[]
            {
                new KeyValuePair<string, string>("key1", "val1"),
                new KeyValuePair<string, string>("key2", "val2"),
            });

            VerifyDefaultsOnRequestTelemetry(telemetry);
            telemetry.ResponseCode.Should().Be("200");
            telemetry.Success.Should().BeTrue();

        }

        [Fact]
        public async Task On_Exception_Should_Pass_It_Further()
        {
            // given
            var configuration = new TelemetryConfigurationBuilder().Build();

            var context = new MockOwinContextBuilder().Build();

            var sut = new HttpRequestTrackingMiddleware(new ThrowingMiddleware(), configuration);

            // when / then
            Func<Task> sutAction = async () => await sut.Invoke(context);

            sutAction.ShouldThrow<OlabogaException>();
        }

        [Fact]
        public async Task On_Exception_Should_Log_500_Request_And_Exception_Telemetry()
        {
            // given
            var context = new MockOwinContextBuilder().Build();

            var configuration = new TelemetryConfigurationBuilder().Build();


            var sut = new OperationIdContextMiddleware(
                new HttpRequestTrackingMiddleware(
                    new ThrowingMiddleware(), configuration),
                new OperationIdContextMiddlewareConfiguration());

            // when
            try 
            {
                await sut.Invoke(context);
            }
            catch(OlabogaException)
            {
                // ignored
            }

            // then
            var channel = configuration.TelemetryChannel as MockTelemetryChannel;
            channel.SentTelemetries.Count.Should().Be(2);

            var requestTelemetry = channel.SentTelemetries.First(t => t is RequestTelemetry) as RequestTelemetry;
            requestTelemetry.Success.Should().BeFalse();
            requestTelemetry.ResponseCode.Should().Be("500");

            VerifyDefaultsOnRequestTelemetry(requestTelemetry);

            var exceptionTelemetry = channel.SentTelemetries.First(t => t is ExceptionTelemetry) as ExceptionTelemetry;
            exceptionTelemetry.Context.Operation.Id.Should().NotBeNullOrEmpty();
            exceptionTelemetry.Exception.Should().BeOfType<OlabogaException>();
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
            var response = Mock.Of<IOwinResponse>(r => r.StatusCode == statusCode);

            var context = new MockOwinContextBuilder()
                .WithResponse(response)
                .Build();

            var configuration = new TelemetryConfigurationBuilder().Build();

            var sut = new HttpRequestTrackingMiddleware(new NoopMiddleware(), configuration);

            // when
            await sut.Invoke(context);

            // then
            var channel = configuration.TelemetryChannel as MockTelemetryChannel;
            var telemetry = channel.SentTelemetries.First() as RequestTelemetry;
            telemetry.Success.Should().Be(expectedSuccess);
        }

    }
}
