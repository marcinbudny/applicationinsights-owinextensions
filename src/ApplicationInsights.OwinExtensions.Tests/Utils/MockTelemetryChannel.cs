using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace ApplicationInsights.OwinExtensions.Tests.Utils
{
    public class MockTelemetryChannel : ITelemetryChannel, ITelemetryModule
    {
        public bool? DeveloperMode { get; set; }
        public string EndpointAddress { get; set; }
        public List<ITelemetry> SentTelemetries { get; } = new List<ITelemetry>();

        public void Dispose() { }

        public void Send(ITelemetry item)
        {
            SentTelemetries.Add(item);
        }

        public void Flush() { }

        public void Initialize(TelemetryConfiguration configuration) { }
    }
}
