using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliWrap.EventStream;

namespace Cupboard.Testing
{
    public sealed class FakeProcessRunner : IProcessRunner
    {
        private readonly Dictionary<string, Queue<ProcessRunnerResult>> _registrations;
        private readonly List<(string File, string Arguments)> _calls;
        private ProcessRunnerResult? _defaultRegistration;

        public FakeProcessRunner()
        {
            _registrations = new Dictionary<string, Queue<ProcessRunnerResult>>(StringComparer.Ordinal);
            _calls = new List<(string, string)>();
        }

        public void RegisterDefault(ProcessRunnerResult result)
        {
            _defaultRegistration = result;
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

        public bool ReceivedCallToFile(string file)
        {
            return _calls.Any(c => c.File.Equals(file, StringComparison.OrdinalIgnoreCase));
        }

        public Task<ProcessRunnerResult> Run(string file, string arguments, Action<CommandEvent>? handler = null)
        {
            _calls.Add((file, arguments));

            var key = GetKey(file, arguments);
            if (_registrations.ContainsKey(key))
            {
                if (_registrations[key].Count > 0)
                {
                    return Task.FromResult(_registrations[key].Dequeue());
                }
            }

            if (_defaultRegistration != null)
            {
                return Task.FromResult(_defaultRegistration);
            }

            throw new InvalidOperationException($"No process registration found for \"{key}\"");
        }

        private static string GetKey(string file, string arguments)
        {
            return $"{file} {arguments}";
        }
    }
}
