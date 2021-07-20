using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cupboard.Testing;
using Shouldly;

namespace Cupboard.Tests.Unit.Resources
{
    public sealed class WingetProviderTests
    {
        public sealed class Manifests
        {
            public sealed class Install : Manifest
            {
                public override void Execute(ManifestContext context)
                {
                    context.Resource<WingetPackage>("GitHub CLI")
                        .Package("GitHub.cli")
                        .Ensure(PackageState.Installed);
                }
            }

            public sealed class Uninstall : Manifest
            {
                public override void Execute(ManifestContext context)
                {
                    context.Resource<WingetPackage>("GitHub CLI")
                        .Package("GitHub.cli")
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
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Install>());

                fixture.Process.Register("winget", "list --source winget --id GitHub.cli",
                    new ProcessRunnerResult(int.MinValue, "No installed package found matching input criteria."),
                    new ProcessRunnerResult(0, "GitHub CLI GitHub.cli 1.12.1"));

                fixture.Process.Register("winget", "install -e --id GitHub.cli --force",
                    new ProcessRunnerResult(0, "Lol installed GitHub.cli"));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<WingetPackage>("GitHub CLI").ShouldBe(ResourceState.Changed);
                fixture.Logger.WasLogged("Installing Winget package [yellow]GitHub.cli[/]");
                fixture.Logger.WasLogged("The Winget package [yellow]GitHub.cli[/] was installed");
            }

            [WindowsFact]
            public void Should_Not_Install_Package_If_Present_lol()
            {
                // Given
                var fixture = new CupboardFixture();
                fixture.Security.IsAdmin = true;
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Install>());

                fixture.Process.Register("winget", "list --source winget --id GitHub.cli",
                    new ProcessRunnerResult(0, "GitHub CLI GitHub.cli 1.12.1"));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<WingetPackage>("GitHub CLI").ShouldBe(ResourceState.Unchanged);
                fixture.Logger.WasLogged("The Winget package [yellow]GitHub.cli[/] is already installed");
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

                fixture.Process.Register("winget", "list --source winget --id GitHub.cli",
                    new ProcessRunnerResult(0, "GitHub CLI GitHub.cli 1.12.1"),
                    new ProcessRunnerResult(int.MinValue, "No installed package found matching input criteria."));

                fixture.Process.Register("winget", "uninstall -e --id GitHub.cli", new ProcessRunnerResult(0));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<WingetPackage>("GitHub CLI").ShouldBe(ResourceState.Changed);
                fixture.Logger.WasLogged("Uninstalling Winget package GitHub.cli...");
                fixture.Logger.WasLogged("The Winget package [yellow]GitHub.cli[/] was uninstalled");
            }

            [WindowsFact]
            public void Should_Not_Uninstall_Package_If_Absent()
            {
                // Given
                var fixture = new CupboardFixture();
                fixture.Security.IsAdmin = true;
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Uninstall>());

                fixture.Process.Register("winget", "list --source winget --id GitHub.cli",
                    new ProcessRunnerResult(int.MinValue, "No installed package found matching input criteria."));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<WingetPackage>("GitHub CLI").ShouldBe(ResourceState.Unchanged);
                fixture.Logger.WasLogged("The Winget package [yellow]GitHub.cli[/] is already uninstalled");
            }
        }
    }
}
