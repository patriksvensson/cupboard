using Spectre.IO;

namespace Cupboard
{
    public sealed class PowerShell : Resource
    {
        public FilePath? Script { get; set; }
        public string? Command { get; set; }
        public string? Unless { get; set; }
        public PowerShellFlavor Flavor { get; set; } = PowerShellFlavor.PowerShell;

        public PowerShell(string name)
                : base(name)
        {
        }
    }
}
