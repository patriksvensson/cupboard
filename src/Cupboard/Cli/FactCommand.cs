namespace Cupboard;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
internal sealed class FactCommand : Command<FactCommand.Settings>
{
    private readonly IFactBuilder _builder;
    private readonly IAnsiConsole _console;

    public sealed class Settings : CommandSettings
    {
        [CommandOption("--env")]
        public bool Environment { get; set; }
    }

    public FactCommand(IFactBuilder builder, IAnsiConsole console)
    {
        _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public override int Execute(CommandContext context, Settings settings)
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
