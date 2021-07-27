using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spectre.Console.Cli;

namespace Cupboard.Internal
{
    internal sealed class ExecutionEngine
    {
        private readonly IFactBuilder _factBuilder;
        private readonly ExecutionPlanBuilder _executionPlanBuilder;
        private readonly ISecurityPrincipal _security;
        private readonly List<Catalog> _specifications;
        private readonly List<Manifest> _manifests;
        private readonly ICupboardLogger _logger;
        private readonly IReportSubscriber? _subscriber;

        public ExecutionEngine(
            IFactBuilder factBuilder,
            IEnumerable<Catalog> specifications,
            IEnumerable<Manifest> manifests,
            ExecutionPlanBuilder executionPlanBuilder,
            ISecurityPrincipal security,
            ICupboardLogger logger,
            IReportSubscriber? subscriber = null)
        {
            _factBuilder = factBuilder ?? throw new ArgumentNullException(nameof(factBuilder));
            _executionPlanBuilder = executionPlanBuilder ?? throw new ArgumentNullException(nameof(executionPlanBuilder));
            _security = security ?? throw new ArgumentNullException(nameof(security));
            _specifications = new List<Catalog>(specifications ?? Array.Empty<Catalog>());
            _manifests = new List<Manifest>(manifests ?? Array.Empty<Manifest>());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriber = subscriber;
        }

        public async Task<Report> Run(IRemainingArguments args, IStatusUpdater status, bool dryRun)
        {
            var facts = _factBuilder.Build(args);

            // Build and execute the plan
            var plan = _executionPlanBuilder.Build(_specifications, _manifests, _factBuilder, args);
            var report = await ExecutePlan(plan, facts, status, dryRun).ConfigureAwait(false);

            // Notify any subscribers about the plan
            _subscriber?.Notify(report);

            return report;
        }

        private async Task<Report> ExecutePlan(ExecutionPlan plan, FactCollection facts, IStatusUpdater status, bool dryRun)
        {
            if (plan.Count == 0)
            {
                return new Report(Array.Empty<ReportItem>(), facts, plan.RequiresAdministrator, dryRun);
            }

            // Do we need administrator privileges?
            // Make sure we're running with elevated permissions
            if (plan.RequiresAdministrator && !_security.IsAdministrator() && !dryRun)
            {
                throw new InvalidOperationException("Not running as administrator");
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
                    _logger.Verbose($"Executing [green]{node.Provider.ResourceType.Name}[/]::[blue]{node.Resource.Name}[/]");

                    // Abort if we cannot run a provider
                    if (!node.Provider.CanRun(facts))
                    {
                        _logger.Error($"The resource [green]{node.Provider.ResourceType.Name}[/]::[blue]{node.Resource.Name}[/] cannot be run");
                        break;
                    }

                    // Run the provider
                    var state = await node.Provider.RunAsync(context, node.Resource).ConfigureAwait(false);
                    results.Add(new ReportItem(node.Provider, node.Resource, state, node.RequireAdministrator));

                    if (state.IsError() && node.Resource.OnError != ErrorHandling.Ignore)
                    {
                        _logger.Error($"Aborting run due to error in [green]{node.Provider.ResourceType.Name}[/]::[blue]{node.Resource.Name}[/].");
                        break;
                    }
                }
            }

            _logger.Debug("Execution done.");
            return new Report(results, facts, plan.RequiresAdministrator, context.DryRun);
        }
    }
}
