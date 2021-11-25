using System;
using System.Collections.Generic;
using Spectre.Console.Cli;

namespace Cupboard;

internal sealed class WindowsFacts : IFactProvider
{
    public IEnumerable<(string Name, object Value)> GetFacts(IRemainingArguments args)
    {
        var isSandboxUser = Environment.UserName.Equals("WDAGUtilityAccount", StringComparison.OrdinalIgnoreCase);
        yield return ("windows.sandbox", isSandboxUser);
    }
}