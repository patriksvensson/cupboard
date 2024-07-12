namespace Cupboard;

public sealed class WindowsModule : ServiceModule
{
    public override void Configure(IServiceCollection services)
    {
        // Resources
        services.AddSingleton<IResourceProvider, ChocolateyPackageProvider>();
        services.AddSingleton<IResourceProvider, WindowsFeatureProvider>();
        services.AddSingleton<IResourceProvider, RegistryValueProvider>();
        services.AddSingleton<IResourceProvider, WingetPackageProvider>();

        // Facts
        services.AddSingleton<IFactProvider, WindowsFacts>();
        services.AddSingleton<IFactProvider, WmiFacts>();
    }
}
