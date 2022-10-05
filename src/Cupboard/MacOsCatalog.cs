namespace Cupboard
{
    public abstract class MacOsCatalog : Catalog
    {
        public override bool CanRun(FactCollection facts) => facts.IsMacOS();
    }
}