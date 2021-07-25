using System;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.EventStream;

namespace Cupboard.Internal
{
    internal sealed class ProcessRunner : IProcessRunner
    {
        public async Task<ProcessRunnerResult> Run(string file, string? arguments = null, Action<CommandEvent>? handler = null)
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
                handler?.Invoke(cmdEvent);

                switch (cmdEvent)
                {
                    case StandardOutputCommandEvent stdOut:
                        standardOut.Append(stdOut.Text);
                        break;
                    case StandardErrorCommandEvent stdErr:
                        standardOut.Append(stdErr.Text);
                        break;
                    case ExitedCommandEvent exited:
                        exitCode = exited.ExitCode;
                        break;
                }
            }

            return new ProcessRunnerResult(exitCode, standardOut.ToString());
        }
    }
}
