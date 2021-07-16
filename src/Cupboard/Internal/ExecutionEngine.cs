using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console.Cli;

namespace Cupboard.Internal
{
    internal sealed class ExecutionEngine
    {
        private readonly ResourceProviderRepository _resourceProviders;
        private readonly FactBuilder _factBuilder;
        private readonly List<Catalog> _specifications;
        private readonly List<Manifest> _manifests;
        private readonly ICupboardLogger _logger;

        public ExecutionEngine(
            ResourceProviderRepository resourceProviders,
            FactBuilder factBuilder,
            IEnumerable<Catalog> specifications,
            IEnumerable<Manifest> manifests,
            ICupboardLogger logger)
        {
            _resourceProviders = resourceProviders ?? throw new ArgumentNullException(nameof(resourceProviders));
            _factBuilder = factBuilder ?? throw new ArgumentNullException(nameof(factBuilder));
            _specifications = new List<Catalog>(specifications ?? Array.Empty<Catalog>());
            _manifests = new List<Manifest>(manifests ?? Array.Empty<Manifest>());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Report> Run(IRemainingArguments args, IStatusUpdater status, bool dryRun)
        {
            var facts = _factBuilder.Build(args);
            var ctx = new CatalogContext(facts);

            foreach (var specification in _specifications)
            {
                specification.Execute(ctx);
            }

            var manifests = new List<Manifest>();
            foreach (var usedManifest in ctx.Manifests)
            {
                var manifest = _manifests.SingleOrDefault(m => m.GetType() == usedManifest);
                if (manifest != null)
                {
                    manifests.Add(manifest);
                }
            }

            // Build the resource graph
            var graph = ResourceGraphBuilder.Build(_resourceProviders, manifests, facts);
            graph.Configurations.ForEach(action => action());

            // Build and run the execution plan
            var plan = BuildExecutionPlan(graph, facts);
            if (plan.Count == 0)
            {
                return new Report(Array.Empty<ReportItem>(), facts, plan.RequiresAdministrator, dryRun);
            }

            return await ExecutePlan(plan, facts, status, dryRun).ConfigureAwait(false);
        }

        private async Task<Report> ExecutePlan(ExecutionPlan plan, FactCollection facts, IStatusUpdater status, bool dryRun)
        {
            // Do we need administrator privileges?
            // Make sure we're running with elevated permissions
            if (plan.RequiresAdministrator)
            {
                if (!SecurityUtilities.IsAdministrator() && !dryRun)
                {
                    throw new InvalidOperationException("Not running as administrator");
                }
            }

            // Create the execution context
            var context = new ExecutionContext(facts)
            {
                DryRun = dryRun,
            };

            var results = new List<ReportItem>();
            foreach (var node in plan)
            {
                if (context.DryRun)
                {
                    results.Add(new ReportItem(node.Provider, node.Resource, ResourceState.Unknown, node.RequireAdministrator));
                }
                else
                {
                    status.Update($"Executing [green]{node.Provider.ResourceType.Name}[/]::[blue]{node.Resource.Name}[/]");

                    var state = await node.Provider.RunAsync(context, node.Resource).ConfigureAwait(false);
                    results.Add(new ReportItem(node.Provider, node.Resource, state, node.RequireAdministrator));

                    if (state.IsError() && node.Resource.OnError != ErrorHandling.Ignore)
                    {
                        _logger.Error($"Aborting run due to error in {node.Resource.Name}.");
                        break;
                    }
                }
            }

            _logger.Debug("Execution done.");
            return new Report(results, facts, plan.RequiresAdministrator, context.DryRun);
        }

        internal ExecutionPlan BuildExecutionPlan(ResourceGraph graph, FactCollection facts)
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
