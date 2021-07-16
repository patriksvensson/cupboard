using System.Collections.Generic;
using Spectre.IO;

namespace Cupboard
{
    public interface ICupboardFileSystem : IFileSystem
    {
    }

    public interface ICupboardEnvironment : IEnvironment
    {
        FilePath GetTempFilePath();
    }

    internal sealed class CupboardFileSystem : ICupboardFileSystem
    {
        private readonly IFileSystem _fileSystem;

        public IFileProvider File => _fileSystem.File;
        public IDirectoryProvider Directory => _fileSystem.Directory;

        public CupboardFileSystem()
        {
            _fileSystem = new FileSystem();
        }
    }

    internal sealed class CupboardEnvironment : ICupboardEnvironment
    {
        private readonly IEnvironment _environment;

        public DirectoryPath WorkingDirectory => _environment.WorkingDirectory;
        public DirectoryPath HomeDirectory => _environment.HomeDirectory;
        public IPlatform Platform => _environment.Platform;

        public CupboardEnvironment(IPlatform platform)
        {
            _environment = new Spectre.IO.Environment(platform);
        }

        public string? GetEnvironmentVariable(string variable)
        {
            return _environment.GetEnvironmentVariable(variable);
        }

        public IDictionary<string, string?> GetEnvironmentVariables()
        {
            return _environment.GetEnvironmentVariables();
        }

        public void SetWorkingDirectory(DirectoryPath path)
        {
            _environment.SetWorkingDirectory(path);
        }

        public FilePath GetTempFilePath()
        {
            return new FilePath(System.IO.Path.GetTempFileName()).MakeAbsolute(this);
        }
    }
}
