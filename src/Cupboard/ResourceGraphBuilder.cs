namespace Cupboard;

internal static class ResourceGraphBuilder
{
    public static ResourceGraph Build(
        ResourceProviderRepository repository,
        IEnumerable<Manifest> manifests,
        FactCollection facts)
    {
        // Execute all manifests.
        var ctx = new ManifestContext(facts);
        foreach (var manifest in manifests)
        {
            manifest.Execute(ctx);
        }

        // Build the graph using the builders in the context.
        var graph = new ResourceGraph();
        foreach (var builder in ctx.Builders)
        {
            var provider = repository.GetProvider(builder.Type);
            if (provider == null)
            {
                throw new InvalidOperationException($"Could not find resource provider for '{builder.Name}' ({builder.Type.Name}).");
            }

            // Create the resource.
            var resource = provider.Create(builder.Name);

            // Add the resource to the graph.
            if (!graph.Resources.Add(resource))
            {
                // TODO 2021-07-11: Log that the resource was skipped
                continue;
            }

            // Connect dependees to resource.
            foreach (var before in builder.RunBefore)
            {
                graph.Connect(new ResourceIdentity(resource), new ResourceIdentity(before));
            }

            // Connect resource to dependencies.
            foreach (var after in builder.RunAfter)
            {
                graph.Connect(new ResourceIdentity(after), new ResourceIdentity(resource));
            }

            // Run configuration.
            foreach (var configuration in builder.Configurations)
            {
                graph.Configurations.Add(() => configuration(resource));
            }
        }

        // Return the graph.
        return graph;
    }
}
