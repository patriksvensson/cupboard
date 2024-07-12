namespace Cupboard;

[PublicAPI]
public interface IHasPackageState
{
    PackageState Ensure { get; }
}
