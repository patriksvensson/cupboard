using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cupboard.Internal
{
    internal sealed class FactCommand : Command<FactCommand.Settings>
    {
        private readonly FactBuilder _builder;
        private readonly IAnsiConsole _console;

        public sealed class Settings : CommandSettings
        {
            [CommandOption("--env")]
            public bool Environment { get; set; }
        }

        public FactCommand(FactBuilder builder, IAnsiConsole console)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var facts = _builder.Build(context.Remaining);

            var table = new Table().BorderColor(Color.Grey);
            table.AddColumns("[grey]Path[/]", "[grey]Type[/]", "[grey]Value[/]");

            foreach (var fact in facts.OrderBy(f => f.FullName))
            {
                if (fact.FullName.StartsWith("env.", StringComparison.OrdinalIgnoreCase) && !settings.Environment)
                {
                    continue;
                }

                var value = fact.Value?.ToString() ?? string.Empty;
                value = value.Replace("\u001b", "ESC");

                table.AddRow(
                    fact.FullName,
                    fact.Value?.GetType().Name ?? "?",
                    "[grey]" + value.EscapeMarkup() + "[/]");
            }

            _console.Write(table);

            return 0;
        }
    }
}
