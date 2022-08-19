using Microsoft.Extensions.DependencyInjection;

namespace Cupboard.Providers.MacOs;

public class MacOsModule : ServiceModule
{
    public override void Configure(IServiceCollection services)
    {
        // Resources
        services.AddSingleton<IResourceProvider, HomebrewPackageProvider>();
        services.AddSingleton<IResourceProvider, BashScriptProvider>();
    }
}