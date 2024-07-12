namespace Cupboard;

[PublicAPI]
public interface IRebootDetector
{
    bool HasPendingReboot();
}
