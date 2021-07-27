using System;
using System.Threading.Tasks;

namespace Cupboard
{
    public sealed class ChocolateyPackageProvider : PackageInstallerProvider<ChocolateyPackage>
    {
        private readonly IProcessRunner _runner;

        protected override string Name { get; } = "Chocolatey";

        public ChocolateyPackageProvider(
            IProcessRunner runner,
            ICupboardLogger logger,
            IEnvironmentRefresher refresher)
                : base(logger, refresher)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        public override ChocolateyPackage Create(string name)
        {
            return new ChocolateyPackage(name)
            {
                Package = name,
            };
        }

        protected override bool IsPackageInstalled(ChocolateyPackage resource, string output)
        {
            return output.Contains(resource.Package, StringComparison.OrdinalIgnoreCase);
        }

        protected override async Task<ProcessRunnerResult> GetPackageState(ChocolateyPackage resource)
        {
            return await _runner.Run("choco", "list -lo", supressOutput: true).ConfigureAwait(false);
        }

        protected override async Task<ProcessRunnerResult> InstallPackage(ChocolateyPackage resource)
        {
            var arguments = $"install {resource.Package} -y";

            if (resource.PreRelease)
            {
                arguments += " --pre";
            }

            if (resource.IgnoreChecksum)
            {
                arguments += " --ignore-checksum";
            }

            if (!string.IsNullOrWhiteSpace(resource.PackageParameters))
            {
                arguments += $" --package-parameters=\"{resource.PackageParameters}\"";
            }

            return await _runner.Run("choco", arguments, t => !t.StartsWith("Progress:")).ConfigureAwait(false);
        }

        protected override async Task<ProcessRunnerResult> UninstallPackage(ChocolateyPackage resource)
        {
            return await _runner.Run("choco", $"uninstall {resource.Package}").ConfigureAwait(false);
        }
    }
}
