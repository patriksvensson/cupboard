using System;
using System.Threading.Tasks;

namespace Cupboard
{
    public sealed class WingetPackageProvider : PackageInstallerProvider<WingetPackage>
    {
        private readonly IProcessRunner _runner;

        protected override string Name { get; } = "Winget";

        public WingetPackageProvider(
            IProcessRunner runner,
            ICupboardLogger logger,
            IEnvironmentRefresher refresher)
                : base(logger, refresher)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        public override WingetPackage Create(string name)
        {
            return new WingetPackage(name)
            {
                Package = name,
            };
        }

        protected override bool IsPackageInstalled(WingetPackage resource, string output)
        {
            return output.Contains(resource.Package, StringComparison.OrdinalIgnoreCase);
        }

        protected override async Task<ProcessRunnerResult> GetPackageState(WingetPackage resource)
        {
            var arguments = $"list --source winget --id {resource.Package}";
            return await _runner.Run("winget", arguments, supressOutput: true).ConfigureAwait(false);
        }

        protected override bool IsError(PackageInstallerOperation operation, ProcessRunnerResult result)
        {
            if (operation == PackageInstallerOperation.RetriveState)
            {
                return result.ExitCode != 0 && (!result.StandardOut?.EndsWith("No installed package found matching input criteria.") ?? true);
            }

            return result.ExitCode != 0;
        }

        protected override async Task<ProcessRunnerResult> InstallPackage(WingetPackage resource)
        {
            var arguments = $"install -e --id {resource.Package}";

            if (resource.Force)
            {
                arguments += " --force";
            }

            if (!string.IsNullOrWhiteSpace(resource.PackageVersion))
            {
                arguments += $" --version {resource.PackageVersion}";
            }

            if (string.IsNullOrWhiteSpace(resource.Override) is false)
            {
                arguments += $" --override \"{resource.Override}\"";
            }

            if (string.IsNullOrWhiteSpace(resource.Source) is false)
            {
                arguments += $" --source {resource.Source}";
            }

            return await _runner.Run("winget", arguments).ConfigureAwait(false);
        }

        protected override async Task<ProcessRunnerResult> UninstallPackage(WingetPackage resource)
        {
            return await _runner.Run("winget", $"uninstall -e --id {resource.Package}").ConfigureAwait(false);
        }
    }
}
