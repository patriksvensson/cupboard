namespace Cupboard.Testing;

public sealed class FakeRebootDetector : IRebootDetector
{
    public bool PendingReboot { get; set; }

    public bool HasPendingReboot()
    {
        return PendingReboot;
    }
}
