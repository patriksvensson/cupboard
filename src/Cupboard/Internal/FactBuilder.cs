using System.Collections.Generic;
using Spectre.Console.Cli;

namespace Cupboard.Internal
{
    internal sealed class FactBuilder
    {
        private readonly IReadOnlyList<IFactProvider> _providers;

        public FactBuilder(IEnumerable<IFactProvider> providers)
        {
            _providers = providers.ToReadOnlyList();
        }

        public FactCollection Build(IRemainingArguments args)
        {
            var facts = new FactCollection();
            foreach (var provider in _providers)
            {
                foreach (var (name, value) in provider.GetFacts(args))
                {
                    facts.Add(name, value ?? string.Empty);
                }
            }

            return facts;
        }
    }
}
