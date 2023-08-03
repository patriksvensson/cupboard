using Spectre.IO;

namespace Cupboard;

public sealed class File : Resource
{
    public FilePath? Destination { get; set; }
    public FilePath? Source { get; set; }
    public FileState Ensure { get; set; } = FileState.Present;
    public bool SymbolicLink { get; set; }
    public Chmod? Permissions { get; set; }

    public File(string name)
        : base(name)
    {
    }
}
