using System;
using System.Collections.Generic;
using System.Linq;

namespace Cupboard;

public sealed class RegistryPath
{
    private static readonly Dictionary<string, string> _rootSubstitutions;
    private static readonly Dictionary<string, RegistryHive> _roots;

    public RegistryHive Hive { get; set; }
    public string Path { get; set; }
    public string SubKey { get; }
    public IReadOnlyList<string> Segments { get; }

    public bool IsValid { get; }

    static RegistryPath()
    {
        _rootSubstitutions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "HKCR", "HKEY_CLASSES_ROOT" },
            { "HKCU", "HKEY_CURRENT_USER" },
            { "HKLM", "HKEY_LOCAL_MACHINE" },
            { "HKU", "HKEY_USERS" },
            { "HKPD", "HKEY_PERFORMANCE_DATA" },
            { "HKCC", "HKEY_CURRENT_CONFIG" },
            { "HKCR:", "HKEY_CLASSES_ROOT" },
            { "HKCU:", "HKEY_CURRENT_USER" },
            { "HKLM:", "HKEY_LOCAL_MACHINE" },
            { "HKU:", "HKEY_USERS" },
            { "HKPD:", "HKEY_PERFORMANCE_DATA" },
            { "HKCC:", "HKEY_CURRENT_CONFIG" },
        };

        _roots = new Dictionary<string, RegistryHive>(StringComparer.OrdinalIgnoreCase)
        {
            { "HKEY_CLASSES_ROOT", RegistryHive.ClassesRoot },
            { "HKEY_CURRENT_CONFIG", RegistryHive.CurrentConfig },
            { "HKEY_CURRENT_USER", RegistryHive.CurrentUser },
            { "HKEY_LOCAL_MACHINE", RegistryHive.LocalMachine },
            { "HKEY_PERFORMANCE_DATA", RegistryHive.PerformanceData },
            { "HKEY_USERS", RegistryHive.Users },
        };
    }

    public RegistryPath(string path)
    {
        path ??= string.Empty;
        var key = path.Replace("/", "\\");

        Path = string.Empty;
        Segments = new List<string>(key.Split('\\'));

        if (Segments.Count > 0)
        {
            Hive = GetRoot(Segments[0]);
            Segments = Segments.Skip(1).ToList();
            Path = string.Join("\\", Segments);
        }

        SubKey = string.Join("\\", Segments.Take(Segments.Count));

        IsValid = Segments.Count > 1
                  && Hive != RegistryHive.Unknown
                  && !string.IsNullOrWhiteSpace(Path);
    }

    private static RegistryHive GetRoot(string rootName)
    {
        if (_rootSubstitutions.ContainsKey(rootName))
        {
            rootName = _rootSubstitutions[rootName];
        }

        if (!_roots.TryGetValue(rootName, out var r))
        {
            return RegistryHive.Unknown;
        }

        return r;
    }

    private string? GetRootName()
    {
        return Hive switch
        {
            RegistryHive.ClassesRoot => "HKEY_CLASSES_ROOT",
            RegistryHive.CurrentUser => "HKEY_CURRENT_USER",
            RegistryHive.LocalMachine => "HKEY_LOCAL_MACHINE",
            RegistryHive.Users => "HKEY_USERS",
            RegistryHive.CurrentConfig => "HKEY_CURRENT_CONFIG",
            _ => null,
        };
    }

    public static implicit operator RegistryPath(string path)
    {
        return new RegistryPath(path);
    }

    public override string ToString()
    {
        return string.Concat(GetRootName(), "\\", Path);
    }
}