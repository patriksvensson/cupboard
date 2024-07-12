namespace Cupboard;

[PublicAPI]
public interface IWindowsRegistryKey
{
    IWindowsRegistryKey? OpenSubKey(string name, bool writable);
    IWindowsRegistryKey? CreateSubKey(string name, bool writable);

    int GetValueCount();

    bool ValueExists(string name);
    object? GetValue(string name);
    void DeleteValue(string name);

    void SetValue(string name, object value, RegistryValueKind kind);
}
