using Microsoft.Extensions.DependencyInjection;

namespace Cupboard
{
    public sealed class ResourceModule : DependencyModule
    {
        public override void Register(IServiceCollection services)
        {
            // Resources
            services.AddSingleton<IResourceProvider, DirectoryProvider>();
            services.AddSingleton<IResourceProvider, DownloadProvider>();
            services.AddSingleton<IResourceProvider, ExecProvider>();
            services.AddSingleton<IResourceProvider, FileProvider>();
            services.AddSingleton<IResourceProvider, PowerShellProvider>();
            services.AddSingleton<IResourceProvider, VSCodeExtensionProvider>();

            // Facts
            services.AddSingleton<IFactProvider, ArgumentFacts>();
            services.AddSingleton<IFactProvider, EnvironmentFacts>();
            services.AddSingleton<IFactProvider, MachineFacts>();
        }
    }
}
