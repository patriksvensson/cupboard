using System;
using System.Collections.Generic;
using Spectre.Console.Cli;

namespace Cupboard
{
    internal sealed class EnvironmentFacts : IFactProvider
    {
        private readonly ICupboardEnvironment _environment;

        public EnvironmentFacts(ICupboardEnvironment environment)
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
