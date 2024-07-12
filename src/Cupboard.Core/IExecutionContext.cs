namespace Cupboard;

[PublicAPI]
public interface IExecutionContext
{
    FactCollection Facts { get; }
    bool DryRun { get; }
}

[PublicAPI]
public sealed class ExecutionContext : IExecutionContext
{
    public FactCollection Facts { get; }
    public bool DryRun { get; init; }

    public ExecutionContext(FactCollection facts)
    {
        Facts = facts ?? throw new ArgumentNullException(nameof(facts));
    }
}
