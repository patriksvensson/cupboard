namespace Cupboard
{
    public static class VSCodeExtensionExtensions
    {
        public static IResourceBuilder<VSCodeExtension> Ensure(this IResourceBuilder<VSCodeExtension> builder, PackageState state)
        {
            return builder.Configure(file => file.Ensure = state);
        }
    }
}
