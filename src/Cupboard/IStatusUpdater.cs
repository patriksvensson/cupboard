using System;
using Spectre.Console;

namespace Cupboard.Internal
{
    internal interface IStatusUpdater
    {
        void Update(string markup);
    }

    internal sealed class StatusUpdater : IStatusUpdater
    {
        private readonly StatusContext _context;

        public StatusUpdater(StatusContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Update(string markup)
        {
            if (!string.IsNullOrWhiteSpace(markup))
            {
                _context.Status = markup;
            }
        }
    }

    internal sealed class DummyUpdater : IStatusUpdater
    {
        public void Update(string markup)
        {
        }
    }
}
