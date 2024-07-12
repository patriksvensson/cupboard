namespace Cupboard;

[PublicAPI]
public interface IResourceIdentity
{
    Type ResourceType { get; }

    string Name { get; }
}
