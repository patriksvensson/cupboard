using System;
using System.Threading.Tasks;

namespace Cupboard
{
    public sealed class ChocolateyPackageProvider : AsyncWindowsResourceProvider<ChocolateyPackage>
    {
        private readonly IProcessRunner _runner;
        private readonly IEnvironmentRefresher _refresher;
        private readonly ICupboardLogger _logger;
        private string? _cachedOutput;
        private bool _dirty;

        private enum ChocolateyPackageState
        {
            Exists,
            Missing,
            Error,
        }

        public ChocolateyPackageProvider(IProcessRunner runner, IEnvironmentRefresher refresher, ICupboardLogger logger)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
            _refresher = refresher ?? throw new ArgumentNullException(nameof(refresher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dirty = true;
        }

        public override ChocolateyPackage Create(string name)
        {
            return new ChocolateyPackage(name)
            {
                Package = name,
            };
        }

        public override bool RequireAdministrator(FactCollection facts)
        {
            return true;
        }

        public override async Task<ResourceState> RunAsync(IExecutionContext context, ChocolateyPackage resource)
        {
            var state = await IsPackageInstalled(resource.Package).ConfigureAwait(false);
            if (state == ChocolateyPackageState.Error)
            {
                return ResourceState.Error;
            }

            if (resource.Ensure == PackageState.Installed)
            {
                if (state == ChocolateyPackageState.Missing)
                {
                    if (context.DryRun)
                    {
                        return ResourceState.Changed;
                    }

                    // Install package
                    state = await InstallPackage(resource.Package).ConfigureAwait(false);
                    if (state == ChocolateyPackageState.Exists)
                    {
                        _logger.Information($"The Chocolatey package [yellow]{resource.Package}[/] was installed");
                        _refresher.Refresh();
                        return ResourceState.Changed;
                    }

                    return ResourceState.Error;
                }

                _logger.Debug($"The Chocolatey package [yellow]{resource.Package}[/] is already installed");
            }
            else
            {
                if (state == ChocolateyPackageState.Exists)
                {
                    if (context.DryRun)
                    {
                        return ResourceState.Changed;
                    }

                    // Uninstall package
                    state = await UninstallPackage(resource.Package).ConfigureAwait(false);
                    if (state == ChocolateyPackageState.Missing)
                    {
                        _logger.Information($"The Chocolatey package [yellow]{resource.Package}[/] was uninstalled");
                        _refresher.Refresh();

                        return ResourceState.Changed;
                    }

                    return ResourceState.Error;
                }

                _logger.Debug($"The Chocolatey package [yellow]{resource.Package}[/] is already uninstalled");
            }

            return ResourceState.Unchanged;
        }

        private async Task<ChocolateyPackageState> IsPackageInstalled(string package)
        {
            if (_dirty || _cachedOutput == null)
            {
                var result = await _runner.Run("choco", "list -lo").ConfigureAwait(false);
                if (result.ExitCode != 0)
                {
                    return ChocolateyPackageState.Error;
                }

                _cachedOutput = result.StandardOut;
                _dirty = false;
            }

            if (_cachedOutput == null)
            {
                _logger.Error("An error occured while retrieving Chocolatey state");
                return ChocolateyPackageState.Error;
            }

            if (_cachedOutput.Contains(package, StringComparison.OrdinalIgnoreCase))
            {
                return ChocolateyPackageState.Exists;
            }

            return ChocolateyPackageState.Missing;
        }

        private async Task<ChocolateyPackageState> InstallPackage(string package)
        {
            _logger.Debug($"Installing Chocolatey package [yellow]{package}[/]");

            var result = await _runner.Run("choco", $"install {package} -y").ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                return ChocolateyPackageState.Error;
            }

            _dirty = true;
            return await IsPackageInstalled(package).ConfigureAwait(false);
        }

        private async Task<ChocolateyPackageState> UninstallPackage(string package)
        {
            _logger.Debug($"Uninstalling Chocolatey package {package}...");

            var result = await _runner.Run("choco", $"uninstall {package}").ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                return ChocolateyPackageState.Error;
            }

            _dirty = true;
            return await IsPackageInstalled(package).ConfigureAwait(false);
        }
    }
}
