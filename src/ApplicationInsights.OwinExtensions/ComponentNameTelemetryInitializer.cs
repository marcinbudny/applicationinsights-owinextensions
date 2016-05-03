using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace ApplicationInsights.OwinExtensions
{
    public class ComponentNameTelemetryInitializer : ITelemetryInitializer
    {
        private readonly string _componentName;

        public ComponentNameTelemetryInitializer(string componentName)
        {
            _componentName = componentName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Properties["ComponentName"] = _componentName;
        }
    }
}