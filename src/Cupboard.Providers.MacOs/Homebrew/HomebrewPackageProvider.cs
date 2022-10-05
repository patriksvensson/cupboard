using System.Text;
using CliWrap;

namespace Cupboard
{
    public sealed class HomebrewPackageProvider : AsyncMacResourceProvider<HomebrewPackage>
    {
        private readonly IProcessRunner _runner;
        private readonly IEnvironmentRefresher _refresher;
        private readonly ICupboardLogger _logger;
        private string? _cachedOutput;
        private bool _dirty;

        private enum HomebrewPackageState
        {
            Exists,
            Missing,
            Error,
        }

        public HomebrewPackageProvider(IProcessRunner runner, IEnvironmentRefresher refresher, ICupboardLogger logger)
        {
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
            _refresher = refresher ?? throw new ArgumentNullException(nameof(refresher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dirty = true;
        }

        public override HomebrewPackage Create(string name)
        {
            return new HomebrewPackage(name);
        }

        public override async Task<ResourceState> RunAsync(IExecutionContext context, HomebrewPackage resource)
        {
            var state = await IsPackageInstalled(resource).ConfigureAwait(false);
            if (state == HomebrewPackageState.Error)
            {
                _logger.Error($"{resource.Package} is not previously installed.");
                return ResourceState.Error;
            }

            if (resource.Ensure == PackageState.Installed)
            {
                if (state == HomebrewPackageState.Missing)
                {
                    if (context.DryRun)
                    {
                        return ResourceState.Changed;
                    }

                    // Install package
                    if (resource.IsCask)
                    {
                        state = await InstallCask(resource).ConfigureAwait(false);
                    }
                    else
                    {
                        state = await InstallPackage(resource).ConfigureAwait(false);
                    }

                    if (state == HomebrewPackageState.Exists)
                    {
                        _logger.Information(
                            $"The Homebrew package [yellow]{resource.Package}[/] was already installed");
                        _refresher.Refresh();
                        return ResourceState.Changed;
                    }

                    return ResourceState.Error;
                }

                _logger.Debug($"The Homebrew package [yellow]{resource.Package}[/] is already installed");
            }
            else
            {
                if (state == HomebrewPackageState.Exists)
                {
                    if (context.DryRun)
                    {
                        return ResourceState.Changed;
                    }

                    // Uninstall package
                    state = await UninstallPackage(resource).ConfigureAwait(false);
                    if (state == HomebrewPackageState.Missing)
                    {
                        _logger.Information($"The Homebrew package [yellow]{resource.Package}[/] was uninstalled");
                        _refresher.Refresh();

                        return ResourceState.Changed;
                    }

                    return ResourceState.Error;
                }

                _logger.Debug($"The Homebrew package [yellow]{resource.Package}[/] is already uninstalled");
            }

            return ResourceState.Unchanged;
        }

        private async Task<HomebrewPackageState> IsPackageInstalled(HomebrewPackage package)
        {
            if (_dirty || _cachedOutput == null)
            {
                try
                {
                    var stdout = new StringBuilder();
                    var result = await Cli.Wrap("brew")
                        .WithArguments(package.List())
                        .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdout))
                        .WithValidation(CommandResultValidation.None)
                        .ExecuteAsync()
                        .ConfigureAwait(false);

                    // var result = await _runner.Run("brew", package.List(), cmd => _logger.Information($"{cmd}")).ConfigureAwait(false);
                    if (result.ExitCode != 0)
                    {
                        _logger.Error($"Received non zero exit code when checking if {package.Package} is installed.");
                        return HomebrewPackageState.Error;
                    }

                    _cachedOutput = stdout.ToString();
                    _dirty = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            if (_cachedOutput == null)
            {
                _logger.Error("An error occured while retrieving Homebrew state");
                return HomebrewPackageState.Error;
            }

            if (_cachedOutput.Contains(package.Package, StringComparison.OrdinalIgnoreCase))
            {
                return HomebrewPackageState.Exists;
            }

            return HomebrewPackageState.Missing;
        }

        private async Task<HomebrewPackageState> InstallCask(HomebrewPackage package)
        {
            _logger.Debug($"Installing Homebrew Cask [yellow]{package}[/]");

            var result = await _runner.Run("brew", $"install --cask {package}").ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                return HomebrewPackageState.Error;
            }

            _dirty = true;
            return await IsPackageInstalled(package).ConfigureAwait(false);
        }

        private async Task<HomebrewPackageState> InstallPackage(HomebrewPackage package)
        {
            _logger.Debug($"Installing Homebrew package [yellow]{package}[/]");

            var result = await _runner.Run("brew", $"install {package} --display-times").ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                return HomebrewPackageState.Error;
            }

            _dirty = true;
            return await IsPackageInstalled(package).ConfigureAwait(false);
        }

        private async Task<HomebrewPackageState> UninstallPackage(HomebrewPackage package)
        {
            _logger.Debug($"Uninstalling Homebrew package {package}...");

            var result = await _runner.Run("brew", $"uninstall {package}").ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                return HomebrewPackageState.Error;
            }

            _dirty = true;
            return await IsPackageInstalled(package).ConfigureAwait(false);
        }
    }
}