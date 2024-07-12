namespace Cupboard;

[PublicAPI]
public sealed class ReportItem
{
    public Resource Resource { get; }
    public IResourceProvider Provider { get; }
    public ResourceState State { get; }
    public bool RequireAdministrator { get; }

    public ReportItem(IResourceProvider provider, Resource resource, ResourceState state, bool requireAdministrator)
    {
        Provider = provider;
        Resource = resource;
        State = state;
        RequireAdministrator = requireAdministrator;
    }
}
