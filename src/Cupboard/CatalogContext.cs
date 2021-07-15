using System;
using System.Collections.Generic;

namespace Cupboard
{
    public sealed class CatalogContext
    {
        public FactCollection Facts { get; }

        internal List<Type> Manifests { get; }

        public CatalogContext(FactCollection facts)
        {
            Facts = facts;
            Manifests = new List<Type>();
        }

        public void UseManifest<TManifest>()
            where TManifest : Manifest
        {
            Manifests.Add(typeof(TManifest));
        }
    }
}
