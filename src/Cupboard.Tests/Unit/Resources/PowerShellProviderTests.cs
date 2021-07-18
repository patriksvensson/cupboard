using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NSubstitute;
using Shouldly;
using Spectre.IO;
using Xunit;

namespace Cupboard.Tests.Unit.Resources
{
    public sealed class PowerShellProviderTests
    {
        [WindowsFact]
        public async Task Should_Run_Script()
        {
            // Given
            var fixture = new Fixture();
            fixture.FileSystem.CreateFile("C:/lol.ps1");
            fixture.ProcessRunner.RegisterDefault(new ProcessRunnerResult(0));

            // When
            var result = await fixture.Run(new PowerShell("My Script")
            {
                Script = "C:/lol.ps1",
            });

            // Then
            result.ShouldBe(ResourceState.Executed);
            fixture.Logger.WasLogged("Running PowerShell script [yellow]C:/lol.ps1[/]");
        }

        [WindowsFact]
        public async Task Should_Run_Command()
        {
            // Given
            var fixture = new Fixture();
            fixture.FileSystem.CreateDirectory("C:/Temp");
            fixture.ProcessRunner.RegisterDefault(new ProcessRunnerResult(0));

            // When
            var result = await fixture.Run(new PowerShell("My Script")
            {
                Command = "Not really executed, but must be present",
            });

            // Then
            result.ShouldBe(ResourceState.Executed);
            fixture.Logger.WasLogged("Running PowerShell command: Not really executed, but must be present");
        }

        [WindowsFact]
        public async Task Should_Skip_Running_Skip_If_Condition_Script_Does_Not_Have_Exit_Code_1()
        {
            // Given
            var fixture = new Fixture();

            fixture.FileSystem.CreateDirectory("C:/Temp");
            fixture.FileSystem.CreateFile("C:/lol.ps1");
            fixture.ProcessRunner.RegisterDefault(new ProcessRunnerResult(1));

            // When
            var result = await fixture.Run(new PowerShell("My Script")
            {
                Unless = "Not really executed, but must be present",
                Script = "C:/lol.ps1",
            });

            // Then
            result.ShouldBe(ResourceState.Skipped);
            fixture.Logger.WasLogged("Skipping PowerShell script since condition did not evaluate to 0 (zero)");
        }

        [WindowsFact]
        public async Task Should_Return_Error_If_Script_Does_Not_Exist()
        {
            // Given
            var fixture = new Fixture();

            // When
            var result = await fixture.Run(new PowerShell("My Script")
            {
                Script = "C:/lol.ps1",
            });

            // Then
            result.ShouldBe(ResourceState.Error);
            fixture.Logger.WasLogged("PowerShell script path does not exist");
        }

        [WindowsFact]
        public async Task Should_Run_Script_Using_PowerShell_Core_On_Windows_If_Specified()
        {
            // Given
            var fixture = new Fixture();
            fixture.FileSystem.CreateFile("C:/lol.ps1");
            fixture.ProcessRunner.RegisterDefault(new ProcessRunnerResult(0));

            // When
            var result = await fixture.Run(new PowerShell("My Script")
            {
                Script = "C:/lol.ps1",
                Flavor = PowerShellFlavor.PowerShellCore,
            });

            // Then
            result.ShouldBe(ResourceState.Executed);
            fixture.Logger.WasLogged("Running PowerShell script [yellow]C:/lol.ps1[/]");
        }

        [Theory]
        [InlineData(PlatformFamily.Linux)]
        [InlineData(PlatformFamily.MacOs)]
        public async Task Should_Run_Script_Using_PowerShell_Core_On_Non_Windows_Platforms(PlatformFamily family)
        {
            // Given
            var fixture = new Fixture(family);
            fixture.FileSystem.CreateFile("/Working/lol.ps1");
            fixture.ProcessRunner.RegisterDefault(new ProcessRunnerResult(0));

            // When
            var result = await fixture.Run(new PowerShell("My Script")
            {
                Script = "/Working/lol.ps1",
            });

            // Then
            result.ShouldBe(ResourceState.Executed);
            fixture.Logger.WasLogged("Running PowerShell script [yellow]/Working/lol.ps1[/]");
        }

        private sealed class Fixture
        {
            public FakeCupboardFileSystem FileSystem { get; }
            public FakeCupboardEnvironment Environment { get; }
            public FakeProcessRunner ProcessRunner { get; }
            public FakeLogger Logger { get; }
            public FactCollection Facts { get; }

            public Fixture(PlatformFamily family = PlatformFamily.Windows)
            {
                Environment = new FakeCupboardEnvironment(family);
                FileSystem = new FakeCupboardFileSystem(Environment);
                ProcessRunner = new FakeProcessRunner();
                Logger = new FakeLogger();
                Facts = new FactCollection();

                Facts.Add("os.platform", family switch
                {
                    PlatformFamily.Windows => OSPlatform.Windows,
                    PlatformFamily.Linux => OSPlatform.Linux,
                    PlatformFamily.MacOs => OSPlatform.OSX,
                    _ => throw new InvalidOperationException("Unknown platform family"),
                });
            }

            public async Task<ResourceState> Run(PowerShell resource)
            {
                var context = new ExecutionContext(Facts);
                var refresher = Substitute.For<IEnvironmentRefresher>();
                var provider = new PowerShellProvider(FileSystem, Environment, ProcessRunner, refresher, Logger);
                return await provider.RunAsync(context, resource);
            }
        }
    }
}
