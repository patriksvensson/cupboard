namespace Cupboard
{
    public static class ChocolateyPackageExtensions
    {
        public static IResourceBuilder<ChocolateyPackage> Ensure(this IResourceBuilder<ChocolateyPackage> builder, PackageState state)
        {
            return builder.Configure(pkg => pkg.Ensure = state);
        }

        public static IResourceBuilder<ChocolateyPackage> Package(this IResourceBuilder<ChocolateyPackage> builder, string package)
        {
            return builder.Configure(pkg => pkg.Package = package);
        }
    }
}
