using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Management.Infrastructure;
using Spectre.Console.Cli;

namespace Cupboard
{
    internal sealed class WmiFacts : IFactProvider
    {
        private static readonly Dictionary<string, string> _properties;

        static WmiFacts()
        {
            _properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Version", "wmi.os.version" },
                { "WindowsDirectory", "wmi.os.windows_dir" },
                { "FreeVirtualMemory", "wmi.os.free_virtual_mem" },
                { "FreePhysicalMemory", "wmi.os.free_physical_mem" },
                { "BuildNumber", "wmi.os.build" },
                { "TotalVirtualMemorySize", "wmi.os.total_virtual_mem" },
                { "TotalVisibleMemorySize", "wmi.os.total_mem" },
            };
        }

        public IEnumerable<(string Name, object Value)> GetFacts(IRemainingArguments args)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                yield break;
            }

            var cimSession = CimSession.Create(null);
            var inst = cimSession.GetInstance("root\\cimv2", new CimInstance("Win32_OperatingSystem", "root\\cimv2"));

            foreach (var prop in inst.CimInstanceProperties)
            {
                if (_properties.ContainsKey(prop.Name))
                {
                    yield return (_properties[prop.Name], prop.Value);
                }
            }

            if (inst.CimInstanceProperties.Count > 0)
            {
                yield return ("wmi.os", true);
            }
        }
    }
}
