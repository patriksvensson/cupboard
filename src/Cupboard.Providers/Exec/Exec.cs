using System;
using Spectre.IO;

namespace Cupboard;

public sealed class Exec : Resource
{
    public FilePath Path { get; set; }
    public string? Args { get; set; }
    public int[]? ValidExitCodes { get; set; }

    public Exec(string name)
        : base(name)
    {
        Path = new FilePath(name ?? throw new ArgumentNullException(nameof(name)));
    }
}