using System;

namespace Cupboard;

[Flags]
public enum Permissions
{
    None = 0,
    Execute = 1,
    Write = 2,
    Read = 4,
    All = Read | Write | Execute,
}