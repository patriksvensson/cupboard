using System.Runtime.InteropServices;
using Platform = System.Runtime.InteropServices.OSPlatform;

namespace Cupboard
{
    public static class FactsExtensions
    {
        public static Platform OSPlatform(this FactCollection facts)
        {
            return facts["os"]["platform"].As<Platform>();
        }

        public static Architecture OSArchitecture(this FactCollection facts)
        {
            return facts["os"]["arch"].As<Architecture>();
        }

        public static bool IsWindows(this FactCollection facts)
        {
            return OSPlatform(facts) == Platform.Windows;
        }

        public static bool IsLinux(this FactCollection facts)
        {
            return OSPlatform(facts) == Platform.Linux;
        }

        public static bool IsMacOS(this FactCollection facts)
        {
            return OSPlatform(facts) == Platform.OSX;
        }

        public static bool IsFreeBSD(this FactCollection facts)
        {
            return OSPlatform(facts) == Platform.FreeBSD;
        }

        public static bool IsX86(this FactCollection facts)
        {
            return OSArchitecture(facts) is Architecture.X86;
        }

        public static bool IsX64(this FactCollection facts)
        {
            return OSArchitecture(facts) is Architecture.X64;
        }

        public static bool IsX86OrX64(this FactCollection facts)
        {
            return IsX86(facts) || IsX64(facts);
        }

        public static bool IsArm(this FactCollection facts)
        {
            return OSArchitecture(facts) is Architecture.Arm or Architecture.Arm64;
        }

        public static bool IsArm64(this FactCollection facts)
        {
            return OSArchitecture(facts) is Architecture.Arm64;
        }

        public static bool IsArmOrArm64(this FactCollection facts)
        {
            return IsX86(facts) || IsX64(facts);
        }
    }
}
