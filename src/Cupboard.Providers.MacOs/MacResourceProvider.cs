using Cupboard;

public abstract class MacResourceProvider<TResource> : ResourceProvider<TResource>
    where TResource : Resource
{
    public override bool CanRun(FactCollection facts)
    {
        return facts.IsMacOS();
    }
}