namespace Cupboard.Internal;

internal sealed class ResourceComparer : IEqualityComparer<IResourceIdentity>
{
    private readonly IEqualityComparer<Type> _typeComparer;
    private readonly IEqualityComparer<string> _stringComparer;

    public ResourceComparer()
    {
        _typeComparer = EqualityComparer<Type>.Default;
        _stringComparer = StringComparer.OrdinalIgnoreCase;
    }

    public bool Equals(IResourceIdentity? x, IResourceIdentity? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return _typeComparer.Equals(x.ResourceType, y.ResourceType)
            && _stringComparer.Equals(x.Name, y.Name);
    }

    public int GetHashCode(IResourceIdentity obj)
    {
        unchecked
        {
            var hash = 27;
            hash = (13 * hash) + _typeComparer.GetHashCode(obj.ResourceType);
            hash = (13 * hash) + _stringComparer.GetHashCode(obj.Name);
            return hash;
        }
    }
}
