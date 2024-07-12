namespace Cupboard.Testing;

public sealed class FakeWindowsRegistryKey : IWindowsRegistryKey
{
    public RegistryPath Path { get; }

    public FakeWindowsRegistryKey(RegistryPath path)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    public IWindowsRegistryKey? CreateSubKey(string name, bool writable)
    {
        throw new NotSupportedException();
    }

    public int GetValueCount()
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

    public bool ValueExists(string name)
    {
        throw new NotSupportedException();
    }

    public IWindowsRegistryKey? OpenSubKey(string name, bool writable)
    {
        throw new NotSupportedException();
    }

    public void SetValue(string name, object value, RegistryValueKind kind)
    {
        throw new NotSupportedException();
    }
}
