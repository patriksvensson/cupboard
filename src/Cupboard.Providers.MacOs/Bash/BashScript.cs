using Spectre.IO;

namespace Cupboard
{
    public sealed class BashScript : Resource
    {
        public BashScript(string name)
            : base(name)
        {
        }

        public FilePath? Script { get; set; }
        public string? Command { get; set; }
        public string? Unless { get; set; }
    }
}