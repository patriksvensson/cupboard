using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Spectre.Console.Cli;
using Platform = System.Runtime.InteropServices.OSPlatform;

namespace Cupboard
{
    internal sealed class MachineFacts : IFactProvider
    {
        IEnumerable<(string Name, object Value)> IFactProvider.GetFacts(IRemainingArguments args)
        {
            yield return ("os.arch", RuntimeInformation.OSArchitecture);
            yield return ("os.platform", GetOSPlatform());
            yield return ("machine.name", Environment.MachineName);
            yield return ("computer.name", Environment.MachineName);
            yield return ("user.name", Environment.UserName);
        }

        private static OSPlatform GetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                return OSPlatform.FreeBSD;
            }
            else
            {
                throw new InvalidOperationException("Unknown OS platform");
            }
        }
    }

    public static partial class MachineFactsExtensions
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
