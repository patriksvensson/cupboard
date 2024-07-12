namespace Cupboard;

public static class DownloadExtensions
{
    public static IResourceBuilder<Download> FromUrl(this IResourceBuilder<Download> builder, string url)
    {
        builder.Configure(res => res.Url = new Uri(url));
        return builder;
    }

    public static IResourceBuilder<Download> Permissions(this IResourceBuilder<Download> builder, string permissions)
    {
        builder.Configure(res => res.Permissions = Chmod.Parse(permissions));
        return builder;
    }

    public static IResourceBuilder<Download> Permissions(this IResourceBuilder<Download> builder, Chmod permissions)
    {
        builder.Configure(res => res.Permissions = permissions);
        return builder;
    }

    public static IResourceBuilder<Download> FromUrl(this IResourceBuilder<Download> builder, Uri url)
    {
        builder.Configure(res => res.Url = url);
        return builder;
    }

    public static IResourceBuilder<Download> ToFile(this IResourceBuilder<Download> builder, FilePath path)
    {
        builder.Configure(res => res.Destination = path);
        return builder;
    }

    public static IResourceBuilder<Download> ToDirectory(this IResourceBuilder<Download> builder, DirectoryPath path)
    {
        builder.Configure(res => res.Destination = path);
        return builder;
    }
}
