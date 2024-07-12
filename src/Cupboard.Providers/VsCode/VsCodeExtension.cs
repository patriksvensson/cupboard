namespace Cupboard;

[PublicAPI]
public sealed class VsCodeExtension : Resource, IHasPackageName, IHasPackageState
{
    public string Package { get; set; }
    public PackageState Ensure { get; set; } = PackageState.Installed;

    public VsCodeExtension(string name)
        : base(name)
    {
        Package = name ?? throw new ArgumentNullException(nameof(name));
    }
}
