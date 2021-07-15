using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Cupboard.Internal;
using Win32Registry = Microsoft.Win32.Registry;
using Win32RegistryKey = Microsoft.Win32.RegistryKey;

namespace Cupboard
{
    public sealed class RegistryKeyProvider : WindowsResourceProvider<RegistryKey>.Sync
    {
        private readonly ICupboardLogger _logger;

        public RegistryKeyProvider(ICupboardLogger logger)
        {
            _logger = logger;
        }

        public override RegistryKey Create(string name)
        {
            return new RegistryKey(name);
        }

        public override ResourceState Run(IExecutionContext context, RegistryKey resource)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _logger.Error("Cannot manipulate registry keys on non-Windows OS");
                return ResourceState.Error;
            }

            if (resource.Path == null)
            {
                _logger.Error($"The registry key for resource '{resource.Name}' has not been set");
                return ResourceState.Error;
            }

            if (!resource.Path.IsValid)
            {
                _logger.Error($"The registry key for resource '{resource.Name}' is invalid");
                return ResourceState.Error;
            }

            if (resource.State == RegistryKeyState.Exist)
            {
                if (resource.Value == null)
                {
                    _logger.Error($"The registry key value for resource '{resource.Name}' has not been set");
                    return ResourceState.Error;
                }

                var root = GetRegistryKey(resource.Path);
                if (root == null)
                {
                    _logger.Error($"The registry key for resource '{resource.Name}' could not be found");
                    return ResourceState.Error;
                }

                var key = root.OpenSubKey(resource.Path.SubKey, true);
                if (key == null)
                {
                    if (context.DryRun)
                    {
                        return ResourceState.Changed;
                    }

                    _logger.Debug("Trying to create registry key");
                    key = root.CreateSubKey(resource.Path.SubKey, true);
                    if (key == null)
                    {
                        _logger.Error("Could not create the registry key");
                        return ResourceState.Error;
                    }
                }

                var value = key.GetValue(resource.Path.Value, null);
                if (value != null)
                {
                    // TODO 2021-07-11: This is good enough for now, but should be properly implemented
                    if (value.ToString()?.Equals(resource.Value.ToString(), StringComparison.Ordinal) ?? false)
                    {
                        _logger.Debug("The registry key already has the expected value");
                        return ResourceState.Unchanged;
                    }
                }

                if (!context.DryRun)
                {
                    _logger.Information("Updating registry key");
                    key.SetValue(resource.Path.Value, resource.Value, resource.ValueKind.ToWin32());
                }

                return ResourceState.Changed;
            }
            else
            {
                var root = GetRegistryKey(resource.Path);
                if (root == null)
                {
                    _logger.Error("The registry key for resource could not be found");
                    return ResourceState.Error;
                }

                var key = root.OpenSubKey(resource.Path.SubKey, true);
                if (key == null)
                {
                    _logger.Debug("The registry key does not exist");
                    return ResourceState.Unchanged;
                }

                var value = key.GetValue(resource.Path.Value, null);
                if (value == null)
                {
                    _logger.Debug("The registry key value does not exist");
                    return ResourceState.Unchanged;
                }

                if (context.DryRun)
                {
                    return ResourceState.Changed;
                }

                _logger.Information("Deleting registry key value");
                return DeleteRegistryKeyValue(key, resource.Path);
            }
        }

        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
        private ResourceState DeleteRegistryKeyValue(Win32RegistryKey key, RegistryKeyPath path)
        {
            try
            {
                key.DeleteValue(path.Value, false);
                return ResourceState.Changed;
            }
            catch (Exception ex)
            {
                _logger.Error($"Could not delete registry key. {ex.Message}");
                return ResourceState.Error;
            }
        }

        [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
        private static Win32RegistryKey? GetRegistryKey(RegistryKeyPath path)
        {
            return path.Root switch
            {
                RegistryKeyRoot.ClassesRoot => Win32Registry.ClassesRoot,
                RegistryKeyRoot.CurrentUser => Win32Registry.CurrentUser,
                RegistryKeyRoot.LocalMachine => Win32Registry.LocalMachine,
                RegistryKeyRoot.Users => Win32Registry.Users,
                RegistryKeyRoot.CurrentConfig => Win32Registry.CurrentConfig,
                _ => null,
            };
        }
    }
}
