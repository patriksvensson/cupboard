namespace Cupboard;

public interface IExecutionContext
{
    FactCollection Facts { get; }
    bool DryRun { get; }
}

public sealed class ExecutionContext : IExecutionContext
{
    public FactCollection Facts { get; }
    public bool DryRun { get; init; } = false;

    public ExecutionContext(FactCollection facts)
    {
        Facts = facts ?? throw new ArgumentNullException(nameof(facts));
    }
}
