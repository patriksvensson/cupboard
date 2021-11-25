using System;

namespace Cupboard;

public static class IWindowsRegistryExtensions
{
    public static IWindowsRegistryKey? GetKey(this IWindowsRegistry registry, RegistryPath path, bool writable)
    {
        if (registry is null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        var root = path.Hive switch
        {
            RegistryHive.ClassesRoot => registry.ClassesRoot,
            RegistryHive.CurrentUser => registry.CurrentUser,
            RegistryHive.LocalMachine => registry.LocalMachine,
            RegistryHive.Users => registry.Users,
            RegistryHive.CurrentConfig => registry.CurrentConfig,
            RegistryHive.PerformanceData => registry.PerformanceData,
            _ => throw new InvalidOperationException("Unknown registry root"),
        };

        return root.OpenSubKey(path.SubKey, writable);
    }
}