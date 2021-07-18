using Cupboard;

namespace Sandbox
{
    public sealed class ChocolateyPackages : Manifest
    {
        public override void Execute(ManifestContext context)
        {
            foreach (var package in new[] { "screentogif", "repoz" })
            {
                context.Resource<ChocolateyPackage>(package)
                    .Ensure(PackageState.Installed)
                    .After<PowerShell>("Install Chocolatey");
            }
        }
    }
}