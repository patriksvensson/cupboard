namespace Cupboard;

internal sealed class CupboardLogger : ICupboardLogger
{
    private readonly IAnsiConsole _console;

    public Verbosity Verbosity { get; private set; }

    public CupboardLogger(IAnsiConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
        Verbosity = Verbosity.Normal;
    }

    public void SetVerbosity(Verbosity verbosity)
    {
        Verbosity = verbosity;
    }

    public void Log(Verbosity verbosity, LogLevel level, string text)
    {
        if (verbosity > Verbosity)
        {
            return;
        }

        switch (level)
        {
            case LogLevel.Fatal:
            case LogLevel.Error:
                _console.MarkupLine($"[red]{text}[/]");
                break;
            case LogLevel.Warning:
                _console.MarkupLine($"[yellow]{text}[/]");
                break;
            case LogLevel.Information:
                _console.MarkupLine($"{text}");
                break;
            case LogLevel.Verbose:
            case LogLevel.Debug:
                _console.MarkupLine($"[grey]{text}[/]");
                break;
        }
    }

    public void Log(Verbosity verbosity, LogLevel level, string title, string text)
    {
        if (verbosity > Verbosity)
        {
            return;
        }

        switch (level)
        {
            case LogLevel.Fatal:
            case LogLevel.Error:
                WriteGrid($"[red]{title}[/]", $"[red]{text}[/]");
                break;
            case LogLevel.Warning:
                WriteGrid($"[yellow]{title}[/]", $"[red]{text}[/]");
                break;
            case LogLevel.Information:
                WriteGrid(title, text);
                break;
            case LogLevel.Verbose:
            case LogLevel.Debug:
                WriteGrid($"[grey]{title}[/]", $"[grey]{text}[/]");
                break;
        }
    }

    private void WriteGrid(string title, string text)
    {
        _console.Write(new Grid()
            .AddColumn(new GridColumn().PadRight(1))
            .AddColumn()
            .AddRow(title, text));
    }
}
