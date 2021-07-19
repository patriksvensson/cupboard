using System;

namespace Cupboard.Testing
{
    public sealed class FakeWindowsRegistryKey : IWindowsRegistryKey
    {
        public RegistryKeyPath Path { get; }

        public FakeWindowsRegistryKey(RegistryKeyPath path)
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public IWindowsRegistryKey? CreateSubKey(string name, bool writable)
        {
            throw new NotSupportedException();
        }

        public void DeleteValue(string name)
        {
            throw new NotSupportedException();
        }

        public object? GetValue(string name)
        {
            throw new NotSupportedException();
        }

        public IWindowsRegistryKey? OpenSubKey(string name, bool writable)
        {
            throw new NotSupportedException();
        }

        public void SetValue(string name, object value, RegistryKeyValueKind registryValueKind)
        {
            throw new NotSupportedException();
        }
    }
}
