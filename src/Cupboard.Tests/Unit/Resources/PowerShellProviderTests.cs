using System.Threading.Tasks;
using NSubstitute;
using Shouldly;

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
            fixture.ProcessRunner.Register("powershell.exe", "–noprofile & 'C:/lol.ps1'", new ProcessRunnerResult(0, "OK"));

            // When
            var result = await fixture.Run(new PowerShellScript("My Script")
            {
                ScriptPath = "C:/lol.ps1",
            });

            // Then
            result.ShouldBe(ResourceState.Executed);
        }

        [WindowsFact]
        public async Task Should_Skip_Running_Skip_If_Condition_Script_Does_Not_Have_Exit_Code_1()
        {
            // Given
            var fixture = new Fixture();

            fixture.FileSystem.CreateDirectory("C:/Temp");
            fixture.FileSystem.CreateFile("C:/lol.ps1");
            fixture.ProcessRunner.Register("powershell.exe", "–noprofile & 'C:/Temp/fake.ps1'", new ProcessRunnerResult(1));

            // When
            var result = await fixture.Run(new PowerShellScript("My Script")
            {
                Unless = "Not really executed, but must be present",
                ScriptPath = "C:/lol.ps1",
            });

            // Then
            result.ShouldBe(ResourceState.Skipped);
            fixture.Logger.WasLogged("Skipping Powershell script since condition did not evaluate to 0 (zero)").ShouldBeTrue();
        }

        [WindowsFact]
        public async Task Should_Return_Error_If_Script_Does_Not_Exist()
        {
            // Given
            var fixture = new Fixture();

            // When
            var result = await fixture.Run(new PowerShellScript("My Script")
            {
                ScriptPath = "C:/lol.ps1",
            });

            // Then
            result.ShouldBe(ResourceState.Error);
            fixture.Logger.WasLogged("Script path does not exist").ShouldBeTrue();
        }

        private sealed class Fixture
        {
            public FakeCupboardFileSystem FileSystem { get; }
            public FakeCupboardEnvironment Environment { get; }
            public FakeProcessRunner ProcessRunner { get; }
            public FakeLogger Logger { get; }
            public FactCollection Facts { get; }

            public Fixture()
            {
                Environment = FakeCupboardEnvironment.CreateWindowsEnvironment();
                FileSystem = new FakeCupboardFileSystem(Environment);
                ProcessRunner = new FakeProcessRunner();
                Logger = new FakeLogger();
                Facts = new FactCollection();
            }

            public async Task<ResourceState> Run(PowerShellScript resource)
            {
                var context = new ExecutionContext(Facts);
                var refresher = Substitute.For<IEnvironmentRefresher>();
                var provider = new PowerShellProvider(FileSystem, Environment, ProcessRunner, refresher, Logger);
                return await provider.RunAsync(context, resource);
            }
        }
    }
}
