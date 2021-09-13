using System;
using System.Collections.Generic;
using Spectre.Console.Cli;

namespace Cupboard
{
    internal sealed class EnvironmentFacts : IFactProvider
    {
        private readonly ICupboardEnvironment _environment;

        public const string Prefix = "env";

        public EnvironmentFacts(ICupboardEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public IEnumerable<(string Name, object Value)> GetFacts(IRemainingArguments args)
        {
            foreach (var (key, value) in _environment.GetEnvironmentVariables())
            {
                yield return ($"{Prefix}.{key}", value ?? string.Empty);
            }
        }
    }

    public static class EnvironmentFactsExtensions
    {
        public static bool HasEnvironmentVariable(this FactCollection facts, string key)
        {
            return facts[$"{EnvironmentFacts.Prefix}.{key}"].As<string>() != null;
        }

        public static string? EnvironmentVariable(this FactCollection facts, string key)
        {
            return facts[$"{EnvironmentFacts.Prefix}.{key}"].As<string>();
        }

        public static string EnvironmentVariable(this FactCollection facts, string key, string defaultValue)
        {
            if (defaultValue is null)
            {
                throw new ArgumentNullException(nameof(defaultValue));
            }

            var value = facts[$"{EnvironmentFacts.Prefix}.{key}"].As<string>();
            return value ?? defaultValue;
        }
    }
}
