namespace Cupboard;

public sealed class VSCodeExtension : Resource, IHasPackageName, IHasPackageState
{
    public string Package { get; set; }
    public PackageState Ensure { get; set; } = PackageState.Installed;

    public VSCodeExtension(string name)
        : base(name)
    {
        Package = name ?? throw new ArgumentNullException(nameof(name));
    }
}
