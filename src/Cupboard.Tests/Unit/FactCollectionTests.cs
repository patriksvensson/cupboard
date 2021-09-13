using Shouldly;
using Xunit;

namespace Cupboard.Tests.Unit
{
    public sealed class FactCollectionTests
    {
        public sealed class TheIsArmOrArm64Method
        {
            [Fact]
            public void Should_Return_True_If_ARM()
            {
                // Given
                var facts = new FactCollection
                {
                    { "os.arch", OSArchitecture.ARM },
                };

                // When
                var result = facts.IsArmOrArm64();

                // Then
                result.ShouldBeTrue();
            }

            [Fact]
            public void Should_Return_True_If_ARM64()
            {
                // Given
                var facts = new FactCollection
                {
                    { "os.arch", OSArchitecture.ARM64 },
                };

                // When
                var result = facts.IsArmOrArm64();

                // Then
                result.ShouldBeTrue();
            }

            [Fact]
            public void Should_Return_False_If_Not_ARM_OR_ARM64()
            {
                // Given
                var facts = new FactCollection
                {
                    { "os.arch", OSArchitecture.X64 },
                };

                // When
                var result = facts.IsArmOrArm64();

                // Then
                result.ShouldBeFalse();
            }
        }
    }
}
