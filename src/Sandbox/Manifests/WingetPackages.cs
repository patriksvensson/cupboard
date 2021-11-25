using Cupboard;

namespace Sandbox;

public sealed class WingetPackages : Manifest
{
    public override void Execute(ManifestContext context)
    {
        foreach (var package in new[] { "GitHub.cli" })
        {
            context.Resource<WingetPackage>(package)
                .Ensure(PackageState.Installed);
        }
    }
}