namespace Cupboard.Sandbox;

public static class Program
{
    public static int Main(string[] args)
    {
        return CupboardHost.CreateBuilder()
            .AddCatalog<SandboxCatalog>()
            .Run(args);
    }
}

public sealed class SandboxCatalog : WindowsCatalog
{
    public override void Execute(CatalogContext context)
    {
        context.UseManifest<VSCode>();
        context.UseManifest<Chocolatey>();
    }
}
