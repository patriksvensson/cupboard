namespace Cupboard
{
    public interface IWindowsRegistryKey
    {
        IWindowsRegistryKey? OpenSubKey(string name, bool writable);
        IWindowsRegistryKey? CreateSubKey(string name, bool writable);
        object? GetValue(string name);
        void SetValue(string name, object value, RegistryKeyValueKind registryValueKind);
        void DeleteValue(string name);
    }
}
