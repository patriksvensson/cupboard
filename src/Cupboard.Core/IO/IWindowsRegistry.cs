namespace Cupboard
{
    public interface IWindowsRegistry
    {
        IWindowsRegistryKey ClassesRoot { get; }
        IWindowsRegistryKey CurrentConfig { get; }
        IWindowsRegistryKey CurrentUser { get; }
        IWindowsRegistryKey LocalMachine { get; }
        IWindowsRegistryKey PerformanceData { get; }
        IWindowsRegistryKey Users { get; }
    }
}
