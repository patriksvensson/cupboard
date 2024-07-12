namespace Cupboard;

public abstract class ResourceBuilder
{
    public string Name { get; }

    public Type Type { get; set; }

    public List<Tuple<Type, string>> RunBefore { get; }

    public List<Tuple<Type, string>> RunAfter { get; }

    public List<Action<Resource>> Configurations { get; }

    protected ResourceBuilder(string name, Type type)
    {
        Name = name;
        Type = type;
        RunBefore = new List<Tuple<Type, string>>();
        RunAfter = new List<Tuple<Type, string>>();
        Configurations = new List<Action<Resource>>();
    }
}

public sealed class ResourceBuilder<TResource> : ResourceBuilder, IResourceBuilder<TResource>
    where TResource : Resource
{
    public ResourceBuilder(string name)
        : base(name, typeof(TResource))
    {
    }

    public IResourceBuilder<TResource> Before<TOther>(string name)
    {
        RunBefore.Add(Tuple.Create(typeof(TOther), name));
        return this;
    }

    public IResourceBuilder<TResource> After<TOther>(string name)
    {
        RunAfter.Add(Tuple.Create(typeof(TOther), name));
        return this;
    }

    public IResourceBuilder<TResource> Configure(Action<TResource> action)
    {
        Configurations.Add(resource => action((TResource)resource));
        return this;
    }
}
