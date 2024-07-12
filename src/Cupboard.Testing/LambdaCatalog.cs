namespace Cupboard.Testing;

public sealed class LambdaCatalog : Catalog
{
    private readonly Action<CatalogContext> _action;

    public LambdaCatalog(Action<CatalogContext> action)
    {
        _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public override void Execute(CatalogContext context)
    {
        _action(context);
    }
}
