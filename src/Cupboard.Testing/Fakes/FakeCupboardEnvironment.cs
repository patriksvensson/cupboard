using System.Collections.Generic;
using Spectre.IO;
using Spectre.IO.Testing;

namespace Cupboard.Testing
{
    public sealed class FakeCupboardEnvironment : ICupboardEnvironment
    {
        private readonly FakeEnvironment _environment;

        public DirectoryPath WorkingDirectory => _environment.WorkingDirectory;
        public DirectoryPath HomeDirectory => _environment.HomeDirectory;
        public IPlatform Platform => _environment.Platform;

        public FakeCupboardEnvironment(PlatformFamily family, bool is64Bit = true)
        {
            _environment = new FakeEnvironment(family, is64Bit);
        }

        public static FakeCupboardEnvironment CreateUnixEnvironment(bool is64Bit = true)
        {
            return new FakeCupboardEnvironment(PlatformFamily.Linux, is64Bit);
        }

        public static FakeCupboardEnvironment CreateWindowsEnvironment(bool is64Bit = true)
        {
            return new FakeCupboardEnvironment(PlatformFamily.Windows, is64Bit);
        }

        public string? GetEnvironmentVariable(string variable)
        {
            return _environment.GetEnvironmentVariable(variable);
        }

        public IDictionary<string, string?> GetEnvironmentVariables()
        {
            return _environment.GetEnvironmentVariables();
        }

        public FilePath GetTempFilePath()
        {
            return new FilePath("C:/Temp/fake.ps1").MakeAbsolute(_environment);
        }

        public void SetWorkingDirectory(DirectoryPath path)
        {
            _environment.SetWorkingDirectory(path);
        }
    }
}
