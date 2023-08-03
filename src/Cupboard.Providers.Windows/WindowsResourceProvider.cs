namespace Cupboard;

public abstract class WindowsResourceProvider<TResource> : ResourceProvider<TResource>
    where TResource : Resource
{
    public override bool CanRun(FactCollection facts)
    {
        return facts.IsWindows();
    }
}

public abstract class AsyncWindowsResourceProvider<TResource> : AsyncResourceProvider<TResource>
    where TResource : Resource
{
    public override bool CanRun(FactCollection facts)
    {
        return facts.IsWindows();
    }
}
