using System;
using System.Threading.Tasks;
using Spectre.Console;

namespace Cupboard.Internal
{
    internal interface IStatusUpdater
    {
        Task<bool> Update(string markup, Func<Task<bool>> func);
    }

    internal sealed class StatusUpdater : IStatusUpdater
    {
        private readonly IAnsiConsole _console;

        public StatusUpdater(IAnsiConsole console)
        {
            _console = console;
        }

        public async Task<bool> Update(string markup, Func<Task<bool>> func)
        {
            return await _console.Status().StartAsync(markup, async _ =>
                await func().ConfigureAwait(false)).ConfigureAwait(false);
        }
    }

    internal sealed class DummyUpdater : IStatusUpdater
    {
        public async Task<bool> Update(string markup, Func<Task<bool>> func)
        {
            return await func().ConfigureAwait(false);
        }
    }
}
