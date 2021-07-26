using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console.Cli;

namespace Cupboard.Internal
{
    internal sealed class ExecutionPlanBuilder
    {
        private readonly ResourceProviderRepository _providers;

        public ExecutionPlanBuilder(ResourceProviderRepository providers)
        {
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));
        }

        public ExecutionPlan Build(
            IEnumerable<Catalog> catalogs,
            IEnumerable<Manifest> manifests,
            IFactBuilder factBuilder,
            IRemainingArguments args)
        {
            // Build facts
            var facts = factBuilder.Build(args);

            // Get all manifests added by catalogs
            var catalogManifests = GetCatalogManifests(catalogs, manifests, facts);

            // Build the resource graph
            var graph = ResourceGraphBuilder.Build(_providers, catalogManifests, facts);
            graph.Configurations.ForEach(action => action());

            // Build the execution plan
            return Build(graph, facts);
        }

        private static IEnumerable<Manifest> GetCatalogManifests(
            IEnumerable<Catalog> catalogs,
            IEnumerable<Manifest> manifests,
            FactCollection facts)
        {
            // Execute all catalogs
            var ctx = new CatalogContext(facts);
            foreach (var catalog in catalogs)
            {
                if (catalog.CanRun(facts))
                {
                    catalog.Execute(ctx);
                }
            }

            var catalogManifests = ctx.GetManifests();
            return manifests.Where(x => catalogManifests.Contains(x.GetType()));
        }

        private ExecutionPlan Build(ResourceGraph graph, FactCollection facts)
        {
            var resources = new List<ExecutionPlanItem>();

            foreach (var node in graph.Traverse())
            {
                // Find the resource.
                var resource = graph.Resources.SingleOrDefault(x => x.Name == node.Name);
                if (resource == null)
                {
                    throw new InvalidOperationException($"Could not find resource '{node.Name}' ({node.ResourceType.Name}).");
                }

                // Get the provider.
                var provider = _providers.GetProvider(resource.ResourceType);
                if (provider == null)
                {
                    throw new InvalidOperationException($"Could not find resource provider for '{node.Name}' ({node.ResourceType.Name}).");
                }

                // Do we need to be administrator for this resource?
                var requireAdministrator = resource.RequireAdministrator || provider.RequireAdministrator(facts);

                resources.Add(new ExecutionPlanItem(provider, resource, requireAdministrator));
            }

            // Do we need to be administrator for this plan?
            var requiresAdministrator = resources.Any(item => item.RequireAdministrator);

            return new ExecutionPlan(resources, requiresAdministrator);
        }
    }
}
