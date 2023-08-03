namespace Cupboard;

public interface IHasPackageState
{
    PackageState Ensure { get; }
}
