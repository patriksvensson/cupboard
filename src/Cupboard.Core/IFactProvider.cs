using System.Collections.Generic;
using Spectre.Console.Cli;

namespace Cupboard;

public interface IFactProvider
{
    IEnumerable<(string Name, object Value)> GetFacts(IRemainingArguments args);
}
