namespace Cupboard.Internal;

internal sealed class ExecutionPlanItem
{
    public IResourceProvider Provider { get; }
    public Resource Resource { get; }
    public bool RequireAdministrator { get; }

    public ExecutionPlanItem(IResourceProvider provider, Resource resource, bool requireAdministrator)
    {
        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        Resource = resource ?? throw new ArgumentNullException(nameof(resource));
        RequireAdministrator = requireAdministrator;
    }
}
