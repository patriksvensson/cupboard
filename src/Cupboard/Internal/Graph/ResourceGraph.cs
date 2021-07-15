using System;
using System.Collections.Generic;
using System.Linq;

namespace Cupboard.Internal
{
    internal sealed class ResourceGraph
    {
        private readonly IEqualityComparer<IResourceIdentity> _comparer;

        public HashSet<IResourceIdentity> Nodes { get; }
        public HashSet<ResourceGraphEdge> Edges { get; }
        public HashSet<Resource> Resources { get; }
        public List<Action> Configurations { get; set; }

        public ResourceGraph()
        {
            _comparer = new ResourceComparer();

            Nodes = new HashSet<IResourceIdentity>();
            Edges = new HashSet<ResourceGraphEdge>();
            Resources = new HashSet<Resource>(_comparer);
            Configurations = new List<Action>();
        }

        public void Add(Resource resource)
        {
            Resources.Add(resource);
        }

        public void Connect(IResourceIdentity from, IResourceIdentity to)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (_comparer.Equals(from, to))
            {
                throw new InvalidOperationException("Reflexive dependencies are not allowed.");
            }

            if (Edges.Any(e => _comparer.Equals(e.From, to) && _comparer.Equals(e.To, from)))
            {
                throw new InvalidOperationException("Unidirectional dependencies are not allowed.");
            }

            if (!Nodes.Contains(from))
            {
                Nodes.Add(from);
            }

            if (!Nodes.Contains(to))
            {
                Nodes.Add(to);
            }

            if (!Edges.Any(e => _comparer.Equals(e.From, from) && _comparer.Equals(e.To, to)))
            {
                Edges.Add(new ResourceGraphEdge(from, to));
            }
        }

        public IEnumerable<IResourceIdentity> Traverse()
        {
            var walker = new ResourceGraphWalker();
            return walker.Walk(this);
        }

        public ResourceGraph ShallowClone()
        {
            var graph = new ResourceGraph();
            foreach (var edge in Edges)
            {
                graph.Connect(edge.From, edge.To);
            }

            foreach (var resource in Resources)
            {
                graph.Add(resource);
            }

            return graph;
        }
    }
}
