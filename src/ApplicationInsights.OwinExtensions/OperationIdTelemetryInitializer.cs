using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace ApplicationInsights.OwinExtensions
{
    public class OperationIdTelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            var context = OperationContext.Get();

            if (context != null)
            {
                telemetry.Context.Operation.Id = context.OperationId;
                telemetry.Context.Operation.ParentId = context.ParentOperationId;
            }
        }
    }
}
