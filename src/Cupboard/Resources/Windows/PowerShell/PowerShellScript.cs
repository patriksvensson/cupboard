using Spectre.IO;

namespace Cupboard
{
    public sealed class PowerShellScript : Resource
    {
        public FilePath? ScriptPath { get; set; }
        public string? Unless { get; set; }

        public PowerShellScript(string name)
            : base(name)
        {
        }
    }
}
