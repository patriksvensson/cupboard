namespace Cupboard
{
    public sealed class WindowsResourceProvider<TResource>
        where TResource : Resource
    {
        public abstract class Sync : ResourceProvider<TResource>
        {
            public override bool CanRun(FactCollection facts)
            {
                return facts.IsWindows();
            }
        }

        public abstract class Async : AsyncResourceProvider<TResource>
        {
            public override bool CanRun(FactCollection facts)
            {
                return facts.IsWindows();
            }
        }
    }
}
