# Application Insights OWIN extensions

This library is a set of extensions, that allow you to easily get some features normally not available for OWIN based applications out-of-the-box (but are available when using the classic ASP.NET pipeline).

## Features

* Sets the common Context.Operation.Id property for all telemetries within one request (even asynchronously called dependencies)
* Pass Context.Operation.Id property between multiple dependencies request chains
* Creates Request telemetry with proper name, id and execution time
* Useable with both self-hosted and System.Web hosted OWIN applications

## Installation

Install Application Insights within the project like you would normally do. You may also want to update related nuget packages to latest versions.

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

## Passing OperationId via header

Let's presume that your system is build of many services communicating by http requests with each other . 
You probably would like to be able to track how the specific operation propagate through your system's components.
To achieve this you should append the operation id to each request with a header. Provided middleware can
acquire that id, use it with its own telemetry and then it can be passed to next component. And so on... 

This behaviour is turned off by default. Following snippets present how to turn it on.

```csharp
public class Startup
{
	public void Configuration(IAppBuilder app)
	{
		app.UseApplicationInsights(			
		  new OperationIdContexMiddlewareConfiguration {ShouldTryGetIdFromHeader = true});
		  
		// rest of the config here...
	}
}
```

Default header name for Context.Operation.Id value is `X-Operation-Id`, but it can also be customized.

```csharp
public class Startup
{
	public void Configuration(IAppBuilder app)
	{
		app.UseApplicationInsights(			
		  new OperationIdContexMiddlewareConfiguration {
		  		ShouldTryGetIdFromHeader = true,
				  OperationIdHeaderName = "Custom-Header-Name"});
				  
		// rest of the config here...
	}
}
```

Example how to perform http request with appended Context.Operation.Id value:

```csharp
using (var client = new HttpClient())
{
	var request = new HttpRequestMessage
	{
		Method = HttpMethod.Get,
		RequestUri = new Uri($"http://{serviceHost}:{servicePort}")
	};

	request.Headers.Add("AI-Operation-Id", OperationIdContext.Get());
	await client.SendAsync(request);
}
```

The OperationIdContext is a static class storing current request Context.Operation.Id value.

### Optional steps

You can  use `ComponentNameTelemetryInitializer` to add `ComponentName` property to your telemetry.
It will simplify filtering telemetries connected with specific component of your system.

```csharp
TelemetryConfiguration.Active
	.TelemetryInitializers.Add(new ComponentNameTelemetryInitializer("MyComponentName"));
```

## How this stuff works

First middleware in the pipeline establishes a new Operation Id context (`Guid.NewGuid()` by default). This value is stored both in OWIN environment dictionary under the `ApplicationInsights.OwinExtensions.OperationIdContext` key, and in the [CallContext](https://msdn.microsoft.com/en-US/library/system.runtime.remoting.messaging.callcontext). There is the [OperationIdContext](src/ApplicationInsights.OwinExtensions/OperationIdContext.cs) class that can be used to access the current value of Operation Id from anywhere within the call context. The telemetry initializer makes use of that and sets appropriate property on the telemetries.

## Contributing

If you would like to contribute, please create a PR against the develop branch.