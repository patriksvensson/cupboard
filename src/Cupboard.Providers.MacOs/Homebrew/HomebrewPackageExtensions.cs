namespace Cupboard
{
    public static class BrewPackageExtensions
    {
        public static IResourceBuilder<HomebrewPackage> Ensure(this IResourceBuilder<HomebrewPackage> builder, PackageState state)
        {
            return builder.Configure(pkg => pkg.Ensure = state);
        }

        public static IResourceBuilder<HomebrewPackage> Package(this IResourceBuilder<HomebrewPackage> builder, string package)
        {
            return builder.Configure(pkg => pkg.Package = package);
        }

        public static IResourceBuilder<HomebrewPackage> IncludePreRelease(this IResourceBuilder<HomebrewPackage> builder)
        {
            return builder.Configure(pkg => pkg.PreRelease = true);
        }

        public static IResourceBuilder<HomebrewPackage> IgnoreChecksum(this IResourceBuilder<HomebrewPackage> builder)
        {
            return builder.Configure(pkg => pkg.IgnoreChecksum = true);
        }

        public static IResourceBuilder<HomebrewPackage> PackageParameters(this IResourceBuilder<HomebrewPackage> builder, string packageParameters)
        {
            return builder.Configure(pkg => pkg.PackageParameters = packageParameters);
        }

        public static IResourceBuilder<HomebrewPackage> UseVersion(this IResourceBuilder<HomebrewPackage> builder, string version)
        {
            return builder.Configure(pkg => pkg.PackageVersion = version);
        }

        public static IResourceBuilder<HomebrewPackage> AllowDowngrade(this IResourceBuilder<HomebrewPackage> builder)
        {
            return builder.Configure(pkg => pkg.AllowDowngrade = true);
        }

        public static IResourceBuilder<HomebrewPackage> IsCask(this IResourceBuilder<HomebrewPackage> builder) =>
            builder.Configure(pkg => pkg.IsCask = true);

        public static IResourceBuilder<HomebrewPackage> OnError(this IResourceBuilder<HomebrewPackage> builder, ErrorOptions handle) =>
            builder.Configure(pkg => pkg.Error = handle);
    }
}
