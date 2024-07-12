namespace Cupboard;

[PublicAPI]
public static class VsCodeExtensionExtensions
{
    public static IResourceBuilder<VsCodeExtension> Ensure(this IResourceBuilder<VsCodeExtension> builder, PackageState state)
    {
        return builder.Configure(file => file.Ensure = state);
    }

    public static IResourceBuilder<VsCodeExtension> Package(this IResourceBuilder<VsCodeExtension> builder, string package)
    {
        return builder.Configure(pkg => pkg.Package = package);
    }
}
