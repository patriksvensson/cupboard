namespace Cupboard;

public abstract class WindowsCatalog : Catalog
{
    public override bool CanRun(FactCollection facts)
    {
        return facts.IsWindows();
    }
}