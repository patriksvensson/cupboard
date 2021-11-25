using System;
using System.Threading.Tasks;

namespace Cupboard;

public sealed class WindowsFeatureProvider : AsyncWindowsResourceProvider<WindowsFeature>
{
    private readonly IProcessRunner _runner;
    private readonly IEnvironmentRefresher _refresher;
    private readonly ICupboardLogger _logger;

    private enum WindowsFeatureStatus
    {
        Enabled,
        Disabled,
        Error,
    }

    public WindowsFeatureProvider(
        IProcessRunner runner,
        IEnvironmentRefresher refresher,
        ICupboardLogger logger)
    {
        _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        _refresher = refresher ?? throw new ArgumentNullException(nameof(refresher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override WindowsFeature Create(string name)
    {
        return new WindowsFeature(name)
        {
            FeatureName = name,
        };
    }

    public override bool RequireAdministrator(FactCollection facts)
    {
        return true;
    }

    public override async Task<ResourceState> RunAsync(IExecutionContext context, WindowsFeature resource)
    {
        var feature = resource.FeatureName;
        if (string.IsNullOrWhiteSpace(feature))
        {
            _logger.Error($"Windows feature '{resource.Name}' does not have a feature name");
            return ResourceState.Error;
        }

        var status = await IsFeatureEnabled(feature).ConfigureAwait(false);
        if (status == WindowsFeatureStatus.Error)
        {
            _logger.Error($"Could not retrieve state of Windows feature [yellow]{feature}[/]");
            return ResourceState.Error;
        }

        if (resource.Ensure == WindowsFeatureState.Enabled)
        {
            if (status == WindowsFeatureStatus.Enabled)
            {
                _logger.Debug($"The Windows feature [yellow]{feature}[/] is already enabled");
                return ResourceState.Unchanged;
            }

            var result = await _runner.Run("dism.exe", $"/online /enable-feature /featurename:{feature}").ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                _logger.Error($"An error occured when enabling Windows feature [yellow]{feature}[/]");
                return ResourceState.Error;
            }

            _refresher.Refresh();
            _logger.Verbose($"Enabled Windows feature [yellow]{feature}[/]");

            return ResourceState.Changed;
        }
        else if (resource.Ensure == WindowsFeatureState.Enabled)
        {
            if (status == WindowsFeatureStatus.Disabled)
            {
                _logger.Debug($"The Windows feature [yellow]{feature}[/] is already disabled");
                return ResourceState.Unchanged;
            }

            var result = await _runner.Run("dism.exe", $"/online /disable-feature /featurename:{feature}").ConfigureAwait(false);
            if (result.ExitCode != 0)
            {
                _logger.Error($"An error occured when disabling Windows feature [yellow]{feature}[/]");
                return ResourceState.Error;
            }

            _refresher.Refresh();
            _logger.Verbose($"Disabled Windows feature [yellow]{feature}[/]");

            return ResourceState.Changed;
        }

        return ResourceState.Error;
    }

    private async Task<WindowsFeatureStatus> IsFeatureEnabled(string feature)
    {
        var result = await _runner.Run("dism.exe", $"/online /Get-FeatureInfo /FeatureName:{feature}").ConfigureAwait(false);
        if (result.ExitCode != 0)
        {
            return WindowsFeatureStatus.Error;
        }

        if (result.StandardOut.Contains("State : Enabled", StringComparison.OrdinalIgnoreCase))
        {
            return WindowsFeatureStatus.Enabled;
        }

        return WindowsFeatureStatus.Disabled;
    }
}