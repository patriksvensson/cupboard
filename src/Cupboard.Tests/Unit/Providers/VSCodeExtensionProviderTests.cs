using Cupboard.Testing;
using Shouldly;
using Spectre.IO;
using Xunit;

namespace Cupboard.Tests.Unit.Providers;

public sealed class VSCodeExtensionProviderTests
{
    private const string ExtensionName = "bar";
    private const string WindowsExecutable = "C:/Program Files/Microsoft VS Code/bin/code.cmd";
    private const string LinuxExecutable = "code";

    public sealed class Manifests
    {
        public sealed class Install : Manifest
        {
            public override void Execute(ManifestContext context)
            {
                context.Resource<VSCodeExtension>(ExtensionName)
                    .Ensure(PackageState.Installed);
            }
        }

        public sealed class Uninstall : Manifest
        {
            public override void Execute(ManifestContext context)
            {
                context.Resource<VSCodeExtension>(ExtensionName)
                    .Ensure(PackageState.Uninstalled);
            }
        }
    }

    public sealed class Windows
    {
        public sealed class EnsureInstalled
        {
            [WindowsFact]
            public void Should_Install_Package_If_Missing()
            {
                // Given
                var fixture = new CupboardFixture(admin: true);
                fixture.Process.RegisterDefaultResult(new ProcessRunnerResult(0));
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Install>());

                fixture.Process.Register(WindowsExecutable, "--list-extensions",
                    new ProcessRunnerResult(0, string.Empty),
                    new ProcessRunnerResult(0, ExtensionName));

                fixture.Process.Register(WindowsExecutable, $"--install-extension {ExtensionName}",
                    new ProcessRunnerResult(0));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<VSCodeExtension>(ExtensionName).ShouldBe(ResourceState.Changed);
                fixture.Logger.WasLogged($"Installing VSCode extension [yellow]{ExtensionName}[/]");
                fixture.Logger.WasLogged($"The VSCode extension [yellow]{ExtensionName}[/] was installed");
            }

            [WindowsFact]
            public void Should_Not_Install_Package_If_Present()
            {
                // Given
                var fixture = new CupboardFixture(admin: true);
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Install>());

                fixture.Process.Register(WindowsExecutable, "--list-extensions",
                    new ProcessRunnerResult(0, ExtensionName));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<VSCodeExtension>(ExtensionName).ShouldBe(ResourceState.Unchanged);
                fixture.Logger.WasLogged($"The VSCode extension [yellow]{ExtensionName}[/] is already installed");
            }
        }

        public sealed class EnsureUninstalled
        {
            [WindowsFact]
            public void Should_Uninstall_Package_If_Present()
            {
                // Given
                var fixture = new CupboardFixture(PlatformFamily.Windows, admin: true);
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Uninstall>());

                fixture.Process.Register(WindowsExecutable, "--list-extensions",
                    new ProcessRunnerResult(0, $"foo\n{ExtensionName}\nbaz"),
                    new ProcessRunnerResult(0, "foo\nbaz"));

                fixture.Process.Register(WindowsExecutable, $"--uninstall-extension {ExtensionName}",
                    new ProcessRunnerResult(0));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<VSCodeExtension>(ExtensionName).ShouldBe(ResourceState.Changed);
                fixture.Logger.WasLogged($"Uninstalling VSCode extension [yellow]{ExtensionName}[/]");
                fixture.Logger.WasLogged($"The VSCode extension [yellow]{ExtensionName}[/] was uninstalled");
            }

            [WindowsFact]
            public void Should_Not_Uninstall_Package_If_Absent()
            {
                // Given
                var fixture = new CupboardFixture(admin: true);
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Uninstall>());

                fixture.Process.Register(WindowsExecutable, "--list-extensions",
                    new ProcessRunnerResult(0, "foo\nbaz"));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<VSCodeExtension>(ExtensionName).ShouldBe(ResourceState.Unchanged);
                fixture.Logger.WasLogged($"The VSCode extension [yellow]{ExtensionName}[/] is already uninstalled");
            }
        }
    }

    public sealed class Linux
    {
        public sealed class EnsureInstalled
        {
            [WindowsFact]
            public void Should_Install_Package_If_Missing()
            {
                // Given
                var fixture = new CupboardFixture(PlatformFamily.Linux, admin: true);
                fixture.Process.RegisterDefaultResult(new ProcessRunnerResult(0));
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Install>());

                fixture.Process.Register(LinuxExecutable, "--list-extensions",
                    new ProcessRunnerResult(0, string.Empty),
                    new ProcessRunnerResult(0, ExtensionName));

                fixture.Process.Register(LinuxExecutable, $"--install-extension {ExtensionName}",
                    new ProcessRunnerResult(0));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<VSCodeExtension>(ExtensionName).ShouldBe(ResourceState.Changed);
                fixture.Logger.WasLogged($"Installing VSCode extension [yellow]{ExtensionName}[/]");
                fixture.Logger.WasLogged($"The VSCode extension [yellow]{ExtensionName}[/] was installed");
            }

            [WindowsFact]
            public void Should_Not_Install_Package_If_Present()
            {
                // Given
                var fixture = new CupboardFixture(PlatformFamily.Linux, admin: true);
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Install>());

                fixture.Process.Register(LinuxExecutable, "--list-extensions",
                    new ProcessRunnerResult(0, ExtensionName));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<VSCodeExtension>(ExtensionName).ShouldBe(ResourceState.Unchanged);
                fixture.Logger.WasLogged($"The VSCode extension [yellow]{ExtensionName}[/] is already installed");
            }
        }

        public sealed class EnsureUninstalled
        {
            [WindowsFact]
            public void Should_Uninstall_Package_If_Present()
            {
                // Given
                var fixture = new CupboardFixture(PlatformFamily.Linux, admin: true);
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Uninstall>());

                fixture.Process.Register(LinuxExecutable, "--list-extensions",
                    new ProcessRunnerResult(0, $"foo\n{ExtensionName}\nbaz"),
                    new ProcessRunnerResult(0, "foo\nbaz"));

                fixture.Process.Register(LinuxExecutable, $"--uninstall-extension {ExtensionName}",
                    new ProcessRunnerResult(0));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<VSCodeExtension>(ExtensionName).ShouldBe(ResourceState.Changed);
                fixture.Logger.WasLogged($"Uninstalling VSCode extension [yellow]{ExtensionName}[/]");
                fixture.Logger.WasLogged($"The VSCode extension [yellow]{ExtensionName}[/] was uninstalled");
            }

            [WindowsFact]
            public void Should_Not_Uninstall_Package_If_Absent()
            {
                // Given
                var fixture = new CupboardFixture(PlatformFamily.Linux, admin: true);
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Uninstall>());

                fixture.Process.Register(LinuxExecutable, "--list-extensions",
                    new ProcessRunnerResult(0, "foo\nbaz"));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<VSCodeExtension>(ExtensionName).ShouldBe(ResourceState.Unchanged);
                fixture.Logger.WasLogged($"The VSCode extension [yellow]{ExtensionName}[/] is already uninstalled");
            }
        }
    }
}
