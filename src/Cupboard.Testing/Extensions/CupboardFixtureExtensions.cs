using System;
using Spectre.IO;

namespace Cupboard.Testing;

public static class CupboardFixtureExtensions
{
    public static void FileShouldExist(this CupboardFixture fixture, FilePath path)
    {
        if (fixture is null)
        {
            throw new ArgumentNullException(nameof(fixture));
        }

        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (!fixture.FileSystem.File.Exists(path))
        {
            throw new InvalidOperationException($"File at {path.FullPath} does not exist");
        }
    }

    public static void FileShouldNotExist(this CupboardFixture fixture, FilePath path)
    {
        if (fixture is null)
        {
            throw new ArgumentNullException(nameof(fixture));
        }

        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (fixture.FileSystem.File.Exists(path))
        {
            throw new InvalidOperationException($"File at {path.FullPath} should not exist");
        }
    }

    public static void FileShouldBeSymbolicLink(this CupboardFixture fixture, FilePath path)
    {
        if (fixture is null)
        {
            throw new ArgumentNullException(nameof(fixture));
        }

        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        FileShouldExist(fixture, path);

        if (fixture.FileSystem.GetFakeFile(path)?.SymbolicLink == null)
        {
            throw new InvalidOperationException($"File at {path.FullPath} is not a symbolic link");
        }
    }
}
