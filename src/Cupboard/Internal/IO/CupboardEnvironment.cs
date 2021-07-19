using System.Collections.Generic;
using Spectre.IO;

namespace Cupboard.Internal
{
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
