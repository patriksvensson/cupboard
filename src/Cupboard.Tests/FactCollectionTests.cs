using Shouldly;
using Xunit;

namespace Cupboard.Tests
{
    public sealed class FactCollectionTests
    {
        [Theory]
        [InlineData("foo")]
        [InlineData("FOO")]
        [InlineData("Foo")]
        [InlineData("FoO")]
        public void Should_Consider_Fact_Name_Case_Insensitive(string name)
        {
            // Then
            var c = new FactCollection
            {
                { "foo", 1 },
            };

            // When
            int value = c[name];

            // Then
            value.ShouldBe(1);
        }

        [Fact]
        public void Should_Get_Value_By_Path_Segments()
        {
            // Then
            var c = new FactCollection
            {
                { "foo.bar.baz", 1 },
            };

            // When
            int value = c["foo"]["bar"]["baz"];

            // Then
            value.ShouldBe(1);
        }

        [Fact]
        public void Should_Get_Value_By_Full_Path()
        {
            // Then
            var c = new FactCollection
            {
                { "foo.bar.baz", 1 },
            };

            // When
            int value = c["foo.bar.baz"];

            // Then
            value.ShouldBe(1);
        }
    }
}
