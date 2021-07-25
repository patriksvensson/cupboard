using Cupboard;

namespace Sandbox
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return CupboardHost.CreateBuilder()
                .AddCatalog<SandboxCatalog>()
                .Build()
                .Run(args);
        }
    }

    public sealed class SandboxCatalog : WindowsCatalog
    {
        public override void Execute(CatalogContext context)
        {
            context.UseManifest<Chocolatey>();
            context.UseManifest<ChocolateyPackages>();
        }
    }
}
