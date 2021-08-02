using Cupboard.Testing;
using Shouldly;
using Spectre.IO;
using Xunit;

namespace Cupboard.Tests.Unit.Providers
{
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

        public sealed class EnsureInstalled
        {
            [Theory]
            [InlineData(PlatformFamily.Windows, WindowsExecutable)]
            [InlineData(PlatformFamily.Linux, LinuxExecutable)]
            public void Should_Install_Package_If_Missing(PlatformFamily platform, string executable)
            {
                // Given
                var fixture = new CupboardFixture(platform);
                fixture.Security.IsAdmin = true;
                fixture.Process.RegisterDefaultResult(new ProcessRunnerResult(0));
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Install>());

                fixture.Process.Register(executable, "--list-extensions",
                    new ProcessRunnerResult(0, string.Empty),
                    new ProcessRunnerResult(0, ExtensionName));

                fixture.Process.Register(executable, $"--install-extension {ExtensionName}",
                    new ProcessRunnerResult(0));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<VSCodeExtension>(ExtensionName).ShouldBe(ResourceState.Changed);
                fixture.Logger.WasLogged($"Installing VSCode extension [yellow]{ExtensionName}[/]");
                fixture.Logger.WasLogged($"The VSCode extension [yellow]{ExtensionName}[/] was installed");
            }

            [Theory]
            [InlineData(PlatformFamily.Windows, WindowsExecutable)]
            [InlineData(PlatformFamily.Linux, LinuxExecutable)]
            public void Should_Not_Install_Package_If_Present(PlatformFamily platform, string executable)
            {
                // Given
                var fixture = new CupboardFixture(platform);
                fixture.Security.IsAdmin = true;
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Install>());

                fixture.Process.Register(executable, "--list-extensions",
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
            [Theory]
            [InlineData(PlatformFamily.Windows, WindowsExecutable)]
            [InlineData(PlatformFamily.Linux, LinuxExecutable)]
            public void Should_Uninstall_Package_If_Present(PlatformFamily platform, string executable)
            {
                // Given
                var fixture = new CupboardFixture(platform);
                fixture.Security.IsAdmin = true;
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Uninstall>());

                fixture.Process.Register(executable, "--list-extensions",
                    new ProcessRunnerResult(0, $"foo\n{ExtensionName}\nbaz"),
                    new ProcessRunnerResult(0, "foo\nbaz"));

                fixture.Process.Register(executable, $"--uninstall-extension {ExtensionName}",
                    new ProcessRunnerResult(0));

                // When
                var result = fixture.Run("-y");

                // Then
                result.Report.GetState<VSCodeExtension>(ExtensionName).ShouldBe(ResourceState.Changed);
                fixture.Logger.WasLogged($"Uninstalling VSCode extension [yellow]{ExtensionName}[/]");
                fixture.Logger.WasLogged($"The VSCode extension [yellow]{ExtensionName}[/] was uninstalled");
            }

            [Theory]
            [InlineData(PlatformFamily.Windows, WindowsExecutable)]
            [InlineData(PlatformFamily.Linux, LinuxExecutable)]
            public void Should_Not_Uninstall_Package_If_Absent(PlatformFamily platform, string executable)
            {
                // Given
                var fixture = new CupboardFixture(platform);
                fixture.Security.IsAdmin = true;
                fixture.Configure(ctx => ctx.UseManifest<Manifests.Uninstall>());

                fixture.Process.Register(executable, "--list-extensions",
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
