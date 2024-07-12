namespace Cupboard;

public sealed class Directory : Resource
{
    public DirectoryPath Path { get; set; }
    public DirectoryState Ensure { get; set; } = DirectoryState.Present;
    public Chmod? Permissions { get; set; }

    public Directory(string name)
        : base(name)
    {
        Path = new DirectoryPath(name);
    }
}
