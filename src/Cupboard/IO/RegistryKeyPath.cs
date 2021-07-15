using System;
using System.Collections.Generic;
using System.Linq;

namespace Cupboard
{
    public sealed class RegistryKeyPath
    {
        private static readonly Dictionary<string, string> _rootSubstitutions;
        private static readonly Dictionary<string, RegistryKeyRoot> _roots;

        public RegistryKeyRoot Root { get; set; }
        public string Path { get; set; }
        public string SubKey { get; }
        public string Value { get; }
        public IReadOnlyList<string> Segments { get; }

        public bool IsValid { get; }

        static RegistryKeyPath()
        {
            _rootSubstitutions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "HKCR", "HKEY_CLASSES_ROOT" },
                { "HKCU", "HKEY_CURRENT_USER" },
                { "HKLM", "HKEY_LOCAL_MACHINE" },
                { "HKU", "HKEY_USERS" },
                { "HKCC", "HKEY_CURRENT_CONFIG" },
                { "HKCR:", "HKEY_CLASSES_ROOT" },
                { "HKCU:", "HKEY_CURRENT_USER" },
                { "HKLM:", "HKEY_LOCAL_MACHINE" },
                { "HKU:", "HKEY_USERS" },
                { "HKCC:", "HKEY_CURRENT_CONFIG" },
            };

            _roots = new Dictionary<string, RegistryKeyRoot>(StringComparer.OrdinalIgnoreCase)
            {
                { "HKEY_CLASSES_ROOT", RegistryKeyRoot.ClassesRoot },
                { "HKEY_CURRENT_CONFIG", RegistryKeyRoot.CurrentConfig },
                { "HKEY_CURRENT_USER", RegistryKeyRoot.CurrentUser },
                { "HKEY_LOCAL_MACHINE", RegistryKeyRoot.LocalMachine },
                { "HKEY_USERS", RegistryKeyRoot.Users },
            };
        }

        public RegistryKeyPath(string path)
        {
            path ??= string.Empty;
            var key = path.Replace("/", "\\");

            Path = string.Empty;
            Segments = new List<string>(key.Split('\\'));

            if (Segments.Count > 0)
            {
                Root = GetRoot(Segments[0]);
                Segments = Segments.Skip(1).ToList();
                Path = string.Join("\\", Segments);
            }

            SubKey = string.Join("\\", Segments.Take(Segments.Count - 1));
            Value = Segments.LastOrDefault() ?? string.Empty;

            IsValid = Segments.Count > 1
                && Root != RegistryKeyRoot.Unknown
                && !string.IsNullOrWhiteSpace(Path);
        }

        private static RegistryKeyRoot GetRoot(string rootName)
        {
            if (_rootSubstitutions.ContainsKey(rootName))
            {
                rootName = _rootSubstitutions[rootName];
            }

            if (!_roots.TryGetValue(rootName, out var r))
            {
                return RegistryKeyRoot.Unknown;
            }

            return r;
        }

        private string? GetRootName()
        {
            return Root switch
            {
                RegistryKeyRoot.ClassesRoot => "HKEY_CLASSES_ROOT",
                RegistryKeyRoot.CurrentUser => "HKEY_CURRENT_USER",
                RegistryKeyRoot.LocalMachine => "HKEY_LOCAL_MACHINE",
                RegistryKeyRoot.Users => "HKEY_USERS",
                RegistryKeyRoot.CurrentConfig => "HKEY_CURRENT_CONFIG",
                _ => null,
            };
        }

        public override string ToString()
        {
            return string.Concat(GetRootName(), "\\", Path);
        }
    }
}
