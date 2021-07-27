using System;
using Spectre.IO;
using FileAttributes = System.IO.FileAttributes;

namespace Cupboard
{
    public sealed class FileProvider : ResourceProvider<File>
    {
        private readonly ICupboardFileSystem _fileSystem;
        private readonly ICupboardEnvironment _environment;
        private readonly ICupboardLogger _logger;

        public FileProvider(
            ICupboardFileSystem fileSystem,
            ICupboardEnvironment environment,
            ICupboardLogger logger)
        {
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override File Create(string name)
        {
            return new File(name)
            {
                Destination = new FilePath(name),
            };
        }

        public override ResourceState Run(IExecutionContext context, File resource)
        {
            if (resource.Destination == null)
            {
                return ResourceState.Error;
            }

            var destination = resource.Destination.MakeAbsolute(_environment);

            return resource.Ensure switch
            {
                FileState.Present => EnsurePresent(resource, destination),
                FileState.Absent => EnsureAbsent(destination),
                _ => ResourceState.Error,
            };
        }

        private ResourceState EnsurePresent(File resource, FilePath destination)
        {
            // Make sure that the destination directory exist.
            var root = destination.GetDirectory();
            if (!_fileSystem.Exist(root))
            {
                _logger.Debug($"Creating root directory {root.FullPath}");
                _fileSystem.Directory.Create(root);
                if (!_fileSystem.Exist(root))
                {
                    // Could not create the root directory
                    return ResourceState.Error;
                }
            }

            if (resource.Source != null)
            {
                var source = resource.Source.MakeAbsolute(_environment);

                // Make sure the source file exist
                if (!_fileSystem.Exist(source))
                {
                    _logger.Error("Source file does not exist");
                    return ResourceState.Error;
                }

                if (resource.SymbolicLink)
                {
                    if (_fileSystem.Exist(destination))
                    {
                        var file = _fileSystem.GetFile(destination);
                        if ((file.Attributes & FileAttributes.ReparsePoint) != 0)
                        {
                            // TODO 2021-07-13: Are the symlinks the same?
                            return ResourceState.Unchanged;
                        }

                        // Delete the file
                        file.Delete();
                        if (_fileSystem.Exist(destination))
                        {
                            // We could not delete the file
                            _logger.Error("Could not delete destination file");
                            return ResourceState.Error;
                        }
                    }

                    // Create symbolic link
                    if (!_fileSystem.File.CreateSymbolicLinkSafe(source, destination))
                    {
                        _logger.Error("Could not create symbolic link");
                        return ResourceState.Error;
                    }

                    _logger.Information($"Created symbolic link to {destination}");
                    return ResourceState.Changed;
                }
                else
                {
                    if (_fileSystem.Exist(destination))
                    {
                        // TODO 2021-07-13: Are the files the same?
                        return ResourceState.Unchanged;
                    }

                    // Copy the file
                    var file = _fileSystem.GetFile(source);
                    file.Copy(destination, true);
                    if (!_fileSystem.Exist(destination))
                    {
                        // We could not copy the file
                        _logger.Error("Could not copy file");
                        return ResourceState.Error;
                    }

                    // Not on Windows and got permissions set?
                    if (resource.Permissions != null && _environment.Platform.Family != PlatformFamily.Windows)
                    {
                        destination.SetPermissions(resource.Permissions);
                    }

                    return ResourceState.Changed;
                }
            }
            else
            {
                var file = _fileSystem.GetFile(destination);
                using (var stream = file.OpenWrite())
                {
                    if (!_fileSystem.Exist(destination))
                    {
                        // We could not create the file
                        _logger.Error("Could not create destination file");
                        return ResourceState.Error;
                    }
                }

                return ResourceState.Changed;
            }
        }

        private ResourceState EnsureAbsent(FilePath destination)
        {
            var file = _fileSystem.GetFile(destination);
            if (!file.Exists)
            {
                return ResourceState.Unchanged;
            }

            file.Delete();
            file.Refresh();

            if (file.Exists)
            {
                _logger.Error("Could not delete destination file");
                return ResourceState.Error;
            }

            return ResourceState.Changed;
        }
    }
}
