namespace Cupboard;

public static class DirectoryExtensions
{
    public static IResourceBuilder<Directory> Ensure(this IResourceBuilder<Directory> builder, DirectoryState state)
    {
        return builder.Configure(directory => directory.Ensure = state);
    }

    public static IResourceBuilder<Directory> Permissions(this IResourceBuilder<Directory> builder, string chmod)
    {
        return builder.Configure(directory => directory.Permissions = ChmodParser.Parse(chmod));
    }

    public static IResourceBuilder<Directory> Permissions(this IResourceBuilder<Directory> builder, Chmod permissions)
    {
        return builder.Configure(directory => directory.Permissions = permissions);
    }
}
