using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CliWrap.EventStream;

namespace Cupboard.Tests
{
    public sealed class FakeProcessRunner : IProcessRunner
    {
        private readonly Dictionary<string, Queue<ProcessRunnerResult>> _registrations;

        public FakeProcessRunner()
        {
            _registrations = new Dictionary<string, Queue<ProcessRunnerResult>>(StringComparer.Ordinal);
        }

        public void Register(string file, string arguments, params ProcessRunnerResult[] results)
        {
            var key = GetKey(file, arguments);
            if (!_registrations.ContainsKey(key))
            {
                _registrations[key] = new Queue<ProcessRunnerResult>();
            }

            foreach (var result in results)
            {
                _registrations[key].Enqueue(result);
            }
        }

        public Task<ProcessRunnerResult> Run(string file, string arguments, Action<CommandEvent> handler = null)
        {
            var key = GetKey(file, arguments);
            if (_registrations.ContainsKey(key))
            {
                if (_registrations[key].Count > 0)
                {
                    return Task.FromResult(_registrations[key].Dequeue());
                }
            }

            throw new InvalidOperationException($"No process registration found for \"{key}\"");
        }

        private string GetKey(string file, string arguments)
        {
            return $"{file} {arguments}";
        }
    }
}
