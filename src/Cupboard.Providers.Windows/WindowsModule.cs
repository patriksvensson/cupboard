using Microsoft.Extensions.DependencyInjection;

namespace Cupboard
{
    public sealed class WindowsModule : ServiceModule
    {
        public override void Configure(IServiceCollection services)
        {
            // Resources
            services.AddSingleton<IResourceProvider, ChocolateyPackageProvider>();
            services.AddSingleton<IResourceProvider, WindowsFeatureProvider>();
            services.AddSingleton<IResourceProvider, RegistryValueProvider>();
            services.AddSingleton<IResourceProvider, WingetPackageProvider>();

#pragma warning disable CS0618 // Type or member is obsolete
            services.AddSingleton<IResourceProvider, RegistryKeyProvider>();
#pragma warning restore CS0618 // Type or member is obsolete

            // Facts
            services.AddSingleton<IFactProvider, WindowsFacts>();
            services.AddSingleton<IFactProvider, WmiFacts>();
        }
    }
}
