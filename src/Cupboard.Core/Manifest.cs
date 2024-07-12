namespace Cupboard;

[PublicAPI]
public abstract class Manifest
{
    public abstract void Execute(ManifestContext context);
}
