namespace Cupboard.Internal;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed class WindowsRegistry : IWindowsRegistry
{
    public IWindowsRegistryKey ClassesRoot => new WindowsRegistryKey(Win32Registry.ClassesRoot);
    public IWindowsRegistryKey CurrentConfig => new WindowsRegistryKey(Win32Registry.CurrentConfig);
    public IWindowsRegistryKey CurrentUser => new WindowsRegistryKey(Win32Registry.CurrentUser);
    public IWindowsRegistryKey LocalMachine => new WindowsRegistryKey(Win32Registry.LocalMachine);
    public IWindowsRegistryKey PerformanceData => new WindowsRegistryKey(Win32Registry.PerformanceData);
    public IWindowsRegistryKey Users => new WindowsRegistryKey(Win32Registry.Users);
}
