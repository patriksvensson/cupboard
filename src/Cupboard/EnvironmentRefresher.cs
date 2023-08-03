using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Cupboard.Internal;

internal class EnvironmentRefresher : IEnvironmentRefresher
{
    public void Refresh()
    {
        if (!RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
        {
            return;
        }

        var roots = new[]
        {
            Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment"),
            Registry.CurrentUser.OpenSubKey("Environment"),
        };

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var root in roots)
        {
            if (root == null)
            {
                continue;
            }

            var keys = root.GetValueNames();
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    if (key.Equals("PATH", StringComparison.OrdinalIgnoreCase))
                    {
                        result.TryGetValue(key, out var path);
                        path ??= string.Empty;
                        result[key] = (path + ";" + root.GetValue(key)?.ToString() ?? string.Empty).TrimStart(';');
                    }
                    else
                    {
                        result[key] = root.GetValue(key)?.ToString() ?? string.Empty;
                    }
                }
            }
        }

        foreach (var envVar in result)
        {
            System.Environment.SetEnvironmentVariable(envVar.Key, envVar.Value, EnvironmentVariableTarget.Process);
        }
    }
}
