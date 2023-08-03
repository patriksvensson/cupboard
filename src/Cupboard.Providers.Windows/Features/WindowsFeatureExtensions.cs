namespace Cupboard;

public static class WindowsFeatureExtensions
{
    public static IResourceBuilder<WindowsFeature> Ensure(this IResourceBuilder<WindowsFeature> builder, WindowsFeatureState state)
    {
        return builder.Configure(file => file.Ensure = state);
    }

    public static IResourceBuilder<WindowsFeature> FeatureName(this IResourceBuilder<WindowsFeature> builder, string name)
    {
        return builder.Configure(file => file.FeatureName = name);
    }
}
