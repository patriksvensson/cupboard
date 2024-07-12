namespace Cupboard;

[Flags]
[PublicAPI]
public enum Permissions
{
    None = 0,
    Execute = 1,
    Write = 2,
    Read = 4,
    All = Read | Write | Execute,
}
