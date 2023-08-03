namespace Cupboard;

public abstract class Catalog
{
    public virtual bool CanRun(FactCollection facts)
    {
        return true;
    }

    public abstract void Execute(CatalogContext context);
}
