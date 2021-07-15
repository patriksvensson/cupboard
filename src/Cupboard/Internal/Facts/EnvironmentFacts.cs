using System;
using System.Collections.Generic;
using Spectre.Console.Cli;
using Spectre.IO;

namespace Cupboard.Internal
{
    internal sealed class EnvironmentFacts : IFactProvider
    {
        private readonly IEnvironment _environment;

        public EnvironmentFacts(IEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public IEnumerable<(string Name, object Value)> GetFacts(IRemainingArguments args)
        {
            foreach (var (key, value) in _environment.GetEnvironmentVariables())
            {
                yield return ($"env.{key}", value ?? string.Empty);
            }
        }
    }
}
