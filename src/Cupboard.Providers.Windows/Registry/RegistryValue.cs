namespace Cupboard;

[PublicAPI]
public sealed class RegistryValue : Resource
{
    public RegistryPath? Path { get; set; }
    public string? Value { get; set; }
    public object? Data { get; set; }
    public RegistryValueKind ValueKind { get; set; } = RegistryValueKind.Unknown;
    public RegistryKeyState State { get; set; } = RegistryKeyState.Exist;

    public RegistryValue(string name)
        : base(name)
    {
    }
}
