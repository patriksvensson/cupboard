using System;
using System.Runtime.InteropServices;

namespace Cupboard
{
    public sealed class RegistryKeyProvider : WindowsResourceProvider<RegistryKey>
    {
        private readonly IWindowsRegistry _registry;
        private readonly ICupboardLogger _logger;

        public RegistryKeyProvider(IWindowsRegistry registry, ICupboardLogger logger)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override RegistryKey Create(string name)
        {
            return new RegistryKey(name);
        }

        public override bool RequireAdministrator(FactCollection facts)
        {
            return true;
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

                var value = key.GetValue(resource.Path.Value);
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
                    key.SetValue(resource.Path.Value, resource.Value, resource.ValueKind);
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

                var value = key.GetValue(resource.Path.Value);
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

        private ResourceState DeleteRegistryKeyValue(IWindowsRegistryKey key, RegistryKeyPath path)
        {
            try
            {
                key.DeleteValue(path.Value);
                return ResourceState.Changed;
            }
            catch (Exception ex)
            {
                _logger.Error($"Could not delete registry key. {ex.Message}");
                return ResourceState.Error;
            }
        }

        private IWindowsRegistryKey? GetRegistryKey(RegistryKeyPath path)
        {
            return path.Root switch
            {
                RegistryKeyRoot.ClassesRoot => _registry.ClassesRoot,
                RegistryKeyRoot.CurrentUser => _registry.CurrentUser,
                RegistryKeyRoot.LocalMachine => _registry.LocalMachine,
                RegistryKeyRoot.Users => _registry.Users,
                RegistryKeyRoot.CurrentConfig => _registry.CurrentConfig,
                _ => null,
            };
        }
    }
}
