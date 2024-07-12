namespace Cupboard;

[PublicAPI]
public sealed class WingetPackage : Resource, IHasPackageName, IHasPackageState
{
    /// <summary>
    /// <para>Gets or sets the id of the package.</para>
    /// <para>This is the id of the package listed by the source. As an example the id for the git cli in the winget source is "Git.Git".</para>
    /// </summary>
    public string Package { get; set; }
    public PackageState Ensure { get; set; }
    public bool Force { get; set; }
    public string? PackageVersion { get; set; }
    public string? Override { get; set; }
    public string? Source { get; set; }

    public WingetPackage(string name)
        : base(name)
    {
        Package = name ?? throw new ArgumentNullException(nameof(name));
    }
}
