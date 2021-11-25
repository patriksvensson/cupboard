namespace Cupboard;

public static class RegistryValueExtensions
{
    public static IResourceBuilder<RegistryValue> Ensure(this IResourceBuilder<RegistryValue> builder, RegistryKeyState state)
    {
        return builder.Configure(reg => reg.State = state);
    }

    public static IResourceBuilder<RegistryValue> Path(this IResourceBuilder<RegistryValue> builder, string path)
    {
        return builder.Configure(reg => reg.Path = new RegistryPath(path));
    }

    public static IResourceBuilder<RegistryValue> Value(this IResourceBuilder<RegistryValue> builder, string value)
    {
        return builder.Configure(reg => reg.Value = value);
    }

    public static IResourceBuilder<RegistryValue> Data(this IResourceBuilder<RegistryValue> builder, string data)
    {
        return builder.Configure(reg => reg.Data = data);
    }

    public static IResourceBuilder<RegistryValue> Data(this IResourceBuilder<RegistryValue> builder, object data)
    {
        return builder.Configure(reg => reg.Data = data);
    }

    public static IResourceBuilder<RegistryValue> Data(this IResourceBuilder<RegistryValue> builder, object data, RegistryValueKind kind)
    {
        return builder.Configure(reg =>
        {
            reg.State = RegistryKeyState.Exist;
            reg.Data = data;
            reg.ValueKind = kind;
        });
    }
}