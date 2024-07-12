namespace Cupboard;

[PublicAPI]
public sealed class ManifestContext
{
    private const BindingFlags Bindings = BindingFlags.Public | BindingFlags.Instance;

    public FactCollection Facts { get; }

    public List<ResourceBuilder> Builders { get; }

    public ManifestContext(FactCollection facts)
    {
        Facts = facts ?? throw new ArgumentNullException(nameof(facts));
        Builders = new List<ResourceBuilder>();
    }

    public void Resources<TResource>(IEnumerable<TResource> resources, Action<IResourceBuilder<TResource>>? config = null)
        where TResource : Resource
    {
        // Get all public instance properties of the resource
        // that are both readable and writable.
        var properties = typeof(TResource)
            .GetProperties(Bindings)
            .Where(p => p is { CanRead: true, CanWrite: true });

        foreach (var resource in resources)
        {
            var builder = new ResourceBuilder<TResource>(resource.Name);
            builder.Configure(c =>
            {
                // Shallow clone of the resource
                foreach (var property in properties)
                {
                    var value = property.GetValue(resource);
                    property.SetValue(c, value);
                }
            });

            config?.Invoke(builder);
            Builders.Add(builder);
        }
    }

    public IResourceBuilder<TResource> Resource<TResource>(string name)
        where TResource : Resource
    {
        var builder = new ResourceBuilder<TResource>(name);
        Builders.Add(builder);
        return builder;
    }
}
