using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spectre.Console.Cli;

namespace Cupboard.Internal;

internal sealed class ExecutionEngine
{
    private readonly IFactBuilder _factBuilder;
    private readonly ExecutionPlanBuilder _executionPlanBuilder;
    private readonly ISecurityPrincipal _security;
    private readonly IRebootDetector _reboot;
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
        IRebootDetector reboot,
        ICupboardLogger logger,
        IReportSubscriber? subscriber = null)
    {
        _factBuilder = factBuilder ?? throw new ArgumentNullException(nameof(factBuilder));
        _executionPlanBuilder = executionPlanBuilder ?? throw new ArgumentNullException(nameof(executionPlanBuilder));
        _security = security ?? throw new ArgumentNullException(nameof(security));
        _reboot = reboot ?? throw new ArgumentNullException(nameof(reboot));
        _specifications = new List<Catalog>(specifications ?? Array.Empty<Catalog>());
        _manifests = new List<Manifest>(manifests ?? Array.Empty<Manifest>());
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _subscriber = subscriber;
    }

    public async Task<Report> Run(
        IRemainingArguments args,
        IStatusUpdater status,
        IExecutionController controller,
        bool dryRun, bool ignoreReboots)
    {
        var facts = _factBuilder.Build(args);

        // Build and execute the plan
        var plan = _executionPlanBuilder.Build(_specifications, _manifests, _factBuilder, args);
        var report = await ExecutePlan(plan, facts, status, controller, dryRun, ignoreReboots).ConfigureAwait(false);

        // Notify any subscribers about the plan
        _subscriber?.Notify(report);

        return report;
    }

    private async Task<Report> ExecutePlan(
        ExecutionPlan plan, FactCollection facts,
        IStatusUpdater status,
        IExecutionController controller,
        bool dryRun, bool ignoreReboots)
    {
        var pendingReboot = _reboot.HasPendingReboot();

        if (plan.Count == 0)
        {
            return new Report(Array.Empty<ReportItem>(), facts, plan.RequiresAdministrator, dryRun, pendingReboot);
        }

        // Do we need administrator privileges?
        // Make sure we're running with elevated permissions
        if (!dryRun && plan.RequiresAdministrator && !_security.IsAdministrator())
        {
            throw new InvalidOperationException("Not running as administrator");
        }

        // Is there a pending reboot?
        if (!dryRun && pendingReboot && !ignoreReboots)
        {
            throw new InvalidOperationException("A pending reboot have been detected");
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
                var request = controller.GetAction(node.Provider, node.Resource);
                if (request == ExecutionAction.Skip)
                {
                    _logger.Error("[yellow]Skipping resource[/]");
                    results.Add(new ReportItem(node.Provider, node.Resource, ResourceState.ManuallySkipped, node.RequireAdministrator));
                    continue;
                }
                else if (request == ExecutionAction.Abort)
                {
                    _logger.Error("Aborting run");
                    break;
                }

                var result = await status.Update($"Executing [green]{node.Provider.ResourceType.Name}[/]::[blue]{node.Resource.Name}[/]", async () =>
                {
                    _logger.Verbose($"Executing [green]{node.Provider.ResourceType.Name}[/]::[blue]{node.Resource.Name}[/]");

                    // Abort if we cannot run a provider
                    if (!node.Provider.CanRun(facts))
                    {
                        _logger.Error($"The resource [green]{node.Provider.ResourceType.Name}[/]::[blue]{node.Resource.Name}[/] cannot be run");
                        return false;
                    }

                    // Run the provider
                    var state = await node.Provider.RunAsync(context, node.Resource).ConfigureAwait(false);
                    results.Add(new ReportItem(node.Provider, node.Resource, state, node.RequireAdministrator));

                    if (state.IsError() && node.Resource.Error != ErrorOptions.IgnoreErrors)
                    {
                        _logger.Error($"Aborting run due to error in [green]{node.Provider.ResourceType.Name}[/]::[blue]{node.Resource.Name}[/]");
                        return false;
                    }

                    // Pending reboot?
                    if (_reboot.HasPendingReboot())
                    {
                        if (!pendingReboot)
                        {
                            _logger.Warning("A pending reboot has been detected");
                        }

                        pendingReboot = true;
                        if (node.Resource.Reboot != RebootOptions.IgnorePendingReboot)
                        {
                            if (!ignoreReboots)
                            {
                                _logger.Information("Aborting due to pending reboot.");
                                return false;
                            }
                        }
                    }

                    return true;
                }).ConfigureAwait(false);
            }
        }

        _logger.Debug("Execution done.");
        return new Report(results, facts, plan.RequiresAdministrator, context.DryRun, pendingReboot);
    }
}