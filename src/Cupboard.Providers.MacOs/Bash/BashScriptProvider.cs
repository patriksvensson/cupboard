using CliWrap;
using CliWrap.EventStream;
using Spectre.IO;

namespace Cupboard
{
    public sealed class BashScriptProvider : AsyncMacResourceProvider<BashScript>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IEnvironment _environment;
        private readonly IEnvironmentRefresher _refresher;
        private readonly ICupboardLogger _logger;

        public BashScriptProvider(
            IFileSystem fileSystem,
            IEnvironment environment,
            IEnvironmentRefresher refresher,
            ICupboardLogger logger)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _refresher = refresher ?? throw new ArgumentNullException(nameof(refresher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override BashScript Create(string name)
        {
            return new BashScript(name);
        }

        public override async Task<ResourceState> RunAsync(IExecutionContext context, BashScript resource)
        {
            if (resource.Script == null)
            {
                _logger.Error("Script path has not been set");
                return ResourceState.Error;
            }

            var path = resource.Script.MakeAbsolute(_environment);
            if (!_fileSystem.Exist(path))
            {
                _logger.Error("Script path does not exist");
                return ResourceState.Error;
            }

            if (resource.Unless != null)
            {
                _logger.Debug("Evaluating condition");
                if (await RunBash(resource.Unless).ConfigureAwait(false) != 0)
                {
                    return ResourceState.Skipped;
                }
            }

            if (!context.DryRun)
            {
                var content = await GetScriptContent(path).ConfigureAwait(false);
                if (await RunBash(content).ConfigureAwait(false) != 0)
                {
                    _logger.Error("Bash script exited with an unexpected exit code");
                    return ResourceState.Error;
                }

                _logger.Debug("Refreshing environment variables for user");
                _refresher.Refresh();
            }

            return ResourceState.Unchanged;
        }

        private async Task<string> GetScriptContent(FilePath path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            await using var stream = _fileSystem.File.OpenRead(path);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

        private async Task<int> RunBash(string content)
        {
            // Get temporary location
            var path = new FilePath(System.IO.Path.GetTempFileName()).ChangeExtension("sh");
            await using (var stream = _fileSystem.File.OpenWrite(path))
            await using (var writer = new StreamWriter(stream))
            {
                writer.Write(content);
            }

            try
            {
                // Create file with content
                var result = Cli.Wrap("/bin/zsh")
                    .WithValidation(CommandResultValidation.None);

                var first = true;
                await foreach (var cmdEvent in result.ListenAsync())
                {
                    switch (cmdEvent)
                    {
                        case StandardOutputCommandEvent stdOut:
                            if (first)
                            {
                                first = false;
                                _logger.Verbose("--------------------------------");
                            }

                            if (!string.IsNullOrWhiteSpace(stdOut.Text))
                            {
                                _logger.Verbose(stdOut.Text);
                            }

                            break;
                        case StandardErrorCommandEvent stdErr:
                            if (first)
                            {
                                first = false;
                                _logger.Verbose("--------------------------------");
                            }

                            _logger.Error(stdErr.Text);
                            break;
                        case ExitedCommandEvent exited:
                            if (!first)
                            {
                                _logger.Verbose("--------------------------------");
                            }

                            return exited.ExitCode;
                    }
                }

                throw new InvalidOperationException("An error occured while executing Bash script");
            }
            finally
            {
                _fileSystem.File.Delete(path);
            }
        }
    }
}
