using System;
using System.Collections.Generic;
using System.Threading;
using Spectre.IO;
using Spectre.IO.Testing;

namespace Cupboard.Testing
{
    public sealed class FakeCupboardEnvironment : ICupboardEnvironment
    {
        private readonly FakeEnvironment _environment;
        private readonly object _lock;
        private int _counter;

        public DirectoryPath WorkingDirectory => _environment.WorkingDirectory;
        public DirectoryPath HomeDirectory => _environment.HomeDirectory;
        public IPlatform Platform => _environment.Platform;

        public FakeCupboardEnvironment(PlatformFamily family, PlatformArchitecture architecture = PlatformArchitecture.X64)
        {
            _environment = new FakeEnvironment(family, architecture);
            _lock = new object();
            _counter = 0;
        }

        public static FakeCupboardEnvironment CreateWindowsEnvironment(PlatformArchitecture architecture = PlatformArchitecture.X64)
        {
            return new FakeCupboardEnvironment(PlatformFamily.Windows, architecture);
        }

        public static FakeCupboardEnvironment CreateLinuxEnvironment(PlatformArchitecture architecture = PlatformArchitecture.X64)
        {
            return new FakeCupboardEnvironment(PlatformFamily.Linux, architecture);
        }

        public static FakeCupboardEnvironment CreateMacOSEnvironment(PlatformArchitecture architecture = PlatformArchitecture.X64)
        {
            return new FakeCupboardEnvironment(PlatformFamily.MacOs, architecture);
        }

        public static FakeCupboardEnvironment CreateFreeBSDEnvironment(PlatformArchitecture architecture = PlatformArchitecture.X64)
        {
            return new FakeCupboardEnvironment(PlatformFamily.FreeBSD, architecture);
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
            lock (_lock)
            {
                var filename = Interlocked.Increment(ref _counter)
                    .ToString()
                    .PadLeft(5, '0');

                var path = _environment.Platform.Family switch
                {
                    PlatformFamily.Windows => new FilePath($"C:/Temp/{filename}.tmp"),
                    PlatformFamily.Linux => new FilePath($"/tmp/{filename}.tmp"),
                    PlatformFamily.MacOs => new FilePath($"/private/tmp/{filename}.tmp"),
                    PlatformFamily.FreeBSD => new FilePath($"/tmp/{filename}.tmp"),
                    _ => throw new InvalidOperationException("Unknown platform family"),
                };

                return path.MakeAbsolute(_environment);
            }
        }

        public void SetWorkingDirectory(DirectoryPath path)
        {
            _environment.SetWorkingDirectory(path);
        }
    }
}
