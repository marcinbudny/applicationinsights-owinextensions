# Application Insights OWIN extensions

This library is a set of extensions, that allow you to easily get some features normally not available for OWIN based applications out-of-the-box (but are available when using the classic ASP.NET pipeline).

## Features

* Sets the common Conext.Operation.Id property for all telemetries within one request (even asynchronously called dependencies)
* Creates Request telemetry with proper name, id and execution time
* Useable with both self-hosted and System.Web hosted OWIN applications

## Installation

Install Application Insights within the project like you would normally do. You may also want to update related nuget packages to latets versions.

Install the extensions package:

```posh
Install-Package ApplicationInsights.OwinExtensions
```

In your `Startup` class, add as a first step of the pipeline:

```csharp
public class Startup
{
	public void Configuration(IAppBuilder app)
	{
		app.UseApplicationInsights();
		// rest of the config here...
	}
}
```

**Note:** If you are using `Microsoft.Owin.Security.*` middlewares, you need to restore the Operation Id context one step after the authentication middleware - otherwise the Operation Id context will be lost (*TODO: figure out why*).

```csharp
// ...
app.UseOAuthBearerTokens(OAuthOptions);
// ...

// now restore the Operation Id context from the OWIN environment dictionary
app.RestoreOperationIdContext();
```

One last thing to do is to configure the Operation Id telemetry initializer. With XML in `ApplicationInsights.config`:

```xml
<TelemetryInitializers>
	<!-- other initializers ... -->
	<Add Type="ApplicationInsights.OwinExtensions.OperationIdTelemetryInitializer, ApplicationInsights.OwinExtensions"/>
</TelemetryInitializers>
```

or in code:

```csharp
TelemetryConfiguration.Active
	.TelemetryInitializers.Add(new OperationIdTelemetryInitializer());
```

### Optional steps

In most cases you can remove following telemetry initializers that are present by default:
* `Microsoft.ApplicationInsights.Web.OperationIdTelemetryInitializer`
* `Microsoft.ApplicationInsights.Web.OperationNameTelemetryInitializer`

and also the `Microsoft.ApplicationInsights.Web.RequestTrackingTelemetryModule`

## How this stuff works

First middleware in the pipeline establishes a new Operation Id context (`Guid.NewGuid()` by default). This value is stored both in OWIN environment dictionary under the `ApplicationInsights.OwinExtensions.OperationIdContext` key, and in the [CallContext](https://msdn.microsoft.com/en-US/library/system.runtime.remoting.messaging.callcontext). There is the [OperationIdContext](src\ApplicationInsights.OwinExtensions\OperationIdContext.cs) class that can be used to access the current value of Operation Id from anywhere withing the call context. The telemetry initializer makes use of that and sets appropriate property on the telemetries.

## Contributing

If you would like to contribute, please create a PR against the develop branch.