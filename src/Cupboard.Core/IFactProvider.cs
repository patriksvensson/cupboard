namespace Cupboard;

public interface IFactProvider
{
    IEnumerable<(string Name, object Value)> GetFacts(IRemainingArguments args);
}
