# Application Insights OWIN extensions

This library is a set of extensions, that allow you to easily get some features normally not available for OWIN based applications out-of-the-box (but are available when using the classic ASP.NET pipeline).

## Features

* Sets the common Context.Operation.Id property for all telemetries within one request (even asynchronously called dependencies)
* Pass Context.Operation.Id property between multiple dependencies request chains
* Creates Request telemetry with proper name, id and execution time
* Useable with both self-hosted and System.Web hosted OWIN applications

## Installation

### Required steps

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

Cleanup the `ApplicationInsights.config` by removing telemetry initializers and modules with overlapping functionality.

* `Microsoft.ApplicationInsights.Web.OperationCorrelationTelemetryInitializer`
* `Microsoft.ApplicationInsights.Web.OperationIdTelemetryInitializer`
* `Microsoft.ApplicationInsights.Web.OperationNameTelemetryInitializer`

and also the `Microsoft.ApplicationInsights.Web.RequestTrackingTelemetryModule`

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

### Possibly required steps

**Note:** If you are using `Microsoft.Owin.Security.*` middlewares, you need to restore the Operation Id context one step after the authentication middleware - otherwise the Operation Id context will be lost. This problem is most probably related to the security middleware taking advantage of the old System.Web pipeline integration and setting the stage markers. The problem will probably surface also with other middlewares using the stage markers.

```csharp
// ...
app.UseOAuthBearerTokens(OAuthOptions);
// ...

// now restore the Operation Id context from the OWIN environment dictionary
app.RestoreOperationIdContext();
```

## Advanced usage

### Selectively tracing requests

You can pass a parameter to the `UseApplicationInsights` method specifying request filtering callback.

```csharp
appBuilder.UseApplicationInsights(new RequestTrackingConfiguration
{
    ShouldTrackRequest = ctx => Task.FromResult(ctx.Request.Method != "OPTIONS")
});
```

### Adding custom properties to telemetry

You can also extend the traced telemetry with custom properties. Note: if your custom properties are not related
to the request or the response, you can alternatively specify additional `TelemetryInitializer` in your 
Application Insights telemetry configuration.

```csharp
appBuilder.UseApplicationInsights(new RequestTrackingConfiguration
{
    GetAdditionalContextProperties = ctx =>
        Task.FromResult(new[] { new KeyValuePair<string, string>("Content-Type", ctx.Request.ContentType)}
            .AsEnumerable())
});
```

### Passing OperationId via header

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
		  new OperationIdContexMiddlewareConfiguration { OperationIdFactory = IdFactory.FromHeader("X-My-Operation-Id") });
		  
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

	request.Headers.Add("X-My-Operation-Id", OperationContext.Get().OperationId);
	await client.SendAsync(request);
}
```

The OperationContext is a static class storing current request Context.Operation.Id value.


### Changing how ids are generated

By default, new ids are generated as Guids. You can change that by providing delegates in `OperationIdContextMiddlewareConfiguration.OperationIdFactory` and `RequestTrackingConfiguration.RequestIdFactory`.

### Operation scope and parent operation id

You can create sub-operations and manage the `OperationParentId` via the `OperationContextScope`.

```csharp
using (new OperationContextScope("operationId", "parentOperationId")) 
{
	// telemetries sent here will have specified ids set
}
```

You should keep the same operation id for all requests, dependencies, etc. that make up a single logical operation you want to track. Change the parent operation id to create a tree of sub-operations. 

If you are implementing a middleware, you may want to also save the new values in the OWIN environment dictionary, so that they can be restored via `RestoreOperationIdContext`. To learn more about parent operation id, [see this article](https://docs.microsoft.com/en-us/azure/application-insights/application-insights-correlation).

```csharp
using (new OperationContextScope("operationId", "parentOperationId")) 
using (new OperationContextStoredInOwinContextScope(owinContext))
{
	// telemetries sent here will have specified ids set
}
```

#### Optional steps

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

## Release notes

### 0.6.0
* [BREAKING] - removed the `IdGenerationStrategy` - use `OperationIdContextMiddlewareConfiguration.OperationIdFactory` and `RequestTrackingConfiguration.RequestIdFactory`
* [FEATURE] - parent operation id can be managed with `OperationContextScope`
* [FEATURE] - establishing OperationId and RequestId can be customized and based on current OWIN context

### 0.5.1
* [FIX] - #24 temporary fix for operation parent id not set on telemetry 

### 0.5.0
* [BREAKING] (possibly) - `UseApplicationInsights` now accepts an instance of `RequestTrackingConfiguration` instead of separate configuration parameters. Old overload has been deprecated
* [FEATURE] - `IOwinContext` is passed to request filter and additional properties extractor delegates
* [FEATURE] - request filter and additional properties extractor delegates are now async 

### 0.4.1
* [FIX] Fixed #17 - incorrect logging when exception thrown from downstream OWIN pipeline

### 0.4.0 
* [FEATURE] It is now possible to add custom properties to the logged request telemetry by providing a delegate in `UseApplicationInsights`

### 0.3.0
* [FEATURE] It is now possible to filter logged request telemetries by providing a delegate in `UseApplicationInsights`
