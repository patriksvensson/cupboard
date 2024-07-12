namespace Cupboard;

[Obsolete("Please use the RegistryValue resource instead")]
public static class RegistryKeyExtensions
{
    public static IResourceBuilder<RegistryKey> Ensure(this IResourceBuilder<RegistryKey> builder, RegistryKeyState state)
    {
        return builder.Configure(reg => reg.State = state);
    }

    public static IResourceBuilder<RegistryKey> Path(this IResourceBuilder<RegistryKey> builder, string path)
    {
        return builder.Configure(reg => reg.Path = new RegistryKeyPath(path));
    }

    public static IResourceBuilder<RegistryKey> Value(this IResourceBuilder<RegistryKey> builder, object value)
    {
        return builder.Configure(reg => reg.Value = value);
    }

    public static IResourceBuilder<RegistryKey> Value(this IResourceBuilder<RegistryKey> builder, object value, RegistryKeyValueKind kind)
    {
        return builder.Configure(reg =>
        {
            reg.State = RegistryKeyState.Exist;
            reg.Value = value;
            reg.ValueKind = kind;
        });
    }
}
