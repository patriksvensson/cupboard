using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Cupboard.Internal
{
    internal sealed class ReportRenderer
    {
        private readonly IAnsiConsole _console;

        public ReportRenderer(IAnsiConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public void Render(Report report, Verbosity verbosity)
        {
            var table = new Table().Expand().BorderColor(Color.Grey).MinimalBorder();
            table.AddColumns("[grey]#[/]", "[grey]Provider[/]", "[grey]Resource[/]");

            if (!report.DryRun)
            {
                table.AddColumn("Status");
            }

            var index = 1;
            foreach (var item in report)
            {
                var resourceName = "[green]" + item.Provider.ResourceType.Name + "[/]";
                if (item.RequireAdministrator && report.DryRun)
                {
                    resourceName += "[yellow]*[/]";
                }

                var columns = new List<IRenderable>
                {
                    new Text(index.ToString(CultureInfo.InvariantCulture), new Style(foreground: Color.Grey)),
                    new Markup(resourceName),
                    new Text(item.Resource.Name, new Style(foreground: Color.Blue)),
                };

                if (!report.DryRun)
                {
                    if (verbosity < Verbosity.Verbose)
                    {
                        if (item.State == ResourceState.Unchanged)
                        {
                            continue;
                        }
                    }

                    var color = item.State switch
                    {
                        ResourceState.Changed => Color.Green,
                        ResourceState.Unchanged => Color.Grey,
                        ResourceState.Skipped => Color.Yellow,
                        ResourceState.Error => Color.Red,
                        _ => Color.Default,
                    };

                    columns.Add(new Text(item.State.ToString(), new Style(foreground: color)));
                }

                table.AddRow(columns.ToArray());
                index++;
            }

            _console.WriteLine();
            _console.Write(
                new Panel(table)
                    .Padding(1, 1)
                    .Expand()
                    .Header(report.DryRun ? "[blue]Plan[/]" : "[blue]Report[/]")
                    .BorderColor(Color.Grey));

            if (!report.DryRun)
            {
                var chart = new BreakdownChart();
                var grouped = report.Items.GroupBy(x => x.State);
                var colors = new ColorPalette<ResourceState>(grouped.Select(g => g.Key));

                foreach (var group in grouped)
                {
                    chart.AddItem(group.Key.ToString(), group.Count(), GetColor(group.Key));
                }

                _console.Write(
                    new Panel(chart)
                        .Padding(2, 1)
                        .Header("Summary")
                        .BorderColor(Color.Grey));
            }

            if (report.DryRun)
            {
                var breakdown = new BreakdownChart();
                var groupedTypes = report.Items.GroupBy(x => x.Resource.GetType());
                var colors = new ColorPalette<Type>(groupedTypes.Select(g => g.Key));

                foreach (var group in groupedTypes)
                {
                    breakdown.AddItem(group.Key.Name, group.Count(), colors.GetColor(group.Key));
                }

                _console.Write(
                    new Panel(breakdown)
                        .Padding(2, 1)
                        .Header("Breakdown")
                        .BorderColor(Color.Grey));
            }
        }

        private static Color GetColor(ResourceState state)
        {
            return state switch
            {
                ResourceState.Unknown => Color.Grey,
                ResourceState.Changed => Color.Green,
                ResourceState.Unchanged => Color.Silver,
                ResourceState.Skipped => Color.Yellow,
                ResourceState.Error => Color.Red,
                _ => Color.Aqua,
            };
        }
    }
}
