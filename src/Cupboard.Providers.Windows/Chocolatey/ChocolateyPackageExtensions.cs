namespace Cupboard;

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

    public static IResourceBuilder<ChocolateyPackage> IncludePreRelease(this IResourceBuilder<ChocolateyPackage> builder)
    {
        return builder.Configure(pkg => pkg.PreRelease = true);
    }

    public static IResourceBuilder<ChocolateyPackage> IgnoreChecksum(this IResourceBuilder<ChocolateyPackage> builder)
    {
        return builder.Configure(pkg => pkg.IgnoreChecksum = true);
    }

    public static IResourceBuilder<ChocolateyPackage> PackageParameters(this IResourceBuilder<ChocolateyPackage> builder, string packageParameters)
    {
        return builder.Configure(pkg => pkg.PackageParameters = packageParameters);
    }

    public static IResourceBuilder<ChocolateyPackage> UseVersion(this IResourceBuilder<ChocolateyPackage> builder, string version)
    {
        return builder.Configure(pkg => pkg.PackageVersion = version);
    }

    public static IResourceBuilder<ChocolateyPackage> AllowDowngrade(this IResourceBuilder<ChocolateyPackage> builder)
    {
        return builder.Configure(pkg => pkg.AllowDowngrade = true);
    }
}