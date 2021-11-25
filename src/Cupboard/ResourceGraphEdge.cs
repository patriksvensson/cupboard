namespace Cupboard.Internal;

internal sealed class ResourceGraphEdge
{
    public IResourceIdentity From { get; }
    public IResourceIdentity To { get; }

    public ResourceGraphEdge(IResourceIdentity from, IResourceIdentity to)
    {
        From = from;
        To = to;
    }
}