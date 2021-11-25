using System;
using System.IO;
using System.Threading.Tasks;
using Spectre.IO;

namespace Cupboard;

public sealed class PowerShellProvider : AsyncResourceProvider<PowerShell>
{
    private readonly ICupboardFileSystem _fileSystem;
    private readonly ICupboardEnvironment _environment;
    private readonly IProcessRunner _runner;
    private readonly IEnvironmentRefresher _refresher;
    private readonly ICupboardLogger _logger;

    public PowerShellProvider(
        ICupboardFileSystem fileSystem,
        ICupboardEnvironment environment,
        IProcessRunner runner,
        IEnvironmentRefresher refresher,
        ICupboardLogger logger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        _refresher = refresher ?? throw new ArgumentNullException(nameof(refresher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override PowerShell Create(string name)
    {
        return new PowerShell(name);
    }

    public override async Task<ResourceState> RunAsync(IExecutionContext context, PowerShell resource)
    {
        if (resource.Unless != null)
        {
            _logger.Debug($"Evaluating PowerShell condition: {resource.Unless}");
            if (await RunPowerShell(context, resource.Flavor, resource.Unless).ConfigureAwait(false) != 0)
            {
                _logger.Verbose("Skipping PowerShell script since condition did not evaluate to 0 (zero)");
                return ResourceState.Skipped;
            }
        }

        if (!context.DryRun)
        {
            if (resource.Script != null)
            {
                // Script
                var path = resource.Script.MakeAbsolute(_environment);
                if (!_fileSystem.Exist(path))
                {
                    _logger.Error("PowerShell script path does not exist");
                    return ResourceState.Error;
                }

                _logger.Debug($"Running PowerShell script [yellow]{path}[/]");
                if (await RunPowerShell(context, resource.Flavor, path).ConfigureAwait(false) != 0)
                {
                    _logger.Error("Powershell script exited with an unexpected exit code");
                    return ResourceState.Error;
                }
            }
            else if (resource.Command != null)
            {
                // Command
                _logger.Debug($"Running PowerShell command: [yellow]{resource.Command}[/]");
                if (await RunPowerShell(context, resource.Flavor, resource.Command).ConfigureAwait(false) != 0)
                {
                    _logger.Error("Powershell script exited with an unexpected exit code");
                    return ResourceState.Error;
                }
            }
            else
            {
                _logger.Error("PowerShell Command or script path has not been set");
                return ResourceState.Error;
            }

            _logger.Debug("Refreshing environment variables for user");
            _refresher.Refresh();
        }

        return ResourceState.Executed;
    }

    private async Task<int> RunPowerShell(IExecutionContext context, PowerShellFlavor flavor, string command)
    {
        var path = _environment
            .GetTempFilePath()
            .ChangeExtension("ps1")
            .MakeAbsolute(_environment);

        try
        {
            // Create file on disk
            using (var stream = _fileSystem.File.OpenWrite(path))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(command);
            }

            return await RunPowerShell(context, flavor, path).ConfigureAwait(false);
        }
        finally
        {
            if (_fileSystem.Exist(path))
            {
                _fileSystem.File.Delete(path);
            }
        }
    }

    private async Task<int> RunPowerShell(IExecutionContext context, PowerShellFlavor flavor, FilePath path)
    {
        var executable = GetPowerShellExecutable(context, flavor);
        var args = flavor switch
        {
            PowerShellFlavor.PowerShell => $"–noprofile & '{path.FullPath}'",
            PowerShellFlavor.PowerShellCore => $"–noprofile \"{path.FullPath}\"",
            _ => throw new InvalidOperationException($"Unknown PowerShell provider '{flavor}'"),
        };

        var result = await _runner.Run(executable, args).ConfigureAwait(false);

        return result.ExitCode;
    }

    private static string GetPowerShellExecutable(IExecutionContext context, PowerShellFlavor flavor)
    {
        if (context.Facts.IsWindows())
        {
            return flavor switch
            {
                PowerShellFlavor.PowerShell => "powershell.exe",
                PowerShellFlavor.PowerShellCore => "pwsh.exe",
                _ => throw new InvalidOperationException($"Unknown PowerShell provider '{flavor}'"),
            };
        }
        else
        {
            return "pwsh";
        }
    }
}