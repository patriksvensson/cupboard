namespace Cupboard
{
    public static class ChocolateyPackageExtensions
    {
        public static IResourceBuilder<ChocolateyPackage> Ensure(this IResourceBuilder<ChocolateyPackage> builder, PackageState state)
        {
            return builder.Configure(pkg => pkg.Ensure = state);
        }
    }
}
