namespace Cupboard;

internal sealed class ArgumentFacts : IFactProvider
{
    public const string Prefix = "arg";

    public IEnumerable<(string Name, object Value)> GetFacts(IRemainingArguments args)
    {
        foreach (var argument in args.Parsed)
        {
            yield return ($"{Prefix}." + argument.Key, argument.Last() ?? string.Empty);
        }
    }
}

[PublicAPI]
public static class ArgumentFactsExtensions
{
    public static bool HasArgument(this FactCollection facts, string key)
    {
        return facts[$"{ArgumentFacts.Prefix}.{key}"].As<string>() != null;
    }

    public static string? Argument(this FactCollection facts, string key)
    {
        return facts[$"{ArgumentFacts.Prefix}.{key}"].As<string>();
    }

    public static string Argument(this FactCollection facts, string key, string defaultValue)
    {
        if (defaultValue is null)
        {
            throw new ArgumentNullException(nameof(defaultValue));
        }

        var value = facts[$"{ArgumentFacts.Prefix}.{key}"].As<string>();
        return value ?? defaultValue;
    }
}
