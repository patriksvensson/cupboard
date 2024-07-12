namespace Cupboard;

public sealed class VSCodeExtensionProvider : PackageInstallerProvider<VSCodeExtension>
{
    private readonly IProcessRunner _runner;
    private readonly ICupboardEnvironment _environment;

    protected override bool ShouldCache { get; } = true;
    protected override string Name { get; } = "VSCode";
    protected override string Kind { get; } = "extension";

    public VSCodeExtensionProvider(
        IProcessRunner runner,
        ICupboardEnvironment environment,
        ICupboardLogger logger,
        IEnvironmentRefresher refresher)
            : base(logger, refresher)
    {
        _runner = runner ?? throw new ArgumentNullException(nameof(runner));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    }

    public override VSCodeExtension Create(string name)
    {
        return new VSCodeExtension(name)
        {
            Package = name,
        };
    }

    protected override bool IsPackageInstalled(VSCodeExtension resource, string output)
    {
        return output.Contains(resource.Package, StringComparison.OrdinalIgnoreCase);
    }

    protected override async Task<ProcessRunnerResult> GetPackageState(VSCodeExtension resource)
    {
        var executable = GetCodeExecutable();
        return await _runner.Run(executable, "--list-extensions", supressOutput: true).ConfigureAwait(false);
    }

    protected override async Task<ProcessRunnerResult> InstallPackage(VSCodeExtension resource)
    {
        var executable = GetCodeExecutable();
        var arguments = $"--install-extension {resource.Package}";
        return await _runner.Run(executable, arguments).ConfigureAwait(false);
    }

    protected override async Task<ProcessRunnerResult> UninstallPackage(VSCodeExtension resource)
    {
        var executable = GetCodeExecutable();
        var arguments = $"--uninstall-extension {resource.Package}";
        return await _runner.Run(executable, arguments).ConfigureAwait(false);
    }

    private string GetCodeExecutable()
    {
        if (_environment.Platform.Family == PlatformFamily.Windows)
        {
            // TODO 2021-07-14: This is not good
            return "C:/Program Files/Microsoft VS Code/bin/code.cmd";
        }
        else
        {
            return "code";
        }
    }
}
