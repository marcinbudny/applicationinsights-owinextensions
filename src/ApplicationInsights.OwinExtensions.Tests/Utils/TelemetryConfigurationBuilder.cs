using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace ApplicationInsights.OwinExtensions.Tests.Utils
{
    public class TelemetryConfigurationBuilder
    {
        private TelemetryConfiguration _configuration = new TelemetryConfiguration();

        public TelemetryConfigurationBuilder()
        {
            _configuration.InstrumentationKey = "123";
            _configuration.TelemetryChannel = new MockTelemetryChannel();
        }

        public TelemetryConfigurationBuilder WithChannel(ITelemetryChannel channel)
        {
            _configuration.TelemetryChannel = channel;
            return this;
        }

        public TelemetryConfigurationBuilder WithTelemetryInitializer(ITelemetryInitializer initializer)
        {
            _configuration.TelemetryInitializers.Add(initializer);
            return this;
        }

        public TelemetryConfiguration Build()
        {
            return _configuration;
        }
    }
}
