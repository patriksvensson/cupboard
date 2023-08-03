using System;
using System.Threading.Tasks;

namespace Cupboard;

public abstract class PackageInstallerProvider<T> : AsyncResourceProvider<T>
    where T : Resource, IHasPackageState, IHasPackageName
{
    private readonly ICupboardLogger _logger;
    private readonly IEnvironmentRefresher _refresher;
    private string? _cachedOutput;

    protected virtual bool ShouldRefresh => true;
    protected virtual bool ShouldCache => false;
    protected abstract string Name { get; }
    protected virtual string Kind { get; } = "package";

    protected PackageInstallerProvider(
        ICupboardLogger logger,
        IEnvironmentRefresher refresher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _refresher = refresher;
    }

    protected abstract Task<ProcessRunnerResult> GetPackageState(T resource);
    protected abstract bool IsPackageInstalled(T resource, string output);

    protected abstract Task<ProcessRunnerResult> InstallPackage(T resource);
    protected abstract Task<ProcessRunnerResult> UninstallPackage(T resource);

    protected virtual bool IsError(PackageInstallerOperation operation, ProcessRunnerResult result)
    {
        return result.ExitCode != 0;
    }

    public override async Task<ResourceState> RunAsync(IExecutionContext context, T resource)
    {
        var state = await CheckIfInstalled(resource).ConfigureAwait(false);
        if (state == PackageInstallerResult.Error)
        {
            return ResourceState.Error;
        }

        if (resource.Ensure == PackageState.Installed)
        {
            if (state == PackageInstallerResult.Missing)
            {
                if (context.DryRun)
                {
                    return ResourceState.Changed;
                }

                // Install package
                return await Install(resource).ConfigureAwait(false);
            }

            _logger.Debug($"The {Name} {Kind} [yellow]{resource.Package}[/] is already installed");
        }
        else
        {
            if (state == PackageInstallerResult.Exists)
            {
                if (context.DryRun)
                {
                    return ResourceState.Changed;
                }

                // Uninstall package
                return await Uninstall(resource).ConfigureAwait(false);
            }

            _logger.Debug($"The {Name} {Kind} [yellow]{resource.Package}[/] is already uninstalled");
        }

        return ResourceState.Unchanged;
    }

    private async Task<PackageInstallerResult> CheckIfInstalled(T resource)
    {
        if (ShouldCache && _cachedOutput is not null)
        {
            return IsPackageInstalled(resource, _cachedOutput)
                ? PackageInstallerResult.Exists
                : PackageInstallerResult.Missing;
        }

        var result = await GetPackageState(resource).ConfigureAwait(false);
        if (IsError(PackageInstallerOperation.RetriveState, result))
        {
            _logger.Error($"An error occured while retrieving {Name} state");
            return PackageInstallerResult.Error;
        }

        if (ShouldCache)
        {
            _cachedOutput = result.StandardOut;
        }

        return IsPackageInstalled(resource, result.StandardOut)
            ? PackageInstallerResult.Exists
            : PackageInstallerResult.Missing;
    }

    private async Task<ResourceState> Install(T resource)
    {
        _logger.Debug($"Installing {Name} {Kind} [yellow]{resource.Package}[/]");

        // Install package
        var result = await InstallPackage(resource).ConfigureAwait(false);
        _cachedOutput = null;

        if (IsError(PackageInstallerOperation.Install, result))
        {
            _logger.Error($"An error occured while installing {Name} package");
            return ResourceState.Error;
        }

        var state = await CheckIfInstalled(resource).ConfigureAwait(false);
        if (state == PackageInstallerResult.Exists)
        {
            _logger.Information($"The {Name} {Kind} [yellow]{resource.Package}[/] was installed");

            if (ShouldRefresh)
            {
                _refresher.Refresh();
            }

            return ResourceState.Changed;
        }

        return ResourceState.Error;
    }

    private async Task<ResourceState> Uninstall(T resource)
    {
        _logger.Debug($"Uninstalling {Name} {Kind} [yellow]{resource.Package}[/]");

        // Uninstall package
        var result = await UninstallPackage(resource).ConfigureAwait(false);
        _cachedOutput = null;

        if (IsError(PackageInstallerOperation.Uninstall, result))
        {
            _logger.Error($"An error occured while uninstalling {Name} package");
            return ResourceState.Error;
        }

        var state = await CheckIfInstalled(resource).ConfigureAwait(false);
        if (state == PackageInstallerResult.Missing)
        {
            _logger.Information($"The {Name} {Kind} [yellow]{resource.Package}[/] was uninstalled");

            if (ShouldRefresh)
            {
                _refresher.Refresh();
            }

            return ResourceState.Changed;
        }

        return ResourceState.Error;
    }
}
