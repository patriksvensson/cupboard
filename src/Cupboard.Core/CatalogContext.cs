using System;
using System.Collections.Generic;

namespace Cupboard;

public sealed class CatalogContext
{
    private readonly HashSet<Type> _manifests;

    public FactCollection Facts { get; }

    public CatalogContext(FactCollection facts)
    {
        Facts = facts ?? throw new ArgumentNullException(nameof(facts));
        _manifests = new HashSet<Type>();
    }

    public void UseManifest<TManifest>()
        where TManifest : Manifest
    {
        _manifests.Add(typeof(TManifest));
    }

    public IEnumerable<Type> GetManifests()
    {
        return _manifests;
    }
}
