using System;
using System.Threading.Tasks;

namespace Cupboard
{
    public sealed class WingetPackageProvider : AsyncWindowsResourceProvider<WingetPackage>
    {
        private readonly IProcessRunner _runner;
        private readonly IEnvironmentRefresher _refresher;
        private readonly ICupboardLogger _logger;
        private string? _cachedOutput;
        private bool _dirty;

        private enum WingetPackageState
        {
            Exists,
            Missing,
            Error,
        }

        public WingetPackageProvider(IProcessRunner runner, IEnvironmentRefresher refresher, ICupboardLogger logger)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
            _refresher = refresher ?? throw new ArgumentNullException(nameof(refresher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dirty = true;
        }

        public override WingetPackage Create(string name)
        {
            return new WingetPackage(name)
            {
                Package = name,
            };
        }

        public override bool RequireAdministrator(FactCollection facts)
        {
            return true;
        }

        public override async Task<ResourceState> RunAsync(IExecutionContext context, WingetPackage resource)
        {
            var state = await IsPackageInstalled(resource.Package).ConfigureAwait(false);
            if (state == WingetPackageState.Error)
            {
                return ResourceState.Error;
            }

            if (resource.Ensure == PackageState.Installed)
            {
                if (state == WingetPackageState.Missing)
                {
                    if (context.DryRun)
                    {
                        return ResourceState.Changed;
                    }

                    // Install package
                    state = await InstallPackage(resource.Package).ConfigureAwait(false);
                    if (state == WingetPackageState.Exists)
                    {
                        _logger.Information($"The Winget package [yellow]{resource.Package}[/] was installed");
                        _refresher.Refresh();
                        return ResourceState.Changed;
                    }

                    return ResourceState.Error;
                }

                _logger.Debug($"The Winget package [yellow]{resource.Package}[/] is already installed");
            }
            else
            {
                if (state == WingetPackageState.Exists)
                {
                    if (context.DryRun)
                    {
                        return ResourceState.Changed;
                    }

                    // Uninstall package
                    state = await UninstallPackage(resource.Package).ConfigureAwait(false);
                    if (state == WingetPackageState.Missing)
                    {
                        _logger.Information($"The Winget package [yellow]{resource.Package}[/] was uninstalled");
                        _refresher.Refresh();

                        return ResourceState.Changed;
                    }

                    return ResourceState.Error;
                }

                _logger.Debug($"The Winget package [yellow]{resource.Package}[/] is already uninstalled");
            }

            return ResourceState.Unchanged;
        }

        private async Task<WingetPackageState> IsPackageInstalled(string package)
        {
            if (_dirty || _cachedOutput == null)
            {
                var result = await _runner.Run("winget", $"list --source winget --id {package}").ConfigureAwait(false);
                if (result.ExitCode != 0 && (!result.StandardOut?.EndsWith("No installed package found matching input criteria.") ?? true))
                {
                    return WingetPackageState.Error;
                }

                _cachedOutput = result.StandardOut;
                _dirty = false;
            }

            if (_cachedOutput == null)
            {
                _logger.Error("An error occured while retrieving Winget state");
                return WingetPackageState.Error;
            }

            if (_cachedOutput.Contains(package, StringComparison.OrdinalIgnoreCase))
            {
                return WingetPackageState.Exists;
            }

            return WingetPackageState.Missing;
        }

        private async Task<WingetPackageState> InstallPackage(string package)
        {
            _logger.Debug($"Installing Winget package [yellow]{package}[/]");

            var result = await _runner.Run("winget", $"install -e --id {package} --force").ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                return WingetPackageState.Error;
            }

            _dirty = true;
            return await IsPackageInstalled(package).ConfigureAwait(false);
        }

        private async Task<WingetPackageState> UninstallPackage(string package)
        {
            _logger.Debug($"Uninstalling Winget package {package}...");

            var result = await _runner.Run("winget", $"uninstall -e --id {package}").ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                return WingetPackageState.Error;
            }

            _dirty = true;
            return await IsPackageInstalled(package).ConfigureAwait(false);
        }
    }
}
