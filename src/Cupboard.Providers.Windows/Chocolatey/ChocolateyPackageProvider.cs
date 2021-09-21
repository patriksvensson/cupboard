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
            var containsPackage = output.Contains(resource.Package, StringComparison.OrdinalIgnoreCase);
            if (string.IsNullOrEmpty(resource.PackageVersion))
            {
                return containsPackage;
            }

            if (!containsPackage)
            {
                return false;
            }

            var indexOfPipe = output.IndexOf('|');
            var indexOfNewLine = output.IndexOfAny(new[] { '\r', '\n' }, indexOfPipe);
            if (indexOfPipe == -1 || indexOfNewLine == -1)
            {
                return false;
            }

            var versionString = output.Substring(indexOfPipe + 1, indexOfNewLine);
            return Version.TryParse(versionString, out var currentPackageVersion)
                && Version.TryParse(resource.PackageVersion, out var packageVersion)
                && currentPackageVersion >= packageVersion;
        }

        protected override async Task<ProcessRunnerResult> GetPackageState(ChocolateyPackage resource)
        {
            var arguments = $"list --limit-output --local-only --exact {resource.Package}";
            return await _runner.Run("choco", arguments, supressOutput: true).ConfigureAwait(false);
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

            if (!string.IsNullOrWhiteSpace(resource.PackageVersion))
            {
                arguments += $" --version {resource.PackageVersion}";
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
