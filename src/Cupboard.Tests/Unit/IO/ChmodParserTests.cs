using Xunit;

namespace Cupboard.Tests.Unit.IO
{
    public sealed class ChmodParserTests
    {
        public sealed class TheParseMethod
        {
            [Theory]
            [InlineData("000", SpecialMode.None, Permissions.None, Permissions.None, Permissions.None)]
            [InlineData("500", SpecialMode.None, Permissions.Read | Permissions.Execute, Permissions.None, Permissions.None)]
            [InlineData("050", SpecialMode.None, Permissions.None, Permissions.Read | Permissions.Execute, Permissions.None)]
            [InlineData("005", SpecialMode.None, Permissions.None, Permissions.None, Permissions.Read | Permissions.Execute)]
            [InlineData("777", SpecialMode.None, Permissions.All, Permissions.All, Permissions.All)]
            [InlineData("0000", SpecialMode.None, Permissions.None, Permissions.None, Permissions.None)]
            [InlineData("0500", SpecialMode.None, Permissions.Read | Permissions.Execute, Permissions.None, Permissions.None)]
            [InlineData("0050", SpecialMode.None, Permissions.None, Permissions.Read | Permissions.Execute, Permissions.None)]
            [InlineData("0005", SpecialMode.None, Permissions.None, Permissions.None, Permissions.Read | Permissions.Execute)]
            [InlineData("0777", SpecialMode.None, Permissions.All, Permissions.All, Permissions.All)]
            [InlineData("1000", SpecialMode.Sticky, Permissions.None, Permissions.None, Permissions.None)]
            [InlineData("2000", SpecialMode.Setgid, Permissions.None, Permissions.None, Permissions.None)]
            [InlineData("4000", SpecialMode.Setuid, Permissions.None, Permissions.None, Permissions.None)]
            [InlineData("7000", SpecialMode.Sticky | SpecialMode.Setgid | SpecialMode.Setuid, Permissions.None, Permissions.None, Permissions.None)]
            public void Should_Return_Correct_Permissions(string pattern, SpecialMode mode, Permissions owner, Permissions group, Permissions other)
            {
                // Given, When
                var result = ChmodParser.Parse(pattern);

                // Then
                Assert.Equal(mode, result.Mode);
                Assert.Equal(owner, result.Owner);
                Assert.Equal(group, result.Group);
                Assert.Equal(other, result.Other);
            }
        }
    }
}
