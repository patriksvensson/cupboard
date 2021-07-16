using System;
using System.IO;
using System.Threading.Tasks;
using CliWrap.EventStream;
using Spectre.IO;

namespace Cupboard
{
    public sealed class PowerShellProvider : AsyncWindowsResourceProvider<PowerShellScript>
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

        public override PowerShellScript Create(string name)
        {
            return new PowerShellScript(name);
        }

        public override async Task<ResourceState> RunAsync(IExecutionContext context, PowerShellScript resource)
        {
            if (resource.ScriptPath == null)
            {
                _logger.Error("Script path has not been set");
                return ResourceState.Error;
            }

            var path = resource.ScriptPath.MakeAbsolute(_environment);
            if (!_fileSystem.Exist(path))
            {
                _logger.Error("Script path does not exist");
                return ResourceState.Error;
            }

            if (resource.Unless != null)
            {
                _logger.Debug("Evaluating 'Unless' condition");
                if (await RunPowerShell(resource.Unless).ConfigureAwait(false) != 0)
                {
                    _logger.Verbose("Skipping Powershell script since condition did not evaluate to 0 (zero)");
                    return ResourceState.Skipped;
                }
            }

            if (!context.DryRun)
            {
                if (await RunPowerShell(path).ConfigureAwait(false) != 0)
                {
                    _logger.Error("Powershell script exited with an unexpected exit code");
                    return ResourceState.Error;
                }

                _logger.Debug("Refreshing environment variables for user");
                _refresher.Refresh();
            }

            return ResourceState.Executed;
        }

        private async Task<int> RunPowerShell(string command)
        {
            var path = _environment.GetTempFilePath().ChangeExtension("ps1");

            try
            {
                // Create file on disk
                using (var stream = _fileSystem.File.OpenWrite(path))
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(command);
                }

                return await RunPowerShell(path);
            }
            finally
            {
                if (_fileSystem.Exist(path))
                {
                    _fileSystem.File.Delete(path);
                }
            }
        }

        private async Task<int> RunPowerShell(FilePath path)
        {
            var result = await _runner.Run("powershell.exe", $"–noprofile & '{path.FullPath}'", @event =>
            {
                if (@event is StandardOutputCommandEvent output)
                {
                    if (!string.IsNullOrWhiteSpace(output.Text))
                    {
                        _logger.Verbose($"OUT> {output.Text}");
                    }
                }
                else if (@event is StandardErrorCommandEvent error)
                {
                    if (!string.IsNullOrWhiteSpace(error.Text))
                    {
                        _logger.Verbose($"ERR> {error.Text}");
                    }
                }
            });

            return result.ExitCode;
        }
    }
}
