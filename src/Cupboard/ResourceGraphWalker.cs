using System;
using System.Collections.Generic;
using System.Linq;

namespace Cupboard.Internal;

internal sealed class ResourceGraphWalker
{
    private readonly ResourceComparer _comparer;

    public ResourceGraphWalker()
    {
        _comparer = new ResourceComparer();
    }

    private class TargetResource : Resource
    {
        public TargetResource()
            : base("__target__")
        {
        }
    }

    public IEnumerable<IResourceIdentity> Walk(ResourceGraph graph)
    {
        // Clone the graph.
        graph = graph.ShallowClone();

        // Sanity check to make sure that all edges exist in the graph.
        foreach (var edge in graph.Edges)
        {
            EnsureResourceExist(graph, edge.From);
            EnsureResourceExist(graph, edge.To);
        }

        // Find all nodes without any edges.
        var orphans = graph.Resources
            .Where(x => !graph.Edges.Any(edge => _comparer.Equals(x, edge.From))
                && !graph.Edges.Any(edge => _comparer.Equals(x, edge.To)))
            .ToArray();

        // Find all leaves in the graph.
        var leaves = graph.Nodes
            .Where(x => graph.Edges.Any(y => _comparer.Equals(x, y.To))
                && !graph.Edges.Any(z => _comparer.Equals(x, z.From)))
            .ToArray();

        // Add an artifical destination node to all leaves.
        // This will be the target to traverse to it's roots.
        var target = new TargetResource();
        foreach (var leaf in leaves.Concat(orphans))
        {
            graph.Connect(leaf, target);
        }

        // Traverse the graph.
        var result = new List<IResourceIdentity>();
        Traverse(graph, target, result);

        // Remove the target node from the results.
        result.RemoveAll(x => x.ResourceType == typeof(TargetResource));

        // Return the result.
        return result;
    }

    private void Traverse(
        ResourceGraph graph,
        IResourceIdentity node,
        ICollection<IResourceIdentity> result,
        ISet<IResourceIdentity>? visited = null)
    {
        visited ??= new HashSet<IResourceIdentity>(_comparer);
        if (!visited.Contains(node))
        {
            visited.Add(node);
            var incoming = graph.Edges.Where(x => _comparer.Equals(x.To, node)).Select(x => x.From);
            foreach (var child in incoming)
            {
                Traverse(graph, child, result, visited);
            }

            result.Add(node);
        }
        else if (!result.Any(x => _comparer.Equals(x, node)))
        {
            throw new InvalidOperationException("Graph contains circular references.");
        }
    }

    private static void EnsureResourceExist(ResourceGraph graph, IResourceIdentity identity)
    {
        if (!graph.Resources.Any(r => r.Name.Equals(identity.Name, StringComparison.OrdinalIgnoreCase)
            && r.ResourceType == identity.ResourceType))
        {
            throw new InvalidOperationException(
                $"Could not find resource '{identity.Name}' of type '{identity.ResourceType.Name}'.");
        }
    }
}
