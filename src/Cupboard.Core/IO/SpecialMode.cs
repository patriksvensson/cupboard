namespace Cupboard;

[Flags]
public enum SpecialMode
{
    None = 0,
    Sticky = 1,
    Setgid = 1 << 1,
    Setuid = 1 << 2,
    All = Sticky | Setgid | Setuid,
}
