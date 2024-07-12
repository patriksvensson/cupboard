namespace Cupboard;

public interface IWindowsRegistryKey
{
    IWindowsRegistryKey? OpenSubKey(string name, bool writable);
    IWindowsRegistryKey? CreateSubKey(string name, bool writable);

    int GetValueCount();

    bool ValueExists(string name);
    object? GetValue(string name);
    void DeleteValue(string name);

    [Obsolete("Please use SetValue overload accepting a RegistryValueKind instead")]
    void SetValue(string name, object value, RegistryKeyValueKind kind);

    void SetValue(string name, object value, RegistryValueKind kind);
}
