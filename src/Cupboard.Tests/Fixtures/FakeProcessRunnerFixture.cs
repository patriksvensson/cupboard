using NSubstitute;

namespace Cupboard.Tests.Resources
{
    public sealed class FakeProcessRunnerFixture
    {
        public IProcessRunner Runner { get; }

        public FakeProcessRunnerFixture()
        {
            Runner = Substitute.For<IProcessRunner>();
        }

        public void Register(string file, string arguments, ProcessRunnerResult result, params ProcessRunnerResult[] results)
        {
            Runner.Run(Arg.Is(file), Arg.Is(arguments)).Returns(result, results);
        }
    }
}
