namespace Cupboard;

public sealed class DirectoryProvider : ResourceProvider<Directory>
{
    private readonly ICupboardFileSystem _fileSystem;
    private readonly ICupboardEnvironment _environment;
    private readonly ICupboardLogger _logger;
    private readonly IDictionary<DirectoryState, Func<IExecutionContext, Directory, ResourceState>> _map;

    public DirectoryProvider(
        ICupboardFileSystem fileSystem,
        ICupboardEnvironment environment,
        ICupboardLogger logger)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _map = new Dictionary<DirectoryState, Func<IExecutionContext, Directory, ResourceState>>
        {
            { DirectoryState.Present, CreateDirectory },
            { DirectoryState.Absent, DeleteDirectory },
        };
    }

    public override Directory Create(string name)
    {
        return new Directory(name);
    }

    public override ResourceState Run(IExecutionContext context, Directory resource)
    {
        if (_map.TryGetValue(resource.Ensure, out var action))
        {
            return action.Invoke(context, resource);
        }

        return ResourceState.Unknown;
    }

    private ResourceState DeleteDirectory(IExecutionContext context, Directory resource)
    {
        if (resource.Path == null)
        {
            _logger.Error($"The resource '{resource.Name}' does not have a path");
            return ResourceState.Error;
        }

        var path = resource.Path.MakeAbsolute(_environment);
        if (_fileSystem.Exist(path))
        {
            if (!context.DryRun)
            {
                _logger.Information($"Deleting directory '{path.FullPath}'.");
                _fileSystem.Directory.Delete(path, true);
            }

            return ResourceState.Changed;
        }
        else
        {
            _logger.Information($"Directory '{path.FullPath}' does not exist.");
            return ResourceState.Unchanged;
        }
    }

    private ResourceState CreateDirectory(IExecutionContext context, Directory resource)
    {
        if (resource.Path == null)
        {
            _logger.Error($"The resource '{resource.Name}' does not have a path");
            return ResourceState.Error;
        }

        var path = resource.Path.MakeAbsolute(_environment);

        if (!_fileSystem.Exist(path))
        {
            if (!context.DryRun)
            {
                _logger.Information($"Creating directory '{path.FullPath}'.");
                _fileSystem.Directory.Create(path);

                // Not on Windows and got permissions set?
                if (resource.Permissions != null && _environment.Platform.Family != PlatformFamily.Windows)
                {
                    path.SetPermissions(resource.Permissions);
                }
            }

            return ResourceState.Changed;
        }
        else
        {
            _logger.Information($"Directory '{path.FullPath}' already exists.");
            return ResourceState.Unchanged;
        }
    }
}
