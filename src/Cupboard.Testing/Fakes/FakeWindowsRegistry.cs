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
            ClassesRoot = new FakeWindowsRegistryKey(new RegistryKeyPath("HKCR"));
            CurrentConfig = new FakeWindowsRegistryKey(new RegistryKeyPath("HKCC"));
            CurrentUser = new FakeWindowsRegistryKey(new RegistryKeyPath("HKCU"));
            LocalMachine = new FakeWindowsRegistryKey(new RegistryKeyPath("HKLM"));
            PerformanceData = new FakeWindowsRegistryKey(new RegistryKeyPath("HKPD"));
            Users = new FakeWindowsRegistryKey(new RegistryKeyPath("HKU"));
        }
    }
}
