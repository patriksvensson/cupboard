using System;
using System.Text;
using System.Threading.Tasks;
using CliWrap;

namespace Cupboard
{
    public sealed class WindowsFeatureProvider : AsyncWindowsResourceProvider<WindowsFeature>
    {
        private readonly IEnvironmentRefresher _refresher;
        private readonly ICupboardLogger _logger;

        private enum WindowsFeatureStatus
        {
            Enabled,
            Disabled,
            Error,
        }

        public WindowsFeatureProvider(IEnvironmentRefresher refresher, ICupboardLogger logger)
        {
            _refresher = refresher;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override WindowsFeature Create(string name)
        {
            return new WindowsFeature(name)
            {
                FeatureName = name,
            };
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

                var result = await Cli.Wrap("dism.exe")
                    .WithValidation(CommandResultValidation.None)
                    .WithArguments($"/online /enable-feature /featurename:{feature}")
                    .ExecuteAsync();

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

                var result = await Cli.Wrap("dism.exe")
                    .WithValidation(CommandResultValidation.None)
                    .WithArguments($"/online /disable-feature /featurename:{feature}")
                    .ExecuteAsync();

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

        private static async Task<WindowsFeatureStatus> IsFeatureEnabled(string feature)
        {
            var stdout = new StringBuilder();
            var result = await Cli.Wrap("dism.exe")
                .WithArguments($"/online /Get-FeatureInfo /FeatureName:{feature}")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdout))
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

            if (result.ExitCode != 0)
            {
                return WindowsFeatureStatus.Error;
            }

            if (stdout.ToString().Contains("State : Enabled", StringComparison.OrdinalIgnoreCase))
            {
                return WindowsFeatureStatus.Enabled;
            }

            return WindowsFeatureStatus.Disabled;
        }
    }
}
