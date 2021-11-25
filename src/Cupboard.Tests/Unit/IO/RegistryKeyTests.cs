using Shouldly;
using Xunit;

namespace Cupboard.Tests.Unit.IO;

public sealed class RegistryKeyPathTests
{
    [Fact]
    public void Should_Not_Consider_Path_Missing_Key_Valid()
    {
        // Given, When
        var path = new RegistryPath("HKEY_CURRENT_USER");

        // Then
        path.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void Should_Not_Consider_Path_Missing_Sub_Key_Valid()
    {
        // Given, When
        var path = new RegistryPath("HKEY_CURRENT_USER\\Foo");

        // Then
        path.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void Should_Normalize_Separators()
    {
        // Given, When
        var path = new RegistryPath("HKEY_CURRENT_USER/Foo/Bar/Baz");

        // Then
        path.ToString().ShouldBe(@"HKEY_CURRENT_USER\Foo\Bar\Baz");
        path.Path.ShouldBe(@"Foo\Bar\Baz");
        path.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("HKCR/Foo/Bar/Baz", RegistryHive.ClassesRoot)]
    [InlineData("HKCU/Foo/Bar/Baz", RegistryHive.CurrentUser)]
    [InlineData("HKLM/Foo/Bar/Baz", RegistryHive.LocalMachine)]
    [InlineData("HKU/Foo/Bar/Baz", RegistryHive.Users)]
    [InlineData("HKPD/Foo/Bar/Baz", RegistryHive.PerformanceData)]
    [InlineData("HKCC/Foo/Bar/Baz", RegistryHive.CurrentConfig)]
    [InlineData("HKCR:/Foo/Bar/Baz", RegistryHive.ClassesRoot)]
    [InlineData("HKCU:/Foo/Bar/Baz", RegistryHive.CurrentUser)]
    [InlineData("HKLM:/Foo/Bar/Baz", RegistryHive.LocalMachine)]
    [InlineData("HKU:/Foo/Bar/Baz", RegistryHive.Users)]
    [InlineData("HKPD:/Foo/Bar/Baz", RegistryHive.PerformanceData)]
    [InlineData("HKCC:/Foo/Bar/Baz", RegistryHive.CurrentConfig)]
    public void Should_Substitute_Roots(string path, RegistryHive expected)
    {
        // Given, When
        var result = new RegistryPath(path);

        // Then
        result.IsValid.ShouldBeTrue();
        result.Hive.ShouldBe(expected);
    }
}