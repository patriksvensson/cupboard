namespace Cupboard;

internal sealed class ResourceProviderRepository
{
    private readonly Dictionary<Type, IResourceProvider> _lookup;

    public ResourceProviderRepository(IEnumerable<IResourceProvider> providers)
    {
        _lookup = new Dictionary<Type, IResourceProvider>();
        foreach (var provider in providers)
        {
            if (_lookup.ContainsKey(provider.ResourceType))
            {
                throw new InvalidOperationException(
                    $"Encountered duplicate providers for {provider.ResourceType}");
            }

            _lookup.Add(provider.ResourceType, provider);
        }
    }

    public IResourceProvider? GetProvider(Type type)
    {
        _lookup.TryGetValue(type, out var provider);
        return provider;
    }
}
