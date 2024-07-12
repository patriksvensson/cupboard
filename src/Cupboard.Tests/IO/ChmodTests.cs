namespace Cupboard.Tests.Unit.IO;

public sealed class ChmodTests
{
    public sealed class TheToFMethod
    {
        [Fact]
        public void Should_Convert_Permissions_To_Correct_File_Access_Permissions_1()
        {
            // Given
            var chmod = Chmod.Parse("766");

            // When
            var result = chmod.ToFileAccessPermissions();

            // Then
            result.ShouldBe(
                FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite | FileAccessPermissions.UserExecute |
                FileAccessPermissions.GroupRead | FileAccessPermissions.GroupWrite |
                FileAccessPermissions.OtherRead | FileAccessPermissions.OtherWrite);
        }

        [Fact]
        public void Should_Convert_Default_Permissions_To_Correct_File_Access_Permissions()
        {
            // Given
            var chmod = Chmod.Parse("666");

            // When
            var result = chmod.ToFileAccessPermissions();

            // Then
            result.ShouldBe(FileAccessPermissions.DefaultPermissions);
            result.ShouldBe(
                FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite |
                FileAccessPermissions.GroupRead | FileAccessPermissions.GroupWrite |
                FileAccessPermissions.OtherRead | FileAccessPermissions.OtherWrite);
        }
    }

    public sealed class TheToStringMethod
    {
        public sealed class WithCustomFormattingOption
        {
            [Fact]
            public void Should_Return_Expected_Output_For_Numeric_Formatting()
            {
                // Given
                var chmod = new Chmod(Permissions.All, Permissions.All, Permissions.All);

                // When
                var result = chmod.ToString(ChmodFormatting.Numeric);

                // Then
                Assert.Equal("0777", result);
            }

            [Theory]
            [InlineData(SpecialMode.None, "0777")]
            [InlineData(SpecialMode.All, "7777")]
            [InlineData(SpecialMode.Sticky, "1777")]
            [InlineData(SpecialMode.Setgid, "2777")]
            [InlineData(SpecialMode.Setuid, "4777")]
            [InlineData(SpecialMode.Sticky | SpecialMode.Setgid, "3777")]
            [InlineData(SpecialMode.Sticky | SpecialMode.Setuid, "5777")]
            [InlineData(SpecialMode.Setuid | SpecialMode.Setgid, "6777")]
            public void Should_Return_Expected_Output_For_Special_Mode_With_Numeric_Formatting(SpecialMode mode, string expected)
            {
                // Given
                var chmod = new Chmod(mode, Permissions.All, Permissions.All, Permissions.All);

                // When
                var result = chmod.ToString(ChmodFormatting.Numeric);

                // Then
                Assert.Equal(expected, result);
            }

            [Fact]
            public void Should_Return_Expected_Output_For_Symbolic_Formatting()
            {
                // Given
                var chmod = new Chmod(
                    Permissions.Read | Permissions.Execute,
                    Permissions.Write,
                    Permissions.Write | Permissions.Execute);

                // When
                var result = chmod.ToString(ChmodFormatting.Symbolic);

                // Then
                Assert.Equal("r-x-w--wx", result);
            }
        }

        [Fact]
        public void Should_Return_Expected_Output()
        {
            // Given
            var permissions = new Chmod(Permissions.All, Permissions.All, Permissions.All);

            // When
            var result = permissions.ToString();

            // Then
            Assert.Equal("0777", result);
        }
    }
}
