# z.Autowired
[![z.Autowired](https://img.shields.io/nuget/v/z.Autowired.svg?style=flat-square&color=blue&logo=nuget)](https://www.nuget.org/packages/z.Autowired/)

Attribute-based dependency injection for .NET. based from [Quickwire](https://github.com/Flavien/quickwire)

## Features

- Service services for dependency injection using attributes and reduce boilerplate code.
- Uses the built-in .NET dependency injection container. No need to use a third party container or to change your existing code.
- Full support for property injection.
- Inject configuration settings seamlessly into your services.
- Use configuration selectors to register different implementations based on the current environment.

## Quick start

1. Install from NuGet:

```
dotnet add package z.Autowired
```

2. Activate z.Autowired for the current assembly in the `AddServices` method of the `Startup` class:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.ScanCurrentAssembly();
}
```

3. Decorate services with this attribute in order to register them for dependency injection:

```csharp
[Service]
public class MyService
{
    // ...
}
```

## Registering a service

There are two ways to register a service using z.Autowired.
- Apply the `[Service]` attribute to a class.
- Apply the `[Bean]` attribute to a static factory method.

## Registering a service using a class

### Registration

When applying the `[Service]` attribute to a class, the class will be registered as the concrete type to use for the service of the same type.

It is also possible to specify the `ServiceType` property on the `[Service]` attribute to register the concrete type under a different service type. The `ServiceType` should be parent type, or interface implemented by the class.

The attribute takes a parameter of type `ServiceLifetime` to specify the lifetime of the service. If left unspecified, the default is `Transient`.

### Instantiation

By default, z.Autowired will use the public constructor to instantiate the concrete type. There must be exactly one public constructor available or an exception will be thrown.

If there are more than one public constructor, or if a non-public constructor should be used, the `[ServiceConstructor]` should be applied to indicate explicitly and unambiguously which constructor to use.

By default, all the constructor parameters will be resolved using dependency injection, however it is also possible to decorate parameters using the `[Value]` attribute to inject a configuration setting. It is also possible to use `[Autowired(Optional = true)]` to make a dependency optional.

### Property injection

Property injection can be achieved by decorating a property with a setter with the `[Autowired]` attribute.

```csharp
[Service(ServiceLifetime.Singleton)]
public class MyService
{
    [Autowired]
    public IHttpClientBuilder HttpClientBuilder { get; init; }

    // ...
}
```
  
## Registering a service using a factory

When applying the `[Bean]` attribute to a static method, the static method will be registered as a factory used 

By default, all the method parameters will be resolved using dependency injection, however it is also possible to decorate parameters using the `[Value]` attribute to inject a configuration setting. It is also possible to use `[Autowired(Optional = true)]` to make a dependency optional.

```csharp
public static class LoggingConfiguration
{
    [Bean]
    public static ILogger CreateLogger()
    {
        // ...
    }
}
```

## Configuration setting injection

Constructor parameters, properties and factory parameters can also be decorated with the `[Value]` attribute in order to inject a configuration setting.

Conversion to most basic types, including enumeration types, is supported.

```csharp
[Service]
public class MyService
{
    [Value("external_api:url")]
    public string ExternalApiUrl { get; init; }

    [Value("external_api:retries")]
    public int Retries { get; init; }

    [Value("external_api:timeout")]
    public TimeSpan Timeout { get; init; }

    // ...
}
```

## Environment selection

It is possible to disable specific services or service factories using the `[Profile]` attribute.

```csharp
[Profile(Enabled = new[] { "Development" })]
public class DebugFactories
{
    // This is only registered in the Development environment
    [Bean]
    public static ILogger CreateDebugLogger()
    {
        // ...
    }

    // ...
}
```

## Selection based on configuration

It is possible to disable specific services or service factories based on the value of specific configuration settings using the `[ConfigurationBasedSelector]`.

```csharp
[ConfigurationBasedSelector("logging:mode", "debug")]
public class DebugFactories
{
    // This is only registered if the logging:mode configuration setting is set to "debug"
    [Bean]
    public static ILogger CreateDebugLogger()
    {
        // ...
    }

    // ...
}
```

## Using z.Autowired with ASP.NET Core controllers

By default, controllers in ASP.NET Core are not activated using the dependency injection container. ASP.NET Core does however offer a simple way to change that behavior. In the `Startup` class, in `ConfigureServices`, use the following extension method:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Activate controllers using the dependency injection container
    services.AddControllers().AddControllersAsServices();

    services.ScanCurrentAssembly();
}
```

Then decorate controllers as any other service:

```csharp
[ApiController]
[Route("[controller]")]
[Service]
public class ShoppingCartController : ControllerBase
{
    [Autowired]
    public IShoppingCartRepository { get; init; }

    // ...
}
```

## Equivalence with Spring Framework in Java

The approach provided by z.Autowired in .NET matches closely the approach that can be used in Java with the Spring Framework. The table below outlines the similarities between both approaches.

|       | z.Autowired | Spring |
| ----- | --------- | ------ |
| Service a class for dependency injection | `[Service]` | `@Service`, `@Component`, `@Repository` |
| Service a factory method | `[Bean]` | `@Bean` |
| Property injection | `[Autowired]` | `@Autowired` |
| Configuration setting injection | `[Value]` | `@Value` |
| Selective activation based on environment | `[Profile]` | `@Profile("profile")`
| Bootstrap | `services.ScanCurrentAssembly()` | `@EnableAutoConfiguration`, `@ComponentScan` |


## License 

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and limitations under the License.