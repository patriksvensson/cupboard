using System;
using System.Collections.Generic;
using System.Linq;

namespace Cupboard
{
    public sealed class CatalogContext
    {
        private readonly HashSet<Type> _manifestTypes;

        public FactCollection Facts { get; }

        public CatalogContext(FactCollection facts)
        {
            Facts = facts;
            _manifestTypes = new HashSet<Type>();
        }

        public void UseManifest<TManifest>()
            where TManifest : Manifest
        {
            _manifestTypes.Add(typeof(TManifest));
        }

        // TODO 2021-07-25: Get rid of this method
        public IEnumerable<Manifest> GetAddedManifests(IEnumerable<Manifest> candidates)
        {
            return candidates.Where(x => _manifestTypes.Contains(x.GetType()));
        }
    }
}
