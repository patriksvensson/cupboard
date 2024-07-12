namespace Cupboard;

[PublicAPI]
public static class MachineFactsExtensions
{
    public static OSPlatform Platform(this FactCollection facts)
    {
        return facts[MachineFacts.Constants.OSPlatform].As<OSPlatform>();
    }

    public static OSArchitecture Architecture(this FactCollection facts)
    {
        return facts[MachineFacts.Constants.OSArchitecture].As<OSArchitecture>();
    }

    public static string MachineName(this FactCollection facts)
    {
        return facts[MachineFacts.Constants.MachineName];
    }

    public static string ComputerName(this FactCollection facts)
    {
        return facts[MachineFacts.Constants.ComputerName];
    }

    public static string UserName(this FactCollection facts)
    {
        return facts[MachineFacts.Constants.UserName];
    }

    public static bool IsWindows(this FactCollection facts)
    {
        return Platform(facts) == OSPlatform.Windows;
    }

    public static bool IsLinux(this FactCollection facts)
    {
        return Platform(facts) == OSPlatform.Linux;
    }

    public static bool IsMacOS(this FactCollection facts)
    {
        return Platform(facts) == OSPlatform.MacOS;
    }

    public static bool IsFreeBSD(this FactCollection facts)
    {
        return Platform(facts) == OSPlatform.FreeBSD;
    }

    public static bool IsX86(this FactCollection facts)
    {
        return Architecture(facts) is OSArchitecture.X86;
    }

    public static bool IsX64(this FactCollection facts)
    {
        return Architecture(facts) is OSArchitecture.X64;
    }

    public static bool IsX86OrX64(this FactCollection facts)
    {
        return IsX86(facts) || IsX64(facts);
    }

    public static bool IsArm(this FactCollection facts)
    {
        return Architecture(facts) is OSArchitecture.ARM or OSArchitecture.ARM64;
    }

    public static bool IsArm64(this FactCollection facts)
    {
        return Architecture(facts) is OSArchitecture.ARM64;
    }

    public static bool IsArmOrArm64(this FactCollection facts)
    {
        return IsArm(facts) || IsArm64(facts);
    }
}
