namespace Cupboard;

[PublicAPI]
public interface IFactBuilder
{
    FactCollection Build(IRemainingArguments args);
}
