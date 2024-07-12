namespace Cupboard;

public sealed class ChocolateyPackageProvider : PackageInstallerProvider<ChocolateyPackage>
{
    private readonly IProcessRunner _runner;

    protected override string Name { get; } = "Chocolatey";

    public ChocolateyPackageProvider(
        IProcessRunner runner,
        ICupboardLogger logger,
        IEnvironmentRefresher refresher)
            : base(logger, refresher)
    {
        _runner = runner ?? throw new ArgumentNullException(nameof(runner));
    }

    public override ChocolateyPackage Create(string name)
    {
        return new ChocolateyPackage(name)
        {
            Package = name,
        };
    }

    protected override bool IsPackageInstalled(ChocolateyPackage resource, string output)
    {
        var outputSpan = output.AsSpan();
        var containsPackage = outputSpan.Contains(resource.Package.AsSpan(), StringComparison.OrdinalIgnoreCase);
        if (string.IsNullOrEmpty(resource.PackageVersion))
        {
            return containsPackage;
        }

        if (!containsPackage)
        {
            return false;
        }

        var indexOfPipe = outputSpan.IndexOf('|');
        if (indexOfPipe == -1)
        {
            return false;
        }

        var indexOfPipePlusOne = indexOfPipe + 1;
        var indexOfNewLine = outputSpan.IndexOfAny('\r', '\n');
        if (indexOfNewLine == -1)
        {
            indexOfNewLine = outputSpan.Length;
        }

        var versionSpan = outputSpan[indexOfPipePlusOne..indexOfNewLine];
        return Version.TryParse(versionSpan, out var currentPackageVersion)
               && Version.TryParse(resource.PackageVersion.AsSpan(), out var packageVersion)
               && ((resource.AllowDowngrade is true && currentPackageVersion == packageVersion)
                   || (resource.AllowDowngrade is false && currentPackageVersion >= packageVersion));
    }

    protected override async Task<ProcessRunnerResult> GetPackageState(ChocolateyPackage resource)
    {
        var arguments = $"list --limit-output --local-only --exact {resource.Package}";
        return await _runner.Run("choco", arguments, supressOutput: true).ConfigureAwait(false);
    }

    protected override async Task<ProcessRunnerResult> InstallPackage(ChocolateyPackage resource)
    {
        var arguments = $"install {resource.Package} -y";

        if (resource.PreRelease)
        {
            arguments += " --pre";
        }

        if (resource.IgnoreChecksum)
        {
            arguments += " --ignore-checksum";
        }

        if (resource.AllowDowngrade)
        {
            arguments += " --allow-downgrade";
        }

        if (!string.IsNullOrWhiteSpace(resource.PackageVersion))
        {
            arguments += $" --version {resource.PackageVersion}";
        }

        if (!string.IsNullOrWhiteSpace(resource.PackageParameters))
        {
            arguments += $" --package-parameters=\"{resource.PackageParameters}\"";
        }

        return await _runner.Run("choco", arguments, t => !t.StartsWith("Progress:")).ConfigureAwait(false);
    }

    protected override async Task<ProcessRunnerResult> UninstallPackage(ChocolateyPackage resource)
    {
        return await _runner.Run("choco", $"uninstall {resource.Package}").ConfigureAwait(false);
    }
}
