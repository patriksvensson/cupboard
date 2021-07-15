using System;
using System.Linq;
using System.Threading.Tasks;
using CliWrap;
using Spectre.IO;

namespace Cupboard
{
    public sealed class ExecProvider : AsyncResourceProvider<Exec>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IEnvironment _environment;
        private readonly IEnvironmentRefresher _refresher;
        private readonly ICupboardLogger _logger;

        public ExecProvider(
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

        public override Exec Create(string name)
        {
            return new Exec(name);
        }

        public override async Task<ResourceState> RunAsync(IExecutionContext context, Exec resource)
        {
            var args = resource.Args ?? string.Empty;

            var path = resource.Path.MakeAbsolute(_environment);
            if (!_fileSystem.Exist(path))
            {
                _logger.Error($"The file {path.FullPath} could not be found");
                return ResourceState.Error;
            }

            if (context.DryRun)
            {
                return ResourceState.Unknown;
            }

            _logger.Debug($"Executing [yellow]{path.FullPath}[/] {resource.Args}".TrimEnd());
            var result = await Cli.Wrap(path.FullPath)
                .WithValidation(CommandResultValidation.None)
                .WithArguments(args)
                .ExecuteAsync();

            if (result.ExitCode != 0)
            {
                if (resource.ValidExitCodes?.Contains(result.ExitCode) == true)
                {
                    _refresher.Refresh();
                    return ResourceState.Changed;
                }

                _logger.Error($"The file {path.FullPath} returned exit code {result.ExitCode}");
                return ResourceState.Error;
            }

            return ResourceState.Changed;
        }
    }
}
