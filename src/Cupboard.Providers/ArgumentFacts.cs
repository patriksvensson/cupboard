using System.Collections.Generic;
using System.Linq;
using Spectre.Console.Cli;

namespace Cupboard
{
    internal sealed class ArgumentFacts : IFactProvider
    {
        public IEnumerable<(string Name, object Value)> GetFacts(IRemainingArguments args)
        {
            foreach (var argument in args.Parsed)
            {
                yield return ("arg." + argument.Key, argument.Last() ?? string.Empty);
            }
        }
    }
}
