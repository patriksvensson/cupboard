using Cupboard.Testing;
using Shouldly;
using Spectre.IO;
using Xunit;

namespace Cupboard.Tests.Unit.Providers;

public sealed class FileProviderTests
{
    public sealed class Manifests
    {
        public sealed class FilePresent : Manifest
        {
            public override void Execute(ManifestContext context)
            {
                context.Resource<File>("~/foo.txt")
                    .Ensure(FileState.Present)
                    .Source("~/bar/qux.txt");
            }
        }

        public sealed class FileAbsent : Manifest
        {
            public override void Execute(ManifestContext context)
            {
                context.Resource<File>("~/foo.txt")
                    .Ensure(FileState.Absent);
            }
        }

        public sealed class SymlinkedFile : Manifest
        {
            public override void Execute(ManifestContext context)
            {
                context.Resource<File>("~/foo.txt")
                    .SymbolicLink()
                    .Ensure(FileState.Present)
                    .Source("~/bar/qux.txt");
            }
        }
    }

    [Fact]
    public void Should_Copy_File()
    {
        // Given
        var fixture = new CupboardFixture(PlatformFamily.Linux);
        fixture.FileSystem.CreateFile("~/bar/qux.txt");
        fixture.Configure(ctx => ctx.UseManifest<Manifests.FilePresent>());

        // When
        var result = fixture.Run("-y");

        // Then
        result.Report.GetState<File>("~/foo.txt").ShouldBe(ResourceState.Changed);
        fixture.FileShouldExist("/home/JohnDoe/foo.txt");
    }

    [Fact]
    public void Should_Create_Symlink()
    {
        // Given
        var fixture = new CupboardFixture(PlatformFamily.Linux);
        fixture.FileSystem.CreateFile("~/bar/qux.txt");
        fixture.Configure(ctx => ctx.UseManifest<Manifests.SymlinkedFile>());

        // When
        var result = fixture.Run("-y");

        // Then
        result.Report.GetState<File>("~/foo.txt").ShouldBe(ResourceState.Changed);
        fixture.FileShouldExist("/home/JohnDoe/foo.txt");
        fixture.FileShouldBeSymbolicLink("/home/JohnDoe/foo.txt");
    }

    [Fact]
    public void Should_Delete_File()
    {
        // Given
        var fixture = new CupboardFixture(PlatformFamily.Linux);
        fixture.FileSystem.CreateFile("~/foo.txt");
        fixture.Configure(ctx => ctx.UseManifest<Manifests.FileAbsent>());

        // When
        var result = fixture.Run("-y");

        // Then
        result.Report.GetState<File>("~/foo.txt").ShouldBe(ResourceState.Changed);
        fixture.FileShouldNotExist("/home/JohnDoe/foo.txt");
    }
}