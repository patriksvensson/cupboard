using System;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.EventStream;
using Spectre.Console;

namespace Cupboard.Internal;

internal sealed class ProcessRunner : IProcessRunner
{
    private readonly ICupboardLogger _logger;

    public ProcessRunner(ICupboardLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProcessRunnerResult> Run(string file, string? arguments = null, Func<string, bool>? filter = null, bool supressOutput = false)
    {
        var cli = Cli.Wrap(file);
        cli = cli.WithValidation(CommandResultValidation.None);

        if (arguments != null)
        {
            cli = cli.WithArguments(arguments);
        }

        var standardOut = new StringBuilder();
        var standardError = new StringBuilder();
        var exitCode = -1;

        await foreach (var cmdEvent in cli.ListenAsync())
        {
            switch (cmdEvent)
            {
                case StandardOutputCommandEvent output:
                    standardOut.Append(output.Text);
                    if (!supressOutput && !string.IsNullOrWhiteSpace(output.Text) && (filter?.Invoke(output.Text) ?? true))
                    {
                        _logger.Verbose("OUT>", output.Text.EscapeMarkup().TrimStart());
                    }

                    break;
                case StandardErrorCommandEvent error:
                    if (!supressOutput && !string.IsNullOrWhiteSpace(error.Text) && (filter?.Invoke(error.Text) ?? true))
                    {
                        _logger.Error("ERR>", error.Text.EscapeMarkup().TrimStart());
                    }

                    standardError.Append(error.Text);
                    break;
                case ExitedCommandEvent exited:
                    exitCode = exited.ExitCode;
                    break;
            }
        }

        return new ProcessRunnerResult(
            exitCode,
            standardOut.ToString(),
            standardError.ToString());
    }
}
