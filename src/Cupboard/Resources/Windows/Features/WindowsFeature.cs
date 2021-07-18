namespace Cupboard
{
    public sealed class WindowsFeature : Resource
    {
        public string? FeatureName { get; set; }
        public WindowsFeatureState Ensure { get; set; } = WindowsFeatureState.Enabled;

        public WindowsFeature(string name)
            : base(name)
        {
        }
    }
}
