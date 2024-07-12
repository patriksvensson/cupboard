namespace Cupboard;

public interface IFactBuilder
{
    FactCollection Build(IRemainingArguments args);
}
