namespace Cupboard;

public static class ResourceExtensions
{
    public static IResourceBuilder<T> OnError<T>(this IResourceBuilder<T> builder, ErrorOptions options)
        where T : Resource
    {
        builder.Configure(res => res.Error = options);
        return builder;
    }

    public static IResourceBuilder<T> OnReboot<T>(this IResourceBuilder<T> builder, RebootOptions options)
        where T : Resource
    {
        builder.Configure(res => res.Reboot = options);
        return builder;
    }

    public static IResourceBuilder<T> RequireAdministrator<T>(this IResourceBuilder<T> builder)
        where T : Resource
    {
        builder.Configure(res => res.RequireAdministrator = true);
        return builder;
    }
}