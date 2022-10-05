namespace Cupboard
{
    public abstract class AsyncMacResourceProvider<TResource> : AsyncResourceProvider<TResource>
        where TResource : Resource
    {
        public override bool CanRun(FactCollection facts)
        {
            return facts.IsMacOS();
        }
    }
}