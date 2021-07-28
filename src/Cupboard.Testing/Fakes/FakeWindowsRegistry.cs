namespace Cupboard.Testing
{
    public sealed class FakeWindowsRegistry : IWindowsRegistry
    {
        public IWindowsRegistryKey ClassesRoot { get; }
        public IWindowsRegistryKey CurrentConfig { get; }
        public IWindowsRegistryKey CurrentUser { get; }
        public IWindowsRegistryKey LocalMachine { get; }
        public IWindowsRegistryKey PerformanceData { get; }
        public IWindowsRegistryKey Users { get; }

        public FakeWindowsRegistry()
        {
            ClassesRoot = new FakeWindowsRegistryKey(new RegistryPath("HKCR"));
            CurrentConfig = new FakeWindowsRegistryKey(new RegistryPath("HKCC"));
            CurrentUser = new FakeWindowsRegistryKey(new RegistryPath("HKCU"));
            LocalMachine = new FakeWindowsRegistryKey(new RegistryPath("HKLM"));
            PerformanceData = new FakeWindowsRegistryKey(new RegistryPath("HKPD"));
            Users = new FakeWindowsRegistryKey(new RegistryPath("HKU"));
        }
    }
}
