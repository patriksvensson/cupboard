using System;
using System.Collections.Generic;
using System.Linq;

namespace Cupboard.Tests
{
    public sealed class FakeLogger : ICupboardLogger
    {
        private readonly List<LoggedMessage> _messages;

        public Verbosity Verbosity { get; private set; } = Verbosity.Normal;

        private sealed class LoggedMessage
        {
            public Verbosity Verbosity { get; init; }
            public LogLevel Level { get; init; }
            public string Message { get; init; }
        }

        public FakeLogger()
        {
            _messages = new List<LoggedMessage>();
        }

        public void Log(Verbosity verbosity, LogLevel level, string markup)
        {
            _messages.Add(new LoggedMessage
            {
                Verbosity = verbosity,
                Level = level,
                Message = markup,
            });
        }

        public void SetVerbosity(Verbosity verbosity)
        {
            Verbosity = verbosity;
        }

        public bool WasLogged(string message, Verbosity? verbosity = null, LogLevel? level = null)
        {
            var messages = _messages.Where(x => x.Message.Equals(message, StringComparison.Ordinal));

            if (verbosity != null)
            {
                messages = messages.Where(x => x.Verbosity == verbosity.Value);
            }

            if (level != null)
            {
                messages = messages.Where(x => x.Level == level.Value);
            }

            return messages.Any();
        }
    }
}
