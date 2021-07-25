using System;
using System.Diagnostics.CodeAnalysis;
using Win32RegistryKey = Microsoft.Win32.RegistryKey;

namespace Cupboard.Internal
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    internal sealed class WindowsRegistryKey : IWindowsRegistryKey
    {
        private readonly Win32RegistryKey _key;

        public WindowsRegistryKey(Win32RegistryKey key)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public IWindowsRegistryKey? CreateSubKey(string name, bool writable)
        {
            var key = _key.CreateSubKey(name, writable);
            return new WindowsRegistryKey(key);
        }

        public void DeleteValue(string name)
        {
            _key.DeleteValue(name);
        }

        public object? GetValue(string name)
        {
            return _key.GetValue(name, null);
        }

        public IWindowsRegistryKey? OpenSubKey(string name, bool writable)
        {
            var key = _key.OpenSubKey(name, writable);
            if (key == null)
            {
                return null;
            }

            return new WindowsRegistryKey(key);
        }

        public void SetValue(string name, object value, RegistryKeyValueKind kind)
        {
            _key.SetValue(name, value, kind.ToWin32());
        }
    }
}
