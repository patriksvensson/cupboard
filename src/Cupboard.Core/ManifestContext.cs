using System;
using System.Collections.Generic;

namespace Cupboard;

public sealed class ManifestContext
{
    public FactCollection Facts { get; }
    public List<ResourceBuilder> Builders { get; }

    public ManifestContext(FactCollection facts)
    {
        Facts = facts ?? throw new ArgumentNullException(nameof(facts));
        Builders = new List<ResourceBuilder>();
    }

    public IResourceBuilder<TResource> Resource<TResource>(string name)
        where TResource : Resource
    {
        var builder = new ResourceBuilder<TResource>(name);

        // Store a reference to the configuration.
        Builders.Add(builder);

        return builder;
    }
}