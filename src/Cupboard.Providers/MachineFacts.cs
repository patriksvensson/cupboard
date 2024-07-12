namespace Cupboard;

internal sealed class MachineFacts : IFactProvider
{
    private readonly IPlatform _platform;

    internal static class Constants
    {
        public const string OSArchitecture = "os.arch";
        public const string OSPlatform = "os.platform";
        public const string MachineName = "machine.name";
        public const string ComputerName = "computer.name";
        public const string UserName = "user.name";
    }

    public MachineFacts(IPlatform platform)
    {
        _platform = platform ?? throw new ArgumentNullException(nameof(platform));
    }

    IEnumerable<(string Name, object Value)> IFactProvider.GetFacts(IRemainingArguments args)
    {
        yield return (Constants.OSArchitecture, GetArchitecture());
        yield return (Constants.OSPlatform, GetPlatform());
        yield return (Constants.MachineName, System.Environment.MachineName);
        yield return (Constants.ComputerName, System.Environment.MachineName);
        yield return (Constants.UserName, System.Environment.UserName);
    }

    private OSArchitecture GetArchitecture()
    {
        return _platform.Architecture switch
        {
            PlatformArchitecture.X86 => OSArchitecture.X86,
            PlatformArchitecture.X64 => OSArchitecture.X64,
            PlatformArchitecture.Arm => OSArchitecture.ARM,
            PlatformArchitecture.Arm64 => OSArchitecture.ARM64,
            _ => throw new InvalidOperationException("Unknown OS architecture"),
        };
    }

    private OSPlatform GetPlatform()
    {
        return _platform.Family switch
        {
            PlatformFamily.Windows => OSPlatform.Windows,
            PlatformFamily.Linux => OSPlatform.Linux,
            PlatformFamily.MacOs => OSPlatform.MacOS,
            PlatformFamily.FreeBSD => OSPlatform.FreeBSD,
            _ => throw new InvalidOperationException("Unknown OS platform"),
        };
    }
}
