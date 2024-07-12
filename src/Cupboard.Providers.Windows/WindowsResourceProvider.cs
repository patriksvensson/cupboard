namespace Cupboard;

[PublicAPI]
public abstract class WindowsResourceProvider<TResource> : ResourceProvider<TResource>
    where TResource : Resource
{
    public override bool CanRun(FactCollection facts)
    {
        return facts.IsWindows();
    }
}

[PublicAPI]
public abstract class AsyncWindowsResourceProvider<TResource> : AsyncResourceProvider<TResource>
    where TResource : Resource
{
    public override bool CanRun(FactCollection facts)
    {
        return facts.IsWindows();
    }
}
