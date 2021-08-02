using Cupboard.Testing;
using Shouldly;

namespace Cupboard.Tests.Unit.Providers
{
    public sealed class ChocolateyProviderTests
    {
        public sealed class Manifests
        {
            public sealed class Install : Manifest
            {
                public override void Execute(ManifestContext context)
                {
                    context.Resource<ChocolateyPackage>("Visual Studio Code")
                        .Package("vscode")
                        .Ensure(PackageState.Installed);
                }
            }

            public sealed class Uninstall : Manifest
            {
                public override void Execute(ManifestContext context)
                {
                    context.Resource<ChocolateyPackage>("Visual Studio Code")
                        .Package("vscode")
                        .Ensure(PackageState.Uninstalled);
                }
            }
        }

        public sealed class EnsureInstalled
        {
            [WindowsFact]
            public void Should_Install_Package_If_Missing()
            {
                // Given
                var fixture = new CupboardFixture();
                fixture.Security.IsAdmin = true;
                fixture.Process.RegisterDefaultResult(new ProcessRunnerResult(0));
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Install>());

                fixture.Process.Register("choco", "list -lo",
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\n1 package installed."),
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\nvscode 1.58.0\n2 packages installed."));

                fixture.Process.Register("choco", "install vscode -y",
                    new ProcessRunnerResult(0, "Lol installed VSCode"));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<ChocolateyPackage>("Visual Studio Code").ShouldBe(ResourceState.Changed);
                fixture.Logger.WasLogged("Installing Chocolatey package [yellow]vscode[/]");
                fixture.Logger.WasLogged("The Chocolatey package [yellow]vscode[/] was installed");
            }

            [WindowsFact]
            public void Should_Not_Install_Package_If_Present()
            {
                // Given
                var fixture = new CupboardFixture();
                fixture.Security.IsAdmin = true;
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Install>());

                fixture.Process.Register("choco", "list -lo",
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\nvscode 1.58.0\n2 packages installed."));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<ChocolateyPackage>("Visual Studio Code").ShouldBe(ResourceState.Unchanged);
                fixture.Logger.WasLogged("The Chocolatey package [yellow]vscode[/] is already installed");
            }
        }

        public sealed class EnsureUninstalled
        {
            [WindowsFact]
            public void Should_Uninstall_Package_If_Present()
            {
                // Given
                var fixture = new CupboardFixture();
                fixture.Security.IsAdmin = true;
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Uninstall>());

                fixture.Process.Register("choco", "list -lo",
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\nvscode 1.58.0\n2 packages installed."),
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\n1 package installed."));

                fixture.Process.Register("choco", "uninstall vscode", new ProcessRunnerResult(0));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<ChocolateyPackage>("Visual Studio Code").ShouldBe(ResourceState.Changed);
                fixture.Logger.WasLogged("Uninstalling Chocolatey package [yellow]vscode[/]");
                fixture.Logger.WasLogged("The Chocolatey package [yellow]vscode[/] was uninstalled");
            }

            [WindowsFact]
            public void Should_Not_Uninstall_Package_If_Absent()
            {
                // Given
                var fixture = new CupboardFixture();
                fixture.Security.IsAdmin = true;
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Uninstall>());

                fixture.Process.Register("choco", "list -lo",
                    new ProcessRunnerResult(0, "Chocolatey v0.10.15\n7zip.install 19.0\n1 package installed."));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<ChocolateyPackage>("Visual Studio Code").ShouldBe(ResourceState.Unchanged);
                fixture.Logger.WasLogged("The Chocolatey package [yellow]vscode[/] is already uninstalled");
            }
        }
    }
}
