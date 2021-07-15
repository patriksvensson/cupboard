using Shouldly;
using Xunit;

namespace Cupboard.Tests.Unit.IO
{
    public sealed class RegistryKeyPathTests
    {
        [Fact]
        public void Should_Not_Consider_Path_Missing_Key_Valid()
        {
            // Given, When
            var path = new RegistryKeyPath("HKEY_CURRENT_USER");

            // Then
            path.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Should_Not_Consider_Path_Missing_Sub_Key_Valid()
        {
            // Given, When
            var path = new RegistryKeyPath("HKEY_CURRENT_USER\\Foo");

            // Then
            path.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Should_Normalize_Separators()
        {
            // Given, When
            var path = new RegistryKeyPath("HKEY_CURRENT_USER/Foo/Bar/Baz");

            // Then
            path.ToString().ShouldBe(@"HKEY_CURRENT_USER\Foo\Bar\Baz");
            path.Path.ShouldBe(@"Foo\Bar\Baz");
            path.IsValid.ShouldBeTrue();
        }

        [Theory]
        [InlineData("HKCR/Foo/Bar/Baz", RegistryKeyRoot.ClassesRoot)]
        [InlineData("HKCU/Foo/Bar/Baz", RegistryKeyRoot.CurrentUser)]
        [InlineData("HKLM/Foo/Bar/Baz", RegistryKeyRoot.LocalMachine)]
        [InlineData("HKU/Foo/Bar/Baz", RegistryKeyRoot.Users)]
        [InlineData("HKCC/Foo/Bar/Baz", RegistryKeyRoot.CurrentConfig)]
        [InlineData("HKCR:/Foo/Bar/Baz", RegistryKeyRoot.ClassesRoot)]
        [InlineData("HKCU:/Foo/Bar/Baz", RegistryKeyRoot.CurrentUser)]
        [InlineData("HKLM:/Foo/Bar/Baz", RegistryKeyRoot.LocalMachine)]
        [InlineData("HKU:/Foo/Bar/Baz", RegistryKeyRoot.Users)]
        [InlineData("HKCC:/Foo/Bar/Baz", RegistryKeyRoot.CurrentConfig)]
        public void Should_Substitute_Roots(string path, RegistryKeyRoot expected)
        {
            // Given, When
            var result = new RegistryKeyPath(path);

            // Then
            result.IsValid.ShouldBeTrue();
            result.Root.ShouldBe(expected);
        }
    }
}
