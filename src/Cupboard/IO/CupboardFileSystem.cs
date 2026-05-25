namespace Cupboard;

internal sealed class CupboardFileSystem : ICupboardFileSystem
{
    public IPathComparer Comparer { get => PathComparer.Default; }
    public IFileProvider File => FileSystem.Shared.File;
    public IDirectoryProvider Directory => FileSystem.Shared.Directory;

    public IFile GetTempFile(IEnvironment environment)
    {
        return FileSystem.Shared.GetTempFile(environment);
    }
}
