namespace Cupboard;

internal sealed class CupboardFileSystem : ICupboardFileSystem
{
    private readonly IFileSystem _fileSystem;

    public IPathComparer Comparer { get; }
    public IFileProvider File => _fileSystem.File;
    public IDirectoryProvider Directory => _fileSystem.Directory;

    public CupboardFileSystem()
    {
        _fileSystem = new FileSystem();
        Comparer = PathComparer.Default;
    }
}
