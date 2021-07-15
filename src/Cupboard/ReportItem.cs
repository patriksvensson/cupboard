namespace Cupboard
{
    public sealed class ReportItem
    {
        public Resource Resource { get; }
        public IResourceProvider Provider { get; }
        public ResourceState State { get; }

        public ReportItem(IResourceProvider provider, Resource resource, ResourceState state)
        {
            Provider = provider;
            Resource = resource;
            State = state;
        }
    }
}
