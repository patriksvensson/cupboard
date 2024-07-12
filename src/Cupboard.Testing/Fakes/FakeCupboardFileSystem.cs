namespace Cupboard.Testing;

[PublicAPI]
public sealed class FakeCupboardFileSystem : ICupboardFileSystem
{
    private readonly FakeFileSystem _fileSystem;

    public IPathComparer Comparer { get; }

    public IFileProvider File => _fileSystem.File;
    public IDirectoryProvider Directory => _fileSystem.Directory;

    public FakeCupboardFileSystem(IEnvironment environment)
    {
        _fileSystem = new FakeFileSystem(environment);
        Comparer = new PathComparer(environment);
    }

    public FakeDirectory CreateDirectory(DirectoryPath path)
    {
        return _fileSystem.CreateDirectory(path);
    }

    public FakeFile CreateFile(FilePath path, FileAttributes attributes = 0)
    {
        return _fileSystem.CreateFile(path, attributes);
    }

    public FakeFile CreateFile(FilePath path, byte[] contentsBytes)
    {
        return _fileSystem.CreateFile(path, contentsBytes);
    }

    public void EnsureFileDoesNotExist(FilePath path)
    {
        _fileSystem.EnsureFileDoesNotExist(path);
    }

    public FakeDirectory GetFakeDirectory(DirectoryPath path)
    {
        return _fileSystem.GetFakeDirectory(path);
    }

    public FakeFile GetFakeFile(FilePath path)
    {
        return _fileSystem.GetFakeFile(path);
    }
}
