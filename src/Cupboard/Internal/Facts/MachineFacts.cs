using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Spectre.Console.Cli;

namespace Cupboard.Internal
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
}
