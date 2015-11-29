using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace ApplicationInsights.OwinExtensions
{
    public class OperationIdTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (OperationIdContext.Get() != null)
                telemetry.Context.Operation.Id = OperationIdContext.Get();
        }
    }
}
