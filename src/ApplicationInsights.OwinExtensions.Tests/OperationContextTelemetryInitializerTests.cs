using System;
using FluentAssertions;
using Microsoft.ApplicationInsights.DataContracts;
using Xunit;

namespace ApplicationInsights.OwinExtensions.Tests
{
    public class OperationContextTelemetryInitializerTests : IDisposable
    {
        public OperationContextTelemetryInitializerTests()
        {
            OperationIdContext.Clear();
        }

        [Fact]
        public void Can_Set_Operation_Id_On_Telemetry()
        {
            var telemetry = new TraceTelemetry();
            var sut = new OperationIdTelemetryInitializer();

            OperationIdContext.Create();
            sut.Initialize(telemetry);

            telemetry.Context.Operation.Id.Should().Be(OperationIdContext.Get());
        }

        [Fact]
        public void Should_Not_Change_Operation_Id_If_Context_Not_Created()
        {
            var telemetry = new TraceTelemetry();
            telemetry.Context.Operation.Id = "test";

            var sut = new OperationIdTelemetryInitializer();

            sut.Initialize(telemetry);

            telemetry.Context.Operation.Id.Should().Be("test");

        }

        public void Dispose()
        {
            OperationIdContext.Clear();
        }
    }
}
