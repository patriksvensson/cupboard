using System;
using System.Threading.Tasks;
using CliWrap.EventStream;

namespace Cupboard
{
    public interface IProcessRunner
    {
        Task<ProcessRunnerResult> Run(string file, string arguments, Action<CommandEvent>? handler = null);
    }
}
