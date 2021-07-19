using Cupboard.Testing;
using Shouldly;
using Spectre.IO;
using Xunit;

namespace Cupboard.Tests.Unit.Resources
{
    public sealed class PowerShellProviderTests
    {
        public sealed class Windows
        {
            public sealed class Manifests
            {
                public class RunScript : Manifest
                {
                    public override void Execute(ManifestContext context)
                    {
                        context.Resource<PowerShell>("Run PowerShell Script")
                            .Script("C:/lol.ps1");
                    }
                }

                public class RunCommand : Manifest
                {
                    public override void Execute(ManifestContext context)
                    {
                        context.Resource<PowerShell>("Run PowerShell Command")
                            .Command("Not really executed, but must be present");
                    }
                }

                public class RunUnless : Manifest
                {
                    public override void Execute(ManifestContext context)
                    {
                        context.Resource<PowerShell>("Run PowerShell Unless")
                            .Unless("Not really executed, but must be present");
                    }
                }

                public class RunCore : Manifest
                {
                    public override void Execute(ManifestContext context)
                    {
                        context.Resource<PowerShell>("Run PowerShell Core Script")
                            .Flavor(PowerShellFlavor.PowerShellCore)
                            .Script("C:/lol.ps1");
                    }
                }
            }

            [WindowsFact]
            public void Should_Run_Script()
            {
                // Given
                var fixture = new CupboardFixture();
                fixture.FileSystem.CreateFile("C:/lol.ps1");
                fixture.Configure(ctx => ctx.UseManifest<Manifests.RunScript>());

                // When
                var result = fixture.Run("-y");

                // Then
                result.ExitCode.ShouldBe(0);
                result.Report.GetState<PowerShell>("Run PowerShell Script").ShouldBe(ResourceState.Executed);
                fixture.Logger.WasLogged("Running PowerShell script [yellow]C:/lol.ps1[/]");
            }

            [WindowsFact]
            public void Should_Run_Command()
            {
                // Given
                var fixture = new CupboardFixture();
                fixture.FileSystem.CreateDirectory("C:/Temp");
                fixture.Configure(ctx => ctx.UseManifest<Manifests.RunCommand>());

                // When
                var result = fixture.Run("-y");

                // Then
                result.ExitCode.ShouldBe(0);
                result.Report.GetState<PowerShell>("Run PowerShell Command").ShouldBe(ResourceState.Executed);
                fixture.Logger.WasLogged("Running PowerShell command: [yellow]Not really executed, but must be present[/]");
            }

            [WindowsFact]
            public void Should_Skip_Running_Skip_If_Condition_Script_Does_Not_Have_Exit_Code_1()
            {
                // Given
                var fixture = new CupboardFixture();

                fixture.FileSystem.CreateDirectory("C:/Temp");
                fixture.FileSystem.CreateFile("C:/lol.ps1");
                fixture.Process.RegisterDefault(new ProcessRunnerResult(1));
                fixture.Configure(ctx => ctx.UseManifest<Manifests.RunUnless>());

                // When
                var result = fixture.Run("-y");

                // Then
                result.ExitCode.ShouldBe(0);
                result.Report.GetState<PowerShell>("Run PowerShell Unless").ShouldBe(ResourceState.Skipped);
                fixture.Logger.WasLogged("Evaluating PowerShell condition: Not really executed, but must be present");
                fixture.Logger.WasLogged("Skipping PowerShell script since condition did not evaluate to 0 (zero)");
            }

            [WindowsFact]
            public void Should_Return_Error_If_Script_Does_Not_Exist()
            {
                // Given
                var fixture = new CupboardFixture();
                fixture.Configure(ctx => ctx.UseManifest<Manifests.RunScript>());

                // When
                var result = fixture.Run("-y");

                // Then
                result.ExitCode.ShouldBe(-1);
                result.Report.GetState<PowerShell>("Run PowerShell Script").ShouldBe(ResourceState.Error);
                fixture.Logger.WasLogged("PowerShell script path does not exist");
            }

            [WindowsFact]
            public void Should_Run_Script_Using_PowerShell_Core_On_Windows_If_Specified()
            {
                // Given
                var fixture = new CupboardFixture();
                fixture.FileSystem.CreateFile("C:/lol.ps1");
                fixture.Configure(ctx => ctx.UseManifest<Manifests.RunCore>());

                // When
                var result = fixture.Run("-y");

                // Then
                result.ExitCode.ShouldBe(0);
                result.Report.GetState<PowerShell>("Run PowerShell Core Script").ShouldBe(ResourceState.Executed);
                fixture.Logger.WasLogged("Running PowerShell script [yellow]C:/lol.ps1[/]");
            }
        }

        public sealed class Unix
        {
            public sealed class Manifests
            {
                public class RunScript : Manifest
                {
                    public override void Execute(ManifestContext context)
                    {
                        context.Resource<PowerShell>("Run PowerShell Script")
                            .Script("/Working/lol.ps1");
                    }
                }
            }

            [Theory]
            [InlineData(PlatformFamily.Linux)]
            [InlineData(PlatformFamily.MacOs)]
            public void Should_Run_Script(PlatformFamily family)
            {
                // Given
                var fixture = new CupboardFixture(family);
                fixture.FileSystem.CreateFile("/Working/lol.ps1");
                fixture.Process.RegisterDefault(new ProcessRunnerResult(0));
                fixture.Configure(ctx => ctx.UseManifest<Manifests.RunScript>());

                // When
                var result = fixture.Run("-y");

                // Then
                result.ExitCode.ShouldBe(0);
                result.Report.GetState<PowerShell>("Run PowerShell Script").ShouldBe(ResourceState.Executed);
                fixture.Logger.WasLogged("Running PowerShell script [yellow]/Working/lol.ps1[/]");
            }
        }
    }
}
