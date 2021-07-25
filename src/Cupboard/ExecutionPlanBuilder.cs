using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console.Cli;

namespace Cupboard.Internal
{
    internal sealed class ExecutionPlanBuilder
    {
        private readonly ResourceProviderRepository _resourceProviders;

        public ExecutionPlanBuilder(ResourceProviderRepository resourceProviders)
        {
            _resourceProviders = resourceProviders ?? throw new ArgumentNullException(nameof(resourceProviders));
        }

        public ExecutionPlan Build(
            IEnumerable<Catalog> catalogs,
            IEnumerable<Manifest> manifests,
            IFactBuilder factBuilder,
            IRemainingArguments args)
        {
            var facts = factBuilder.Build(args);
            var ctx = new CatalogContext(facts);

            // Execute all catalogs
            foreach (var catalog in catalogs)
            {
                if (catalog.CanRun(facts))
                {
                    catalog.Execute(ctx);
                }
            }

            // Get all used manifests
            var usedManifests = ctx.GetAddedManifests(manifests);

            // Build the resource graph
            var graph = ResourceGraphBuilder.Build(_resourceProviders, usedManifests, facts);
            graph.Configurations.ForEach(action => action());

            // Build the execution plan
            return Build(graph, facts);
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
                var provider = _resourceProviders.GetProvider(resource.ResourceType);
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
