namespace Cupboard;

public static class VSCodeExtensionExtensions
{
    public static IResourceBuilder<VSCodeExtension> Ensure(this IResourceBuilder<VSCodeExtension> builder, PackageState state)
    {
        return builder.Configure(file => file.Ensure = state);
    }

    public static IResourceBuilder<VSCodeExtension> Package(this IResourceBuilder<VSCodeExtension> builder, string package)
    {
        return builder.Configure(pkg => pkg.Package = package);
    }
}