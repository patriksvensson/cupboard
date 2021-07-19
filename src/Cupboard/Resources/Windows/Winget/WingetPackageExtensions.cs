namespace Cupboard
{
    public static class WingetPackageExtensions
    {
        public static IResourceBuilder<WingetPackage> Ensure(this IResourceBuilder<WingetPackage> builder, PackageState state)
        {
            return builder.Configure(pkg => pkg.Ensure = state);
        }

        public static IResourceBuilder<WingetPackage> Package(this IResourceBuilder<WingetPackage> builder, string package)
        {
            return builder.Configure(pkg => pkg.Package = package);
        }
    }
}
