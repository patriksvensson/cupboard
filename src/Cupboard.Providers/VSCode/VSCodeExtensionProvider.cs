using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CliWrap;

namespace Cupboard
{
    public sealed class VSCodeExtensionProvider : AsyncResourceProvider<VSCodeExtension>
    {
        private readonly ICupboardLogger _logger;
        private string? _cachedOutput;
        private bool _dirty;

        private enum VSCodeExtensionState
        {
            Exists,
            Missing,
            Error,
        }

        public VSCodeExtensionProvider(ICupboardLogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dirty = true;
        }

        public override VSCodeExtension Create(string name)
        {
            return new VSCodeExtension(name)
            {
                Package = name,
            };
        }

        public override async Task<ResourceState> RunAsync(IExecutionContext context, VSCodeExtension resource)
        {
            var state = await IsPackageInstalled(resource.Package).ConfigureAwait(false);
            if (state == VSCodeExtensionState.Error)
            {
                return ResourceState.Error;
            }

            if (resource.Ensure == PackageState.Installed)
            {
                if (state == VSCodeExtensionState.Missing)
                {
                    if (context.DryRun)
                    {
                        return ResourceState.Changed;
                    }

                    // Install package
                    state = await InstallPackage(resource.Package).ConfigureAwait(false);
                    if (state == VSCodeExtensionState.Exists)
                    {
                        _logger.Information($"The VSCode extension [yellow]{resource.Package}[/] was installed");
                        return ResourceState.Changed;
                    }

                    return ResourceState.Error;
                }

                _logger.Debug($"The VSCode extension [yellow]{resource.Package}[/] is already installed");
            }
            else
            {
                if (state == VSCodeExtensionState.Exists)
                {
                    if (context.DryRun)
                    {
                        return ResourceState.Changed;
                    }

                    // Uninstall package
                    state = await UninstallPackage(resource.Package).ConfigureAwait(false);
                    if (state == VSCodeExtensionState.Missing)
                    {
                        _logger.Information($"The VSCode extension [yellow]{resource.Package}[/] was uninstalled");
                        return ResourceState.Changed;
                    }

                    return ResourceState.Error;
                }

                _logger.Debug($"The VSCode extension [yellow]{resource.Package}[/] is already uninstalled");
            }

            return ResourceState.Unchanged;
        }

        private async Task<VSCodeExtensionState> InstallPackage(string package)
        {
            _logger.Debug($"Installing VSCode extension [yellow]{package}[/]");

            var stdout = new StringBuilder();
            var result = await Cli.Wrap(GetCodeExecutable())
                .WithArguments($"--install-extension {package}")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdout))
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

            if (result.ExitCode != 0)
            {
                _logger.Error("Exit code: " + result.ExitCode);
                return VSCodeExtensionState.Error;
            }

            _dirty = true;
            return await IsPackageInstalled(package).ConfigureAwait(false);
        }

        private async Task<VSCodeExtensionState> UninstallPackage(string package)
        {
            _logger.Debug($"Uninstalling VSCode extension [yellow]{package}[/]");

            var stdout = new StringBuilder();
            var result = await Cli.Wrap(GetCodeExecutable())
                .WithArguments($"---uninstall-extension {package}")
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdout))
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

            if (result.ExitCode != 0)
            {
                return VSCodeExtensionState.Error;
            }

            _dirty = true;
            return await IsPackageInstalled(package).ConfigureAwait(false);
        }

        private async Task<VSCodeExtensionState> IsPackageInstalled(string package)
        {
            if (_dirty || _cachedOutput == null)
            {
                var stdout = new StringBuilder();
                var result = await Cli.Wrap(GetCodeExecutable())
                    .WithArguments("--list-extensions")
                    .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdout))
                    .WithValidation(CommandResultValidation.None)
                    .ExecuteAsync();

                if (result.ExitCode != 0)
                {
                    return VSCodeExtensionState.Error;
                }

                _cachedOutput = stdout.ToString();
                _dirty = false;
            }

            if (_cachedOutput == null)
            {
                _logger.Error("An error occured while retrieving VSCode extension state");
                return VSCodeExtensionState.Error;
            }

            if (_cachedOutput.Contains(package, StringComparison.OrdinalIgnoreCase))
            {
                return VSCodeExtensionState.Exists;
            }

            return VSCodeExtensionState.Missing;
        }

        private static string GetCodeExecutable()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // TODO 2021-07-14: This is not good
                return "C:/Program Files/Microsoft VS Code/bin/code.cmd";
            }
            else
            {
                return "code";
            }
        }
    }
}
