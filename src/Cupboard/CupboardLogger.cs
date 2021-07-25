using System;
using Spectre.Console;

namespace Cupboard.Internal
{
    internal sealed class CupboardLogger : ICupboardLogger
    {
        private readonly IAnsiConsole _console;
        private Verbosity _verbosity;

        public Verbosity Verbosity => _verbosity;

        public CupboardLogger(IAnsiConsole console)
        {
            _console = console ?? throw new ArgumentNullException(nameof(console));
            _verbosity = Verbosity.Normal;
        }

        public void SetVerbosity(Verbosity verbosity)
        {
            _verbosity = verbosity;
        }

        public void Log(Verbosity verbosity, LogLevel level, string markup)
        {
            if (verbosity > _verbosity)
            {
                return;
            }

            switch (level)
            {
                case LogLevel.Fatal:
                case LogLevel.Error:
                    _console.MarkupLine($"[red]{markup}[/]");
                    break;
                case LogLevel.Warning:
                    _console.MarkupLine($"[yellow]{markup}[/]");
                    break;
                case LogLevel.Information:
                    _console.MarkupLine($"{markup}");
                    break;
                case LogLevel.Verbose:
                case LogLevel.Debug:
                    _console.MarkupLine($"[grey]{markup}[/]");
                    break;
            }
        }
    }
}
