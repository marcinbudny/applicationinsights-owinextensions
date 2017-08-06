using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Owin;

namespace ApplicationInsights.OwinExtensions
{
    public class RequestTrackingConfiguration
    {
        public TelemetryConfiguration TelemetryConfiguration { get; set; }

        public Func<IOwinContext, Task<bool>> ShouldTrackRequest { get; set; }

        public Func<IOwinContext, Task<IEnumerable<KeyValuePair<string, string>>>> GetAdditionalContextProperties { get; set; }
    }
}
