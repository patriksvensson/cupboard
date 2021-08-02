using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.IO;

namespace Cupboard.Internal
{
    internal sealed class RunCommand : AsyncCommand<RunCommand.Settings>
    {
        private readonly ExecutionEngine _executor;
        private readonly ReportRenderer _renderer;
        private readonly ICupboardLogger _logger;
        private readonly ICupboardFileSystem _fileSystem;
        private readonly ICupboardEnvironment _environment;
        private readonly ISecurityPrincipal _security;
        private readonly IAnsiConsole _console;

        public sealed class Settings : CommandSettings
        {
            [CommandOption("--dryrun")]
            [Description("Performs a dry run of the configuration.")]
            public bool DryRun { get; set; }

            [CommandOption("--debug")]
            [Description("Launches a debugger when starting up.")]
            public bool Debug { get; set; }

            [CommandOption("-y|--yes|--confirm")]
            [Description("Confirm all prompts. Chooses affirmative answer instead of prompting.")]
            public bool AutoConfirm { get; set; }

            [CommandOption("--ignore-reboots")]
            [Description("Ignore any pending reboots.")]
            public bool IgnoreReboots { get; set; }

            [CommandOption("-w|--working|--workingdir")]
            [Description("Sets the working directory.")]
            [TypeConverter(typeof(DirectoryPathConverter))]
            public DirectoryPath? WorkingDirectory { get; set; }

            [CommandOption("-v|--verbosity")]
            [DefaultValue(Verbosity.Normal)]
            [TypeConverter(typeof(VerbosityConverter))]
            [Description(
                "Specifies the amount of information to be displayed.\n"
                + "([blue]q[/]uiet, [blue]m[/]inimal, [blue]n[/]ormal, "
                + "[blue]v[/]erbose, [blue]d[/]iagnostic)")]
            public Verbosity Verbosity { get; set; }
        }

        public RunCommand(
            ExecutionEngine executor,
            ReportRenderer renderer,
            ICupboardLogger logger,
            ICupboardFileSystem fileSystem,
            ICupboardEnvironment environment,
            ISecurityPrincipal security,
            IAnsiConsole console)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _security = security ?? throw new ArgumentNullException(nameof(security));
            _console = console ?? throw new ArgumentNullException(nameof(console));
        }

        public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            if (settings.WorkingDirectory != null)
            {
                settings.WorkingDirectory = settings.WorkingDirectory.MakeAbsolute(_environment);
                if (!_fileSystem.Exist(settings.WorkingDirectory))
                {
                    return ValidationResult.Error("The specified working directory does not exist");
                }
            }

            return ValidationResult.Success();
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            else if (settings is null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (settings.Debug)
            {
                if (!Debugger.IsAttached)
                {
                    Debugger.Launch();
                }
            }

            _logger.SetVerbosity(settings.Verbosity);

            if (settings.WorkingDirectory != null)
            {
                _environment.SetWorkingDirectory(settings.WorkingDirectory);
            }

            var report = await Run(context, settings).ConfigureAwait(false);
            if (report == null)
            {
                return -1;
            }

            if (report.Items.Count == 0)
            {
                _console.MarkupLine("[red]Error:[/] No manifest were found.");
                _console.MarkupLine("Have you added a [yellow]Catalog[/]?");
                _console.WriteLine();
                _console.MarkupLine("Write [grey]dotnet run -- --help[/] for more information.");

                return -1;
            }

            _renderer.Render(report, _logger.Verbosity);
            return report.Successful ? 0 : -1;
        }

        private async Task<Report?> Run(CommandContext context, Settings settings)
        {
            if (settings.DryRun)
            {
                return await _executor.Run(context.Remaining, new DummyUpdater(),
                    dryRun: true, ignoreReboots: settings.IgnoreReboots).ConfigureAwait(false);
            }
            else
            {
                if (!settings.AutoConfirm)
                {
                    var report = await _executor.Run(
                        context.Remaining, new DummyUpdater(), dryRun: true,
                        ignoreReboots: settings.IgnoreReboots).ConfigureAwait(false);

                    if (report.Items.Count == 0)
                    {
                        return report;
                    }

                    _renderer.Render(report, _logger.Verbosity);
                    _console.WriteLine();

                    if (report.RequiresAdministrator)
                    {
                        if (!_security.IsAdministrator())
                        {
                            _console.MarkupLine("[red]ERROR:[/] The current execution plan [yellow]require administrative permissions[/].");
                            _console.MarkupLine("[grey]Restart the application as administrator to execute plan.[/]");
                            return null;
                        }
                    }

                    if (report.PendingReboot)
                    {
                        if (settings.IgnoreReboots)
                        {
                            _console.MarkupLine("[yellow]WARNING:[/] A pending reboot have been detected.");
                        }
                        else
                        {
                            _console.MarkupLine("[red]ERROR:[/] A pending reboot have been detected.");
                            _console.MarkupLine("[grey]To ignore, pass the [yellow]--ignore-reboots[/] flag.[/]");
                            return null;
                        }
                    }

                    _console.MarkupLine("[yellow]WARNING[/]: This will change the state of the current machine.");
                    if (!_console.Confirm("Are you sure you want to continue?", defaultValue: false))
                    {
                        return null;
                    }
                }

                return await _console.Status().StartAsync("Executing", async status =>
                {
                    return await _executor.Run(
                        context.Remaining, new StatusUpdater(status), dryRun: false,
                        ignoreReboots: settings.IgnoreReboots).ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
        }
    }
}
