# windsor-service-provider
Service Provider using Castle Windsor container for ASP.NET Core 3.x. Fully replaces ASP.NET Core Default Service Provider (container). Supports both registering services
* using `IServiceCollection` (`Startup.ConfigureServices`) - these get translated to Castle Windsor registrations
* `Startup.ConfigureContainer`
    ```
    public void ConfigureContainer (IWindsorContainer container)
    {
        container.Install(new MyInstaller());
    }
    ```

The work has been heavily inspired by https://github.com/volosoft/castle-windsor-ms-adapter and https://github.com/seesharper/LightInject.Microsoft.DependencyInjection

## Usage
1. Add `UseWindsorContainerServiceProvider()` when creating the Host
    ```
    Host.CreateDefaultBuilder(args)
        .UseWindowsService()
        .UseWindsorContainerServiceProvider()
    ```
    This will register `IServiceProviderFactory`
2. Any services registred in `Startup.ConfigureServices` will be registered with `IWindsorContainer`. No need to cross-wire since `IWindsorContainer` is the only `IServiceProvider`
3. To match lifecycle your custom components to ASP.NET Core scope, use `LifeStyle.ScopedToNetCoreScope()`
4. To access the container inject either `IWindsorContainer` or `IServiceProvider`

## Licenses
Apache 2.0