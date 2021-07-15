using System.Threading.Tasks;
using Cupboard.Tests.Resources;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Cupboard.Tests.Unit.Resources
{
    public sealed class ChocolateyProviderTests
    {
        public sealed class IfPackageShouldBeInstalled
        {
            [Fact]
            public async Task Should_Install_Package_If_Missing()
            {
                // Given
                var runner = new FakeProcessRunnerFixture();
                var logger = Substitute.For<ICupboardLogger>();
                var refresher = Substitute.For<IEnvironmentRefresher>();
                var provider = new ChocolateyPackageProvider(runner.Runner, refresher, logger);
                var package = new ChocolateyPackage("Visual Studio Code")
                {
                    Package = "vscode",
                    Ensure = PackageState.Installed,
                };

                runner.Register("choco", "list -lo",
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\n1 package installed."),
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\nvscode 1.58.0\n2 packages installed."));

                runner.Register("choco", "install vscode -y",
                    new ProcessRunnerResult(0, "Lol installed VSCode"));

                // When
                var result = await provider.RunAsync(new ExecutionContext(new FactCollection()), package);

                // Then
                result.ShouldBe(ResourceState.Changed);
                logger.Received(1).Log(Verbosity.Normal, LogLevel.Information, "The Chocolatey package [yellow]vscode[/] was installed");
            }

            [Fact]
            public async Task Should_Not_Install_Package_If_Present()
            {
                // Given
                var runner = new FakeProcessRunnerFixture();
                var logger = Substitute.For<ICupboardLogger>();
                var refresher = Substitute.For<IEnvironmentRefresher>();
                var provider = new ChocolateyPackageProvider(runner.Runner, refresher, logger);
                var package = new ChocolateyPackage("Visual Studio Code")
                {
                    Package = "vscode",
                    Ensure = PackageState.Installed,
                };

                runner.Register("choco", "list -lo",
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\nvscode 1.58.0\n2 packages installed."),
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\nvscode 1.58.0\n2 packages installed."));

                runner.Register("choco", "install vscode -y",
                    new ProcessRunnerResult(0, "Lol installed VSCode"));

                // When
                var result = await provider.RunAsync(new ExecutionContext(new FactCollection()), package);

                // Then
                result.ShouldBe(ResourceState.Unchanged);
                logger.Received(1).Log(Verbosity.Diagnostic, LogLevel.Debug, "The Chocolatey package [yellow]vscode[/] is already installed");
            }
        }

        public sealed class IfPackageShouldNotBeInstalled
        {
            [Fact]
            public async Task Should_Uninstall_Package_If_Present()
            {
                // Given
                var runner = new FakeProcessRunnerFixture();
                var logger = Substitute.For<ICupboardLogger>();
                var refresher = Substitute.For<IEnvironmentRefresher>();
                var provider = new ChocolateyPackageProvider(runner.Runner, refresher, logger);
                var package = new ChocolateyPackage("Visual Studio Code")
                {
                    Package = "vscode",
                    Ensure = PackageState.Uninstalled,
                };

                runner.Register("choco", "list -lo",
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\nvscode 1.58.0\n2 packages installed."),
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\n1 package installed."));

                runner.Register("choco", "uninstall vscode",
                    new ProcessRunnerResult(0, "Lol uninstalled VSCode"));

                // When
                var result = await provider.RunAsync(new ExecutionContext(new FactCollection()), package);

                // Then
                result.ShouldBe(ResourceState.Changed);
                logger.Received(1).Log(Verbosity.Normal, LogLevel.Information, "The Chocolatey package [yellow]vscode[/] was uninstalled");
            }

            [Fact]
            public async Task Should_Not_Uninstall_Package_If_Absent()
            {
                // Given
                var runner = new FakeProcessRunnerFixture();
                var logger = Substitute.For<ICupboardLogger>();
                var refresher = Substitute.For<IEnvironmentRefresher>();
                var provider = new ChocolateyPackageProvider(runner.Runner, refresher, logger);
                var package = new ChocolateyPackage("Visual Studio Code")
                {
                    Package = "vscode",
                    Ensure = PackageState.Uninstalled,
                };

                runner.Register("choco", "list -lo",
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\n1 package installed."));

                runner.Register("choco", "uninstall vscode",
                    new ProcessRunnerResult(0, "Lol uninstalled VSCode"));

                // When
                var result = await provider.RunAsync(new ExecutionContext(new FactCollection()), package);

                // Then
                result.ShouldBe(ResourceState.Unchanged);
                logger.Received(1).Log(Verbosity.Diagnostic, LogLevel.Debug, "The Chocolatey package [yellow]vscode[/] is already uninstalled");
            }
        }
    }
}
